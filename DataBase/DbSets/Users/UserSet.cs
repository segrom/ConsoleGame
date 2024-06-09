using Domain.Models;

namespace DataAccess.DbSets.Users;

public class UserSet: IUserSet
{

    private static readonly Dictionary<Guid, UserModel> Storage = new();
    
    public async Task<bool> CreateAsync(UserModel model)
    {
        if (Storage.ContainsKey(model.Id)) return false;
        await Task.Delay(100);
        Storage.Add(model.Id, model);
        return true;
    }

    public async Task<bool>  UpdateAsync(UserModel model)
    {
        await Task.Delay(100);
        if (!Storage.ContainsKey(model.Id)) return false;
        Storage[model.Id].Nickname = model.Nickname;
        Storage[model.Id].PasswordHash = model.PasswordHash;
        return true;
    }

    public async Task<UserModel?> GetAsync(Guid id)
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

    public async Task<IEnumerable<UserModel>> AllAsync()
    {
        await Task.Delay(300);
        return Storage.Values;
    }

    public async Task<UserModel?> GetUserByNicknameAsync(string nickname)
    {
        await Task.Delay(120);
        var user = Storage.Values.FirstOrDefault(u => u.Nickname.Equals(nickname));
        return user;
    }
}