using DataAccess;
using Domain.Common;
using Domain.Communications.Login;
using Domain.Communications.Registration;
using Domain.Models;
using Server.Common;
using Server.Services;
using Server.Utils;

namespace Server.Controllers.Users;

public class UserController: BaseController, IUserController
{
    private readonly IDbContext _context;
    private readonly IAuthService _authService;
    
    public UserController(IDbContext context, IAuthService authService)
    {
        _context = context;
        _authService = authService;
    }
    
    public async Task<BaseResponse> Login(LoginRequest request)
    {
        var user = await _context.UserSet.GetUserByNicknameAsync(request.Nickname);
        
        if(user is null || !PasswordHash.ValidatePassword(request.Password, user.PasswordHash)) 
            return Unauthorized("Wrong nickname or password");
        var session = _authService.AuthUser(user);
        return Ok(session.ToString());
    }
    
    public async Task<BaseResponse> Registration(RegistrationRequest request)
    {
        var user = await _context.UserSet.GetUserByNicknameAsync(request.Nickname);
        if(user is not null) return BadRequest($"Nickname {user.Nickname} already exists");

        user = new UserModel()
        {
            Nickname = request.Nickname,
            PasswordHash = PasswordHash.CreateHash(request.Password),
            Id = Guid.NewGuid()
        };

        if(!await _context.UserSet.CreateAsync(user)) return InternalError($"Failed to create a new user");
        
        var session = _authService.AuthUser(user);
        return Ok(session.ToString());
    }

    public async Task<BaseResponse> GetUser(string session)
    {
        var userId = _authService.GetUser(Guid.Parse(session));
        if(userId is null) return Unauthorized("Unauthorized");
        var user = await _context.UserSet.GetAsync(userId.Value);
        if(user is null) return Unauthorized("Unauthorized");
        return Ok(user);
    }
}