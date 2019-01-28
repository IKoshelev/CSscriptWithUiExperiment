using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Terminal.Gui;
using Testbed.Certificates;
using static Designer.TerminalGuiDSL;

namespace Testbed
{
    class Program
    {
        static void Main(string[] args)
        {
            Test();
        }

        static void Test()
        {
            Application.Init();
            var top = Application.Top;

            CertificatesVM certificatesVM = new CertificatesVM();
           

            var win = Window("Certificate files util",
                      w => { w.Width = Dim.Fill() - 1; w.Height = Dim.Fill() - 1; }

                            , Button(" Scan folder",
                            b =>
                            {
                                b.X = 0; b.Y = 0;
                                b.Clicked = () =>
                                {
                                    certificatesVM.ScanCurrentPath(true);
                                };
                            })

                            , Button("Import certificate by index",
                            b =>
                            {
                                b.X = 0; b.Y = 1;
                                b.Clicked = () =>
                                {
                                    Dialog dialog = CreateCertificateImportDialog(top, certificatesVM);
                                    Application.Run(dialog);
                                };
                            })

                            , TextField(
                            f =>
                            {
                                f.X = 18; f.Y = 0; f.Width = Dim.Fill();
                                BindTwoWay(f, el => el.Text,
                                     certificatesVM, vm => vm.ScanPath,
                                     (el, onChanged) => el.Changed += (o, e) => onChanged(((TextField)o).Text.ToString())
                                    );
                            })

                            , FrameView("Certificates",
                            f => { f.Y = 2; f.Width = Dim.Fill(); f.Height = Dim.Fill(); }

                                , TextView(
                                tw => BindOneWay(certificatesVM, vm => vm.CertificatesListText, 
                                                (newText) => tw.Text = newText))
                            )
                );

            top.Add(win);

            Application.Run();
        }

        private static Dialog CreateCertificateImportDialog(Toplevel top, CertificatesVM certificatesVM)
        {
            var storeNames = Enum.GetValues(typeof(StoreName))
                             .Cast<StoreName>()
                             .ToArray();

            var selectedIndexOfStore = 4; //me

            var storeNameOptions = storeNames
                                    .Select(x => x.ToString())
                                    .ToArray();

            var storeLocations = Enum.GetValues(typeof(StoreLocation))
                        .Cast<StoreLocation>()
                        .ToArray();

            var storeLocationOptions = storeLocations
                                            .Select(x => x.ToString())
                                            .ToArray();

            var selectedIndexOfLocation = 1;

            int? selectedIndexOfCertificate = 0;

            var buttons = new[]
            {
                Button("Import", b => b.Clicked = () => 
                {
                    if(selectedIndexOfCertificate.HasValue == false
                        || certificatesVM.Certificates.Count <= selectedIndexOfCertificate
                        || certificatesVM.Certificates[selectedIndexOfCertificate.Value].Exception != null)
                    {
                        MessageBox.ErrorQuery(34, 6, "Error", "Chosen certificate invalid", "Ok");
                        return;
                    }

                    var cert = certificatesVM.Certificates[selectedIndexOfCertificate.Value];

                    certificatesVM.ImportCertificate(
                        cert.Cert[0], 
                        storeNames[selectedIndexOfStore], 
                        storeLocations[selectedIndexOfLocation]);

                    MessageBox.Query(34, 6, "Success", "Import successful", "Ok");

                    Application.RequestStop();
                                     
                }),
                Button("Cancel", b => b.Clicked = () => Application.RequestStop())
            };

            TextView certificateInfoTextView = null;

            void updateSelectedIndexOfCertificate(string text)
            {
                if (int.TryParse(text, out var index))
                {
                    selectedIndexOfCertificate = index;
                }
                else
                {
                    selectedIndexOfCertificate = null;
                }

                if(selectedIndexOfCertificate.HasValue == false
                    || certificatesVM.Certificates.Count <= selectedIndexOfCertificate)
                {
                    certificateInfoTextView.Text = 
                        $"Certificate {selectedIndexOfCertificate} not found. "
                        + "\nPlease scan a folder for certificates on previous page "
                        + "and choose one by its index.";
                    return;
                }

                var cert = certificatesVM.Certificates[selectedIndexOfCertificate.Value];

                var descr = CertificatesVM.GetCertificateDescription(cert);

                certificateInfoTextView.Text = descr;
            }

            var dialog =

                Dialog("Import certificate", top.Frame.Width, top.Frame.Height, buttons
                , d => { }

                      , Label("Certificate index to impot: ",
                                l => { l.X = 1; l.Y = 1; })

                     , TextField(
                            f => { f.X = 32; f.Y = 1; f.Width = 3;
                                f.Text = selectedIndexOfCertificate.ToString();
                                f.Changed += (o, e) => updateSelectedIndexOfCertificate(((TextField)o).Text.ToString());
                            })

                     , FrameView("Selected certificate",
                            fw => { fw.X = 0; fw.Y = 2; fw.Width = Dim.Fill(); fw.Height = 5; }

                                , TextView(out certificateInfoTextView,
                                          tw => { tw.Height = 3; tw.Width = Dim.Fill();
                                                  tw.Text = ""; }))

                     , Label("Store name:",
                            l => { l.X = 3; l.Y = 7; })

                     , RadioGroup(storeNameOptions,
                            r => {
                                r.X = 3; r.Y = 8;
                                r.Selected = selectedIndexOfStore;
                                r.SelectionChanged = (newStoreIndex) => selectedIndexOfStore = newStoreIndex;
                            })

                     , Label("Store lcation:",
                            l => { l.X = 32; l.Y = 7; })

                     , RadioGroup(storeLocationOptions,
                          r => {
                              r.X = 32; r.Y = 8;
                              r.Selected = selectedIndexOfLocation;
                              r.SelectionChanged = (newSelectedIndexLocation) => selectedIndexOfLocation = newSelectedIndexLocation;
                          })
            );

            updateSelectedIndexOfCertificate(selectedIndexOfCertificate.ToString());

            return dialog;
        }
    }
}