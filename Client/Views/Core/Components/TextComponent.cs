using ClientServerDb.Common;

namespace ClientServerDb.Views.Core.Components;

public class TextComponent: BaseViewComponent
{
    public ReactString Text { get; }
    
    public TextComponent(ReactString text)
    {
        Text = text;
    }
    
    public override string Render()
    {
        return Text.Value;
    }
}