using DataAccess;
using Domain.Common;
using Server.Common;
using Server.Services;

namespace Server.Controllers.GameServers;

public class GameServersController: BaseController, IGameServersController
{
    private readonly IDbContext _context;
    private readonly IAuthService _authService;
    private readonly IGameService _gameService;
    
    public GameServersController(IDbContext context, IAuthService authService, IGameService gameService)
    {
        _context = context;
        _authService = authService;
        _gameService = gameService;
    }

    public async Task<BaseResponse> GetAll(string session)
    {
        var userId = _authService.GetUser(Guid.Parse(session));
        if(userId is null) return Unauthorized("Unauthorized");

        var servers = await _context.GameServerSet.AllAsync();
        
        return Ok(servers.ToArray());
    }
    
    public async Task<BaseResponse> ConnectToServer(string session, Guid serverId)
    {
        var userId = _authService.GetUser(Guid.Parse(session));
        if(userId is null) return Unauthorized("Unauthorized");

        var server = await _context.GameServerSet.GetAsync(serverId);
        if(server is null) return BadRequest("Server not found");
        
        var connection = _gameService.CreateGameConnection(server.Address, userId.Value);
        
        return Ok(connection);
    }
}