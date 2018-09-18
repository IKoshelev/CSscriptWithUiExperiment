#r ".\bin\Debug\Terminal.Gui.dll"
#load ".\SumPlugin\SumUI.csx"
#load ".\MultiplyPlugin\MultiplyUI.csx"
using Terminal.Gui;



var registeredPlugins = new (int position, ScriptPlugin plugin)[]
{
    (position: 1, plugin: sumPlugin ),
    (position: 2, plugin: multiplyPlugin )
};

Application.Init();
var top = Application.Top;

var win = new Window(new Rect(0, 1, top.Frame.Width, top.Frame.Height - 1), "MyApp");

top.Add(win);

Window pluginListWindow = new Window("Available plugins");
pluginListWindow.Add(new Label(3, 1, "Choose plugin to load"));

var pluginActivationButtons = registeredPlugins
                                    .Select(x =>
                                    {
                                        return new Button(3, 1 + (x.position * 2), x.plugin.Name)
                                        {
                                            Clicked = () =>
                                            {
                                                x.plugin.OnBeforeActivate(new Rect(0, 0, win.Frame.Width - 2, win.Frame.Height - 2));
                                                setView(x.plugin.View);
                                                x.plugin.OnAfterActivate();
                                            }
                                        };
                                    })
                                    .ToArray();

pluginListWindow.Add(pluginActivationButtons);

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

void setView(Window view)
{
    var subView = win.Subviews[0];

    //this seems to throw with Terminal.Gui 0.16 (probably RemoveAll fix not yet implemented)
    //please use Terminal.Gui built from source
    subView.RemoveAll();

    win.Add(view);
}

win.Add(pluginListWindow);
pluginListWindow.FocusFirst();

Application.Run();

