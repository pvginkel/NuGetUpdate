using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NuGetUpdate.Shared
{
    public class PathLabel : Control
    {
        private const TextFormatFlags FormatFlags = TextFormatFlags.NoPrefix | TextFormatFlags.PathEllipsis | TextFormatFlags.SingleLine;

        private int _height;

        [EditorBrowsable(EditorBrowsableState.Always)]
        [DefaultValue(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Localizable(true)]
        [Browsable(true)]
        [RefreshProperties(RefreshProperties.All)]
        public override bool AutoSize
        {
            get { return base.AutoSize; }
            set
            {
                base.AutoSize = value;

                EnforceHeight();
            }
        }

        private int CalculateHeight()
        {
            return TextRenderer.MeasureText("W", Font).Height;
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);

            PerformLayout();

            Invalidate();
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);

            EnforceHeight();
        }

        private void EnforceHeight()
        {
            if (!AutoSize)
                return;

            Height = _height = CalculateHeight();

            PerformLayout();

            Invalidate();
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            if (AutoSize)
                height = _height;

            base.SetBoundsCore(x, y, width, height, specified);
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            if (AutoSize)
            {
                var result = TextRenderer.MeasureText(Text, Font, proposedSize, FormatFlags);

                return new Size(Math.Min(result.Width, proposedSize.Width), _height);
            }
            else
            {
                return base.GetPreferredSize(proposedSize);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            TextRenderer.DrawText(
                e.Graphics,
                Text,
                Font,
                ClientRectangle,
                ForeColor,
                BackColor,
                FormatFlags
            );
        }
    }
}
