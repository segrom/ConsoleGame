using System.Numerics;
using Domain.Models.Game;

namespace Server.Connections;

public interface IGameConnectionClient
{
    Guid PlayerId { get; }
    event Action<GameStateModel> StateChanged; 
    void Move(Vector2 newPos);
    void Attack(Guid targetId);
    void Enter();
    void Exit();
}