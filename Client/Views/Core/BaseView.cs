using ClientServerDb.Common;
using ClientServerDb.Views.Core.Components;

namespace ClientServerDb.Views.Core;

public abstract class BaseView
{
    public bool IsDirty { get; private set; }
    public abstract string Title { get; }
    protected Router Router { get; }
    protected readonly Dictionary<string, BaseViewComponent> Components = new();
    protected readonly List<SelectableComponent> SelectableComponents = new();

    private const string BackButtonKey = "!@back";
    protected bool _withBackButton;
    protected abstract string Scheme { get; }

    protected BaseView(Router router)
    {
        Router = router;
        if (router.ViewStack.Count > 0)
        {
            AddButton(BackButtonKey, new ReactString("Back"), Router.Back);
            _withBackButton = true;
        }
        StateChanged();
    }
    
    public void Update()
    {
        Console.Clear();
        
        var output = Components.Keys.Aggregate(GetHeader() + Scheme, (current, pKey) 
            => current.Replace(pKey, Components[pKey].Render()));
        
        Console.Write(output);
        IsDirty = false;
    }

    protected void StateChanged()
    {
        IsDirty = true;
    }
    
    private string GetHeader()
    {
        return $"use Tab to selection\nuse Esc to exit\n\n{Title} {(_withBackButton? BackButtonKey : "")} \n________________\n";
    }

    protected void AddCustomComponent(string key, BaseViewComponent component)
    {
        if (Components.ContainsKey(key)) throw new Exception($"Key \'{key}\' is duplicated");
        Components.Add(key, component);
        
        if (component is not SelectableComponent selectableComponent) return;
        SelectableComponents.Add(selectableComponent);
        Select(0);
    }
    
    protected void AddText(string key, ReactString value) 
        => AddCustomComponent(key, new TextComponent(value));
    
    protected void AddInput(string key, ReactString value)
        => AddCustomComponent(key, new InputComponent(value));
    
    protected void AddButton(string key, ReactString label, Action callback, ReactBoolean? isLoading = null)
        => AddCustomComponent(key, new ButtonComponent(label, callback, isLoading));
    
    public void OnKey(ConsoleKeyInfo key)
    {
        if (key.Key == ConsoleKey.Tab)
        {
            SelectNext();
            return;
        }
        
        foreach (var component in Components.Values)
        {
            if (component is not SelectableComponent { IsSelected: true } selectableComponent) continue;
            selectableComponent.OnKeyPressed(key);
            return;
        }
    }

    private void SelectNext()
    {
        var selected = SelectableComponents.FindIndex(component => component.IsSelected);
        var newSelected = selected < SelectableComponents.Count - 1 
            ? selected + 1 
            : 0;
        Select(newSelected);
    }
    
    private void Select(int id)
    {
        for (int i = 0; i < SelectableComponents.Count; i++)
        {
            SelectableComponents[i].IsSelected = i == id;
        }
    }
}