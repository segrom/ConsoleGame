namespace ClientServerDb.Views.Core;

public class Router
{
    public Stack<BaseView> ViewStack = new Stack<BaseView>();
    public BaseView Current => ViewStack.Peek();

    public void GoTo<T>(T view) where T: BaseView
    {
        ViewStack.Clear();
        ViewStack.Push(view);
    }
    
    public void OpenOverView<T>(T view) where T: BaseView
    {
        ViewStack.Push(view);
    }
    
    public void Back()
    {
        ViewStack.Pop();
    }
}