using ClientServerDb.Common;
using ClientServerDb.Networking;
using ClientServerDb.Views.Core;

namespace ClientServerDb.Views;

public class RegistrationView: BaseView
{
    private readonly ReactString _errors = new("");
    private readonly ReactString _enterButtonLabel = new("Enter");
    private readonly ReactString _nickname = new("");
    private readonly ReactString _password = new("");
    private readonly ReactString _passagain = new("");
    private readonly ReactBoolean _isLoading = new(false);
    
    public override string Title => "Registration";
    protected override string Scheme => @"
    Registration:
    nickname: @nickname
    password: @password
    password again: @passagain
    @errors
   @enterButton
    ";
    
    public RegistrationView(Router router) : base(router)
    {
        AddText("@errors",_errors);
        AddInput("@nickname",_nickname);
        AddInput("@password",_password);
        AddInput("@passagain",_passagain);
        AddButton("@enterButton", _enterButtonLabel, OnRegistration, _isLoading);
    }

    private async void OnRegistration()
    {
        if (!_password.Value.Equals(_passagain.Value))
        {
            _errors.Value = $"The passwords are not the same!";
            return;
        }
        
        _isLoading.Value = true;
        var errors = await Api.Instance.Registration(_nickname.Value, _password.Value);
        
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
}