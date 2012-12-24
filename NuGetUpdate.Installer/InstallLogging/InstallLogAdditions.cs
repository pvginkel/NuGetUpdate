using System;
using System.Collections.Generic;
using System.Text;

namespace NuGetUpdate.Installer.InstallLogging
{
    partial class InstallLogCreateFile
    {
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            var other = obj as InstallLogCreateFile;

            return
                other != null &&
                String.Equals(Path, other.Path, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return Path != null ? Path.GetHashCode() : 0;
        }
    }

    partial class InstallLogCreateDirectory
    {
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            var other = obj as InstallLogCreateDirectory;

            return
                other != null &&
                String.Equals(Path, other.Path, StringComparison.OrdinalIgnoreCase) &&
                Force == other.Force;
        }

        public override int GetHashCode()
        {
            return
                (Path != null ? Path.GetHashCode() : 0) * 31 +
                Force.GetHashCode();
        }
    }
}
