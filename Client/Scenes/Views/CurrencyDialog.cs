using Client.Controls;
using Client.Envir;
using Client.UserModels;
using Library;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Client.Scenes.Views
{
    public sealed class CurrencyDialog : DXWindow
    {
        #region Properties

        private CurrencyTree BindTree;

        public override void OnClientAreaChanged(Rectangle oValue, Rectangle nValue)
        {
            base.OnClientAreaChanged(oValue, nValue);
        }

        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);

            BindTree.TreeList.Clear();

            LoadList();
        }

        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        public CurrencyDialog()
        {
            TitleLabel.Text = CEnvir.Language.CurrencyDialogTitle;

            HasFooter = false;
            Movable = true;

            SetClientSize(new Size(227, 7 * 43 + 1));

            BindTree = new CurrencyTree
            {
                Parent = this,
                Location = new Point(ClientArea.X, ClientArea.Y),
                Size = new Size(ClientArea.Width, ClientArea.Height)
            };
        }

        public void LoadList()
        {
            BindTree.TreeList.Clear();

            foreach (ClientUserCurrency bind in GameScene.Game.User.Currencies.OrderBy(x => x.Info.Category))
            {
                List<ClientUserCurrency> list;

                var category = bind.Info.Category.ToString();

                if (!BindTree.TreeList.TryGetValue(category, out list))
                    BindTree.TreeList[category] = list = new List<ClientUserCurrency>();

                list.Add(bind);
            }

            foreach (KeyValuePair<string, List<ClientUserCurrency>> pair in BindTree.TreeList)
                pair.Value.Sort((x1, x2) => String.Compare(x1.Info.Name, x2.Info.Name, StringComparison.Ordinal));

            BindTree.ListChanged();
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (BindTree != null)
                {
                    if (!BindTree.IsDisposed)
                        BindTree.Dispose();

                    BindTree = null;
                }
            }
        }

        #endregion
    }

    public class CurrencyTree : DXControl
    {

        #region Properties

        #region SelectedEntry

        public CurrencyItem SelectedEntry
        {
            get => _SelectedEntry;
            set
            {
                CurrencyItem oldValue = _SelectedEntry;
                _SelectedEntry = value;

                OnSelectedEntryChanged(oldValue, value);
            }
        }
        private CurrencyItem _SelectedEntry;
        public event EventHandler<EventArgs> SelectedEntryChanged;
        public virtual void OnSelectedEntryChanged(CurrencyItem oValue, CurrencyItem nValue)
        {
            SelectedEntryChanged?.Invoke(this, EventArgs.Empty);

            if (oValue != null)
                oValue.Selected = false;

            if (nValue != null)
                nValue.Selected = true;
        }

        #endregion

        public static Dictionary<string, bool> ExpandedInfo = new Dictionary<string, bool>();
        public Dictionary<string, List<ClientUserCurrency>> TreeList = new Dictionary<string, List<ClientUserCurrency>>();

        private DXVScrollBar ScrollBar;

        public List<DXControl> Lines = new List<DXControl>();

        private const int HeaderHeight = 22;
        private const int CurrencyHeight = 42;
        private int HeaderCount = 0;
        private int CurrencyCount = 0;
        private int TotalCount => (HeaderCount * HeaderHeight) + (CurrencyCount * CurrencyHeight);

        public override void OnSizeChanged(Size oValue, Size nValue)
        {
            base.OnSizeChanged(oValue, nValue);

            ScrollBar.Size = new Size(14, Size.Height);
            ScrollBar.Location = new Point(Size.Width - 14, 0);
            ScrollBar.VisibleSize = Size.Height;
        }

        #endregion

        public CurrencyTree()
        {
            Border = true;
            BorderColour = Color.FromArgb(198, 166, 99);

            ScrollBar = new DXVScrollBar
            {
                Parent = this,
                Change = 22,
            };
            ScrollBar.ValueChanged += (o, e) => UpdateScrollBar();

            MouseWheel += ScrollBar.DoMouseWheel;
        }

        #region Methods

        public void UpdateScrollBar()
        {
            ScrollBar.MaxValue = TotalCount;

            int current = 0;

            for (int i = 0; i < Lines.Count; i++)
            {
                Lines[i].Location = new Point(Lines[i].Location.X, current - ScrollBar.Value);

                if (Lines[i] is CurrencyTreeHeader)
                {
                    current += HeaderHeight;
                }
                else if (Lines[i] is CurrencyItem)
                {
                    current += CurrencyHeight;
                }
            }
        }

        public void ListChanged()
        {
            ClientUserCurrency selectedKeyBind = SelectedEntry?.Currency;

            foreach (DXControl control in Lines)
                control.Dispose();

            Lines.Clear();
            HeaderCount = 0;
            CurrencyCount = 0;

            _SelectedEntry = null;
            CurrencyItem firstEntry = null;

            foreach (KeyValuePair<string, List<ClientUserCurrency>> pair in TreeList)
            {
                CurrencyTreeHeader header = new CurrencyTreeHeader
                {
                    Parent = this,
                    Location = new Point(1, TotalCount),
                    Size = new Size(Size.Width - 17, HeaderHeight - 2),
                    HeaderLabel = { Text = pair.Key }
                };
                header.ExpandButton.MouseClick += (o, e) => ListChanged();
                header.MouseWheel += ScrollBar.DoMouseWheel;
                Lines.Add(header);
                HeaderCount++;

                bool expanded;

                if (!ExpandedInfo.TryGetValue(header.HeaderLabel.Text, out expanded))
                    ExpandedInfo[header.HeaderLabel.Text] = expanded = true;

                header.Expanded = expanded;

                if (!header.Expanded) continue;

                foreach (ClientUserCurrency KeyBind in pair.Value)
                {
                    CurrencyItem entry = new CurrencyItem
                    {
                        Parent = this,
                        Location = new Point(1, TotalCount),
                        Size = new Size(Size.Width - 17, CurrencyHeight - 2),
                        Currency = KeyBind,
                        Selected = KeyBind == selectedKeyBind,
                    };

                    entry.MouseWheel += ScrollBar.DoMouseWheel;

                    entry.CurrencyNameLabel.MouseWheel += ScrollBar.DoMouseWheel;
                    entry.AmountLabel.MouseWheel += ScrollBar.DoMouseWheel;
                    entry.MouseClick += Currency_MouseClick;
                    entry.DropItemCell.MouseClick += (o, e) => Currency_MouseClick(entry, e);
                    entry.MouseWheel += ScrollBar.DoMouseWheel;

                    if (firstEntry == null)
                        firstEntry = entry;

                    if (entry.Selected)
                        SelectedEntry = entry;

                    Lines.Add(entry);
                    CurrencyCount++;
                }
            }

            UpdateScrollBar();
        }

        private void Currency_MouseClick(object sender, MouseEventArgs e)
        {
            if (GameScene.Game.SelectedCell == null)
            {
                var cell = (CurrencyItem)sender;

                if (cell.Currency == null || !cell.Currency.CanPickup) return;

                DXSoundManager.Play(SoundIndex.GoldPickUp);

                if (GameScene.Game.CurrencyPickedUp == null && cell.Currency.Amount > 0)
                    GameScene.Game.CurrencyPickedUp = cell.Currency;
                else
                    GameScene.Game.CurrencyPickedUp = null;
            }
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                TreeList.Clear();
                TreeList = null;

                _SelectedEntry = null;
                SelectedEntryChanged = null;

                if (ScrollBar != null)
                {
                    if (!ScrollBar.IsDisposed)
                        ScrollBar.Dispose();

                    ScrollBar = null;
                }

                if (Lines != null)
                {
                    for (int i = 0; i < Lines.Count; i++)
                    {
                        if (Lines[i] != null)
                        {
                            if (!Lines[i].IsDisposed)
                                Lines[i].Dispose();

                            Lines[i] = null;
                        }

                    }

                    Lines.Clear();
                    Lines = null;
                }
            }

        }

        #endregion
    }

    public sealed class CurrencyTreeHeader : DXControl
    {
        #region Properties

        #region Expanded

        public bool Expanded
        {
            get => _Expanded;
            set
            {
                if (_Expanded == value) return;

                bool oldValue = _Expanded;
                _Expanded = value;

                OnExpandedChanged(oldValue, value);
            }
        }
        private bool _Expanded;
        public event EventHandler<EventArgs> ExpandedChanged;
        public void OnExpandedChanged(bool oValue, bool nValue)
        {
            ExpandButton.Index = Expanded ? 4871 : 4870;
            CurrencyTree.ExpandedInfo[HeaderLabel.Text] = Expanded;

            ExpandedChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public DXButton ExpandButton;
        public DXLabel HeaderLabel;

        #endregion

        public CurrencyTreeHeader()
        {
            ExpandButton = new DXButton
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter,
                Index = 4870,
                Location = new Point(2, 2)
            };
            ExpandButton.MouseClick += (o, e) => Expanded = !Expanded;

            HeaderLabel = new DXLabel
            {
                Parent = this,
                ForeColour = Color.White,
                IsControl = false,
                Location = new Point(25, 2)
            };
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _Expanded = false;
                ExpandedChanged = null;

                if (ExpandButton != null)
                {
                    if (!ExpandButton.IsDisposed)
                        ExpandButton.Dispose();

                    ExpandButton = null;
                }

                if (HeaderLabel != null)
                {
                    if (!HeaderLabel.IsDisposed)
                        HeaderLabel.Dispose();

                    HeaderLabel = null;
                }
            }

        }

        #endregion
    }

    public sealed class CurrencyItem : DXControl
    {
        #region Properties

        #region Currency

        public ClientUserCurrency Currency
        {
            get => _Currency;
            set
            {
                if (_Currency == value) return;

                ClientUserCurrency oldValue = _Currency;
                _Currency = value;

                OnCurrencyChanged(oldValue, value);
            }
        }
        private ClientUserCurrency _Currency;
        public event EventHandler<EventArgs> CurrencyChanged;
        public void OnCurrencyChanged(ClientUserCurrency oValue, ClientUserCurrency nValue)
        {
            if (Currency.Info.DropItem != null)
            {
                DropItemCell.Item = new ClientUserItem(Currency.Info.DropItem, Currency.Amount);
                CurrencyImage.Visible = false;
            }
            else
            {
                DropItemCell.Item = null;
                CurrencyImage.Visible = true;
                CurrencyImage.Index = 2683;
            }

            CurrencyNameLabel.Text = Currency.Info.Name;

            UpdateAmount();

            CurrencyChanged?.Invoke(this, EventArgs.Empty);
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
        public void OnSelectedChanged(bool oValue, bool nValue)
        {
            Border = Selected;
            //BackColour = Selected ? Color.FromArgb(80, 80, 125) : Color.FromArgb(25, 20, 0);
            DropItemCell.BorderColour = Selected ? Color.FromArgb(198, 166, 99) : Color.FromArgb(99, 83, 50);
            SelectedChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public DXItemCell DropItemCell;

        public DXLabel CurrencyNameLabel, AmountLabel;

        public DXImageControl CurrencyImage;

        #endregion

        public CurrencyItem()
        {
            DrawTexture = true;
            BackColour = Color.FromArgb(25, 20, 0);

            BorderColour = Color.FromArgb(198, 166, 99);
            Size = new Size(219, 40);

            DropItemCell = new DXItemCell
            {
                Parent = this,
                Location = new Point((Size.Height - DXItemCell.CellHeight) / 2, (Size.Height - DXItemCell.CellHeight) / 2),
                FixedBorder = true,
                Border = true,
                ReadOnly = true,
                ItemGrid = new ClientUserItem[1],
                Slot = 0,
                FixedBorderColour = true,
                ShowCountLabel = false,
            };

            CurrencyImage = new DXImageControl
            {
                Parent = this,
                Location = new Point((Size.Height - DXItemCell.CellHeight) / 2 + 1, (Size.Height - DXItemCell.CellHeight) / 2 + 1),
                Visible = false,
                LibraryFile = LibraryFile.StoreItems
            };

            CurrencyNameLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(DropItemCell.Location.X * 2 + DropItemCell.Size.Width, DropItemCell.Location.Y),
                ForeColour = Color.White,
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
            };

            AmountLabel = new DXLabel
            {
                Parent = this,
                Text = "Amount",
                IsControl = false,
                ForeColour = Color.Aquamarine
            };
            AmountLabel.Location = new Point(DropItemCell.Location.X * 2 + DropItemCell.Size.Width, DropItemCell.Location.Y + DropItemCell.Size.Height - AmountLabel.Size.Height);
        }

        #region Methods

        public void UpdateAmount()
        {
            AmountLabel.Text = Currency.Amount.ToString("#,##0");
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _Currency = null;
                CurrencyChanged = null;

                _Selected = false;
                SelectedChanged = null;

                if (DropItemCell != null)
                {
                    if (!DropItemCell.IsDisposed)
                        DropItemCell.Dispose();

                    DropItemCell = null;
                }

                if (CurrencyNameLabel != null)
                {
                    if (!CurrencyNameLabel.IsDisposed)
                        CurrencyNameLabel.Dispose();

                    CurrencyNameLabel = null;
                }

                if (CurrencyImage != null)
                {
                    if (!CurrencyImage.IsDisposed)
                        CurrencyImage.Dispose();

                    CurrencyImage = null;
                }
            }

        }

        #endregion
    }
}