using Domain.Common;

namespace Domain.Models.Game;

public class GameStateModel: IModel
{
    public Guid Id { get; set; }
    public MapCell[,] Map { get; set; }
    public Dictionary<Guid, PlayerModel> Players { get; set; }
}