using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;

namespace CSscriptWithUiExperiment
{
    class Program
    {
        static void Main(string[] args)
        {
            CompiledAppTest();
            Console.WriteLine("Project is just to manage NuGet packages. Run .ps1 (or .csx dirctly) files included for demo.");
        }

        public static void CompiledAppTest()
        {
            Application.Init();
            var top = Application.Top;

            // Creates the top-level window to show
            var win = new Window(new Rect(0, 1, top.Frame.Width, top.Frame.Height - 1), "MyApp");
            top.Add(win);

            // Creates a menubar, the item "New" has a help menu.
            var menu = new MenuBar(new MenuBarItem[] {
            new MenuBarItem ("_File", new MenuItem [] {
                new MenuItem ("_New", "Creates new file", () => { }),
                new MenuItem ("_Close", "",() => { }),
                new MenuItem ("_Quit", "", () => { top.Running = false; })
            }),
            new MenuBarItem ("_Edit", new MenuItem [] {
                new MenuItem ("_Copy", "", null),
                new MenuItem ("C_ut", "", null),
                new MenuItem ("_Paste", "", null)
            })
        });
            top.Add(menu);

            var a = new TextField(14, 2, 40, "");
            a.Changed += (e,o) => {
                var b = a.Text;
                
            };

            // Add some controls
            win.Add(
                    new Label(3, 2, "Login: "),
                    a,
                    new Label(3, 4, "Password: "),
                    new TextField(14, 4, 40, "") { Secret = true },
                    new CheckBox(3, 6, "Remember me"),
                    new RadioGroup(3, 8, new[] { "_Personal", "_Company" }),
                    new Button(3, 14, "Ok"),
                    new Button(10, 14, "Cancel"),
                    new Label(3, 18, "Press ESC and 9 to activate the menubar"));

            Application.Run();
        }
    }
}
