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
                    "{0}/GetUpdates()?packageIds='{1}'&versions='{2}'&includePrerelease=false&includeAllVersions=false",
                    metadata.NuGetSite.TrimEnd('/'),
                    Uri.EscapeDataString(metadata.PackageCode),
                    Uri.EscapeDataString(metadata.InstalledVersion)
                );

                try
                {
                    string content;

                    using (var client = new WebClient())
                    {
                        content = client.DownloadString(url);
                    }

                    var document = new XmlDocument();

                    document.LoadXml(content);

                    var entry = document.GetElementsByTagName("entry", Constants.AtomNs);

                    if (entry.Count == 0)
                        return false;

                    if (metadata.AttemptedVersion != null)
                    {
                        if (String.Equals(
                            metadata.AttemptedVersion,
                            GetVersionFromEntry((XmlElement)entry[0]),
                            StringComparison.OrdinalIgnoreCase
                        ))
                            return false;
                    }

                    return true;
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
            if (restartArguments == null)
                restartArguments = new string[0];

            using (var metadata = Metadata.Open(packageCode))
            {
                string nguPath = Path.Combine(
                    Path.Combine(metadata.InstallPath, Constants.BinFolder),
                    Constants.NuGetUpdateFileName
                );

                if (!File.Exists(nguPath))
                    throw new NuGetUpdateException(UILabels.NuGetUpdateNotFound);

                AllowSetForegroundWindow(ASFW_ANY);

                using (Process.Start(new ProcessStartInfo
                {
                    FileName = nguPath,
                    Arguments = String.Format(
                        "-du -p {0} -- {1}",
                        Escaping.ShellEncode(packageCode),
                        Escaping.ShellEncode(restartArguments)
                    ),
                    UseShellExecute = false
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
