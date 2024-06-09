using Domain.Common;
using Domain.Communications.Login;
using Domain.Communications.Registration;
using Domain.Enums;
using Domain.Models;
using Server;
using Server.Connections;

namespace ClientServerDb.Networking;

public class Api: IApi
{
    private static IApi? _instance;
    public static IApi Instance => _instance ??= new Api();

    private readonly IServerConnection _serverConnection = new ServerConnection();

    private string? _session;

    // there is a lot of repetitive code that can be shortened
    
    public async Task<string[]?> Login(string nickname, string password)
    {
        var result = await _serverConnection.UserController.Login(new LoginRequest()
        {
            Nickname = nickname,
            Password = password
        });

        if (result.Status != ResponseStatus.Ok)
        {
            return result.Errors;
        }

        if (result.Value is not string session) return new []{"Bad response"};
        _session = session;
        return null;
    }

    public async Task<string[]?> Registration(string nickname, string password)
    {
        var result = await _serverConnection.UserController.Registration(new RegistrationRequest()
        {
            Nickname = nickname,
            Password = password
        });

        if (result.Status != ResponseStatus.Ok)
        {
            return result.Errors;
        }

        if (result.Value is not string session) return new []{"Bad response"};
        _session = session;
        return null;
    }
    
    public async Task<(UserModel?, string[]?)> GetUser()
    {
        var result = await _serverConnection.UserController.GetUser(_session);
        if (result.Status != ResponseStatus.Ok)
        {
            return (null, result.Errors);
        }
        
        if (result.Value is not UserModel user) return (null, new []{"Bad response"});
        return (user, null);
    }

    public async Task<(GameServerModel[]?, string[]?)> GetAllServers()
    {
        var result = await _serverConnection.GameServersController.GetAll(_session);
        if (result.Status != ResponseStatus.Ok)
        {
            return (null, result.Errors);
        }
        
        if (result.Value is not GameServerModel[] servers) return (null, new []{"Bad response"});
        return (servers, null);
    }

    public async Task<(IGameConnectionClient?, string[]?)> GetConnection(Guid serverId)
    {
        var result = await _serverConnection.GameServersController.ConnectToServer(_session, serverId);
        if (result.Status != ResponseStatus.Ok)
        {
            return (null, result.Errors);
        }
        
        if (result.Value is not IGameConnectionClient connection) return (null, new []{"Bad response"});
        return (connection, null);
    }
    
    public void Logout()
    {
        _session = null;
    }
}