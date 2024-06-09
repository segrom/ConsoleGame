using DataAccess;
using Domain.Models;
using Server.Common;
using Server.Connections;
using Server.Controllers.Users;

namespace Server.Services;

public class GameService: IGameService
{
    private IDbContext _context;

    private Dictionary<string, GameServer> _servers = new();
    
    public GameService(IDbContext dbContext)
    {
        _context = dbContext;
    }

    public async Task RunServers(IUserStatisticsService statisticsService)
    {
        var list = await _context.GameServerSet.AllAsync();

        foreach (GameServerModel serverModel in list)
        {
            _servers[serverModel.Address] = new GameServer(statisticsService,  serverModel.PlayerCount, 10);
        }
    }

    public IGameConnectionClient CreateGameConnection(string serverAddress, Guid userId)
    {
        if (!_servers.TryGetValue(serverAddress, out var server)) throw new Exception("Server not found");

        return server.ConnectUser(userId);
    }
}