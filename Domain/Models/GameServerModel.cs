using Domain.Common;

namespace Domain.Models;

public class GameServerModel: IModel
{
    public Guid Id { get; set; }
    public string Address { get; set; }
    public int PlayerCount { get; set; }
}