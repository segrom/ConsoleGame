using DataAccess;
using DataAccess.DbSets.GameServers;
using DataAccess.DbSets.Users;
using Server.Controllers.GameServers;
using Server.Controllers.Users;
using Server.Services;

namespace Server.Connections;

public class ServerConnection: IServerConnection
{
    private readonly IAuthService _authService;
    private readonly IGameService _gameService;
    private readonly IDbContext _dbContext;
    
    public IUserController UserController { get; }
    public IGameServersController GameServersController { get; }

    public ServerConnection()
    {
        _dbContext = new DbContext(new UserSet(), new GameServerSet());
        _authService = new AuthService();
        _gameService = new GameService(_dbContext);
        _gameService.RunServers();

        UserController = new UserController(_dbContext, _authService);
        GameServersController = new GameServersController(_dbContext, _authService, _gameService);
    }
}