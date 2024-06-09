using Domain.Common;
using Domain.Communications.Login;
using Domain.Communications.Registration;

namespace Server.Controllers.Users;

public interface IUserController
{
    Task<BaseResponse> Login(LoginRequest request);
    Task<BaseResponse> Registration(RegistrationRequest request);
    Task<BaseResponse> GetUser(string session);
}