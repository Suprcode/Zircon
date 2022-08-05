using System.Drawing;
using System.Windows.Forms;
using Client.Controls;
using Client.Envir;
using Client.UserModels;
using Library;
using C = Library.Network.ClientPackets;

//Cleaned
namespace Client.Scenes.Views
{
    public sealed class TradeDialog :DXWindow
    {
        #region Properties

        public DXLabel UserLabel, PlayerLabel;
        public DXItemGrid UserGrid, PlayerGrid;
        public DXLabel UserGoldLabel, PlayerGoldLabel;
        public DXButton ConfirmButton;

        public ClientUserItem[] PlayerItems;
        public bool IsTrading;


        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);
            
            UserGrid.ClearLinks();

            if (!IsTrading || GameScene.Game.Observer) return;

            CEnvir.Enqueue(new C.TradeClose());
        }

        public override WindowType Type => WindowType.TradeBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;
        #endregion

        public TradeDialog()
        {
            TitleLabel.Text = "Trade Window";

            Location = new Point(40, 40);
            
            UserGrid = new DXItemGrid
            {
                GridSize = new Size(5, 3),
                Parent = this,
                Location = new Point(ClientArea.X + 5, ClientArea.Y + 25),
                GridType = GridType.TradeUser,
                Linked = true,
            };

            foreach (DXItemCell cell in UserGrid.Grid)
            {
                cell.LinkChanged += (o, e) =>
                {
                    cell.ReadOnly = cell.Item != null;

                    if (cell.Item == null || GameScene.Game.Observer) return;

                    CEnvir.Enqueue(new C.TradeAddItem
                    {
                        Cell = new CellLinkInfo { Slot = cell.Link.Slot, Count = cell.LinkedCount, GridType = cell.Link.GridType }
                    });
                };
            }

            PlayerGrid = new DXItemGrid
            {
                GridSize = new Size(5, 3),
                Parent = this,
                Location = new Point(UserGrid.Location.X + UserGrid.Size.Width + 20, ClientArea.Y + 25),
                ItemGrid = PlayerItems = new ClientUserItem[15],
                GridType = GridType.TradePlayer,
                ReadOnly =  true,
            };

            UserLabel = new DXLabel
            {
                Text = "User",
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
            };
            UserLabel.SizeChanged += (o, e) => UserLabel.Location = new Point(UserGrid.Location.X + (UserGrid.Size.Width - UserLabel.Size.Width) / 2, ClientArea.Y);
            UserLabel.Location = new Point(UserGrid.Location.X + (UserGrid.Size.Width - UserLabel.Size.Width) / 2, ClientArea.Y);


            PlayerLabel = new DXLabel
            {
                Text = "Player",
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
            };
            PlayerLabel.SizeChanged += (o, e) => PlayerLabel.Location = new Point(PlayerGrid.Location.X + (PlayerGrid.Size.Width - PlayerLabel.Size.Width) / 2, ClientArea.Y);
            PlayerLabel.Location = new Point(PlayerGrid.Location.X + (PlayerGrid.Size.Width - PlayerLabel.Size.Width) / 2, ClientArea.Y);

            UserGoldLabel = new DXLabel
            {
                AutoSize = false,
                Border = true,
                BorderColour = Color.FromArgb(99, 83, 50),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter,
                Parent = this,
                Location = new Point(UserGrid.Location.X + 60, UserGrid.Location.Y + UserGrid.Size.Height + 5),
                Text = "0",
                Size = new Size(UserGrid.Size.Width - 61, 20),
                Sound = SoundIndex.GoldPickUp
            };
            UserGoldLabel.MouseClick += UserGoldLabel_MouseClick;

            new DXLabel
            {
                AutoSize = false,
                Border = true,
                BorderColour = Color.FromArgb(99, 83, 50),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Parent = this,
                Location = new Point(UserGrid.Location.X + 1, UserGrid.Location.Y + UserGrid.Size.Height + 5),
                Text = "Gold",
                Size = new Size(58, 20),
                IsControl = false,
            };

            PlayerGoldLabel = new DXLabel
            {
                AutoSize = false,
                Border = true,
                BorderColour = Color.FromArgb(99, 83, 50),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter,
                Parent = this,
                Location = new Point(PlayerGrid.Location.X + 60, UserGrid.Location.Y + UserGrid.Size.Height + 5),
                Text = "0",
                Size = new Size(UserGrid.Size.Width - 61, 20),
                Sound = SoundIndex.GoldPickUp
            };


            new DXLabel
            {
                AutoSize = false,
                Border = true,
                BorderColour = Color.FromArgb(99, 83, 50),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Parent = this,
                Location = new Point(PlayerGrid.Location.X + 1, UserGrid.Location.Y + UserGrid.Size.Height + 5),
                Text = "Gold",
                Size = new Size(58, 20),
                IsControl = false,
            };

            ConfirmButton = new DXButton
            {
                Parent = this,
                Location = new Point(UserGrid.Location.X + UserGrid.Size.Width - 80, UserGoldLabel.Location.Y + 25),
                Label = { Text = "Confirm" },
                ButtonType = ButtonType.SmallButton,
                Size = new Size(80, SmallButtonHeight),
            };
            ConfirmButton.MouseClick += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                ConfirmButton.Enabled = false;

                CEnvir.Enqueue(new C.TradeConfirm());
            };

            SetClientSize(new Size(PlayerGrid.Size.Width * 2 + 30, PlayerGrid.Size.Height + UserLabel.Size.Height + 15 + UserGoldLabel.Size.Height + ConfirmButton.Size.Height));
        }

        #region Methods

        private void UserGoldLabel_MouseClick(object sender, MouseEventArgs e)
        {
            if (GameScene.Game.Observer) return;

            DXItemAmountWindow window = new DXItemAmountWindow("Trade Gold", new ClientUserItem(Globals.GoldInfo, GameScene.Game.User.Gold.Amount));

            window.ConfirmButton.MouseClick += (o1, e1) =>
            {
                if (window.Amount <= 0) return;

                CEnvir.Enqueue(new C.TradeAddGold { Gold = window.Amount });
            };
        }

        public void Clear()
        {
            UserGoldLabel.Text = "0";
            PlayerGoldLabel.Text = "0";
            ConfirmButton.Enabled = true;

            IsTrading = false;

            foreach (DXItemCell cell in PlayerGrid.Grid)
                cell.Item = null;
        }

        #endregion


        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                IsTrading = false;
                PlayerItems = null;

                if (UserLabel != null)
                {
                    if (!UserLabel.IsDisposed)
                        UserLabel.Dispose();

                    UserLabel = null;
                }

                if (PlayerLabel != null)
                {
                    if (!PlayerLabel.IsDisposed)
                        PlayerLabel.Dispose();

                    PlayerLabel = null;
                }

                if (UserGrid != null)
                {
                    if (!UserGrid.IsDisposed)
                        UserGrid.Dispose();

                    UserGrid = null;
                }

                if (PlayerGrid != null)
                {
                    if (!PlayerGrid.IsDisposed)
                        PlayerGrid.Dispose();

                    PlayerGrid = null;
                }

                if (UserGoldLabel != null)
                {
                    if (!UserGoldLabel.IsDisposed)
                        UserGoldLabel.Dispose();

                    UserGoldLabel = null;
                }

                if (PlayerGoldLabel != null)
                {
                    if (!PlayerGoldLabel.IsDisposed)
                        PlayerGoldLabel.Dispose();

                    PlayerGoldLabel = null;
                }

                if (ConfirmButton != null)
                {
                    if (!ConfirmButton.IsDisposed)
                        ConfirmButton.Dispose();

                    ConfirmButton = null;
                }
            }

        }

        #endregion
    }
}
