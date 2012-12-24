using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using NuGetUpdate.Shared;

namespace NuGetUpdate.Installer.ScriptEngine
{
    public class ScriptEnvironment
    {
        private readonly IWin32Window _owner;

        public ScriptConfig Config { get; private set; }

        public ScriptEnvironment(IWin32Window owner, ScriptConfig config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            _owner = new NativeWindowWrapper(owner);

            Config = config;
        }

        public string ExpandPath(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            return Util.ExpandFolderPath(_owner, path);
        }
    }
}
