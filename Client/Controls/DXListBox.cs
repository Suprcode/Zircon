using System;
using System.Drawing;
using System.Windows.Forms;

//Cleaned
namespace Client.Controls
{
    public sealed class DXListBox : DXControl
    {
        #region Properties

        #region SelectedItem

        public DXListBoxItem SelectedItem
        {
            get => _SelectedItem;
            set
            {
                if (_SelectedItem == value) return;

                DXListBoxItem oldValue = _SelectedItem;
                _SelectedItem = value;

                OnselectedItemChanged(oldValue, value);
            }
        }
        private DXListBoxItem _SelectedItem;
        public event EventHandler<EventArgs> selectedItemChanged;
        public void OnselectedItemChanged(DXListBoxItem oValue, DXListBoxItem nValue)
        {
            if (oValue != null)
                oValue.Selected = false;

            if (nValue != null)
                nValue.Selected = true;

            selectedItemChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public DXVScrollBar ScrollBar;

        public override void OnSizeChanged(Size oValue, Size nValue)
        {
            base.OnSizeChanged(oValue, nValue);

            if (ScrollBar == null) return;

            foreach (DXControl control in Controls)
                if (control is DXListBoxItem)
                    control.Size = new Size(Size.Width - ScrollBar.Size.Width - 1, control.Size.Height);

            ScrollBar.Size = new Size(14, Size.Height);
            ScrollBar.Location = new Point(Size.Width - ScrollBar.Size.Width, 0);

            UpdateScrollBar();
        }

        #endregion

        public DXListBox()
        {
            Border = true;
            DrawTexture = true;
            BorderColour = Color.FromArgb(198, 166, 99);

            ScrollBar = new DXVScrollBar
            {
                VisibleSize = Size.Height,
                Size = new Size(14, Size.Height),
                Parent = this,
            };
            ScrollBar.ValueChanged += ScrollBar_ValueChanged;

            MouseWheel += ScrollBar.DoMouseWheel;
        }

        #region Methods
        public override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            SelectedItem = null;
        }

        public void ScrollBar_ValueChanged(object sender, EventArgs e)
        {
            UpdateItems();
        }
        public void UpdateItems()
        {
            foreach (DXControl control in Controls)
            {
                DXListBoxItem item = control as DXListBoxItem;

                item?.UpdateLocation();
            }
        }

        public void UpdateScrollBar()
        {
            ScrollBar.VisibleSize = Size.Height;

            int height = 0;

            foreach (DXControl control in Controls)
            {
                if (!(control is DXListBoxItem)) continue;

                height += control.Size.Height;
            }

            ScrollBar.MaxValue = height;
        }

        public void SelectItem(object ob)
        {
            foreach (DXControl control in Controls)
            {
                DXListBoxItem listItem = control as DXListBoxItem;
                if (listItem == null) continue;

                if (ob == null)
                {
                    if (listItem.Item != null) continue;

                    SelectedItem = listItem;
                    break;
                }
                if (!ob.Equals(listItem.Item)) continue;

                SelectedItem = listItem;

                break;
            }
        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (_SelectedItem != null)
                {
                    if (!_SelectedItem.IsDisposed)
                        _SelectedItem.Dispose();
                    _SelectedItem = null;
                }

                if (ScrollBar != null)
                {
                    if (!ScrollBar.IsDisposed)
                        ScrollBar.Dispose();
                    ScrollBar = null;
                }


            }

        }

        #endregion
    }

    public class DXListBoxItem : DXControl
    {
        #region Properties

        #region Item

        public object Item
        {
            get => _Item;
            set
            {
                if (_Item == value) return;

                object oldValue = _Item;
                _Item = value;

                OnItemChanged(oldValue, value);
            }
        }
        private object _Item;
        public event EventHandler<EventArgs> ItemChanged;
        public virtual void OnItemChanged(object oValue, object nValue)
        {
            ItemChanged?.Invoke(this, EventArgs.Empty);
        }
        
        #endregion

        #region Selected

        public bool Selected
        {
            get => _Selected;
            set
            {
                if (_Selected == value) return;

                bool oldValue = _Selected;
                _Selected = value;

                OnSelectedChanged(oldValue, value);
            }
        }
        private bool _Selected;
        public event EventHandler<EventArgs> SelectedChanged;
        public virtual void OnSelectedChanged(bool oValue, bool nValue)
        {
            UpdateColours();

            SelectedChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public DXLabel Label { get; protected set; }

        public override void OnParentChanged(DXControl oValue, DXControl nValue)
        {
            base.OnParentChanged(oValue, nValue);

            DXListBox listBox = Parent as DXListBox;
            if (listBox == null) return;

            Size = new Size(Parent.Size.Width - listBox.ScrollBar.Size.Width - 1, Label.Size.Height);

            UpdateLocation();

            listBox.UpdateScrollBar();

            MouseWheel += listBox.ScrollBar.DoMouseWheel;
        }
        #endregion

        public DXListBoxItem()
        {
            DrawTexture = true;

            Label = new DXLabel
            {
                Parent = this,
                Text = "List Box Item",
                IsControl = false
            };
        }

        #region Methods

        public override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            DXListBox listBox = Parent as DXListBox;
            if (listBox == null) return;


            listBox.SelectedItem = this;
        }

        public override void OnMouseEnter()
        {
            base.OnMouseEnter();

            UpdateColours();
        }
        public override void OnMouseLeave()
        {
            base.OnMouseLeave();

            UpdateColours();
        }


        public void UpdateLocation()
        {
            DXListBox listBox = Parent as DXListBox;

            if (listBox == null) return;

            int y = -listBox.ScrollBar.Value;

            foreach (DXControl control in Parent.Controls)
            {
                if (!(control is DXListBoxItem)) continue;
                if (control == this) break;
                y += control.Size.Height;
            }

            Location = new Point(0, y);
        }
        

        public virtual void UpdateColours()
        {
            if (Selected)
            {
                Label.ForeColour = Color.White;
                BackColour = Color.FromArgb(128, 64, 64);
            }
            else if (MouseControl == this)
            {
                Label.ForeColour = Color.FromArgb(198, 166, 99);
                BackColour = Color.FromArgb(64, 32, 32);
            }
            else
            {
                Label.ForeColour = Color.FromArgb(198, 166, 99);
                BackColour = Color.Empty;
            }
        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _Item = null;
                _Selected = false;

                ItemChanged = null;
                SelectedChanged = null;

                if (Label != null)
                {
                    if (!Label.IsDisposed)
                        Label.Dispose();

                    Label = null;
                }
            }

        }

        #endregion
    }
}
