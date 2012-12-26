using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
#if !NGU_LIBRARY
using System.Collections;
using System.IO;
using System.Xml.Serialization;
using NuGetUpdate.Installer.InstallLogging;
using NuGetUpdate.Shared;
#endif

#if NGU_LIBRARY
namespace NuGetUpdate
{
    internal class Metadata : IDisposable
#else
namespace NuGetUpdate.Installer
{
    public class Metadata : IDisposable
#endif
    {
        private const string InstalledVersionKey = "Installed Version";
        private const string AttemptedVersionKey = "Attempted Version";
        private const string NuGetSiteKey = "NuGet Site";
        private const string InstallPathKey = "Installation Path";
        private const string InstallLogKey = "Installation Log";
        private const string SetupTitleKey = "Setup Title";

        private RegistryKey _key;
        private bool _disposed;

        public static RegistryKey BaseKey
        {
            get
            {
                return Registry.CurrentUser.CreateSubKey(
                    "Software\\" + Constants.Registry.BaseKey + "\\" + Constants.Registry.PackagesKey
                );
            }
        }

        public string PackageCode { get; private set; }

        public string InstalledVersion
        {
            get { return (string)_key.GetValue(InstalledVersionKey); }
            set
            {
                if (String.IsNullOrEmpty(value))
                    _key.DeleteValue(InstalledVersionKey, false);
                else
                    _key.SetValue(InstalledVersionKey, value);
            }
        }

        public string SetupTitle
        {
            get { return (string)_key.GetValue(SetupTitleKey); }
            set
            {
                if (String.IsNullOrEmpty(value))
                    _key.DeleteValue(SetupTitleKey, false);
                else
                    _key.SetValue(SetupTitleKey, value);
            }
        }

        public string AttemptedVersion
        {
            get { return (string)_key.GetValue(AttemptedVersionKey); }
            set
            {
                if (String.IsNullOrEmpty(value))
                    _key.DeleteValue(AttemptedVersionKey, false);
                else
                    _key.SetValue(AttemptedVersionKey, value);
            }
        }

        public string NuGetSite
        {
            get { return (string)_key.GetValue(NuGetSiteKey); }
            set
            {
                if (String.IsNullOrEmpty(value))
                    _key.DeleteValue(NuGetSiteKey, false);
                else
                    _key.SetValue(NuGetSiteKey, value);
            }
        }

        public string InstallPath
        {
            get { return (string)_key.GetValue(InstallPathKey); }
            set
            {
                if (String.IsNullOrEmpty(value))
                    _key.DeleteValue(InstallPathKey, false);
                else
                    _key.SetValue(InstallPathKey, value);
            }
        }

#if !NGU_LIBRARY
        public InstallLog InstallLog
        {
            get
            {
                string value = (string)_key.GetValue(InstallLogKey);

                if (value == null)
                    return null;

                return DeserializeInstallLog(value);
            }
            set
            {
                if (value == null)
                    _key.DeleteValue(InstallLogKey, false);
                else
                    _key.SetValue(InstallLogKey, SerializeInstallLog(value));
            }
        }
#endif

        private Metadata(RegistryKey key, string packageCode)
        {
            PackageCode = packageCode;
            _key = key;
        }

        public static Metadata Open(string packageCode)
        {
            return Open(packageCode, true);
        }

        public static Metadata Open(string packageCode, bool throwWhenMissing)
        {
            return Open(packageCode, throwWhenMissing, false);
        }

        public static Metadata Open(string packageCode, bool throwWhenMissing, bool writable)
        {
            if (packageCode == null)
                throw new ArgumentNullException("packageCode");

            using (var baseKey = BaseKey)
            {
                var key = baseKey.OpenSubKey(packageCode, writable);

                if (key != null && !Validate(key))
                {
                    key.Close();
                    key = null;
                }

                if (key == null)
                {
                    if (throwWhenMissing)
                        throw new NuGetUpdateException(String.Format(UILabels.PackageNotInstalled, packageCode));

                    return null;
                }

                return new Metadata(key, packageCode);
            }
        }

        private static bool Validate(RegistryKey key)
        {
            return VerifyNotNull(
                key,
                InstalledVersionKey,
                NuGetSiteKey,
                InstallPathKey,
                SetupTitleKey
            );
        }

        private static bool VerifyNotNull(RegistryKey key, params string[] keyNames)
        {
            foreach (string keyName in keyNames)
            {
                if (key.GetValue(keyName) == null)
                    return false;
            }

            return true;
        }

        public static Metadata Create(string packageCode)
        {
            if (packageCode == null)
                throw new ArgumentNullException("packageCode");

            using (var baseKey = BaseKey)
            {
                return new Metadata(baseKey.CreateSubKey(packageCode), packageCode);
            }
        }

        public static void Delete(string packageCode)
        {
            if (packageCode == null)
                throw new ArgumentNullException("packageCode");

            using (var key = BaseKey)
            {
                key.DeleteSubKeyTree(packageCode);
            }
        }

#if !NGU_LIBRARY
        private InstallLog DeserializeInstallLog(string installLog)
        {
            if (installLog == null)
                throw new ArgumentNullException("installLog");

            var content = Convert.FromBase64String(installLog);

            var serializer = new XmlSerializer(typeof(InstallLog));

            using (var stream = new MemoryStream(content))
            {
                return (InstallLog)serializer.Deserialize(stream);
            }
        }

        private string SerializeInstallLog(InstallLog installLog)
        {
            if (installLog == null)
                throw new ArgumentNullException("installLog");

            var serializer = new XmlSerializer(typeof(InstallLog));

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, installLog);

                return Convert.ToBase64String(stream.ToArray());
            }
        }
#endif

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_key != null)
                {
                    _key.Close();
                    _key = null;
                }

                _disposed = true;
            }
        }
    }
}
