using ClientServerDb.Common;

namespace ClientServerDb.Views.Core.Components;

public class InputComponent: SelectableComponent
{
    public ReactString Content { get; }
    private bool _isCursorShown = true;
    private int _cursorPosition;
    
    public InputComponent(ReactString content)
    {
        Content = content;
        _cursorPosition = Content.Value.Length;
    }
    public override void OnKeyPressed(ConsoleKeyInfo key)
    {
        switch (key.Key)
        {
            case ConsoleKey.Enter: return;
            case ConsoleKey.Backspace: 
                Delete();
                return;
            case ConsoleKey.RightArrow: 
                SaveChangeCursorPos(_cursorPosition + 1);
                return;
            case ConsoleKey.LeftArrow: 
                SaveChangeCursorPos(_cursorPosition - 1);
                return;
        }
        
        Content.Value = Content.Value.Insert(_cursorPosition, key.KeyChar.ToString());
        _cursorPosition++;
    }

    private void SaveChangeCursorPos(int newPosition)
    {
        _cursorPosition = Math.Clamp(newPosition, 0, Content.Value.Length);
    }
    
    private void Delete()
    {
        if(_cursorPosition - 1 < 0) return;
        Content.Value = Content.Value.Remove(_cursorPosition - 1, 1);
        _cursorPosition--;
    }

    public override string Render()
    {
        var value = Content.Value;
        if (!IsSelected) return $"\"{value}\"";
        
        var cursor = _isCursorShown ? "|" : " ";
        value = _cursorPosition >= Content.Value.Length 
            ? Content.Value + cursor 
            : Content.Value.Insert(_cursorPosition, cursor);
        return  $"[{value}] ({_cursorPosition}/{Content.Value.Length})";

    }
    
}