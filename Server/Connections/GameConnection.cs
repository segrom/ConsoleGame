using System.Numerics;
using Domain.Models.Game;

namespace Server.Connections;

public class GameConnection: IGameConnectionClient, IGameConnectionServer
{
    public Guid PlayerId { get; set; }

    public GameConnection(Guid playerId)
    {
        PlayerId = playerId;
    }

    #region Client
   
    public event Action<GameStateModel>? StateChanged;
    
    public void Move(Vector2 newPos) => PlayerMove?.Invoke(PlayerId, newPos);

    public void Attack(Guid targetId) => PlayerAttack?.Invoke(PlayerId, targetId);
    

    public void Enter() => PlayerEnter?.Invoke(PlayerId);


    public void Exit() => PlayerExit?.Invoke(PlayerId);

    #endregion
    
    #region Server
    
    public event Action<Guid, Vector2>? PlayerMove;
    public event Action<Guid, Guid>? PlayerAttack;
    public event Action<Guid>? PlayerEnter;
    public event Action<Guid>? PlayerExit;

    void IGameConnectionServer.StateChanged(GameStateModel state)
    {
        StateChanged?.Invoke(state);
    }
    
    #endregion
    
}