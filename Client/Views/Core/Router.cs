namespace ClientServerDb.Views.Core;

public class Router
{
    public readonly Stack<BaseView> ViewStack = new();
    public BaseView Current => ViewStack.Peek();

    public void GoTo<T>(T view) where T: BaseView
    {
        ViewStack.Clear();
        ViewStack.Push(view);
        view.AddBackButton();
    }
    
    public void OpenOverView<T>(T view) where T: BaseView
    {
        ViewStack.Push(view);
        view.AddBackButton();
    }
    
    public void Back()
    {
        ViewStack.Pop();
    }
}