using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Testbed.Certificates
{
    public interface ICertificateOperations
    {
        CertificateImportResult OpenCertificate(string certFullPath);
        CertificateImportResult[] GetCertificates(string searchStartPath, bool searchSubfolders = false);
        void ImportCertificate(
            X509Certificate2 cert, 
            StoreName storeName = StoreName.Root, 
            StoreLocation storeLocation = StoreLocation.LocalMachine);
    }

    public class CertificateImportResult
    {
        public CertificateImportResult(string path, X509Certificate2Collection cert)
        {
            Path = path;
            Cert = cert;
        }

        public CertificateImportResult(string path, Exception exception)
        {
            Path = path;
            Exception = exception;
        }

        public string Path { get; private set; }
        public X509Certificate2Collection Cert { get; private set; }
        public Exception Exception { get; private set; }
    }

    public class CertificateOperations : ICertificateOperations
    {
        public void ImportCertificate(
                            X509Certificate2 cert, 
                            StoreName storeName = StoreName.My, 
                            StoreLocation storeLocation = StoreLocation.LocalMachine)
        {
            using (X509Store store = new X509Store(storeName, storeLocation))
            {
                store.Open(OpenFlags.ReadWrite);
                store.Add(cert);
                store.Close();
            }          
        }

        public CertificateImportResult[] GetCertificates(string searchStartPath, bool searchSubfolders = false)
        {
            var searchOption = searchSubfolders
                                        ? SearchOption.AllDirectories
                                        : SearchOption.TopDirectoryOnly;

            var certificatePaths = Directory.EnumerateFiles(searchStartPath, "*.pfx", searchOption)
                                    .Union(Directory.EnumerateFiles(searchStartPath, "*.p12", searchOption));

            var certificates = certificatePaths
                                    .Select(x => OpenCertificate(x))
                                    .ToArray();

            return certificates;
        }

        public CertificateImportResult OpenCertificate(string certFullPath)
        {
            var candidatePasswords = GetCandidatePasswords(certFullPath);

            foreach (var passwrod in candidatePasswords)
            {
                try
                {
                    X509Certificate2Collection cert = new X509Certificate2Collection();
                    cert.Import(certFullPath, passwrod, X509KeyStorageFlags.PersistKeySet);
                    return new CertificateImportResult(certFullPath, cert);
                }
                catch { }
            }

            try
            {
                X509Certificate2Collection certNoPassword = new X509Certificate2Collection();
                certNoPassword.Import(certFullPath);
                return new CertificateImportResult(certFullPath, certNoPassword);
            }
            catch(Exception ex)
            {
                return new CertificateImportResult(certFullPath, ex);
            }
        }

        public static int MaxPasswordLength { get; set; } = 270;
        private Dictionary<string, string[]> passwordsCache = new Dictionary<string, string[]>();  
        private string[] GetCandidatePasswords(string certFullPath)
        {
            string folderPath = Path.GetDirectoryName(certFullPath);

            if (passwordsCache.ContainsKey(folderPath))
            {
                return passwordsCache[folderPath];
            }
         
            var smallFiles = Directory.EnumerateFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly);

            var passwrodsFromFiles = smallFiles
                                        .Where(x => new FileInfo(x).Length <= MaxPasswordLength)
                                        .Select(x => File.ReadAllText(x));

            var emptyStringPassword = new string[] { "" };

            var passwords = emptyStringPassword
                                    .Union(passwrodsFromFiles)
                                    .ToArray();

            passwordsCache[folderPath] = passwords;

            return passwords;
        }
    }
}
