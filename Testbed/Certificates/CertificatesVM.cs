using NStack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
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
                                            $"Index: {Certificates.IndexOf(x).ToString()}",
                                            GetCertificateDescription(x),
                                            ""
                                        };
                                });

                return string.Join("\n", certDesc);
            }
        }

        public static string GetCertificateDescription(CertificateImportResult importResult)
        {
            var cert = importResult.Cert?[0];
            var descr = new string[] 
            {
                            importResult.Path,
                            importResult.Exception?.Message ?? cert?.SubjectName?.Name ?? "",
                            cert != null ? $"Valid till: {cert.NotAfter.ToShortDateString()}" : ""
            };
            return string.Join("\n", descr);
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

        CertificateImportResult TryGetCertificateByIndex(int index)
        {
            try
            {
                return Certificates.ElementAt(index);
            }
            catch(IndexOutOfRangeException ex)
            {
                return null;
            }
        }

        public void ImportCertificate(
                                X509Certificate2 cert,
                                StoreName storeName = StoreName.Root,
                                StoreLocation storeLocation = StoreLocation.LocalMachine)
        {
            certificateOperations.ImportCertificate(cert, storeName, storeLocation);
        }

        public void SetDefaultNamesForCertificateFiles()
        {
            foreach(var importResult in Certificates)
            {
                var cert = importResult.Cert?[0];
                var subject = cert?.SubjectName?.Name;

                if (string.IsNullOrWhiteSpace(subject))
                {
                    continue;
                }

                subject = subject
                                .Split(',')
                                .First()
                                .Replace("CN=", "");

                var dir = System.IO.Path.GetDirectoryName(importResult.Path);
                var validDate = cert.NotAfter.ToString("MM-dd-yyyy");

                var oldName = System.IO.Path
                                        .GetFileName(importResult.Path)
                                        .Replace($"{subject};", "")
                                        .Replace($"{validDate};", "");

                var newName = $"{subject};{validDate};{oldName}";
                var newFullPath = System.IO.Path.Combine(dir, newName);

                System.IO.File.Copy(importResult.Path, newFullPath);
            }
        }

        public void CopyCertificateInoToClipboard()
        {
            var val = CertificatesListText.ToString();

            val = val.Replace("\n", "`n");

            var args = $"\"{val}\" | clip";

            var escapedArgs = args
                                .Replace("\"", "\\\"");

            string result = Run("powershell", $"/c {escapedArgs}");
        }

        private static string Run(string filename, string arguments)
        {
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = filename,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = false,
                }
            };
            process.Start();
            string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return result;
        }

    }
}
