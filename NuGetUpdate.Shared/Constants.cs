using System;
using System.Collections.Generic;
using System.Text;

#if NGU_LIBRARY
namespace NuGetUpdate
{
    internal static class Constants
#else
namespace NuGetUpdate.Shared
{
    public static class Constants
#endif
    {
        public const string ToolsFolder = "Tools";
        public const string ScriptFileName = "NguScript.xml";
        public const string NuGetUpdateFileName = "ngu.exe";
        public const string ScriptNs = "https://github.com/pvginkel/NuGetUpdate/Script/v1";
        public const string NuSpecNs = "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd";
        public const string AtomNs = "http://www.w3.org/2005/Atom";
        public const string DataServicesNs = "http://schemas.microsoft.com/ado/2007/08/dataservices";
        public const string BinFolder = "Bin";

        public static class ScriptVariables
        {
            public const string TargetPath = "TargetPath";
            public const string StartMenuPath = "StartMenuPath";
            public const string CreateShortcuts = "CreateShortcuts";
            public const string CreateDesktopShortcuts = "CreateDesktopShortcuts";
        }

        public static class Registry
        {
            public const string BaseKey = "NuGet Update";
            public const string PackagesKey = "Packages";
        }
    }
}
