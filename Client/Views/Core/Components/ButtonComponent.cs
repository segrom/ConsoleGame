using ClientServerDb.Common;

namespace ClientServerDb.Views.Core.Components;

public class ButtonComponent: SelectableComponent
{
    public ReactBoolean? IsLoading { get; }
    public ReactString Label { get; }
    public Action Callback { get; }
    
    public ButtonComponent(ReactString label, Action callback, ReactBoolean? isLoading = null)
    {
        Label = label;
        Callback = callback;
        IsLoading = isLoading;
    }
    
    public override void OnKeyPressed(ConsoleKeyInfo key)
    {
        if(IsLoading?.Value ?? false) return;
        if(key.Key == ConsoleKey.Enter) Callback.Invoke();
    }
    
    public override string Render()
    {
        if(IsLoading?.Value ?? false) return IsSelected ? $"[Loading]" : $" Loading ";
        return IsSelected ? $"[{Label.Value}]" : $" {Label.Value} ";
    }
    
}