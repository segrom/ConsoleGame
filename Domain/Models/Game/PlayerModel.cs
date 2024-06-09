using System.Numerics;
using Domain.Common;

namespace Domain.Models.Game;

public class PlayerModel: IModel
{
    public Guid Id { get; set; }
    public Vector2 Position { get; set; }
    public int Health { get; set; }
    public Guid UserId { get; set; }
}