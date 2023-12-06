using Client.Controls;
using Client.Envir;
using Client.UserModels;
using Library;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Client.Scenes.Views
{
    public sealed class CurrencyDialog : DXWindow
    {
        #region Properties

        private DXVScrollBar ScrollBar;

        public List<CurrencyCell> Cells = new List<CurrencyCell>();

        public DXControl ClientPanel;

        public override void OnClientAreaChanged(Rectangle oValue, Rectangle nValue)
        {
            base.OnClientAreaChanged(oValue, nValue);

            if (ClientPanel == null) return;

            ClientPanel.Size = ClientArea.Size;
            ClientPanel.Location = ClientArea.Location;
        }

        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            SetCurrencies();

            base.OnIsVisibleChanged(oValue, nValue);
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

            ClientPanel = new DXControl
            {
                Parent = this,
                Size = ClientArea.Size,
                Location = ClientArea.Location,
                PassThrough = true,
            };

            ScrollBar = new DXVScrollBar
            {
                Parent = this,
                Size = new Size(14, ClientArea.Height - 1),
            };
            ScrollBar.Location = new Point(ClientArea.Right - ScrollBar.Size.Width - 2, ClientArea.Y + 1);
            ScrollBar.ValueChanged += (o, e) => UpdateLocations();

            MouseWheel += ScrollBar.DoMouseWheel;
        }

        #region Methods

        public void SetCurrencies()
        {
            foreach (CurrencyCell cell in Cells)
                cell.Dispose();

            Cells.Clear();

            foreach (var currency in GameScene.Game.User.Currencies)
            {
                CurrencyCell cell;
                Cells.Add(cell = new CurrencyCell
                {
                    Parent = ClientPanel,
                    Currency = currency
                });
                cell.MouseClick += Currency_MouseClick;
                cell.MouseWheel += ScrollBar.DoMouseWheel;
            }

            ScrollBar.MaxValue = GameScene.Game.User.Currencies.Count * 43 - 2;
            SetClientSize(new Size(ClientArea.Width, Math.Min(ScrollBar.MaxValue, 7 * 43 - 3) + 1));
            ScrollBar.VisibleSize = ClientArea.Height;
            ScrollBar.Size = new Size(ScrollBar.Size.Width, ClientArea.Height - 2);

            ScrollBar.Value = 0;
            UpdateLocations();
        }

        private void UpdateLocations()
        {
            int y = -ScrollBar.Value + 1;

            foreach (CurrencyCell cell in Cells)
            {
                cell.Location = new Point(1, y);

                y += cell.Size.Height + 3;
            }
        }


        private void Currency_MouseClick(object sender, MouseEventArgs e)
        {
            if (GameScene.Game.SelectedCell == null)
            {
                var cell = (CurrencyCell)sender;

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
                if (ScrollBar != null)
                {
                    if (!ScrollBar.IsDisposed)
                        ScrollBar.Dispose();

                    ScrollBar = null;
                }

                if (ClientPanel != null)
                {
                    if (!ClientPanel.IsDisposed)
                        ClientPanel.Dispose();

                    ClientPanel = null;
                }

                if (Cells != null)
                {
                    for (int i = 0; i < Cells.Count; i++)
                    {
                        if (Cells[i] != null)
                        {
                            if (!Cells[i].IsDisposed)
                                Cells[i].Dispose();

                            Cells[i] = null;
                        }
                    }

                    Cells.Clear();
                    Cells = null;
                }
            }

        }

        #endregion
    }

    public sealed class CurrencyCell : DXControl
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
            }
            else
            {
                DropItemCell.Item = null;
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

        #endregion

        public CurrencyCell()
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
            }

        }

        #endregion
    }
}
