using System.Numerics;
using Domain.Models.Game;

namespace Server.Connections;

public interface IGameConnectionServer
{
    void StateChanged(GameStateModel state);
    event Action<Guid, Vector2> PlayerMove; // id player, new position
    event Action<Guid, Guid>  PlayerAttack; // id player, id target
    event Action<Guid>  PlayerEnter; // id player
    event Action<Guid>  PlayerExit; // id player
}