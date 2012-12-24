using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using NuGetUpdate.Shared;

namespace NuGetUpdate.Demo
{
    public partial class MainForm : Shared.Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void _resetVersion_Click(object sender, EventArgs e)
        {
            using (var key = Registry.CurrentUser.CreateSubKey(
                "Software\\" + Shared.Constants.Registry.BaseKey + "\\" + Shared.Constants.Registry.PackagesKey + "\\" + Constants.PackageCode
            ))
            {
                key.SetValue("Installed Version", "0.0.0.0");
            }
        }

        private void _checkForUpdates_Click(object sender, EventArgs e)
        {
            bool updateAvailable = NuGetUpdate.Update.IsUpdateAvailable(Constants.PackageCode);

            if (updateAvailable)
            {
                var result = MessageBox.Show(
                    this,
                    "A new update is available. Do you want to update to the latest version?",
                    Text,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                    _startUpdate.PerformClick();
            }
            else
            {
                MessageBox.Show(
                    this,
                    "No new updates are available at this moment.",
                    Text,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }

        private void _startUpdate_Click(object sender, EventArgs e)
        {
            NuGetUpdate.Update.StartUpdate(Constants.PackageCode, "-updated");

            Close();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            if (((IList<string>)Environment.GetCommandLineArgs()).Contains("-updated"))
            {
                MessageBox.Show(
                    this,
                    Text + " was updated.",
                    Text,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }
    }
}
