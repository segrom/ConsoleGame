using System.Numerics;
using Domain.Models.Game;
using Server.Connections;

namespace Server.Common;

public class GameServer
{
    private const float MaxStepLength = 1.2f;
    private const float MaxAttackRange = 2f;
    private const int AttackDamage = 10;
    
    private readonly Dictionary<Guid, GameConnection> _connections = new();
    private GameStateModel _state;
    
    public GameServer(int initialPlayerCount, int mapSize)
    {
        var map = GenerateMap(mapSize);
        var players = GeneratePlayers(initialPlayerCount, map, mapSize);
        
        _state = new GameStateModel()
        {
            Id = Guid.NewGuid(),
            Map = map,
            Players = players
        };
    }

    private Dictionary<Guid,PlayerModel> GeneratePlayers(int count, MapCell[,] map, int mapSize)
    {
        var players = new Dictionary<Guid, PlayerModel>();

        for (int i = 0; i < count; i++)
        {
            var id = Guid.NewGuid();
            Vector2 position;
            do
            {
                position = new Vector2(Random.Shared.Next(0, mapSize), Random.Shared.Next(0, mapSize));
            } while (map[(int)position.X, (int)position.Y].Containment == MapCellContainment.Wall);
            players[id] = new PlayerModel()
            {
                Id = id,
                Health = 100,
                Position = position,
                UserId = Guid.NewGuid()
            };
        }

        return players;
    }

    private MapCell[,] GenerateMap(int mapSize)
    {
        var map = new MapCell[mapSize, mapSize];
        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                map[x, y] = new MapCell()
                {
                    Containment = Random.Shared.NextSingle() < 0.1f ? MapCellContainment.Wall : MapCellContainment.Empty,
                };
            }
        }

        return map;
    }

    public IGameConnectionClient ConnectUser(Guid userId)
    {
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
        target.Health -= AttackDamage;
        
        UpdateState();
    }

    private void OnPlayerExit(Guid playerId)
    {
        _state.Players.Remove(playerId);
        UpdateState();
    }
    
}