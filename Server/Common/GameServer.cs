using System.Numerics;
using Domain.Models.Game;
using Server.Connections;
using Server.Controllers.Users;
using Server.Services;
using Server.Utils;

namespace Server.Common;

public class GameServer
{
    private const float MaxStepLength = 1.2f;
    private const float MaxAttackRange = 1.5f;
    private const int AttackDamage = 2;

    private IUserStatisticsService _statisticsService;
    
    private readonly Dictionary<Guid, GameConnection> _connections = new();
    private GameStateModel _state;
    
    private Task _gameLoop;
    
    public GameServer(IUserStatisticsService statisticsService, int initialPlayerCount, int mapSize)
    {
        _statisticsService = statisticsService;
        var map = GameGenerator.GenerateMap(mapSize);
        var players = GameGenerator.GeneratePlayers(initialPlayerCount, map, mapSize);
        
        _state = new GameStateModel()
        {
            Id = Guid.NewGuid(),
            Map = map,
            Players = players
        };

        _gameLoop = GameLoop();
    }

    private async Task GameLoop()
    {
        while (_state.Players.Count > 0)
        {
            await Task.Delay(1000);
            bool isDirty = false;
            foreach (var attacker in _state.Players.Where(p=>p.Value.AttackTargetId.HasValue))
            {
                var player = attacker.Value;
                if (player.AttackTargetId == null) continue;
                
                var target = _state.Players[player.AttackTargetId.Value];
                if ((player.Position - target.Position).Length() > MaxAttackRange)
                {
                    isDirty = true;
                    player.AttackTargetId = null;
                    target.AttackTargetId = null;
                    continue;
                }

                isDirty = true;
                target.Health -= AttackDamage;

                if (target.Health <= 0)
                {
                    _state.Players.Remove(target.Id);
                    player.AttackTargetId = null;
                    _statisticsService.UserIncrementKillScore(player.UserId);
                }
            }
            if(isDirty) UpdateState();
        }
    }

    public IGameConnectionClient ConnectUser(Guid userId)
    {
        // run gameLoop if it was stopped
        if (!_connections.ContainsKey(userId))
        {
            var pos = _state.Map.GetLength(0) / 2;
            var newPlayer = new PlayerModel()
            {
                Id = Guid.NewGuid(),
                Health = 100,
                Position = new Vector2(pos, pos),
                UserId = userId
            };
            _state.Players.Add(newPlayer.Id, newPlayer);
            _connections[userId] = new GameConnection(newPlayer.Id);
            SubscribeToConnection(_connections[userId]);
        }
        return _connections[userId];
    }

    private void SubscribeToConnection(IGameConnectionServer connection)
    {
        connection.PlayerAttack += OnPlayerAttack;
        connection.PlayerMove += OnPlayerMove;
        connection.PlayerEnter += guid => {UpdateState();};
        connection.PlayerExit += OnPlayerExit;
    }

    private void UnsubscribeToConnection(IGameConnectionServer connection)
    {
        connection.PlayerAttack -= OnPlayerAttack;
        connection.PlayerMove -= OnPlayerMove;
        connection.PlayerExit -= OnPlayerExit;
    }

    public void UpdateState()
    {
        foreach (IGameConnectionServer connection in _connections.Values)   
        {
            connection.StateChanged(_state);
        }
    }
    
    private void OnPlayerMove(Guid playerId, Vector2 newPose)
    {
        var player = _state.Players[playerId];
        
        // make some action validation
        var mapSize = _state.Map.GetLength(0);
        newPose = new Vector2(Math.Clamp(newPose.X, 0, mapSize - 1), Math.Clamp(newPose.Y, 0, mapSize - 1));
        
        if((player.Position - newPose).Length() > MaxStepLength) return;
        if(_state.Map[(int)newPose.X, (int)newPose.Y].Containment == MapCellContainment.Wall) return;
        if(_state.Players.Any(p => (p.Value.Position - newPose).Length() < 0.5f)) return;
        
        player.Position = newPose;
        
        UpdateState();
    }

    private void OnPlayerAttack(Guid playerId, Guid targetId)
    {
        var player = _state.Players[playerId];
        var target= _state.Players[targetId];
        
        // make some action validation
        if((player.Position - target.Position).Length() > MaxAttackRange) return;
        player.AttackTargetId = target.Id;
        target.AttackTargetId = playerId;
        
        UpdateState();
    }

    private void OnPlayerExit(Guid playerId)
    {
        _state.Players.Remove(playerId);
        UpdateState();
    }
    
}