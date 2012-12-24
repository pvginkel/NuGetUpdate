using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace NuGetUpdate.Shared
{
    public static class Util
    {
        private static readonly Regex _folderPathRe = new Regex("%([^%]+)%", RegexOptions.Compiled);

        private static readonly Dictionary<string, int> _csidls = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            { "ADMINTOOLS", 0x30 },
            { "ALTSTARTUP", 0x1d },
            { "APPDATA", 0x1a },
            { "BITBUCKET", 0xa },
            { "CDBURN_AREA", 0x3b },
            { "COMMON_ADMINTOOLS", 0x2f },
            { "COMMON_ALTSTARTUP", 0x1e },
            { "COMMON_APPDATA", 0x23 },
            { "COMMON_DESKTOPDIRECTORY", 0x19 },
            { "COMMON_DOCUMENTS", 0x2e },
            { "COMMON_FAVORITES", 0x1f },
            { "COMMON_MUSIC", 0x35 },
            { "COMMON_OEM_LINKS", 0x3a },
            { "COMMON_PICTURES", 0x36 },
            { "COMMON_PROGRAMS", 0x17 },
            { "COMMON_STARTMENU", 0x16 },
            { "COMMON_STARTUP", 0x18 },
            { "COMMON_TEMPLATES", 0x2d },
            { "COMMON_VIDEO", 0x37 },
            { "COMPUTERSNEARME", 0x3d },
            { "CONNECTIONS", 0x31 },
            { "CONTROLS", 0x3 },
            { "COOKIES", 0x21 },
            { "DESKTOP", 0x0 },
            { "DESKTOPDIRECTORY", 0x10 },
            { "DRIVES", 0x11 },
            { "FAVORITES", 0x6 },
            { "FONTS", 0x14 },
            { "HISTORY", 0x22 },
            { "INTERNET", 0x1 },
            { "INTERNET_CACHE", 0x20 },
            { "LOCAL_APPDATA", 0x1c },
            { "MYDOCUMENTS", 0x5 },
            { "MYMUSIC", 0xd },
            { "MYPICTURES", 0x27 },
            { "MYVIDEO", 0xe },
            { "NETHOOD", 0x13 },
            { "NETWORK", 0x12 },
            { "PERSONAL", 0x5 },
            { "PRINTERS", 0x4 },
            { "PRINTHOOD", 0x1b },
            { "PROFILE", 0x28 },
            { "PROGRAM_FILES", 0x26 },
            { "PROGRAM_FILES_COMMON", 0x2b },
            { "PROGRAM_FILES_COMMONX86", 0x2c },
            { "PROGRAM_FILESX86", 0x2a },
            { "PROGRAMS", 0x2 },
            { "RECENT", 0x8 },
            { "RESOURCES", 0x38 },
            { "RESOURCES_LOCALIZED", 0x39 },
            { "SENDTO", 0x9 },
            { "STARTMENU", 0xb },
            { "STARTUP", 0x7 },
            { "SYSTEM", 0x25 },
            { "SYSTEMX86", 0x29 },
            { "TEMPLATES", 0x15 },
            { "WINDOWS", 0x24 }
        };

        public static void ValidateDownloadFolder(string downloadFolder)
        {
            if (downloadFolder == null)
                throw new ArgumentNullException("downloadFolder");

            string toolsFolder = Path.Combine(downloadFolder, Constants.ToolsFolder);

            if (
                !File.Exists(Path.Combine(toolsFolder, Constants.ScriptFileName))
#if !DEBUG
                || !File.Exists(Path.Combine(toolsFolder, Constants.NuGetUpdateFileName))
#endif
            )
                throw new NuGetUpdateException(UILabels.InvalidPackage);

            var files = Directory.GetFiles(downloadFolder, "*.nuspec");

            if (files.Length != 1)
                throw new NuGetUpdateException(UILabels.InvalidPackage);
        }

        public static string ExpandFolderPath(IWin32Window owner, string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            return _folderPathRe.Replace(path, m => PerformReplace(owner, m));
        }

        private static string PerformReplace(IWin32Window owner, Match match)
        {
            int csidl;

            if (!_csidls.TryGetValue(match.Groups[1].Value, out csidl))
                return match.Value;

            var sb = new StringBuilder(300);

            int result = NativeMethods.SHGetFolderPath(
                owner.Handle,
                csidl,
                IntPtr.Zero,
                0,
                sb
            );

            if (result != 0)
                return match.Value;

            return sb.ToString();
        }

        public static string FormatSize(double size)
        {
            double value = size;

            if (value > 1024)
            {
                value /= 1024;

                if (value > 1024)
                {
                    value /= 1024;

                    if (value > 1024)
                    {
                        return String.Format(UILabels.SizeGigaBytes, value / 1024);
                    }
                    else
                    {
                        return String.Format(UILabels.SizeMegaBytes, value);
                    }
                }
                else
                {
                    return String.Format(UILabels.SizeKiloBytes, value);
                }
            }
            else
            {
                return String.Format(UILabels.SizeBytes, value);
            }
        }
    }
}
