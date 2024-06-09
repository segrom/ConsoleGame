using ClientServerDb.Common;
using ClientServerDb.Networking;
using ClientServerDb.Views.Core;

namespace ClientServerDb.Views;

public class MainView: BaseView
{
    private readonly ReactString _userInfo = new("");
    public override string Title => "Main";

    protected override string Scheme => @"
    User: @user
    
    @play
    @logout";
    
    public MainView(Router router) : base(router)
    {
        AddText("@user", _userInfo);
        AddButton("@play", new ReactString("Play"), OnPlay);
        AddButton("@logout", new ReactString("Logout"), OnLogout);
        FetchUser();
    }

    private async void FetchUser()
    {
        var (user, errors) = await Api.Instance.GetUser();
        if(errors != null)
        {
            throw new Exception(string.Join(", ", errors));
        }

        _userInfo.Value = $"{user.Nickname}";
        StateChanged();
    }

    private void OnPlay()
    {
        Router.OpenOverView(new ServerSelectView(Router));
    }
    
    private void OnLogout()
    {
        Api.Instance.Logout();
        Router.GoTo(new LoginView(Router));
    }
}