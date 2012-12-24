using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NuGetUpdate.Shared
{
    public class FormHeader : Panel
    {
        private Color _bumpLightColor = SystemColors.ControlDark;
        private Color _bumpDarkColor = SystemColors.ControlLightLight;
        private string _text = "";
        private string _subText = "";
        private Image _image;
        private ContentAlignment _imageAlign = ContentAlignment.MiddleCenter;
        private bool _autosize = true;
        private int _lineSpacing = 5;
        private int _indentSubText = 7;

        public FormHeader()
        {
            BackColor = SystemColors.Window;
            Dock = DockStyle.Top;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            UpdateHeight();
        }

        protected override Padding DefaultPadding
        {
            get { return new Padding(13, 8, 13, 8); }
        }

        [Category("Appearance")]
        [Browsable(true)]
        [DefaultValue(typeof(Color), "Window")]
        public override Color BackColor
        {
            get { return base.BackColor; }
            set { base.BackColor = value; }
        }

        [Category("Layout")]
        [Browsable(true)]
        [DefaultValue(typeof(DockStyle), "Top")]
        public override DockStyle Dock
        {
            get { return base.Dock; }
            set { base.Dock = value; }
        }

        [Category("Appearance")]
        [Browsable(true)]
        [DefaultValue(typeof(Color), "ControlDark")]
        public Color BumpLightColor
        {
            get { return _bumpLightColor; }
            set
            {
                _bumpLightColor = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        [Browsable(true)]
        [DefaultValue(typeof(Color), "ControlLightLight")]
        public Color BumpDarkColor
        {
            get { return _bumpDarkColor; }
            set
            {
                _bumpDarkColor = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        [Browsable(true)]
        [DefaultValue("")]
        public override string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                UpdateHeight();
            }
        }

        [Category("Appearance")]
        [Browsable(true)]
        [DefaultValue("")]
        public string SubText
        {
            get { return _subText; }
            set
            {
                _subText = value;
                UpdateHeight();
            }
        }

        [Category("Appearance")]
        [Browsable(true)]
        [DefaultValue(5)]
        public int LineSpacing
        {
            get { return _lineSpacing; }
            set
            {
                _lineSpacing = value;
                UpdateHeight();
            }
        }

        [Category("Appearance")]
        [Browsable(true)]
        [DefaultValue(7)]
        public int IndentSubText
        {
            get { return _indentSubText; }
            set
            {
                _indentSubText = value;
                UpdateHeight();
            }
        }

        [Category("Appearance")]
        [Browsable(true)]
        [DefaultValue(null)]
        public Image Image
        {
            get { return _image; }
            set
            {
                _image = value;
                UpdateHeight();
            }
        }

        [Category("Appearance")]
        [Browsable(true)]
        [DefaultValue(typeof(ContentAlignment), "MiddleCenter")]
        public ContentAlignment ImageAlign
        {
            get { return _imageAlign; }
            set
            {
                _imageAlign = value;
                UpdateHeight();
            }
        }

        [Category("Layout")]
        [Browsable(true)]
        [DefaultValue(true)]
        public override bool AutoSize
        {
            get { return _autosize; }
            set
            {
                _autosize = value;
                UpdateHeight();
            }
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);

            UpdateHeight();
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            // Repaint when the layout has changed.

            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;

            g.Clear(BackColor);

            base.OnPaint(e);

            if (_image != null)
            {
                var location = CalculateImageLocation();

                g.DrawImage(_image, new Rectangle(location, _image.Size));
            }

            var lightPen = new Pen(_bumpLightColor);
            var darkPen = new Pen(_bumpDarkColor);

            g.DrawLine(lightPen, 0, Height - 2, Width - 1, Height - 2);
            g.DrawLine(darkPen, 0, Height - 1, Width - 1, Height - 1);

            var textPos =
                new Rectangle(
                    Padding.Left, Padding.Top,
                    Width - (Padding.Left + Padding.Right),
                    Height - (Padding.Top + Padding.Bottom)
                );

            int topOffset = 0;

            if (_text != "")
            {
                var heavyFont = new Font(Font, FontStyle.Bold);

                TextRenderer.DrawText(g, _text, heavyFont, textPos, ForeColor,
                    TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);

                int fontHeight = (int)Math.Ceiling(heavyFont.GetHeight(g));

                if (_subText != "")
                {
                    fontHeight += _lineSpacing;
                }

                topOffset = fontHeight;
            }

            if (_subText != "")
            {
                textPos =
                    new Rectangle(
                        textPos.Left + _indentSubText, textPos.Top + topOffset,
                        textPos.Width, Height - (textPos.Top + topOffset + Padding.Bottom)
                    );

                TextRenderer.DrawText(g, _subText, Font, textPos, ForeColor,
                    TextFormatFlags.WordBreak | TextFormatFlags.NoPrefix);
            }
        }

        private Point CalculateImageLocation()
        {
            int x = 0;
            int y = 0;

            switch (_imageAlign)
            {
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.MiddleRight:
                    y = (int)Math.Floor(((double)Height - _image.Height) / 2.0);
                    break;

                case ContentAlignment.BottomCenter:
                case ContentAlignment.BottomLeft:
                case ContentAlignment.BottomRight:
                    y = Height - _image.Height;
                    break;
            }

            switch (_imageAlign)
            {
                case ContentAlignment.BottomCenter:
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.TopCenter:
                    x = (int)Math.Floor(((double)Width - _image.Width) / 2.0);
                    break;

                case ContentAlignment.BottomRight:
                case ContentAlignment.MiddleRight:
                case ContentAlignment.TopRight:
                    x = Width - _image.Width;
                    break;
            }

            return new Point(x, y);
        }

        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);

            UpdateHeight();
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case NativeMethods.WM_ERASEBKGND:
                    // Ignore
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        private void UpdateHeight()
        {
            int height = CalculateHeight();

            if (height != Height)
            {
                Height = height;
            }

            Invalidate();
        }

        private int CalculateHeight()
        {
            if (_autosize)
            {
                int height = Padding.Top + Padding.Bottom;

                var g = CreateGraphics();

                if (_text != "")
                {
                    var heavyFont = new Font(Font, FontStyle.Bold);

                    height += (int)Math.Ceiling(heavyFont.GetHeight(g));

                    if (_subText != "")
                    {
                        height += _lineSpacing;
                    }
                }

                if (_subText != "")
                {
                    var size = TextRenderer.MeasureText(g, _subText, Font);

                    int fontHeight = (int)Math.Ceiling(Font.GetHeight(g));

                    if (size.Width > Width - (Padding.Left + Padding.Right + _indentSubText))
                    {
                        fontHeight *= 2;
                    }

                    height += fontHeight;
                }

                g.Dispose();

                if (_image != null)
                {
                    if ((_image.Height + 2) > height)
                    {
                        height = _image.Height + 2;
                    }
                }

                return height;
            }
            else
            {
                return Height;
            }
        }
    }
}