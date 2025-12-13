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
        public DXButton SettingsButton, HelpButton, GuildButton, StorageButton, RankingButton, CompanionButton, LeaveButton;

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
            Index = 279;
            Sort = true;
            Movable = true;
            DropShadow = true;

            CloseButton = new DXButton
            {
                Parent = this,
                Index = 15,
                LibraryFile = LibraryFile.Interface,
                Hint = CEnvir.Language.CommonControlClose,
                HintPosition = HintPosition.TopLeft
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
                Hint = string.Format(CEnvir.Language.MenuDialogSettingsButtonHint, CEnvir.GetKeyBindLabel(KeyBindAction.ConfigWindow)),
            };
            SettingsButton.MouseClick += (o, e) => GameScene.Game.ConfigBox.Visible = !GameScene.Game.ConfigBox.Visible;

            HelpButton = new DXButton
            {
                Location = new Point(26, 70),
                Size = new Size(100, DefaultHeight),
                Parent = this,
                Label = { Text = CEnvir.Language.MenuDialogHelpButtonLabel },
                Hint = string.Format(CEnvir.Language.MenuDialogHelpButtonHint, CEnvir.GetKeyBindLabel(KeyBindAction.HelpWindow)),
            };
            HelpButton.MouseClick += (o, e) => GameScene.Game.HelpBox.Visible = !GameScene.Game.HelpBox.Visible;

            GuildButton = new DXButton
            {
                Location = new Point(26, 100),
                Size = new Size(100, DefaultHeight),
                Parent = this,
                Label = { Text = CEnvir.Language.MenuDialogGuildButtonLabel },
                Hint = string.Format(CEnvir.Language.MenuDialogGuildButtonHint, CEnvir.GetKeyBindLabel(KeyBindAction.GuildWindow)),
            };
            GuildButton.MouseClick += (o, e) => GameScene.Game.GuildBox.Visible = !GameScene.Game.GuildBox.Visible;

            StorageButton = new DXButton
            {
                Location = new Point(26, 130),
                Size = new Size(100, DefaultHeight),
                Parent = this,
                Label = { Text = CEnvir.Language.MenuDialogStorageButtonLabel },
                Hint = string.Format(CEnvir.Language.MenuDialogStorageButtonHint, CEnvir.GetKeyBindLabel(KeyBindAction.StorageWindow)),
            };
            StorageButton.MouseClick += (o, e) => GameScene.Game.StorageBox.Visible = !GameScene.Game.StorageBox.Visible;

            RankingButton = new DXButton
            {
                Location = new Point(26, 160),
                Size = new Size(100, DefaultHeight),
                Parent = this,
                Label = { Text = CEnvir.Language.MenuDialogRankingButtonLabel },
                Hint = string.Format(CEnvir.Language.MenuDialogRankingButtonHint, CEnvir.GetKeyBindLabel(KeyBindAction.RankingWindow)),
            };
            RankingButton.MouseClick += (o, e) => GameScene.Game.RankingBox.Visible = !GameScene.Game.RankingBox.Visible;

            CompanionButton = new DXButton
            {
                Location = new Point(26, 190),
                Size = new Size(100, DefaultHeight),
                Parent = this,
                Label = { Text = CEnvir.Language.MenuDialogCompanionButtonLabel },
                Hint = string.Format(CEnvir.Language.MenuDialogCompanionButtonHint, CEnvir.GetKeyBindLabel(KeyBindAction.CompanionWindow)),
            };
            CompanionButton.MouseClick += (o, e) => GameScene.Game.CompanionBox.Visible = !GameScene.Game.CompanionBox.Visible;

            LeaveButton = new DXButton
            {
                Location = new Point(26, 220),
                Size = new Size(100, DefaultHeight),
                Parent = this,
                Label = { Text = CEnvir.Language.MenuDialogLeaveButtonLabel },
                Hint = string.Format(CEnvir.Language.MenuDialogLeaveButtonHint, CEnvir.GetKeyBindLabel(KeyBindAction.ExitGameWindow)),
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

                if (HelpButton != null)
                {
                    if (!HelpButton.IsDisposed)
                        HelpButton.Dispose();

                    HelpButton = null;
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
