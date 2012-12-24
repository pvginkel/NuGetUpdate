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
    public partial class InstallDestinationFolderPage : PageControl
    {
        private readonly ScriptRunner _runner;
        private readonly IScriptContinuation _continuation;
        private readonly string _originalSpaceAvailable;
        private string _spaceAvailableRoot;

        public override IButtonControl AcceptButton
        {
            get { return _acceptButton; }
        }

        public override IButtonControl CancelButton
        {
            get { return _cancelButton; }
        }

        public InstallDestinationFolderPage(ScriptRunner runner, PageInstallDestinationFolder action, IScriptContinuation continuation)
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
            _introduction.Text = String.Format(
                _introduction.Text,
                runner.Environment.Config.SetupTitle,
                action.IsLast ? UILabels.InstallContinue : UILabels.NextContinue
            );

            string targetPath = _runner.Variables.GetRequired<string>(
                Constants.ScriptVariables.TargetPath
            );

            if (!Path.IsPathRooted(targetPath))
                throw new ScriptException(UILabels.TargetPathNotRooted);
            if (!Directory.Exists(Path.GetPathRoot(targetPath)))
                throw new ScriptException(UILabels.InvalidTargetPathRoot);

            long downloadFolderSize = CalculateDownloadFolderSize(
                runner.Environment.Config.PackageFolder
            );

            _spaceRequired.Text = String.Format(_spaceRequired.Text, Util.FormatSize(downloadFolderSize));

            _originalSpaceAvailable = _spaceAvailable.Text;

            _targetPath.Text = targetPath;

            _destination.Enabled = action.Enabled;

            PageUtil.UpdateAcceptButton(_acceptButton, action.IsLast);
        }

        private long CalculateDownloadFolderSize(string path)
        {
            long size = 0;

            foreach (string fileName in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
            {
                size += new FileInfo(fileName).Length;
            }

            return size;
        }

        private void _browse_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                    _targetPath.Text = dialog.SelectedPath;
            }
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            FindForm().Close();
        }

        private void _acceptButton_Click(object sender, EventArgs e)
        {
            _runner.Variables.AddOrSet(Constants.ScriptVariables.TargetPath, _targetPath.Text);

            _continuation.Resume();
        }

        private void _targetPath_TextChanged(object sender, EventArgs e)
        {
            string targetPath = CorrectPath(_targetPath.Text);

            if (
                !String.IsNullOrEmpty(targetPath) &&
                Path.IsPathRooted(targetPath)
            ) {
                string pathRoot = Path.GetPathRoot(targetPath);

                _acceptButton.Enabled =
                    Directory.Exists(pathRoot) &&
                    !String.Equals(
                        targetPath,
                        pathRoot,
                        StringComparison.OrdinalIgnoreCase
                    );

                if (pathRoot == _spaceAvailableRoot)
                    return;

                _spaceAvailableRoot = pathRoot;

                if (Directory.Exists(_spaceAvailableRoot))
                {
                    while (!Directory.Exists(targetPath))
                    {
                        targetPath = Path.GetDirectoryName(targetPath);
                    }

                    ulong freeBytesAvailable;
                    ulong totalNumberOfBytes;
                    ulong totalNumberOfFreeBytes;

                    bool success = NativeMethods.GetDiskFreeSpaceEx(
                        targetPath,
                        out freeBytesAvailable,
                        out totalNumberOfBytes,
                        out totalNumberOfFreeBytes
                    );

                    if (success)
                    {
                        _spaceAvailable.Text = String.Format(_originalSpaceAvailable, Util.FormatSize(freeBytesAvailable));

                        return;
                    }
                }
            }

            _spaceAvailable.Text = "";
        }

        private string CorrectPath(string path)
        {
            if (String.IsNullOrEmpty(path))
                return null;

            if (path.EndsWith(":"))
                path += Path.DirectorySeparatorChar;

            path = Path.GetFullPath(path);

            return path;
        }
    }
}
