using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace NuGetUpdate.Shared
{
    [CLSCompliant(false)]
    public static class NativeMethods
    {
        [DllImport("shell32.dll")]
        public static extern int SHGetFolderPath(IntPtr hwndOwner, int nFolder, IntPtr hToken, uint dwFlags, [Out]StringBuilder pszPath);

        public static string SHGetFolderPath(IntPtr hwndOwner, SpecialFolderCSIDL nFolder, IntPtr hToken, uint dwFlags)
        {
            var sb = new StringBuilder(300);

            int result = SHGetFolderPath(hwndOwner, (int)nFolder, hToken, dwFlags, sb);

            if (result != 0)
                return null;

            return sb.ToString();
        }

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetDiskFreeSpaceEx(string lpDirectoryName, out ulong lpFreeBytesAvailable, out ulong lpTotalNumberOfBytes, out ulong lpTotalNumberOfFreeBytes);

        [DllImport("user32.dll")]
        public static extern int EnableMenuItem(IntPtr hMenu, int wIDEnableItem, int wEnable);

        [DllImport("user32.dll")]
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        /// <SecurityNote>
        /// Critical: Suppresses unmanaged code security.
        /// </SecurityNote>
        [SecurityCritical, SuppressUnmanagedCodeSecurity]
        [DllImport("ole32.dll")]
        public static extern int PropVariantClear(PROPVARIANT pvar);

        [DllImport("user32.dll")]
        public static extern IntPtr SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool AllowSetForegroundWindow(int dwProcessId);

        public const string IID_IPropertyStore = "886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99";

        public const int HTBOTTOMLEFT = 16;
        public const int HTBOTTOMRIGHT = 17;

        public const int WM_NCHITTEST = 0x84;
        public const int WM_ERASEBKGND = 0x14;
        public const int WM_NCPAINT = 0x0085;

        public const int SC_CLOSE = 0xF060;

        public const int MF_ENABLED = 0x00000000;
        public const int MF_GRAYED = 0x00000001;

        public const uint SLR_ANY_MATCH = 0x2;
        public const uint SLR_INVOKE_MSI = 0x80;
        public const uint SLR_NOLINKINFO = 0x40;
        public const uint SLR_NO_UI = 0x1;
        public const uint SLR_NO_UI_WITH_MSG_PUMP = 0x101;
        public const uint SLR_NOUPDATE = 0x8;
        public const uint SLR_NOSEARCH = 0x10;
        public const uint SLR_NOTRACK = 0x20;
        public const uint SLR_UPDATE = 0x4;

        public const int ASFW_ANY = -1;

        public static class Util
        {
            public static int SignedHIWORD(IntPtr n)
            {
                return SignedHIWORD(unchecked((int)(long)n));
            }

            public static int SignedLOWORD(IntPtr n)
            {
                return SignedLOWORD(unchecked((int)(long)n));
            }

            public static int SignedHIWORD(int n)
            {
                return (short)((n >> 16) & 0xffff);
            }

            public static int SignedLOWORD(int n)
            {
                return (short)(n & 0xFFFF);
            }
        }

        public enum SpecialFolderCSIDL
        {
            CSIDL_DESKTOP = 0x0000,    // <desktop>
            CSIDL_INTERNET = 0x0001,    // Internet Explorer (icon on desktop)
            CSIDL_PROGRAMS = 0x0002,    // Start Menu\Programs
            CSIDL_CONTROLS = 0x0003,    // My Computer\Control Panel
            CSIDL_PRINTERS = 0x0004,    // My Computer\Printers
            CSIDL_PERSONAL = 0x0005,    // My Documents
            CSIDL_FAVORITES = 0x0006,    // <user name>\Favorites
            CSIDL_STARTUP = 0x0007,    // Start Menu\Programs\Startup
            CSIDL_RECENT = 0x0008,    // <user name>\Recent
            CSIDL_SENDTO = 0x0009,    // <user name>\SendTo
            CSIDL_BITBUCKET = 0x000a,    // <desktop>\Recycle Bin
            CSIDL_STARTMENU = 0x000b,    // <user name>\Start Menu
            CSIDL_DESKTOPDIRECTORY = 0x0010,    // <user name>\Desktop
            CSIDL_DRIVES = 0x0011,    // My Computer
            CSIDL_NETWORK = 0x0012,    // Network Neighborhood
            CSIDL_NETHOOD = 0x0013,    // <user name>\nethood
            CSIDL_FONTS = 0x0014,    // windows\fonts
            CSIDL_TEMPLATES = 0x0015,
            CSIDL_COMMON_STARTMENU = 0x0016,    // All Users\Start Menu
            CSIDL_COMMON_PROGRAMS = 0x0017,    // All Users\Programs
            CSIDL_COMMON_STARTUP = 0x0018,    // All Users\Startup
            CSIDL_COMMON_DESKTOPDIRECTORY = 0x0019,    // All Users\Desktop
            CSIDL_APPDATA = 0x001a,    // <user name>\Application Data
            CSIDL_PRINTHOOD = 0x001b,    // <user name>\PrintHood
            CSIDL_LOCAL_APPDATA = 0x001c,    // <user name>\Local Settings\Applicaiton Data (non roaming)
            CSIDL_ALTSTARTUP = 0x001d,    // non localized startup
            CSIDL_COMMON_ALTSTARTUP = 0x001e,    // non localized common startup
            CSIDL_COMMON_FAVORITES = 0x001f,
            CSIDL_INTERNET_CACHE = 0x0020,
            CSIDL_COOKIES = 0x0021,
            CSIDL_HISTORY = 0x0022,
            CSIDL_COMMON_APPDATA = 0x0023,    // All Users\Application Data
            CSIDL_WINDOWS = 0x0024,    // GetWindowsDirectory()
            CSIDL_SYSTEM = 0x0025,    // GetSystemDirectory()
            CSIDL_PROGRAM_FILES = 0x0026,    // C:\Program Files
            CSIDL_MYPICTURES = 0x0027,    // C:\Program Files\My Pictures
            CSIDL_PROFILE = 0x0028,    // USERPROFILE
            CSIDL_SYSTEMX86 = 0x0029,    // x86 system directory on RISC
            CSIDL_PROGRAM_FILESX86 = 0x002a,    // x86 C:\Program Files on RISC
            CSIDL_PROGRAM_FILES_COMMON = 0x002b,    // C:\Program Files\Common
            CSIDL_PROGRAM_FILES_COMMONX86 = 0x002c,    // x86 Program Files\Common on RISC
            CSIDL_COMMON_TEMPLATES = 0x002d,    // All Users\Templates
            CSIDL_COMMON_DOCUMENTS = 0x002e,    // All Users\Documents
            CSIDL_COMMON_ADMINTOOLS = 0x002f,    // All Users\Start Menu\Programs\Administrative Tools
            CSIDL_ADMINTOOLS = 0x0030,    // <user name>\Start Menu\Programs\Administrative Tools
            CSIDL_CONNECTIONS = 0x0031,    // Network and Dial-up Connections
        };

        [ComImport]
        [Guid("000214F9-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IShellLink
        {
            //[helpstring("Retrieves the path and filename of a shell link object")]
            void GetPath(
                [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile,
                int cchMaxPath,
                ref _WIN32_FIND_DATAW pfd,
                uint fFlags);

            //[helpstring("Retrieves the list of shell link item identifiers")]
            void GetIDList(out IntPtr ppidl);

            //[helpstring("Sets the list of shell link item identifiers")]
            void SetIDList(IntPtr pidl);

            //[helpstring("Retrieves the shell link description string")]
            void GetDescription(
                [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile,
                int cchMaxName);

            //[helpstring("Sets the shell link description string")]
            void SetDescription(
                [MarshalAs(UnmanagedType.LPWStr)] string pszName);

            //[helpstring("Retrieves the name of the shell link working directory")]
            void GetWorkingDirectory(
                [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir,
                int cchMaxPath);

            //[helpstring("Sets the name of the shell link working directory")]
            void SetWorkingDirectory(
                [MarshalAs(UnmanagedType.LPWStr)] string pszDir);

            //[helpstring("Retrieves the shell link command-line arguments")]
            void GetArguments(
                [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs,
                int cchMaxPath);

            //[helpstring("Sets the shell link command-line arguments")]
            void SetArguments(
                [MarshalAs(UnmanagedType.LPWStr)] string pszArgs);

            //[propget, helpstring("Retrieves or sets the shell link hot key")]
            void GetHotkey(out short pwHotkey);
            //[propput, helpstring("Retrieves or sets the shell link hot key")]
            void SetHotkey(short pwHotkey);

            //[propget, helpstring("Retrieves or sets the shell link show command")]
            void GetShowCmd(out uint piShowCmd);
            //[propput, helpstring("Retrieves or sets the shell link show command")]
            void SetShowCmd(uint piShowCmd);

            //[helpstring("Retrieves the location (path and index) of the shell link icon")]
            void GetIconLocation(
                [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath,
                int cchIconPath,
                out int piIcon);

            //[helpstring("Sets the location (path and index) of the shell link icon")]
            void SetIconLocation(
                [MarshalAs(UnmanagedType.LPWStr)] string pszIconPath,
                int iIcon);

            //[helpstring("Sets the shell link relative path")]
            void SetRelativePath(
                [MarshalAs(UnmanagedType.LPWStr)] string pszPathRel,
                uint dwReserved);

            //[helpstring("Resolves a shell link. The system searches for the shell link object and updates the shell link path and its list of identifiers (if necessary)")]
            void Resolve(
                IntPtr hWnd,
                uint fFlags);

            //[helpstring("Sets the shell link path and filename")]
            void SetPath(
                [MarshalAs(UnmanagedType.LPWStr)] string pszFile);
        }

        [SuppressUnmanagedCodeSecurity]
        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid(IID_IPropertyStore)]
        public interface IPropertyStore
        {
            uint GetCount();
            PKEY GetAt(uint iProp);
            void GetValue([In] ref PKEY pkey, [In, Out] PROPVARIANT pv);
            void SetValue([In] ref PKEY pkey, PROPVARIANT pv);
            void Commit();
        }

        [Guid("00021401-0000-0000-C000-000000000046")]
        [ClassInterface(ClassInterfaceType.None)]
        [ComImport]
        public class CShellLink
        {
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4, Size = 0, CharSet = CharSet.Unicode)]
        public struct _WIN32_FIND_DATAW
        {
            public uint dwFileAttributes;
            public _FILETIME ftCreationTime;
            public _FILETIME ftLastAccessTime;
            public _FILETIME ftLastWriteTime;
            public uint nFileSizeHigh;
            public uint nFileSizeLow;
            public uint dwReserved0;
            public uint dwReserved1;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)] // MAX_PATH
            public string cFileName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
            public string cAlternateFileName;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4, Size = 0)]
        public struct _FILETIME
        {
            public uint dwLowDateTime;
            public uint dwHighDateTime;
        }

        public enum EShellLinkGP : uint
        {
            SLGP_SHORTPATH = 1,
            SLGP_UNCPRIORITY = 2
        }

        [ComImport]
        [Guid("0000010B-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IPersistFile
        {
            // can't get this to go if I extend IPersist, so put it here:
            [PreserveSig]
            void GetClassID(out Guid pClassID);

            //[helpstring("Checks for changes since last file write")]		
            void IsDirty();

            //[helpstring("Opens the specified file and initializes the object from its contents")]		
            void Load(
                [MarshalAs(UnmanagedType.LPWStr)] string pszFileName,
                uint dwMode);

            //[helpstring("Saves the object into the specified file")]		
            void Save(
                [MarshalAs(UnmanagedType.LPWStr)] string pszFileName,
                [MarshalAs(UnmanagedType.Bool)] bool fRemember);

            //[helpstring("Notifies the object that save is completed")]		
            void SaveCompleted(
                [MarshalAs(UnmanagedType.LPWStr)] string pszFileName);

            //[helpstring("Gets the current name of the file associated with the object")]		
            void GetCurFile(
                [MarshalAs(UnmanagedType.LPWStr)] out string ppszFileName);
        }

        [SecurityCritical]
        [StructLayout(LayoutKind.Explicit)]
        public class PROPVARIANT : IDisposable
        {
            [FieldOffset(0)]
            private ushort vt;
            [FieldOffset(8)]
            private IntPtr pointerVal;
            [FieldOffset(8)]
            private byte byteVal;
            [FieldOffset(8)]
            private long longVal;
            [FieldOffset(8)]
            private short boolVal;
            [FieldOffset(8)]
            private uint ulVal;

            public VarEnum VarType
            {
                [SecurityCritical]
                get { return (VarEnum)vt; }
            }

            [SecurityCritical]
            public string GetString()
            {
                if (vt == (ushort)VarEnum.VT_LPWSTR)
                    return Marshal.PtrToStringUni(pointerVal);

                return null;
            }

            [SecurityCritical]
            public bool? GetBool()
            {
                if (vt == (ushort)VarEnum.VT_BOOL)
                    return boolVal != 0;

                return null;
            }

            [SecurityCritical]
            public uint? GetUInt32()
            {
                if (vt == (ushort)VarEnum.VT_UI4)
                    return ulVal;

                return null;
            }

            [SecurityCritical]
            public void SetValue(bool f)
            {
                Clear();
                vt = (ushort)VarEnum.VT_BOOL;
                boolVal = (short)(f ? -1 : 0);
            }

            [SecurityCritical]
            public void SetValue(uint f)
            {
                Clear();
                vt = (ushort)VarEnum.VT_UI4;
                ulVal = f;
            }

            [SecurityCritical]
            public void SetValue(string val)
            {
                Clear();
                vt = (ushort)VarEnum.VT_LPWSTR;
                pointerVal = Marshal.StringToCoTaskMemUni(val);
            }

            [SecurityCritical]
            public void Clear()
            {
                NativeMethods.PropVariantClear(this);
            }

            [SecurityCritical]
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            [SecurityCritical]
            ~PROPVARIANT()
            {
                Dispose(false);
            }

            [SecurityCritical]
            private void Dispose(bool disposing)
            {
                Clear();
            }
        }

        public static NativeMethods.PKEY GetPkey(PropertyStoreProperty property)
        {
            switch (property)
            {
                case PropertyStoreProperty.AppUserModel_ID: return NativeMethods.PKEY.AppUserModel_ID;
                case PropertyStoreProperty.AppUserModel_IsDestListSeparator: return NativeMethods.PKEY.AppUserModel_IsDestListSeparator;
                case PropertyStoreProperty.AppUserModel_RelaunchCommand: return NativeMethods.PKEY.AppUserModel_RelaunchCommand;
                case PropertyStoreProperty.AppUserModel_RelaunchDisplayNameResource: return NativeMethods.PKEY.AppUserModel_RelaunchDisplayNameResource;
                case PropertyStoreProperty.AppUserModel_RelaunchIconResource: return NativeMethods.PKEY.AppUserModel_RelaunchIconResource;
                case PropertyStoreProperty.AppUserModel_StartPinOption: return NativeMethods.PKEY.AppUserModel_StartPinOption;
                case PropertyStoreProperty.AppUserModel_ExcludeFromShowInNewInstall: return NativeMethods.PKEY.AppUserModel_ExcludeFromShowInNewInstall;
                case PropertyStoreProperty.Title: return NativeMethods.PKEY.Title;
                default: throw new ArgumentOutOfRangeException("property");
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct PKEY
        {
            /// <summary>fmtid</summary>
            private readonly Guid _fmtid;
            /// <summary>pid</summary>
            private readonly uint _pid;

            private PKEY(Guid fmtid, uint pid)
            {
                _fmtid = fmtid;
                _pid = pid;
            }

            /// <summary>PKEY_Title</summary>
            public static readonly PKEY Title = new PKEY(new Guid("F29F85E0-4FF9-1068-AB91-08002B27B3D9"), 2);
            /// <summary>PKEY_AppUserModel_ID</summary>
            public static readonly PKEY AppUserModel_ID = new PKEY(new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3"), 5);
            /// <summary>PKEY_AppUserModel_IsDestListSeparator</summary>
            public static readonly PKEY AppUserModel_IsDestListSeparator = new PKEY(new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3"), 6);
            /// <summary>PKEY_AppUserModel_RelaunchCommand</summary>
            public static readonly PKEY AppUserModel_RelaunchCommand = new PKEY(new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3"), 2);
            /// <summary>PKEY_AppUserModel_RelaunchDisplayNameResource</summary>
            public static readonly PKEY AppUserModel_RelaunchDisplayNameResource = new PKEY(new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3"), 4);
            /// <summary>PKEY_AppUserModel_RelaunchIconResource</summary>
            public static readonly PKEY AppUserModel_RelaunchIconResource = new PKEY(new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3"), 3);
            /// <summary>PKEY_AppUserModel_StartPinOption</summary>
            public static readonly PKEY AppUserModel_StartPinOption = new PKEY(new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3"), 12);
            /// <summary>PKEY_AppUserModel_ExcludeFromShowInNewInstall</summary>
            public static readonly PKEY AppUserModel_ExcludeFromShowInNewInstall = new PKEY(new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3"), 8);
        }

        [Flags]
        public enum EShowWindowFlags : uint
        {
            SW_HIDE = 0,
            SW_SHOWNORMAL = 1,
            SW_NORMAL = 1,
            SW_SHOWMINIMIZED = 2,
            SW_SHOWMAXIMIZED = 3,
            SW_MAXIMIZE = 3,
            SW_SHOWNOACTIVATE = 4,
            SW_SHOW = 5,
            SW_MINIMIZE = 6,
            SW_SHOWMINNOACTIVE = 7,
            SW_SHOWNA = 8,
            SW_RESTORE = 9,
            SW_SHOWDEFAULT = 10,
            SW_MAX = 10
        }
    }
}
