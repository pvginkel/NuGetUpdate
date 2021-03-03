using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace NuGetUpdate.Shared
{
    public class PackageDownloader : IDisposable
    {
        private readonly string _site;
        private readonly string _siteUserName;
        private readonly string _sitePassword;
        private readonly string _package;
        private Thread _thread;
        private bool _aborted;
        private readonly byte[] _buffer = new byte[4096];
        private bool _disposed;

        public string Status { get; private set; }
        public double Progress { get; private set; }
        public string PackageCode { get; private set; }

        public event DownloadCompletedEventHandler DownloadCompleted;

        protected virtual void OnDownloadCompleted(DownloadCompletedEventArgs e)
        {
            var ev = DownloadCompleted;
            if (ev != null)
                ev(this, e);
        }

        public event DownloadFailedEventHandler DownloadFailed;

        protected virtual void OnDownloadFailed(DownloadFailedEventArgs e)
        {
            var ev = DownloadFailed;
            if (ev != null)
                ev(this, e);
        }

        public PackageDownloader(string site, string siteUserName, string sitePassword, string package)
        {
            if (site == null)
                throw new ArgumentNullException("site");
            if (package == null)
                throw new ArgumentNullException("package");

            _site = site;
            _siteUserName = siteUserName;
            _sitePassword = sitePassword;
            _package = package;
        }

        public void Start()
        {
            if (_thread != null)
                throw new InvalidOperationException();

            _thread = new Thread(ThreadProc)
            {
                IsBackground = true
            };

            _thread.Start();
        }

        private void ThreadProc()
        {
            string downloadTarget = Path.GetTempFileName();
            string downloadFolder = null;
            bool success = false;

            try
            {
                ResetProgress();

                string packagePath;

                if (Path.IsPathRooted(_site))
                {
                    packagePath = FindPackageFile();
                }
                else
                {
                    PerformDownload(downloadTarget);

                    packagePath = downloadTarget;
                }

                ResetProgress();

                downloadFolder = PerformExtract(packagePath);

                GetPackageCode(downloadFolder);

                Util.ValidateDownloadFolder(downloadFolder);

                success = true;

                OnDownloadCompleted(new DownloadCompletedEventArgs(downloadFolder));
            }
            catch (AbortedException)
            {
            }
            catch (Exception ex)
            {
                OnDownloadFailed(new DownloadFailedEventArgs(ex));
            }
            finally
            {
                SEH.SinkExceptions(() => File.Delete(downloadTarget));

                if (!success && downloadFolder != null)
                    SEH.SinkExceptions(() => Directory.Delete(downloadFolder, true));
            }
        }

        private void GetPackageCode(string downloadFolder)
        {
            var files = Directory.GetFiles(downloadFolder, "*.nuspec");

            if (files.Length != 1)
                return;

            var document = new XmlDocument();

            document.Load(files[0]);

            if (!Constants.TryGetDetails(document, out string packageCode, out _))
                return;

            PackageCode = packageCode;
        }

        private string FindPackageFile()
        {
            var files = Directory.GetFiles(_site, _package + ".*.nupkg");

            if (files.Length != 1)
                throw new NuGetUpdateException(UILabels.PackageNotFound);

            return files[0];
        }

        private void ResetProgress()
        {
            Status = null;
            Progress = 0;
        }

        private void PerformDownload(string downloadTarget)
        {
            Status = String.Format(UILabels.ConnectingTo, new Uri(_site).Host);

            var uri = ResolvePackageUri();
            if (uri == null)
                throw new NuGetUpdateException("Cannot find latest version");

            var request = (HttpWebRequest)WebRequest.Create(uri);

            if (!String.IsNullOrEmpty(_siteUserName))
                request.Credentials = new NetworkCredential(_siteUserName, _sitePassword);

            int totalRead = 0;

            var stopwatch = new Stopwatch();

            using (var response = request.GetResponse())
            using (var source = response.GetResponseStream())
            using (var destination = File.Create(downloadTarget))
            {
                stopwatch.Start();

                while (true)
                {
                    CheckAborted();

                    int read = source.Read(_buffer, 0, _buffer.Length);

                    CheckAborted();

                    if (read == 0)
                        break;

                    destination.Write(_buffer, 0, read);

                    totalRead += read;

                    if (response.ContentLength <= 0)
                        Progress = double.NaN;
                    else
                        Progress = (double)totalRead / response.ContentLength;

                    Status = String.Format(
                        UILabels.Downloaded,
                        Util.FormatSize(totalRead),
                        Util.FormatSize((int)(totalRead / stopwatch.Elapsed.TotalSeconds))
                    );
                }
            }
        }

        private string ResolvePackageUri()
        {
            string url = String.Format(
                "{0}/FindPackagesById()?$filter={1}&$orderby=Version%20desc&$top=1&id={2}",
                _site.TrimEnd('/'),
                Uri.EscapeDataString("IsLatestVersion eq true and IsPrerelease eq false"),
                Uri.EscapeDataString("'" + _package + "'")
            );

            string content;

            using (var client = new WebClient())
            {
                if (!String.IsNullOrEmpty(_siteUserName))
                    client.Credentials = new NetworkCredential(_siteUserName, _sitePassword);

                content = client.DownloadString(url);
            }

            var document = new XmlDocument();

            document.LoadXml(content);

            var entry = document.GetElementsByTagName("entry", Constants.AtomNs);

            if (entry.Count == 0)
                return null;

            var contentElements = ((XmlElement)entry[0]).GetElementsByTagName(
                "content",
                Constants.AtomNs
            );

            XmlElement contentElement = null;

            switch (contentElements.Count)
            {
                case 0:
                    return null;
                case 1:
                    contentElement = (XmlElement)contentElements[0];
                    break;
                default:
                    foreach (XmlElement item in contentElements)
                    {
                        var typeAttribute = item.Attributes["type"];
                        if (typeAttribute != null && String.Equals(typeAttribute.Value, "application/zip", StringComparison.OrdinalIgnoreCase))
                        {
                            contentElement = item;
                            break;
                        }
                    }
                    break;
            }

            if (contentElement == null)
                return null;

            var attribute = contentElements[0].Attributes["src"];
            if (attribute != null)
                return attribute.Value;

            return null;
        }

        private string PerformExtract(string packagePath)
        {
            string downloadFolder = CreateDownloadFolder();

            var extractNameTransform = (INameTransform)new WindowsNameTransform(downloadFolder);

            using (var stream = File.OpenRead(packagePath))
            using (var zipFile = new ZipFile(stream))
            {
                long processedSize = 0;
                long totalSize = 0;

                CheckAborted();

                foreach (ZipEntry entry in zipFile)
                {
                    totalSize += entry.Size;
                }

                foreach (ZipEntry entry in zipFile)
                {
                    CheckAborted();

                    string entryName = Escaping.UriDecode(entry.Name);

                    if (!entry.IsFile)
                        continue;

                    Status = String.Format(UILabels.Extracting, entryName);
                    Progress = (double)processedSize / totalSize;

                    ExtractEntry(zipFile, entry, entryName, extractNameTransform);

                    processedSize += entry.Size;
                }
            }

            return downloadFolder;
        }

        private void ExtractEntry(ZipFile zipFile, ZipEntry entry, string entryName, INameTransform extractNameTransform)
        {
            if (!entry.IsCompressionMethodSupported())
                return;

            if (entry.IsFile)
                entryName = extractNameTransform.TransformFile(entryName);
            else if (entry.IsDirectory)
                entryName = extractNameTransform.TransformDirectory(entryName);

            if (!String.IsNullOrEmpty(entryName))
            {
                Directory.CreateDirectory(
                    !entry.IsDirectory ? Path.GetDirectoryName(Path.GetFullPath(entryName)) : entryName
                );

                if (entry.IsFile)
                    ExtractFileEntry(zipFile, entry, entryName);
            }
        }

        private void ExtractFileEntry(ZipFile zipFile, ZipEntry entry, string targetName)
        {
            using (FileStream fileStream = File.Create(targetName))
            {
                StreamUtils.Copy(zipFile.GetInputStream(entry), fileStream, _buffer);
            }
        }

        private string CreateDownloadFolder()
        {
            for (int i = 1; ; i++)
            {
                string downloadFolder = Path.Combine(
                    Path.GetTempPath(),
                    "ngu~" + i
                );

                if (Directory.Exists(downloadFolder))
                    continue;

                Directory.CreateDirectory(downloadFolder);

                return downloadFolder;
            }
        }

        private void CheckAborted()
        {
            if (_aborted)
                throw new AbortedException();
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_thread != null)
                {
                    _aborted = true;

                    _thread.Join(TimeSpan.FromSeconds(3));
                    _thread = null;
                }

                _disposed = true;
            }
        }
    }
}
