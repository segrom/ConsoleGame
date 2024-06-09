using ClientServerDb.Common;
using ClientServerDb.Networking;
using ClientServerDb.Views.Core;
using Domain.Models;

namespace ClientServerDb.Views;

public class ServerSelectView:  BaseView
{
    private readonly ReactString _errors = new("");
    private readonly ReactBoolean _isLoading = new(false);
    public override string Title => "Select Server";
    protected override string Scheme => $"@errors\n\tAvailable servers:\n\t{(_isLoading.Value? "loading":"")}{string.Join("\n\t",ButtonKeys)}";

    private List<string> ButtonKeys = new List<string>();
    
    public ServerSelectView(Router router) : base(router)
    {
        AddText("@errors", _errors);
        FetchServers();
    }

    private async void FetchServers()
    {
        _isLoading.Value = true;
        var (servers, errors) = await Api.Instance.GetAllServers();
        _isLoading.Value = false;
        if(errors != null)
        {
            _errors.Value = $"(Error: {string.Join("\n", errors)})\n";
        }
        else
        {
            for (int i = 0; i < servers.Length; i++)
            {
                var serverKey = "@server" + i;
                var server = servers[i];
                ButtonKeys.Add(serverKey);
                AddButton(serverKey, new ReactString($"{servers[i].Address} Playing: {servers[i].PlayerCount}"), () =>
                {
                    SelectServer(server);
                });
            }
        }
        StateChanged();
    }

    private void SelectServer(GameServerModel server)
    {
        Router.GoTo(new GameView(Router, server));
    }
}
