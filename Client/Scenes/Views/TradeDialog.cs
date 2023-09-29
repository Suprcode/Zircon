using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Client.Controls;
using Client.Envir;
using Client.UserModels;
using Library;
using C = Library.Network.ClientPackets;

namespace Client.Scenes.Views
{
    public sealed class TradeDialog : DXImageControl
    {
        #region Properties

        public DXLabel TitleLabel, UserLabel, PlayerLabel;
        public DXItemGrid UserGrid, PlayerGrid;
        public DXLabel UserGoldLabel, PlayerGoldLabel;
        public DXButton CloseButton, ConfirmButton;

        public ClientUserItem[] PlayerItems;
        public bool IsTrading;

        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);
            
            UserGrid.ClearLinks();

            if (!IsTrading || GameScene.Game.Observer) return;

            CEnvir.Enqueue(new C.TradeClose());
        }

        #region Settings

        public WindowSetting Settings;
        public WindowType Type => WindowType.TradeBox;

        public void LoadSettings()
        {
            if (Type == WindowType.None || !CEnvir.Loaded) return;

            Settings = CEnvir.WindowSettings.Binding.FirstOrDefault(x => x.Resolution == Config.GameSize && x.Window == Type);

            if (Settings != null)
            {
                ApplySettings();
                return;
            }

            Settings = CEnvir.WindowSettings.CreateNewObject();
            Settings.Resolution = Config.GameSize;
            Settings.Window = Type;
            Settings.Size = Size;
            Settings.Visible = Visible;
            Settings.Location = Location;
        }

        public void ApplySettings()
        {
            if (Settings == null) return;

            Location = Settings.Location;

            Visible = Settings.Visible;
        }

        #endregion

        #endregion

        public TradeDialog()
        {
            LibraryFile = LibraryFile.Interface;
            Index = 125;
            Movable = true;

            CloseButton = new DXButton
            {
                Parent = this,
                Index = 15,
                LibraryFile = LibraryFile.Interface,
            };
            CloseButton.Location = new Point(DisplayArea.Width - CloseButton.Size.Width - 3, 3);
            CloseButton.MouseClick += (o, e) => Visible = false;

            TitleLabel = new DXLabel
            {
                Text = CEnvir.Language.TradeDialogTitle,
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
            };
            TitleLabel.Location = new Point((DisplayArea.Width - TitleLabel.Size.Width) / 2, 8);

            Location = new Point(40, 40);

            UserGrid = new DXItemGrid
            {
                GridSize = new Size(5, 2),
                Parent = this,
                Location = new Point(15, 73),
                GridType = GridType.TradeUser,
                Linked = true,
                GridPadding = 1,
                BackColour = Color.Empty,
                Border = false
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
                GridSize = new Size(5, 2),
                Parent = this,
                Location = new Point(UserGrid.Location.X + UserGrid.Size.Width + 25, 73),
                ItemGrid = PlayerItems = new ClientUserItem[15],
                GridType = GridType.TradePlayer,
                ReadOnly = true,
                GridPadding = 1,
                BackColour = Color.Empty,
                Border = false
            };

            UserLabel = new DXLabel
            {
                Text = CEnvir.Language.TradeDialogUserLabel,
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
            };
            UserLabel.SizeChanged += (o, e) => UserLabel.Location = new Point(UserGrid.Location.X + (UserGrid.Size.Width - UserLabel.Size.Width) / 2, 38);
            UserLabel.Location = new Point(UserGrid.Location.X + (UserGrid.Size.Width - UserLabel.Size.Width) / 2, 38);

            PlayerLabel = new DXLabel
            {
                Text = CEnvir.Language.TradeDialogPlayerLabel,
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
            };
            PlayerLabel.SizeChanged += (o, e) => PlayerLabel.Location = new Point(PlayerGrid.Location.X + (PlayerGrid.Size.Width - PlayerLabel.Size.Width) / 2, 38);
            PlayerLabel.Location = new Point(PlayerGrid.Location.X + (PlayerGrid.Size.Width - PlayerLabel.Size.Width) / 2, 38);

            UserGoldLabel = new DXLabel
            {
                AutoSize = false,
                Border = false,
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.Right,
                Parent = this,
                Location = new Point(UserGrid.Location.X + 60, UserGrid.Location.Y + UserGrid.Size.Height + 20),
                Text = "0",
                Size = new Size(UserGrid.Size.Width - 56, 15),
                Sound = SoundIndex.GoldPickUp
            };
            UserGoldLabel.MouseClick += UserGoldLabel_MouseClick;

            new DXLabel
            {
                AutoSize = false,
                Border = false,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Bold),
                ForeColour = Color.Goldenrod,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.Left,
                Parent = this,
                Location = new Point(UserGrid.Location.X - 4, UserGrid.Location.Y + UserGrid.Size.Height + 20),
                Text = CEnvir.Language.TradeDialogGoldLabel,
                Size = new Size(63, 15),
                IsControl = false,
            };

            PlayerGoldLabel = new DXLabel
            {
                AutoSize = false,
                Border = false,
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.Right,
                Parent = this,
                Location = new Point(PlayerGrid.Location.X + 60, UserGrid.Location.Y + UserGrid.Size.Height + 20),
                Text = "0",
                Size = new Size(UserGrid.Size.Width - 56, 15),
                Sound = SoundIndex.GoldPickUp
            };

            new DXLabel
            {
                AutoSize = false,
                Border = false,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Bold),
                ForeColour = Color.Goldenrod,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.Left,
                Parent = this,
                Location = new Point(PlayerGrid.Location.X - 4, UserGrid.Location.Y + UserGrid.Size.Height + 20),
                Text = CEnvir.Language.TradeDialogGoldLabel,
                Size = new Size(63, 15),
                IsControl = false,
            };

            ConfirmButton = new DXButton
            {
                Parent = this,
                Location = new Point(UserGrid.Location.X + UserGrid.Size.Width - 75, UserGoldLabel.Location.Y + 35),
                Label = { Text = CEnvir.Language.CommonControlConfirm },
                ButtonType = ButtonType.SmallButton,
                Size = new Size(80, SmallButtonHeight),
            };
            ConfirmButton.MouseClick += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                ConfirmButton.Enabled = false;

                CEnvir.Enqueue(new C.TradeConfirm());
            };
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

                if (TitleLabel != null)
                {
                    if (!TitleLabel.IsDisposed)
                        TitleLabel.Dispose();

                    TitleLabel = null;
                }

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

                if (CloseButton != null)
                {
                    if (!CloseButton.IsDisposed)
                        CloseButton.Dispose();

                    CloseButton = null;
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
