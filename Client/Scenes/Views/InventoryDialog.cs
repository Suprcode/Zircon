using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Client.Controls;
using Client.Envir;
using Client.Models;
using Client.UserModels;
using Library;

//Cleaned
namespace Client.Scenes.Views
{
    public sealed class InventoryDialogOld : DXWindow
    {
        #region Properties

        public DXItemGrid Grid;

        public DXLabel GoldLabel, WeightLabel;
        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            if (!IsVisible)
                Grid.ClearLinks();

            base.OnIsVisibleChanged(oValue, nValue);
        }

        public override WindowType Type => WindowType.InventoryBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisiblity => true;

        #endregion

        public InventoryDialogOld()
        {
            TitleLabel.Text = "Inventory";
            
            Grid = new DXItemGrid
            {
                GridSize = new Size(7, 7),
                Parent = this,
                ItemGrid = GameScene.Game.Inventory,
                GridType = GridType.Inventory
            };

            SetClientSize(new Size(Grid.Size.Width, Grid.Size.Height+ 45));
            Grid.Location = ClientArea.Location;


            GoldLabel = new DXLabel
            {
                AutoSize = false,
                Border = true,
                BorderColour = Color.FromArgb(99, 83, 50),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter,
                Parent = this,
                Location = new Point(ClientArea.Left + 80, ClientArea.Bottom - 41),
                Text = "0",
                Size = new Size(ClientArea.Width - 81, 20),
                Sound = SoundIndex.GoldPickUp
            };
            GoldLabel.MouseClick += GoldLabel_MouseClick;

            new DXLabel
            {
                AutoSize = false,
                Border = true,
                BorderColour = Color.FromArgb(99, 83, 50),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Parent = this,
                Location = new Point(ClientArea.Left + 1, ClientArea.Bottom - 41),
                Text = "Gold",
                Size = new Size(78, 20),
                IsControl = false,
            };


            WeightLabel = new DXLabel
            {
                AutoSize = false,
                Border = true,
                BorderColour = Color.FromArgb(99, 83, 50),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter,
                Parent = this,
                Location = new Point(ClientArea.Left + 80, ClientArea.Bottom - 20),
                Text = "0",
                Size = new Size(ClientArea.Width - 81, 20),
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
                Location = new Point(ClientArea.Left + 1, ClientArea.Bottom - 20),
                Text = "Weight",
                Size = new Size(78, 20),
                IsControl = false,
            };
        }

