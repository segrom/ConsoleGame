using ClientServerDb.Views;
using ClientServerDb.Views.Core;
using Server;

Console.WriteLine("Client started");
Console.CursorVisible = false;

var router = new Router();

router.GoTo(new LoginView(router));
    
while (true)
{
    router.Current.Update();
    while (Console.KeyAvailable == false)
    {
        if(router.Current.IsDirty) router.Current.Update();
        Thread.Sleep(20);
    }

    var key = Console.ReadKey(true);
    if (key.Key is ConsoleKey.Escape) break;
    router.Current.OnKey(key);
}