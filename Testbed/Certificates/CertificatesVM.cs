using NStack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Testbed.Certificates
{
    public class CertificatesVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyProperyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private ustring scanPath = @"C:\Users\Me\Downloads";
        public ustring ScanPath
        {
            get
            {
                return scanPath;
            }
            set
            {
                scanPath = value;
                NotifyProperyChanged();
            }
        }

        public ObservableCollection<CertificateImportResult> Certificates { get; private set; } 
                                            = new ObservableCollection<CertificateImportResult>();

        public ustring CertificatesListText
        {
            get
            {
                var certDesc = Certificates
                                .SelectMany(x =>
                                {
                                    var cert = x.Cert?[0];
                                    return new string[] 
                                        {
                                            x.Path,
                                            x.Exception?.Message ?? cert?.SubjectName?.Name ?? "",
                                            cert?.NotAfter.ToShortDateString() ?? "",
                                            ""
                                        };
                                });

                return string.Join("\r\n", certDesc);
            }
        }

            ICertificateOperations certificateOperations = new CertificateOperations();

        public void ScanCurrentPath(bool searchSubfolders = false)
        {
            var certs = certificateOperations.GetCertificates(ScanPath.ToString(), searchSubfolders);

            Certificates.Clear();
            foreach(var cert in certs)
            {
                Certificates.Add(cert);
            }

            NotifyProperyChanged(nameof(CertificatesListText));
        }
    }
}
