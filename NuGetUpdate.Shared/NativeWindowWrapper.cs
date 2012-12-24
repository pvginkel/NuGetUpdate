using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace NuGetUpdate.Shared
{
    public class NativeWindowWrapper : IWin32Window
    {
        public IntPtr Handle { get; private set; }

        public NativeWindowWrapper(IWin32Window owner)
        {
            if (owner == null)
                throw new ArgumentNullException("owner");

            Handle = owner.Handle;
        }
    }
}
