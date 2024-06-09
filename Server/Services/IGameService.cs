using Domain.Models;
using Server.Connections;

namespace Server.Services;

public interface IGameService
{
    IGameConnectionClient CreateGameConnection(string serverAddress, Guid userId);

    Task RunServers();
}