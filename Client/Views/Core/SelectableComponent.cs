namespace ClientServerDb.Views.Core;

public abstract class SelectableComponent: BaseViewComponent
{
    public bool IsSelected { get; set; }

    public abstract void OnKeyPressed(ConsoleKeyInfo key);
}