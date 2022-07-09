using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;

namespace NuGetUpdate
{
    public static class Update
    {
        public static bool IsUpdateAvailable(string packageCode)
        {
            using (var metadata = Metadata.Open(packageCode))
            {
                string url = String.Format(
                    "{0}/FindPackagesById()?$filter={1}&$orderby=Version%20desc&$top=1&id={2}",
                    metadata.NuGetSite.TrimEnd('/'),
                    Uri.EscapeDataString("IsLatestVersion eq true and IsPrerelease eq false"),
                    Uri.EscapeDataString("'" + metadata.PackageCode + "'")
                );

                try
                {
                    string content;

                    using (var client = new WebClient())
                    {
                        if (!String.IsNullOrEmpty(metadata.NuGetSiteUserName))
                            client.Credentials = new NetworkCredential(metadata.NuGetSiteUserName, metadata.NuGetSitePassword);

                        content = client.DownloadString(url);
                    }

                    var document = new XmlDocument();

                    document.LoadXml(content);

                    var entry = document.GetElementsByTagName("entry", Constants.AtomNs);

                    if (entry.Count == 0)
                        return false;

                    string checkVersion = metadata.AttemptedVersion ?? metadata.InstalledVersion;
                    string availableVersion = GetVersionFromEntry((XmlElement)entry[0]);

                    return !String.Equals(checkVersion, availableVersion, StringComparison.OrdinalIgnoreCase);
                }
                catch (Exception ex)
                {
                    throw new NuGetUpdateException(UILabels.CheckVersionFailed, ex);
                }
            }
        }

        private static string GetVersionFromEntry(XmlElement element)
        {
            var versionElements = element.GetElementsByTagName(
                "Version",
                Constants.DataServicesNs
            );

            if (versionElements.Count == 1)
                return versionElements[0].InnerText;

            return null;
        }

        public static void StartUpdate(string packageCode, params string[] restartArguments)
        {
            StartUpdate(packageCode, UpdateMode.Normal, restartArguments);
        }

        public static void StartUpdate(string packageCode, UpdateMode mode, params string[] restartArguments)
        {
            if (restartArguments == null)
                restartArguments = new string[0];

            using (var metadata = Metadata.Open(packageCode))
            {
                string nguPath = Path.Combine(
                    Path.Combine(metadata.InstallPath, Constants.BinFolder),
                    Constants.NuGetUpdateFileName
                );

                if (!File.Exists(nguPath))
                {
                    nguPath = Path.Combine(
                        metadata.InstallPath,
                        Constants.NuGetUpdateFileName
                    );
                }

                if (!File.Exists(nguPath))
                    throw new NuGetUpdateException(UILabels.NuGetUpdateNotFound);

                AllowSetForegroundWindow(ASFW_ANY);

                string extra = null;
                if (mode == UpdateMode.Silent)
                    extra = "-l";

                using (Process.Start(new ProcessStartInfo
                {
                    FileName = nguPath,
                    Arguments = String.Format(
                        "-du -p {0} {1} -- {2}",
                        Escaping.ShellEncode(packageCode),
                        extra,
                        Escaping.ShellEncode(restartArguments)
                    ),
                    UseShellExecute = false,
                    WorkingDirectory = metadata.InstallPath
                }))
                {
                }
            }
        }

        [DllImport("user32.dll")]
        private static extern bool AllowSetForegroundWindow(int dwProcessId);

        private const int ASFW_ANY = -1;
    }
}