        #region Methods
        private void GoldLabel_MouseClick(object sender, MouseEventArgs e)
        {
            if (GameScene.Game.SelectedCell == null)
                GameScene.Game.GoldPickedUp = !GameScene.Game.GoldPickedUp && MapObject.User.Gold.Amount > 0;
        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (Grid != null)
                {
                    if (!Grid.IsDisposed)
                        Grid.Dispose();

                    Grid = null;
                }

                if (GoldLabel != null)
                {
                    if (!GoldLabel.IsDisposed)
                        GoldLabel.Dispose();

                    GoldLabel = null;
                }

                if (WeightLabel != null)
                {
                    if (!WeightLabel.IsDisposed)
                        WeightLabel.Dispose();

                    WeightLabel = null;
                }
            }

        }

        #endregion
    }

    public sealed class InventoryDialog : DXImageControl
    {
        #region Properties

        public DXItemGrid Grid;

        public DXLabel TitleLabel, GoldLabel, GameGoldLabel, WeightLabel, CurrencyLabel, GoldTitle, GameGoldTitle;
        public DXButton CloseButton, SortButton, TrashButton;

        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            if (!IsVisible)
                Grid.ClearLinks();

            if (IsVisible)
                BringToFront();

            if (Settings != null)
                Settings.Visible = nValue;

            base.OnIsVisibleChanged(oValue, nValue);
        }

        public override void OnLocationChanged(Point oValue, Point nValue)
        {
            base.OnLocationChanged(oValue, nValue);

            if (Settings != null && IsMoving)
                Settings.Location = nValue;
        }

        #endregion

        #region Settings

        public WindowSetting Settings;
        public WindowType Type => WindowType.InventoryBox;

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

        public InventoryDialog()
        {
            LibraryFile = LibraryFile.Interface;
            Index = 130;
            Movable = true;

            CloseButton = new DXButton
            {
                Parent = this,
                Index = 15,
                LibraryFile = LibraryFile.Interface,
            };
            CloseButton.Location = new Point(DisplayArea.Width - CloseButton.Size.Width - 5, 5);
            CloseButton.MouseClick += (o, e) => Visible = false;

            TitleLabel = new DXLabel
            {
                Text = "Inventory",
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
            };
            TitleLabel.Location = new Point((DisplayArea.Width - TitleLabel.Size.Width) / 2, 8);

            Grid = new DXItemGrid
            {
                GridSize = new Size(6, 8),
                Parent = this,
                ItemGrid = GameScene.Game.Inventory,
                GridType = GridType.Inventory,
                Location = new Point(20, 39),
                GridPadding = 1,
                BackColour = Color.Empty,
                Border = false
            };

            CEnvir.LibraryList.TryGetValue(LibraryFile.GameInter, out MirLibrary library);

            DXControl WeightBar = new DXControl
            {
                Parent = this,
                Location = new Point(53, 355),
                Size = library.GetSize(360),
            };
            WeightBar.BeforeDraw += (o, e) =>
            {
                if (library == null) return;

                if (MapObject.User.Stats[Stat.BagWeight] == 0) return;

                float percent = Math.Min(1, Math.Max(0, MapObject.User.BagWeight / (float)MapObject.User.Stats[Stat.BagWeight]));

                if (percent == 0) return;

                MirImage image = library.CreateImage(360, ImageType.Image);

                if (image == null) return;

                PresentTexture(image.Image, this, new Rectangle(WeightBar.DisplayArea.X, WeightBar.DisplayArea.Y, (int)(image.Width * percent), image.Height), Color.White, WeightBar);
            };

            WeightLabel = new DXLabel
            {
                Parent = this,
                ForeColour = Color.White,
                Outline = true,
                OutlineColour = Color.Black,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter
            };
            WeightLabel.SizeChanged += (o, e) =>
            {
                WeightLabel.Location = new Point(WeightBar.Location.X + (WeightBar.Size.Width - WeightLabel.Size.Width) / 2, WeightBar.Location.Y - 1 + (WeightBar.Size.Height - WeightLabel.Size.Height) / 2);
            };

            GoldTitle = new DXLabel
            {
                AutoSize = false,
                ForeColour = Color.Goldenrod,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.Left,
                Parent = this,
                Location = new Point(55, 381),
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Bold),
                Text = "Gold",
                Size = new Size(97, 20)
            };

            GoldLabel = new DXLabel
            {
                AutoSize = false,
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter  | TextFormatFlags.Right,
                Parent = this,
                Location = new Point(80, 381),
                Text = "0",
                Size = new Size(97, 20),
                Sound = SoundIndex.GoldPickUp
            };
            GoldLabel.MouseClick += GoldLabel_MouseClick;

            GameGoldTitle = new DXLabel
            {
                AutoSize = false,
                ForeColour = Color.DarkOrange,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.Left,
                Parent = this,
                Location = new Point(55, 400),
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Bold),
                Text = "GG",
                Size = new Size(97, 20)
            };

            GameGoldLabel = new DXLabel
            {
                AutoSize = false,
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.Right,
                Parent = this,
                Location = new Point(80, 400),
                Text = "0",
                Size = new Size(97, 20)
            };

            SortButton = new DXButton
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 364,
                Parent = this,
                Location = new Point(180, 384),
                Hint = "Sort",
                Enabled = false
            };

            TrashButton = new DXButton
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 358,
                Parent = this,
                Location = new Point(218, 384),
                Hint = "Trash",
                Enabled = false
            };

            CurrencyLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(8, 380),
                Hint = "Wallet [Ctrl + C]",
                Size = new Size(45, 40),
                Sound = SoundIndex.GoldPickUp
            };
            CurrencyLabel.MouseClick += CurrencyLabel_MouseClick;
        }

        #region Methods

        private void GoldLabel_MouseClick(object sender, MouseEventArgs e)
        {
            if (GameScene.Game.SelectedCell == null)
                GameScene.Game.GoldPickedUp = !GameScene.Game.GoldPickedUp && MapObject.User.Gold.Amount > 0;
        }

        private void CurrencyLabel_MouseClick(object sender, MouseEventArgs e)
        {
            GameScene.Game.CurrencyBox.Visible = !GameScene.Game.CurrencyBox.Visible;
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (Grid != null)
                {
                    if (!Grid.IsDisposed)
                        Grid.Dispose();

                    Grid = null;
                }


                if (TitleLabel != null)
                {
                    if (!TitleLabel.IsDisposed)
                        TitleLabel.Dispose();

                    TitleLabel = null;
                }

                if (GoldLabel != null)
                {
                    if (!GoldLabel.IsDisposed)
                        GoldLabel.Dispose();

                    GoldLabel = null;
                }

                if (GameGoldLabel != null)
                {
                    if (!GameGoldLabel.IsDisposed)
                        GameGoldLabel.Dispose();

                    GameGoldLabel = null;
                }

                if (WeightLabel != null)
                {
                    if (!WeightLabel.IsDisposed)
                        WeightLabel.Dispose();

                    WeightLabel = null;
                }

                if (CurrencyLabel != null)
                {
                    if (!CurrencyLabel.IsDisposed)
                        CurrencyLabel.Dispose();

                    CurrencyLabel = null;
                }

                if (CloseButton != null)
                {
                    if (!CloseButton.IsDisposed)
                        CloseButton.Dispose();

                    CloseButton = null;
                }

                if (SortButton != null)
                {
                    if (!SortButton.IsDisposed)
                        SortButton.Dispose();

                    SortButton = null;
                }

                if (TrashButton != null)
                {
                    if (!TrashButton.IsDisposed)
                        TrashButton.Dispose();

                    TrashButton = null;
                }

                if (GoldTitle != null)
                {
                    if (!GoldTitle.IsDisposed)
                        GoldTitle.Dispose();

                    GameGoldTitle = null;
                }

                if (GameGoldTitle != null)
                {
                    if (!GameGoldTitle.IsDisposed)
                        GameGoldTitle.Dispose();

                    GameGoldTitle = null;
                }
            }
        }

        #endregion
    }
}