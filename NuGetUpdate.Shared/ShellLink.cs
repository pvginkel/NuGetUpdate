using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;
using System.Text;

// Taken from http://vbaccelerator.com/home/NET/Code/Libraries/Shell_Projects/Creating_and_Modifying_Shortcuts/article.asp

namespace NuGetUpdate.Shared
{
    public sealed class ShellLink : IDisposable
    {
        private NativeMethods.IShellLink _link;
        private NativeMethods.IPropertyStore _propertyStore;
        private bool _disposed;

        public ShellLink()
        {
            _link = (NativeMethods.IShellLink)new NativeMethods.CShellLink();

            // Ignore exceptions from the cast. Windows XP doesn't support this
            // interface. Result is that _propertyStore will be null, which we
            // support.

            _propertyStore = _link as NativeMethods.IPropertyStore;
        }

        public ShellLink(string linkFile)
            : this()
        {
            Open(linkFile);
        }

        ~ShellLink()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (_propertyStore != null)
                {
                    Marshal.FinalReleaseComObject(_propertyStore);
                    _propertyStore = null;
                }

                if (_link != null)
                {
                    Marshal.FinalReleaseComObject(_link);
                    _link = null;
                }

                _disposed = true;
            }
        }

        public string ShortcutFile { get; set; }

        public string IconPath
        {
            get
            {
                StringBuilder iconPath = new StringBuilder(260, 260);
                int iconIndex;
                _link.GetIconLocation(iconPath, iconPath.Capacity, out iconIndex);
                return iconPath.ToString();
            }
            set
            {
                StringBuilder iconPath = new StringBuilder(260, 260);
                int iconIndex;
                _link.GetIconLocation(iconPath, iconPath.Capacity, out iconIndex);
                _link.SetIconLocation(value, iconIndex);
            }
        }

        public int IconIndex
        {
            get
            {
                StringBuilder iconPath = new StringBuilder(260, 260);
                int iconIndex;
                _link.GetIconLocation(iconPath, iconPath.Capacity, out iconIndex);
                return iconIndex;
            }
            set
            {
                StringBuilder iconPath = new StringBuilder(260, 260);
                int iconIndex;
                _link.GetIconLocation(iconPath, iconPath.Capacity, out iconIndex);
                _link.SetIconLocation(iconPath.ToString(), value);
            }
        }

        public string Target
        {
            get
            {
                StringBuilder target = new StringBuilder(260, 260);
                NativeMethods._WIN32_FIND_DATAW fd = new NativeMethods._WIN32_FIND_DATAW();
                _link.GetPath(target, target.Capacity, ref fd, (uint)NativeMethods.EShellLinkGP.SLGP_UNCPRIORITY);
                return target.ToString();
            }
            set
            {
                _link.SetPath(value);
            }
        }

        public string WorkingDirectory
        {
            get
            {
                StringBuilder path = new StringBuilder(260, 260);
                _link.GetWorkingDirectory(path, path.Capacity);
                return path.ToString();
            }
            set
            {
                _link.SetWorkingDirectory(value);
            }
        }

        public string Description
        {
            get
            {
                StringBuilder description = new StringBuilder(1024, 1024);
                _link.GetDescription(description, description.Capacity);
                return description.ToString();
            }
            set
            {
                _link.SetDescription(value);
            }
        }

        public string Arguments
        {
            get
            {
                StringBuilder arguments = new StringBuilder(260, 260);
                _link.GetArguments(arguments, arguments.Capacity);
                return arguments.ToString();
            }
            set
            {
                _link.SetArguments(value);
            }
        }

        [CLSCompliant(false)]
        public LinkDisplayMode DisplayMode
        {
            get
            {
                uint cmd;
                _link.GetShowCmd(out cmd);
                return (LinkDisplayMode)cmd;
            }
            set
            {
                _link.SetShowCmd((uint)value);
            }
        }

        public void Save()
        {
            Save(ShortcutFile);
        }

        public void Save(string linkFile)
        {
            // Save the object to disk
            ((NativeMethods.IPersistFile)_link).Save(linkFile, true);
            ShortcutFile = linkFile;
        }

        public void Open(string linkFile)
        {
            Open(linkFile,
                IntPtr.Zero,
                (ShellLinkResolveType)(NativeMethods.SLR_ANY_MATCH | NativeMethods.SLR_NO_UI),
                1);
        }

        [CLSCompliant(false)]
        public void Open(string linkFile, IntPtr hWnd, ShellLinkResolveType resolveFlags)
        {
            Open(linkFile,
                hWnd,
                resolveFlags,
                1);
        }

        [CLSCompliant(false)]
        public void Open(string linkFile, IntPtr hWnd, ShellLinkResolveType resolveFlags, ushort timeout)
        {
            uint flags;

            if (((uint)resolveFlags & NativeMethods.SLR_NO_UI)
                == NativeMethods.SLR_NO_UI)
            {
                flags = (uint)((int)resolveFlags | (timeout << 16));
            }
            else
            {
                flags = (uint)resolveFlags;
            }

            ((NativeMethods.IPersistFile)_link).Load(linkFile, 0); //STGM_DIRECT)
            _link.Resolve(hWnd, flags);
            ShortcutFile = linkFile;
        }

        public string GetPropertyValue(PropertyStoreProperty property)
        {
            if (_propertyStore == null)
                return null;

            using (var propertyValue = new NativeMethods.PROPVARIANT())
            {
                _propertyStore.GetValue(NativeMethods.GetPkey(property), propertyValue);

                return propertyValue.GetValue();
            }
        }

        public void SetPropertyValue(PropertyStoreProperty property, string value)
        {
            if (_propertyStore != null)
            {
                using (var propertyValue = new NativeMethods.PROPVARIANT())
                {
                    propertyValue.SetValue(value);

                    _propertyStore.SetValue(NativeMethods.GetPkey(property), propertyValue);
                }
            }
        }
    }

    public enum PropertyStoreProperty
    {
        Title,
        AppUserModel_ID,
        AppUserModel_IsDestListSeparator,
        AppUserModel_RelaunchCommand,
        AppUserModel_RelaunchDisplayNameResource,
        AppUserModel_RelaunchIconResource
    }
}
