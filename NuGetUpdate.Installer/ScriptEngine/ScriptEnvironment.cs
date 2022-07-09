using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using NuGetUpdate.Shared;

namespace NuGetUpdate.Installer.ScriptEngine
{
    public class ScriptEnvironment
    {
        public ScriptConfig Config { get; private set; }

        public ScriptEnvironment(ScriptConfig config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            Config = config;
        }

        public string ExpandPath(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            return Util.ExpandFolderPath(path);
        }
    }
}
