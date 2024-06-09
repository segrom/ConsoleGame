using Server.Controllers.GameServers;
using Server.Controllers.Users;

namespace Server.Connections;

public interface IServerConnection
{
    IUserController UserController { get; }
    IGameServersController GameServersController { get; }
}