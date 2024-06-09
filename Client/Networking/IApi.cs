using Domain.Common;
using Domain.Communications.Login;
using Domain.Models;
using Server.Connections;

namespace ClientServerDb.Networking;

public interface IApi
{
    Task<string[]?> Login(string nickname, string password);
    Task<(UserModel?, string[]?)> GetUser();
    Task<(GameServerModel[]?, string[]?)> GetAllServers();
    void Logout();
    Task<string[]?> Registration(string nicknameValue, string passwordValue);
    Task<(IGameConnectionClient?, string[]?)> GetConnection(Guid serverId);
}