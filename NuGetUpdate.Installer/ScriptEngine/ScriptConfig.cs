using System;
using System.Collections.Generic;
using System.Text;

namespace NuGetUpdate.Installer.ScriptEngine
{
    public class ScriptConfig
    {
        public string PackageFolder { get; private set; }
        public string PackageCode { get; private set; }
        public string SetupTitle { get; private set; }
        public string PackageVersion { get; private set; }
        public string InstalledVersion { get; private set; }
        public string RestartArguments { get; private set; }

        public ScriptConfig(string packageFolder, string packageCode, string setupTitle, string packageVersion, string installedVersion, string restartArguments)
        {
            if (packageCode == null)
                throw new ArgumentNullException("packageCode");
            if (setupTitle == null)
                throw new ArgumentNullException("setupTitle");

            PackageFolder = packageFolder;
            PackageCode = packageCode;
            SetupTitle = setupTitle;
            PackageVersion = packageVersion;
            InstalledVersion = installedVersion;
            RestartArguments = restartArguments;
        }
    }
}
