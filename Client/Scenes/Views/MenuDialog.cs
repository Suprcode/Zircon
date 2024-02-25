using Client.Controls;
using Client.Envir;
using Client.UserModels;
using Library;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Client.Scenes.Views
{
    public class MenuDialog : DXImageControl
    {
        public DXLabel TitleLabel;

        public DXButton CloseButton;
        public DXButton SettingsButton, GuildButton, StorageButton, RankingButton, CompanionButton, LeaveButton;

        public WindowSetting Settings;
        public WindowType Type => WindowType.MenuBox;

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
            Settings.Location = Location;
        }

        public void ApplySettings()
        {
            if (Settings == null) return;

            Location = Settings.Location;

            Visible = Settings.Visible;
        }

        public override void OnLocationChanged(Point oValue, Point nValue)
        {
            base.OnLocationChanged(oValue, nValue);

            if (Settings != null && IsMoving)
                Settings.Location = nValue;
        }

        public override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            switch (e.KeyCode)
            {
                case Keys.Escape:
                    if (CloseButton.Visible)
                    {
                        CloseButton.InvokeMouseClick();
                        if (!Config.EscapeCloseAll)
                            e.Handled = true;
                    }
                    break;
            }
        }

        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            if (IsVisible)
                BringToFront();

            base.OnIsVisibleChanged(oValue, nValue);
        }

        public MenuDialog()
        {
            LibraryFile = LibraryFile.Interface;
            Index = 280;
            Sort = true;
            Movable = true;

            CloseButton = new DXButton
            {
                Parent = this,
                Index = 15,
                LibraryFile = LibraryFile.Interface,
            };
            CloseButton.Location = new Point(152 - CloseButton.Size.Width - 3, 3);
            CloseButton.MouseClick += (o, e) => Visible = false;

            TitleLabel = new DXLabel
            {
                Text = CEnvir.Language.MenuDialogTitle,
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
            };
            TitleLabel.Location = new Point((DisplayArea.Width - TitleLabel.Size.Width) / 2, 8);

            SettingsButton = new DXButton
            {
                Location = new Point(26, 40),
                Size = new Size(100, DefaultHeight),
                Parent = this,
                Label = { Text = CEnvir.Language.MenuDialogSettingsButtonLabel },
                //Hint = CEnvir.Language.MenuDialogSettingsButtonHint
            };
            SettingsButton.MouseClick += (o, e) => GameScene.Game.ConfigBox.Visible = !GameScene.Game.ConfigBox.Visible;

            GuildButton = new DXButton
            {
                Location = new Point(26, 70),
                Size = new Size(100, DefaultHeight),
                Parent = this,
                Label = { Text = CEnvir.Language.MenuDialogGuildButtonLabel },
                //Hint = CEnvir.Language.MenuDialogGuildButtonHint
            };
            GuildButton.MouseClick += (o, e) => GameScene.Game.GuildBox.Visible = !GameScene.Game.GuildBox.Visible;

            StorageButton = new DXButton
            {
                Location = new Point(26, 100),
                Size = new Size(100, DefaultHeight),
                Parent = this,
                Label = { Text = CEnvir.Language.MenuDialogStorageButtonLabel },
                //Hint = CEnvir.Language.MenuDialogStorageButtonHint
            };
            StorageButton.MouseClick += (o, e) => GameScene.Game.StorageBox.Visible = !GameScene.Game.StorageBox.Visible;

            RankingButton = new DXButton
            {
                Location = new Point(26, 130),
                Size = new Size(100, DefaultHeight),
                Parent = this,
                Label = { Text = CEnvir.Language.MenuDialogRankingButtonLabel },
                //Hint = CEnvir.Language.MenuDialogRankingButtonHint
            };
            RankingButton.MouseClick += (o, e) => GameScene.Game.RankingBox.Visible = !GameScene.Game.RankingBox.Visible;

            CompanionButton = new DXButton
            {
                Location = new Point(26, 160),
                Size = new Size(100, DefaultHeight),
                Parent = this,
                Label = { Text = CEnvir.Language.MenuDialogCompanionButtonLabel },
                //Hint = CEnvir.Language.MenuDialogCompanionButtonHint
            };
            CompanionButton.MouseClick += (o, e) => GameScene.Game.CompanionBox.Visible = !GameScene.Game.CompanionBox.Visible;

            LeaveButton = new DXButton
            {
                Location = new Point(26, 190),
                Size = new Size(100, DefaultHeight),
                Parent = this,
                Label = { Text = CEnvir.Language.MenuDialogLeaveButtonLabel },
                //Hint = CEnvir.Language.MenuDialogLeaveButtonHint
            };
            LeaveButton.MouseClick += (o, e) => GameScene.Game.ExitBox.Visible = true;
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (TitleLabel != null)
                {
                    if (!TitleLabel.IsDisposed)
                        TitleLabel.Dispose();

                    TitleLabel = null;
                }

                if (CloseButton != null)
                {
                    if (!CloseButton.IsDisposed)
                        CloseButton.Dispose();

                    CloseButton = null;
                }

                if (SettingsButton != null)
                {
                    if (!SettingsButton.IsDisposed)
                        SettingsButton.Dispose();

                    SettingsButton = null;
                }

                if (GuildButton != null)
                {
                    if (!GuildButton.IsDisposed)
                        GuildButton.Dispose();

                    GuildButton = null;
                }

                if (StorageButton != null)
                {
                    if (!StorageButton.IsDisposed)
                        StorageButton.Dispose();

                    StorageButton = null;
                }

                if (RankingButton != null)
                {
                    if (!RankingButton.IsDisposed)
                        RankingButton.Dispose();

                    RankingButton = null;
                }

                if (CompanionButton != null)
                {
                    if (!CompanionButton.IsDisposed)
                        CompanionButton.Dispose();

                    CompanionButton = null;
                }

                if (LeaveButton != null)
                {
                    if (!LeaveButton.IsDisposed)
                        LeaveButton.Dispose();

                    LeaveButton = null;
                }
            }
        }

        #endregion
    }
}
