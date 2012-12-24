using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace NuGetUpdate.Installer.Pages
{
    internal static class PageUtil
    {
        public static void UpdateAcceptButton(Button button, bool isLast)
        {
            if (isLast)
                button.Text = UILabels.InstallButtonLabel;
            else
                button.Text = UILabels.NextButtonLabel;
        }
    }
}
