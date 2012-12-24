using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using NuGetUpdate.Installer.ScriptEngine;
using NuGetUpdate.Shared;

namespace NuGetUpdate.Installer.Pages
{
    public partial class FinishPage : PageControl, IWaitablePage, IControlHostPage
    {
        private readonly ScriptRunner _runner;
        private bool _initialized;

        private static readonly Dictionary<Type, Action<FinishPage, IScriptAction>> _controlHandlers = new Dictionary<Type, Action<FinishPage, IScriptAction>>
        {
            { typeof(ControlCheckBox), (page, action) => page.AddControl((ControlCheckBox)action) },
            { typeof(ControlLabel), (page, action) => page.AddControl((ControlLabel)action) },
            { typeof(ControlLink), (page, action) => page.AddControl((ControlLink)action) }
        };

        private IScriptContinuation _continuation;

        public override IButtonControl AcceptButton
        {
            get { return _acceptButton; }
        }

        public override IButtonControl CancelButton
        {
            get { return _cancelButton; }
        }

        public FinishPage(ScriptRunner runner, IScriptContinuation continuation)
        {
            if (runner == null)
                throw new ArgumentNullException("runner");
            if (continuation == null)
                throw new ArgumentNullException("continuation");

            _runner = runner;

            InitializeComponent();

            switch (_runner.Mode)
            {
                case ScriptRunnerMode.Install:
                    _headerLabel.Text = String.Format(UILabels.InstallComplete, runner.Environment.Config.SetupTitle);
                    _wizardLabel.Text = String.Format(UILabels.InstallCompleteSubText, runner.Environment.Config.SetupTitle);
                    break;

                case ScriptRunnerMode.Update:
                    _headerLabel.Text = String.Format(UILabels.UpdateComplete, runner.Environment.Config.SetupTitle);
                    _wizardLabel.Text = String.Format(UILabels.UpdateCompleteSubText, runner.Environment.Config.SetupTitle);
                    break;

                case ScriptRunnerMode.Uninstall:
                    _headerLabel.Text = String.Format(UILabels.UninstallComplete, runner.Environment.Config.SetupTitle);
                    _wizardLabel.Text = String.Format(UILabels.UninstallCompleteSubText, runner.Environment.Config.SetupTitle);
                    break;
            }

            continuation.Resume();
        }

        public void WaitForClose(IScriptContinuation continuation)
        {
            if (continuation == null)
                throw new ArgumentNullException("continuation");

            _continuation = continuation;
        }

        private void InstallFinishPage_ParentChanged(object sender, EventArgs e)
        {
            if (_initialized)
                return;

            _initialized = true;

            ((MainForm)FindForm()).CloseButtonEnabled = false;
        }

        public void AddControl(IScriptAction action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            _controlHandlers[action.GetType()](this, action);
        }

        public void AddControl(ControlCheckBox action)
        {
            AddControl(new CheckBox
            {
                Text = _runner.ParseTemplate(action.Text),
                Checked = true,
                Tag = action,
                FlatStyle = FlatStyle.System,
                Margin = new Padding(6, 5, 3, 5),
                AutoSize = true,
                AutoEllipsis = true
            });
        }

        public void AddControl(ControlLabel action)
        {
            AddControl(new Label
            {
                AutoSize = true,
                Text = _runner.ParseTemplate(action.Value),
                Margin = new Padding(3, 5, 3, 5)
            });
        }

        public void AddControl(ControlLink action)
        {
            var link = new LinkLabel
            {
                Text = _runner.ParseTemplate(action.Text),
                Tag = action,
                AutoSize = true,
                Margin = new Padding(3, 5, 3, 5)
            };

            link.Click += link_Click;

            AddControl(link);
        }

        private void AddControl(Control control)
        {
            _controlContainer.RowCount++;

            for (int i = 0; i < _controlContainer.RowCount; i++)
            {
                _controlContainer.RowStyles[i] = new RowStyle(SizeType.AutoSize);
            }

            _controlContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            control.Dock = DockStyle.Fill;

            _controlContainer.Controls.Add(control);
            _controlContainer.SetRow(control, _controlContainer.RowCount - 2);
        }

        void link_Click(object sender, EventArgs e)
        {
            try
            {
                _runner.ExecuteChildren((ControlLink)((LinkLabel)sender).Tag);
            }
            catch (Exception ex)
            {
                ((MainForm)FindForm()).ShowException(ex);
            }
        }

        private void _acceptButton_Click(object sender, EventArgs e)
        {
            foreach (var control in _controlContainer.Controls)
            {
                var checkBox = control as CheckBox;

                if (checkBox != null && checkBox.Checked)
                {
                    try
                    {
                        _runner.ExecuteChildren((ControlCheckBox)checkBox.Tag);
                    }
                    catch (Exception ex)
                    {
                        ((MainForm)FindForm()).ShowException(ex);
                        break;
                    }
                }
            }

            _continuation.Resume();
        }
    }
}
