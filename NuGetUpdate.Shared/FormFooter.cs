using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Security.Permissions;

namespace NuGetUpdate.Shared
{
    public class FormFlowFooter : FlowLayoutPanel
    {
        private static readonly Padding LinePadding = new Padding(4, 0, 10, 0);
        private const int SizeGripSize = 16;

        private System.Windows.Forms.Form _form;
        private bool _renderSizeGrip;
        private VisualStyleRenderer _sizeGripRenderer;

        public FormFlowFooter()
        {
            Dock = DockStyle.Bottom;
            Padding = DefaultPadding;
            base.FlowDirection = FlowDirection.RightToLeft;
            AutoSize = true;
        }

        protected override Padding DefaultPadding
        {
            get
            {
                var height = TextRenderer.MeasureText("W", SystemFonts.MessageBoxFont).Height;

                return new Padding(8, height + 2, 1, 7);
            }
        }

        [Category("Layout")]
        [Browsable(true)]
        [DefaultValue(true)]
        public override bool AutoSize
        {
            get { return base.AutoSize; }
            set { base.AutoSize = value; }
        }

        [Category("Layout")]
        [Browsable(true)]
        [DefaultValue(typeof(FlowDirection), "RightToLeft")]
        public new FlowDirection FlowDirection
        {
            get { return base.FlowDirection; }
            set { base.FlowDirection = value; }
        }

        [Category("Layout")]
        [Browsable(true)]
        [DefaultValue(typeof(DockStyle), "Bottom")]
        public override DockStyle Dock
        {
            get { return base.Dock; }
            set { base.Dock = value; }
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);

            // Repaint when the layout has changed.

            Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                var g = e.Graphics;

                g.Clear(BackColor);

                base.OnPaint(e);

                string version = GetType().Assembly.GetName().Version.ToString();

                while (version.EndsWith(".0"))
                {
                    version = version.Substring(0, version.Length - 2);
                }

                string text = String.Format(UILabels.FooterSetupLine, version);

                var offset = RenderGrayText(e.Graphics, text, new Point(LinePadding.Left, 0));

                g.DrawLine(
                    SystemPens.ControlDark,
                    offset.X,
                    offset.Y / 2,
                    Width - LinePadding.Right - 1,
                    offset.Y / 2
                );

                g.DrawLine(
                    SystemPens.ControlLightLight,
                    offset.X,
                    (offset.Y / 2) + 1,
                    Width - LinePadding.Right - 1,
                    (offset.Y / 2) + 1
                );

                g.DrawLine(
                    SystemPens.ControlLightLight,
                    Width - LinePadding.Right,
                    offset.Y / 2,
                    Width - LinePadding.Right,
                    (offset.Y / 2) + 1
                );

                if (_renderSizeGrip)
                {
                    Size sz = ClientSize;

                    if (Application.RenderWithVisualStyles)
                    {
                        if (_sizeGripRenderer == null)
                        {
                            _sizeGripRenderer = new VisualStyleRenderer(VisualStyleElement.Status.Gripper.Normal);
                        }

                        _sizeGripRenderer.DrawBackground(e.Graphics, new Rectangle(sz.Width - SizeGripSize, sz.Height - SizeGripSize, SizeGripSize, SizeGripSize));
                    }
                    else
                    {
                        ControlPaint.DrawSizeGrip(e.Graphics, BackColor, sz.Width - SizeGripSize, sz.Height - SizeGripSize, SizeGripSize, SizeGripSize);
                    }
                }
            }
            catch
            {
                // Suppress all exceptions coming from GDI actions.
            }
        }

        private Point RenderGrayText(Graphics graphics, string text, Point point)
        {
            const TextFormatFlags flags = TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix;

            TextRenderer.DrawText(
                graphics,
                text,
                SystemFonts.MessageBoxFont,
                new Point(point.X + 1, point.Y + 1),
                SystemColors.ControlLightLight,
                Color.Transparent,
                flags
            );

            TextRenderer.DrawText(
                graphics,
                text,
                SystemFonts.MessageBoxFont,
                point,
                SystemColors.ControlDark,
                Color.Transparent,
                flags
            );

            var size = TextRenderer.MeasureText(
                graphics,
                text,
                SystemFonts.MessageBoxFont,
                new Size(int.MaxValue, int.MaxValue),
                flags
            );

            return new Point(
                point.X + size.Width + 1,
                size.Height
            );
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);

            GetSizeGripDetails();
        }

        private void GetSizeGripDetails()
        {
            var form = FindForm();

            if (!Object.ReferenceEquals(form, _form))
            {
                if (_form != null)
                {
                    _form.StyleChanged -= Form_StyleChanged;
                }

                _form = form;

                if (_form != null)
                {
                    _form.StyleChanged += Form_StyleChanged;
                }
            }

            _renderSizeGrip = false;

            if (_form != null)
            {
                switch (_form.FormBorderStyle)
                {
                    case FormBorderStyle.None:
                    case FormBorderStyle.FixedSingle:
                    case FormBorderStyle.Fixed3D:
                    case FormBorderStyle.FixedDialog:
                    case FormBorderStyle.FixedToolWindow:
                        _renderSizeGrip = false;
                        break;

                    case FormBorderStyle.Sizable:
                    case FormBorderStyle.SizableToolWindow:
                        switch (form.SizeGripStyle)
                        {
                            case SizeGripStyle.Show:
                                _renderSizeGrip = true;
                                break;

                            case SizeGripStyle.Hide:
                                _renderSizeGrip = false;
                                break;

                            case SizeGripStyle.Auto:
                                _renderSizeGrip = _form.Modal;
                                break;
                        }
                        break;
                }
            }
        }

        void Form_StyleChanged(object sender, EventArgs e)
        {
            GetSizeGripDetails();
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            switch (m.Msg)
            {
                case NativeMethods.WM_NCHITTEST:
                    WmNCHitTest(ref m);
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        private void WmNCHitTest(ref System.Windows.Forms.Message m)
        {
            if (_renderSizeGrip)
            {
                int x = NativeMethods.Util.SignedLOWORD((int)m.LParam);
                int y = NativeMethods.Util.SignedHIWORD((int)m.LParam);

                // Convert to client coordinates
                //
                var pt = PointToClient(new Point(x, y));

                Size clientSize = ClientSize;

                // If the grip is not fully visible the grip area could overlap with the system control box; we need to disable
                // the grip area in this case not to get in the way of the control box.  We only need to check for the client's
                // height since the window width will be at least the size of the control box which is always bigger than the
                // grip width.
                if (pt.X >= (clientSize.Width - SizeGripSize) &&
                    pt.Y >= (clientSize.Height - SizeGripSize) &&
                    clientSize.Height >= SizeGripSize)
                {
                    m.Result = IsMirrored ? (IntPtr)NativeMethods.HTBOTTOMLEFT : (IntPtr)NativeMethods.HTBOTTOMRIGHT;
                    return;
                }
            }

            base.WndProc(ref m);
        }
    }
}
