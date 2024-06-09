using ClientServerDb.Common;
using ClientServerDb.Networking;
using ClientServerDb.Views.Core;
using Domain.Communications.Login;
using Domain.Enums;
using Server;

namespace ClientServerDb.Views;

public class LoginView: BaseView
{
    private readonly ReactString _errors = new("");
    private readonly ReactString _enterButtonLabel = new("Enter");
    private readonly ReactString _registrationButtonLabel = new("Registration");
    private readonly ReactString _nickname = new("");
    private readonly ReactString _password = new("");
    private readonly ReactBoolean _isLoading = new(false);

    public override string Title => "Login";

    protected override string Scheme => @"
    Login:
    nickname: @nickname
    password: @password
    @errors
   @enterButton
    
   @registrationButton
    ";

    public LoginView(Router router) : base(router)
    {
        AddText("@errors",_errors);
        AddInput("@nickname",_nickname);
        AddInput("@password",_password);
        AddButton("@enterButton", _enterButtonLabel, OnLogin, _isLoading);
        AddButton("@registrationButton", _registrationButtonLabel, GoToRegistration);
    }

    private async void OnLogin()
    {
        _isLoading.Value = true;
        var errors = await Api.Instance.Login(_nickname.Value, _password.Value);
        
        _isLoading.Value = false;
        if(errors != null)
        {
             _errors.Value = $"(Error: {string.Join("\n", errors)})";
        }
        else
        {
            Router.GoTo(new MainView(Router));
        }
        
        StateChanged();
    }

    private void GoToRegistration()
    {
        Router.OpenOverView(new RegistrationView(Router));
    }
}