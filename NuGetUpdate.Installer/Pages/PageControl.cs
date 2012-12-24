using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace NuGetUpdate.Installer.Pages
{
    public class PageControl : UserControl
    {
        public virtual IButtonControl AcceptButton
        {
            get { return null; }
        }

        public virtual IButtonControl CancelButton
        {
            get { return null; }
        }
    }
}
