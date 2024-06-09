using Domain.Common;
using Domain.Models;

namespace Server.Services;

public class AuthService: IAuthService
{
    private readonly Dictionary<Guid, Guid> _sessions = new();
    
    public Guid? GetUser(Guid sessionId)
    {
        return _sessions.TryGetValue(sessionId, out var userId)? userId : null ;
    }

    public Guid AuthUser(UserModel user)
    {
        var guid = Guid.NewGuid();
        _sessions[guid] = user.Id;
        return guid;
    }
}