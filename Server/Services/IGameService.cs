using Domain.Models;
using Server.Connections;
using Server.Controllers.Users;

namespace Server.Services;

public interface IGameService
{
    IGameConnectionClient CreateGameConnection(string serverAddress, Guid userId);

    Task RunServers(IUserStatisticsService statisticsService);
}