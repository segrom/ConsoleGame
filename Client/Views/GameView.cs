using ClientServerDb.Common;
using ClientServerDb.Networking;
using ClientServerDb.Views.Core;
using ClientServerDb.Views.CustomComponents;
using Domain.Models;
using Domain.Models.Game;
using Server.Connections;

namespace ClientServerDb.Views;

public class GameView: BaseView
{

    private readonly ReactString _help = new($" \'{Configuration.PlayerChar}\'- player, \'{Configuration.EnemyChar}\'- other player, \'{Configuration.WallChar}\'- wall");
    private readonly ReactBoolean _isLoading = new(false);
    private readonly ReactValue<IGameConnectionClient?> _connection = new(null);
    private readonly ReactValue<GameStateModel?> _gameState = new(null);
    
    public override string Title => $"Game on server [{_serverModel.Address}]";
    protected override string Scheme => @"
@gameHelp
 
@gameMap

@exit
    ";

    private GameServerModel _serverModel;
    
    public GameView(Router router, GameServerModel server) : base(router)
    {
        _isLoading.Value = true;
        _serverModel = server;
        
        AddText("@gameHelp", _help);
        AddButton("@exit", new ReactString("Exit"), Exit);
        AddCustomComponent("@gameMap", new GameMapComponent(_gameState, _connection, _isLoading));

        ConnectToServer();
    }

    private void Exit()
    {
        _connection.Value?.Exit();
        _connection.Value = null;
        Router.GoTo(new MainView(Router));
    }

    private async void ConnectToServer()
    {
        var (connection, errors) = await Api.Instance.GetConnection(_serverModel.Id);
        
        if(errors != null || connection is null)
        {
            _isLoading.Value = false;
            throw new Exception(string.Join(", ", errors));
        }

        _connection.Value = connection;
        SubscribeEvents(_connection.Value);
        _isLoading.Value = false;
        _connection.Value.Enter();
        StateChanged();
    }

    private void SubscribeEvents(IGameConnectionClient connection)
    {
        connection.StateChanged += state =>
        {
            _gameState.Value = state;
            StateChanged();
        };
        
    }
}