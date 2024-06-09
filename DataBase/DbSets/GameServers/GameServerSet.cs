using DataAccess.DbSets.Users;
using Domain.Models;

namespace DataAccess.DbSets.GameServers;

public class GameServerSet: IGameServerSet
{

    private static readonly Dictionary<Guid, GameServerModel> Storage = new()
    {
        {
            new Guid(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11),
            new GameServerModel()
            {
                Id = new Guid(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11),
                Address = "192.213.12.31",
                PlayerCount = 2,
            }
        },
        {
            new Guid(22, 2, 3, 31, 5, 6, 7, 8, 55, 10, 11),
            new GameServerModel()
            {
                Id = new Guid(22, 2, 3, 31, 5, 6, 7, 8, 55, 10, 11),
                Address = "155.321.221.3",
                PlayerCount = 0,
            }
        }
    };
    
    public async Task<bool> CreateAsync(GameServerModel model)
    {
        if (Storage.ContainsKey(model.Id)) return false;
        await Task.Delay(100);
        Storage.Add(model.Id, model);
        return true;
    }

    public async Task<bool>  UpdateAsync(GameServerModel model)
    {
        await Task.Delay(100);
        if (!Storage.ContainsKey(model.Id)) return false;
        Storage[model.Id].Address = model.Address;
        Storage[model.Id].PlayerCount = model.PlayerCount;
        return true;
    }

    public async Task<GameServerModel?> GetAsync(Guid id)
    {
        await Task.Delay(100);
        return !Storage.ContainsKey(id) ? null : Storage[id];
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        if (Storage.ContainsKey(id)) return false;
        await Task.Delay(100);
        Storage.Remove(id);
        return true;
    }

    public async Task<IEnumerable<GameServerModel>> AllAsync()
    {
        await Task.Delay(300);
        return Storage.Values;
    }
    
}