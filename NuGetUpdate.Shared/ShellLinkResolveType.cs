using System;
using System.Collections.Generic;
using System.Text;

namespace NuGetUpdate.Shared
{
    [Flags]
    [CLSCompliant(false)]
    public enum ShellLinkResolveType : uint
    {
        AnyMatch = NativeMethods.SLR_ANY_MATCH,
        InvokeMsi = NativeMethods.SLR_INVOKE_MSI,
        NoLinkInfo = NativeMethods.SLR_NOLINKINFO,
        NoUI = NativeMethods.SLR_NO_UI,
        NoUIWithMessagePump = NativeMethods.SLR_NO_UI_WITH_MSG_PUMP,
        NoUpdate = NativeMethods.SLR_NOUPDATE,
        NoSearch = NativeMethods.SLR_NOSEARCH,
        NoTrack = NativeMethods.SLR_NOTRACK,
        Update = NativeMethods.SLR_UPDATE
    }
}
