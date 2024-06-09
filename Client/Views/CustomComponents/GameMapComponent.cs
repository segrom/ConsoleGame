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
        if (_connection.Value is null) return "Something wrong, connection is null";

        var output = new StringBuilder();
        
        (char side, char flat) borderChars = IsSelected ? ('|', '_') : (' ', ' ');
        
        var mapSize = _gameState.Value.Map.GetLength(0);
        
        output.Append(new string(borderChars.flat, mapSize) + "\n");
        
        for (int y = 0; y < mapSize; y++)
        {
            output.Append(borderChars.side);
            
            for (int x = 0; x < mapSize; x++)
            {
                var player = _gameState.Value.Players.FirstOrDefault(p=>p.Value.Position == new Vector2(x,y)).Value;
                if (player != null)
                {
                    output.Append(_connection.Value.PlayerId.Equals(player.Id) ? '@' : '$');
                    continue;
                }
                
                switch (_gameState.Value.Map[x,y].Containment)
                {
                    case MapCellContainment.Wall: output.Append('#');
                        continue;
                    
                    default:
                    case MapCellContainment.Empty: output.Append('.');
                        continue;
                }
            }
            
            output.Append(borderChars.side);
            output.Append('\n');
        }
        
        output.Append(new string(borderChars.flat, mapSize) + "\n\n");

        output.Append(GenerateDescription());
        output.Append(GenerateActions());
        
        return output.ToString();
    }

    private string? GenerateDescription()
    {
        if (!IsSelected) return "\n";
        if (MyPlayer is null) return "Player not found\n";
        return $"Health: {MyPlayer.Health}\n";
    }

    private string GenerateActions()
    {
        if (!IsSelected) return "\n";

        return "User arrows to move, and X to attack\n";
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
        }
    }
}