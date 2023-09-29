using System;
using System.Drawing;
using System.Windows.Forms;
using Library;

//Cleaned
namespace Client.Controls
{
    public sealed class DXCheckBox : DXControl
    {
        #region Properites

        #region Checked

        public bool Checked
        {
            get => _Checked;
            set
            {
                if (_Checked == value) return;

                bool oldValue = _Checked;
                _Checked = value;

                OnCheckedChanged(oldValue, value);
            }
        }
        private bool _Checked;
        public event EventHandler<EventArgs> CheckedChanged;
        public void OnCheckedChanged(bool oValue, bool nValue)
        {
            CheckedChanged?.Invoke(this, EventArgs.Empty);

            Box.Index = Checked ? 162 : 161;
        }

        #endregion

        #region ReadOnly

        public bool ReadOnly
        {
            get => _ReadOnly;
            set
            {
                if (_ReadOnly == value) return;

                bool oldValue = _ReadOnly;
                _ReadOnly = value;

                OnReadOnlyChanged(oldValue, value);
            }
        }
        private bool _ReadOnly;
        public event EventHandler<EventArgs> ReadOnlyChanged;
        public void OnReadOnlyChanged(bool oValue, bool nValue)
        {
            ReadOnlyChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public DXLabel Label { get; private set; }
        public DXImageControl Box { get; private set; }
        
        public override void OnDisplayAreaChanged(Rectangle oValue, Rectangle nValue)
        {
            base.OnDisplayAreaChanged(oValue, nValue);

            UpdateControl();
        }

        #endregion
        
        public DXCheckBox()
        {
            Label = new DXLabel
            {
                Parent = this,
                IsControl = false,
                Location = new Point(0, 0)
            };

            Label.DisplayAreaChanged += (o, e) =>
            {
                Size = new Size(Label.DisplayArea.Width + Box.DisplayArea.Width, Box.DisplayArea.Height);
                Box.Location = new Point(Label.DisplayArea.Width, 1);
            };


            Box = new DXImageControl
            {
                Location = new Point(Label.Size.Width + 2, 1),
                Index = 161,
                LibraryFile = LibraryFile.GameInter,
                Parent = this,
                IsControl = false,
            };
            Box.DisplayAreaChanged += (o, e) =>
            {
                Size = new Size(Label.DisplayArea.Width + Box.DisplayArea.Width, Box.DisplayArea.Height);
                Box.Location = new Point(Label.DisplayArea.Width, 1);
            };


            Size = new Size(18, 18);
        }

        #region Methods

        private void UpdateControl()
        {
            if (Label == null) return;

            Label.Location = new Point(0, 0);
            Size = new Size(Label.DisplayArea.Width + Box.DisplayArea.Width, Box.DisplayArea.Height);
            Box.Location = new Point(Label.DisplayArea.Width, 0);
        }

        public override void OnMouseClick(MouseEventArgs e)
        {
            if (!IsEnabled) return;

            base.OnMouseClick(e);

            if (ReadOnly) return;

            Checked = !Checked;
        }

        public void SetSilentState(bool check)
        {
            _Checked = check;
            Box.Index = Checked ? 162 : 161;
        }

        #endregion

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (Label != null)
                {
                    if (!Label.IsDisposed)
                        Label.Dispose();
                    Label = null;
                }

                if (Box != null)
                {
                    if (!Box.IsDisposed)
                        Box.Dispose();
                    Box = null;
                }

                _Checked = false;
                CheckedChanged = null;

                _ReadOnly = false;
                ReadOnlyChanged = null;
            }
        }
        #endregion
    }
}
