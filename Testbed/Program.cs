using System;
using Terminal.Gui;
using Testbed.Certificates;
using static Designer.TerminalGuiDSL;

namespace Testbed
{
    class Program
    {
        static void Main(string[] args)
        {
            Application.Init();
            var top = Application.Top;

            var certificatesVM = new CertificatesVM();

            var dialog = new Dialog("Dialog", top.Frame.Width, top.Frame.Height,
                new Button("Ok") { Clicked = () => Application.RequestStop() },
                new Button("Cancel"));


            var win = Window("MyApp",
                      w => { w.Width = top.Frame.Width; w.Height = top.Frame.Height; }

                            , Button("Scan certs",
                            b => { b.X = 0; b.Y = 0; b.Width = 10;
                                b.Clicked = () => {
                                    certificatesVM.ScanCurrentPath(true);
                                };
                            })

                            , TextField(
                            f => { f.X = 16; f.Y = 0; f.Width = top.Frame.Width - 18;
                                BindTwoWay(f,
                                     el => el.Text,
                                     certificatesVM,
                                     vm => vm.ScanPath,
                                     (el, onChanged) => el.Changed += (o, e) => onChanged(((TextField)o).Text.ToString())
                                    );
                            })

                            , FrameView("Certificates",
                            f => { f.Y = 2; f.Width = top.Frame.Width - 2; f.Height = top.Frame.Height - 4; }

                                , TextView(
                                tw => BindOneWay(certificatesVM, vm => vm.CertificatesListText, (newText) => tw.Text = newText ))
                            )
                );


            top.Add(win);

            Application.Run();         
        }
    }
}
