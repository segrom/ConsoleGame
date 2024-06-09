using Domain.Common;
using Domain.Models;

namespace Server.Services;

public interface IAuthService
{
    // TODO: Make session expiration logic
    Guid? GetUser(Guid sessionId);
    Guid AuthUser(UserModel userId);
}