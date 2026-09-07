using Client.Envir;
using System;
using System.Drawing;
using System.Windows.Forms;
using Font = System.Drawing.Font;

namespace Client.Controls
{
    public sealed class DXWindowTitleLabel : DXLabel
    {
        private const int OpticalVerticalOffset = 3;

        public DXWindowTitleLabel()
        {
            AutoSize = false;
            Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Regular);
            ForeColour = Constants.PrimaryColour;
            Outline = true;
            OutlineColour = Color.Black;
            DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                         TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix;
            IsControl = false;
        }

        public override void OnParentChanged(DXControl oValue, DXControl nValue)
        {
            if (oValue != null)
                oValue.SizeChanged -= Parent_SizeChanged;

            base.OnParentChanged(oValue, nValue);

            if (nValue != null)
                nValue.SizeChanged += Parent_SizeChanged;

            UpdateBounds();
        }

        private void Parent_SizeChanged(object sender, EventArgs e)
        {
            UpdateBounds();
        }

        private void UpdateBounds()
        {
            if (Parent == null) return;

            Location = new Point(0, OpticalVerticalOffset);
            Size = new Size(Parent.Size.Width, DXWindow.HeaderSize);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && Parent != null)
                Parent.SizeChanged -= Parent_SizeChanged;

            base.Dispose(disposing);
        }
    }
}
