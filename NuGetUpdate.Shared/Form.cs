using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NuGetUpdate.Shared
{
    public class Form : System.Windows.Forms.Form
    {
        private readonly string _defaultFontName;
        private readonly float _defaultFontSize;
        private readonly string _correctFontName;
        private readonly float _correctFontSize;
        private bool _initializeCalled;
        private bool _closeButtonEnabled = true;

        public bool InDesignMode { get; private set; }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new AutoScaleMode AutoScaleMode
        {
            get { return base.AutoScaleMode; }
            set
            {
                // This value is set by the designer. To not have to manually change the
                // defaults set by the designer, it's silently ignored here at runtime.

                if (InDesignMode)
                    base.AutoScaleMode = value;
            }
        }

        [DefaultValue(true)]
        public bool CloseButtonEnabled
        {
            get { return _closeButtonEnabled; }
            set
            {
                if (_closeButtonEnabled != value)
                {
                    _closeButtonEnabled = value;

                    NativeMethods.EnableMenuItem(
                        NativeMethods.GetSystemMenu(Handle, false),
                        NativeMethods.SC_CLOSE,
                        value ? NativeMethods.MF_ENABLED : NativeMethods.MF_GRAYED
                    );

                    InvalidateNonClient();
                }
            }
        }

        public Form()
        {
            InDesignMode = ControlUtil.GetIsInDesignMode(this);

            if (!InDesignMode)
            {
                _defaultFontName = Font.Name;
                _defaultFontSize = Font.Size;

                AutoScaleMode = AutoScaleMode.None;
                Font = SystemFonts.MessageBoxFont;

                _correctFontName = Font.Name;
                _correctFontSize = Font.Size;
            }
        }

        protected override void SetVisibleCore(bool value)
        {
            if (value && !_initializeCalled && !InDesignMode)
            {
                _initializeCalled = true;

                FixFonts(Controls);
            }

            base.SetVisibleCore(value);
        }

        private void FixFonts(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                FixFonts(control);
            }
        }

        protected void FixFonts(Control control)
        {
            string fontName = control.Font.Name;

            if (fontName != _correctFontName)
            {
                float fontSize = control.Font.Size;

                if (fontName == _defaultFontName)
                    fontName = _correctFontName;
                if (fontSize == _defaultFontSize)
                    fontSize = _correctFontSize;

                control.Font = new Font(
                    fontName,
                    fontSize,
                    control.Font.Style,
                    control.Font.Unit,
                    control.Font.GdiCharSet
                );
            }

            FixFonts(control.Controls);
        }

        private void InvalidateNonClient()
        {
            NativeMethods.SendMessage(Handle, NativeMethods.WM_NCPAINT, (IntPtr)1, (IntPtr)0);
        }
    }
}
