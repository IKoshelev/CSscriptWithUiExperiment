#r ".\bin\Debug\Terminal.Gui.dll"
#load ".\SumPlugin\SumUI.csx"
#load ".\MultiplyPlugin\MultiplyUI.csx"
using Terminal.Gui;

Application.Init();
var top = Application.Top;

Window pluginListWindow = new Window("Available plugins");
pluginListWindow.Add(
    new Label(3, 2, "Choose plugin to load"),
    new Button(3, 4, sumPlugin.Name)
    {
        Clicked = () =>
        {           
            setView(sumPlugin.View);
            sumPlugin.OnActivate();
        }
    },
    new Button(3, 6, multiplyPlugin.Name)
    {
        Clicked = () =>
        {           
            setView(multiplyPlugin.View);
            multiplyPlugin.OnActivate();
        }
    }
);

var menu = new MenuBar(new MenuBarItem[] {
            new MenuBarItem ("_File", new MenuItem [] {
                new MenuItem ("_Quit", "", () => { top.Running = false; })
            }),
            new MenuBarItem ("_Plugin", new MenuItem [] {
                 new MenuItem ("_Back to list", "", () =>
                 {
                    setView(pluginListWindow);
                    pluginListWindow.FocusFirst();
                 })
            }),
        });

top.Add(menu);

var win = new Window(new Rect(0, 1, top.Frame.Width, top.Frame.Height - 1), "MyApp");

top.Add(win);

void setView(Window view)
{
    var subView = win.Subviews[0];

    //this seems to throw with Terminal.Gui 0.16 (probably RemoveAll fix not yet implemented)
    //please use Terminal.Gui built from source
    subView.RemoveAll();

    win.Add(view);

}

win.Add(sumPlugin.View);
win.Add(multiplyPlugin.View);
win.Add(pluginListWindow);
pluginListWindow.FocusFirst();

Application.Run();

