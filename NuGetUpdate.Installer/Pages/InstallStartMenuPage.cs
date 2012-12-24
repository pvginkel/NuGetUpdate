using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using NuGetUpdate.Installer.ScriptEngine;
using NuGetUpdate.Shared;

namespace NuGetUpdate.Installer.Pages
{
    public partial class InstallStartMenuPage : PageControl
    {
        private readonly ScriptRunner _runner;
        private readonly IScriptContinuation _continuation;
        private readonly string _originalStartMenuFolder;

        public override IButtonControl AcceptButton
        {
            get { return _acceptButton; }
        }

        public override IButtonControl CancelButton
        {
            get { return _cancelButton; }
        }

        public InstallStartMenuPage(ScriptRunner runner, PageInstallStartMenu action, IScriptContinuation continuation)
        {
            if (runner == null)
                throw new ArgumentNullException("runner");
            if (action == null)
                throw new ArgumentNullException("action");
            if (continuation == null)
                throw new ArgumentNullException("continuation");

            _runner = runner;
            _continuation = continuation;

            InitializeComponent();

            _header.SubText = String.Format(_header.SubText, runner.Environment.Config.SetupTitle);

            _container.Enabled = action.Enabled;
            _createStartMenu.Visible = action.CreateStartMenuVisible;
            _createOnDesktop.Visible = action.CreateOnDesktopVisible;

            _originalStartMenuFolder = runner.Variables.GetRequired<string>(
                Constants.ScriptVariables.StartMenuPath
            );
            _createStartMenu.Checked = runner.Variables.GetOptional(
                Constants.ScriptVariables.CreateShortcuts, true
            );
            _createOnDesktop.Checked = runner.Variables.GetOptional(
                Constants.ScriptVariables.CreateDesktopShortcuts, true
            );

            _startMenuFolder.Text = _originalStartMenuFolder;

            var startMenuFolders = new List<string>();

            AddStartMenuFolders(startMenuFolders, NativeMethods.SpecialFolderCSIDL.CSIDL_PROGRAMS);
            AddStartMenuFolders(startMenuFolders, NativeMethods.SpecialFolderCSIDL.CSIDL_COMMON_PROGRAMS);

            startMenuFolders.Sort((a, b) => String.Compare(a, b, StringComparison.OrdinalIgnoreCase));

            foreach (string directory in startMenuFolders)
            {
                _startMenuFolders.Items.Add(directory);
            }

            PageUtil.UpdateAcceptButton(_acceptButton, action.IsLast);
        }

        private void AddStartMenuFolders(List<string> startMenuFolders, NativeMethods.SpecialFolderCSIDL specialFolderCSIDL)
        {
            string startMenuFolder = NativeMethods.SHGetFolderPath(
                Handle,
                specialFolderCSIDL,
                IntPtr.Zero,
                0
            );

            foreach (string directory in Directory.GetDirectories(startMenuFolder))
            {
                string path = Path.GetFileName(directory);

                if (!startMenuFolders.Contains(path))
                    startMenuFolders.Add(path);
            }
        }

        private void _acceptButton_Click(object sender, EventArgs e)
        {
            _runner.Variables.AddOrSet(
                Constants.ScriptVariables.CreateShortcuts, _createStartMenu.Checked
            );
            _runner.Variables.AddOrSet(
                Constants.ScriptVariables.CreateDesktopShortcuts,
                _createOnDesktop.Visible && _createOnDesktop.Checked
            );
            _runner.Variables.AddOrSet(
                Constants.ScriptVariables.StartMenuPath,
                _startMenuFolder.Text
            );

            _continuation.Resume();
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            FindForm().Close();
        }

        private void _startMenuFolders_SelectedIndexChanged(object sender, EventArgs e)
        {
            _startMenuFolder.Text = Path.Combine(
                (string)_startMenuFolders.SelectedItem,
                _originalStartMenuFolder
            );
        }
    }
}
