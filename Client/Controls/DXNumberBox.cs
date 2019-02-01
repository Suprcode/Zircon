using System.Drawing;
using Library;

//Cleaned
namespace Client.Controls
{
    public sealed class DXNumberBox : DXControl
    {
        #region Properties
        public DXButton UpButton, DownButton;
        public DXNumberTextBox ValueTextBox;

        public long Value
        {
            get => ValueTextBox.Value;
            set => ValueTextBox.Value = value;
        }
        public long MinValue
        {
            get => ValueTextBox.MinValue;
            set => ValueTextBox.MinValue = value;
        }
        public long MaxValue
        {
            get => ValueTextBox.MaxValue;
            set => ValueTextBox.MaxValue = value;
        }

        public long Change = 10;
        #endregion

        public DXNumberBox()
        {
            Size = new Size(90, 18);

            ValueTextBox = new DXNumberTextBox
            {
                Size = new Size(50, 20),
                Location = new Point(19, 1),
                Parent = this,
                TextBox = { Text = "0" }
            };
            
            ValueTextBox.TextBox.KeyPress += (o, e) =>
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                    e.Handled = true;
            };
            DownButton = new DXButton
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 1011,
                Location = new Point(0, 1),
                Parent = this,
            };
            DownButton.MouseClick += (o, e) => ValueTextBox.TextBox.Text = (Value - Change).ToString();

            UpButton = new DXButton
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 1010,
                Location = new Point(73, 1),
                Parent = this,
            };
            UpButton.MouseClick += (o, e) => ValueTextBox.TextBox.Text = (Value + Change).ToString();
        }

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                Change = 0;

                if (UpButton != null)
                {
                    if (!UpButton.IsDisposed)
                        UpButton.Dispose();

                    UpButton = null;
                }

                if (DownButton != null)
                {
                    if (!DownButton.IsDisposed)
                        DownButton.Dispose();

                    DownButton = null;
                }

                if (ValueTextBox != null)
                {
                    if (!ValueTextBox.IsDisposed)
                        ValueTextBox.Dispose();

                    ValueTextBox = null;
                }
            }

        }
        #endregion
    }
}
