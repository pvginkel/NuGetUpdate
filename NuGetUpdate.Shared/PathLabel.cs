using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace NuGetUpdate.Shared
{
    public class PathLabel : Control
    {
        public PathLabel()
        {
            Height = CalculateHeight();
        }

        private int CalculateHeight()
        {
            return TextRenderer.MeasureText("W", Font).Height;
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);

            Invalidate();
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);

            Height = CalculateHeight();
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            height = CalculateHeight();

            base.SetBoundsCore(x, y, width, height, specified);
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
                TextFormatFlags.NoPrefix | TextFormatFlags.PathEllipsis | TextFormatFlags.SingleLine
            );
        }
    }
}
