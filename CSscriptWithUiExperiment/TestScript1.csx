#r ".\bin\Debug\Terminal.Gui.dll"
#load ".\SumPlugin\SumUI.csx"
#load ".\MultiplyPlugin\MultiplyUI.csx"
using Terminal.Gui;

Application.Init();


Window GetWindow(Toplevel top)
{
    return new Window(new Rect(0, 1, top.Frame.Width, top.Frame.Height - 1), "MyApp"); ;
}

Window pluginListWindow = new Window("Available plugins");

pluginListWindow.Add(
    new Label(3, 2, "Choose plugin to load"),
    new Button(3, 4, sumPlugin.Name)
    {
        Clicked = () => setView(sumPlugin.View)
    },
    new Button(3, 6, multiplyPlugin.Name)
    {
        Clicked = () => setView(multiplyPlugin.View)
    }
);

var top = Application.Top;

var menu = new MenuBar(new MenuBarItem[] {
            new MenuBarItem ("_File", new MenuItem [] {
                new MenuItem ("_Quit", "", () => { top.Running = false; })
            }),
            new MenuBarItem ("_Plugin", new MenuItem [] {
                 new MenuItem ("_Back to list", "", () => setView(pluginListWindow))
            }),
        });

top.Add(menu);

var win = GetWindow(top);

top.Add(win);

void setView(Window view)
{
    top.Remove(win);

    win = GetWindow(top);

    top.Add(win);

    win.Add(view);
}

// all wievs have to be added before first run appraently :-(
win.Add(sumPlugin.View);
win.Add(multiplyPlugin.View);
win.Add(pluginListWindow);

Application.Run();


