using System.Numerics;
using System.Text;
using ClientServerDb.Common;
using ClientServerDb.Views.Core;
using Domain.Models;
using Domain.Models.Game;
using Server.Connections;

namespace ClientServerDb.Views.CustomComponents;

public class GameMapComponent: SelectableComponent
{

    private ReactValue<GameStateModel?> _gameState;
    private ReactValue<IGameConnectionClient?> _connection;
    private ReactBoolean? _isLoading;

    private PlayerModel? _selectedTarget = null;
    private List<PlayerModel>? _reachablePlayers = null;

    private PlayerModel? MyPlayer 
        => _connection.Value is null
            ? null
            : _gameState.Value?.Players[_connection.Value.PlayerId];
    
    public GameMapComponent(ReactValue<GameStateModel?> gameState, ReactValue<IGameConnectionClient?> connection, ReactBoolean? isLoading = null)
    {
        _connection = connection;
        _gameState = gameState;
        _isLoading = isLoading;
    }
    
    public override string Render()
    {
        if ((_isLoading?.Value ?? false) || _gameState.Value is null) return "Loading";
        if (_connection.Value is null || MyPlayer is null) return "Something wrong, connection is null";

        _reachablePlayers = _gameState.Value.Players
            .Where(p => p.Key != MyPlayer.Id 
                        && p.Key != MyPlayer.AttackTargetId 
                        && (p.Value.Position - MyPlayer.Position).Length() < 1.5f)
            .Select(p=>p.Value).ToList();
        if (_selectedTarget != null && !_reachablePlayers.Any(p => p.Id.Equals(_selectedTarget.Id))) _selectedTarget = null;
        
        var output = new StringBuilder();
        
        (char side, char flat) borderChars = IsSelected ? ('\u2551', '\u2550') : ('\u2502', '\u2500');
        
        var mapSize = _gameState.Value.Map.GetLength(0);
        
        output.Append(new string(borderChars.flat, mapSize + 2) + "\n");
        
        for (int y = 0; y < mapSize; y++)
        {
            output.Append(borderChars.side);
            
            for (int x = 0; x < mapSize; x++)
            {
                var player = _gameState.Value.Players.FirstOrDefault(p=>p.Value.Position == new Vector2(x,y)).Value;
                if (player != null)
                {
                    if (MyPlayer.AttackTargetId.HasValue && player.Id.Equals(MyPlayer.AttackTargetId))
                    {
                        output.Append(Configuration.EnemyBattledChar);
                    }
                    else if(_selectedTarget != null && player.Id.Equals(_selectedTarget.Id))
                        output.Append(Configuration.EnemySelectedChar);
                    else 
                        output.Append(_connection.Value.PlayerId.Equals(player.Id) ? Configuration.PlayerChar : Configuration.EnemyChar);
                    continue;
                }
                
                switch (_gameState.Value.Map[x,y].Containment)
                {
                    case MapCellContainment.Wall: output.Append(Configuration.WallChar);
                        continue;
                    
                    default:
                    case MapCellContainment.Empty: output.Append(' ');
                        continue;
                }
            }
            
            output.Append(borderChars.side);
            output.Append('\n');
        }
        
        output.Append(new string(borderChars.flat, mapSize + 2) + "\n\n");

        output.Append(GenerateDescription());
        output.Append(GenerateActions());
        
        return output.ToString();
    }

    private string? GenerateDescription()
    {
        if (!IsSelected) return "Select game map use TAB to control\n";
        if (MyPlayer is null) return "Player not found\n";
        return $"You have: {MyPlayer.Health}hp\n";
    }

    private string GenerateActions()
    {
        if (!IsSelected || _gameState.Value is null || MyPlayer is null) return "\n";
        var output = $"Use arrows to move\n{(_reachablePlayers?.Count > 0 ? $"Use SPACE to select target to attack (target shows like \'{Configuration.EnemySelectedChar}\')" : "No reachable players")}\n";

        if (_selectedTarget != null) output += $"Selected target have {_selectedTarget.Health}hp\n Press ENTER to Attack";
        if (MyPlayer.AttackTargetId.HasValue) output += $"You fighting with \'X\'. His health = {_gameState.Value.Players[MyPlayer.AttackTargetId.Value].Health}hp";
        
        
        return output;
    }

    public override void OnKeyPressed(ConsoleKeyInfo key)
    {
        if(!IsSelected || MyPlayer is null || _connection.Value is null) return;
        switch (key.Key)
        {
            case ConsoleKey.UpArrow: _connection.Value.Move(MyPlayer.Position + new Vector2(0, -1));
                return;
            case ConsoleKey.DownArrow: _connection.Value.Move(MyPlayer.Position + new Vector2(0, 1));
                return;
            case ConsoleKey.RightArrow: _connection.Value.Move(MyPlayer.Position + new Vector2(1, 0));
                return;
            case ConsoleKey.LeftArrow: _connection.Value.Move(MyPlayer.Position + new Vector2(-1, 0));
                return;
            case ConsoleKey.Spacebar: SelectedTarget();
                return;
            case ConsoleKey.Enter: StartFight();
                return;
        }
    }

    private void StartFight()
    {
        if(_connection.Value is null || _selectedTarget is null) return;
        _connection.Value.Attack(_selectedTarget.Id);
        _selectedTarget = null;
    }

    private void SelectedTarget()
    {
        if(_reachablePlayers is null || _reachablePlayers.Count <=0)
        {
            _selectedTarget = null;
            return;
        }

        if (_selectedTarget is null)
        {
            _selectedTarget = _reachablePlayers.First();
            return;
        }
        
        var index = _reachablePlayers.FindIndex(p => p == _selectedTarget);
        
        if (index < 0 || index + 1 >= _reachablePlayers.Count)
        {
            _selectedTarget = _reachablePlayers.First();
            return;
        }

        _selectedTarget = _reachablePlayers[index + 1];
    }
}