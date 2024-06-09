using Domain.Common;

namespace Server.Controllers.GameServers;

public interface IGameServersController
{
    Task<BaseResponse> GetAll(string session);

    Task<BaseResponse> ConnectToServer(string session, Guid serverId);
}