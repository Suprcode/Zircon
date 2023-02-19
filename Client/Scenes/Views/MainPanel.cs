﻿using System;
using System.Drawing;
using System.Windows.Forms;
using Client.Controls;
using Client.Envir;
using Client.Models;
using Library;

//Cleaned
namespace Client.Scenes.Views
{
    public sealed class MainPanel : DXImageControl
    {
        #region Properties

        public DXControl HealthBar, ManaBar, FocusBar;
        public DXImageControl ExperienceBar, NewMailIcon, CompletedQuestIcon, AvailableQuestIcon;

        public DXLabel ClassLabel, LevelLabel, ACLabel, MRLabel, DCLabel, MCLabel, SCLabel, AccuracyLabel, AgilityLabel, HealthLabel, ManaLabel, FocusLabel, ExperienceLabel, AttackModeLabel, PetModeLabel;

        #endregion

        public MainPanel()
        {
            LibraryFile = LibraryFile.GameInter;
            Index = 50;

            CEnvir.LibraryList.TryGetValue(LibraryFile, out MirLibrary barLibrary);

            ExperienceBar = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter,
                Index = 51,
            };
            ExperienceBar.Location = new Point((Size.Width - ExperienceBar.Size.Width) / 2 + 1, 2 + 1);
            ExperienceBar.BeforeDraw += (o, e) =>
            {
                if (barLibrary == null) return;

                decimal maxExperience = MapObject.User.MaxExperience;
                if (maxExperience <= 0) return;

                //Get percent.
                MirImage image = barLibrary.CreateImage(56, ImageType.Image);

                if (image == null) return;

                int x = (ExperienceBar.Size.Width - image.Width) / 2;
                int y = (ExperienceBar.Size.Height - image.Height) / 2;


                float percent = (float)Math.Min(1, Math.Max(0, MapObject.User.Experience / maxExperience));

                if (percent == 0) return;



                PresentTexture(image.Image, this, new Rectangle(ExperienceBar.DisplayArea.X + x, ExperienceBar.DisplayArea.Y + y - 1, (int) (image.Width * percent), image.Height), Color.White, ExperienceBar);
            };

            HealthBar = new DXControl
            {
                Parent = this,
                Location = new Point(35, 22),
                Size = barLibrary.GetSize(52),
            };
            HealthBar.BeforeDraw += (o, e) =>
            {
                if (barLibrary == null) return;

                if (MapObject.User.Stats[Stat.Health] == 0) return;

                float percent = Math.Min(1, Math.Max(0, MapObject.User.CurrentHP / (float)MapObject.User.Stats[Stat.Health]));

                if (percent == 0) return;

                MirImage image = barLibrary.CreateImage(52, ImageType.Image);

                if (image == null) return;

                PresentTexture(image.Image, this, new Rectangle(HealthBar.DisplayArea.X, HealthBar.DisplayArea.Y, (int) (image.Width*percent), image.Height), Color.White, HealthBar);
            };
            ManaBar = new DXControl
            {
                Parent = this,
                Location = new Point(35, 36),
                Size = barLibrary.GetSize(52),
            };
            ManaBar.BeforeDraw += (o, e) =>
            {
                if (barLibrary == null) return;

                if (MapObject.User.Stats[Stat.Mana] == 0) return;

                float percent = Math.Min(1, Math.Max(0, MapObject.User.CurrentMP / (float)MapObject.User.Stats[Stat.Mana]));

                if (percent == 0) return;

                MirImage image = barLibrary.CreateImage(54, ImageType.Image);

                if (image == null) return;

                PresentTexture(image.Image, this, new Rectangle(ManaBar.DisplayArea.X, ManaBar.DisplayArea.Y, (int) (image.Width * percent), image.Height), Color.White, ManaBar);
            };

            FocusBar =  new DXImageControl
            {
                Parent = this,
                Location = new Point(35, 50),
                LibraryFile = LibraryFile.GameInter,
                Size = barLibrary.GetSize(58),
            };
            FocusBar.BeforeDraw += (o, e) =>
            {
                if (barLibrary == null) return;

                if (MapObject.User.Stats[Stat.Focus] == 0) return;

                float percent = Math.Min(1, Math.Max(0, MapObject.User.CurrentFP / (float)MapObject.User.Stats[Stat.Focus]));

                if (percent == 0) return;

                var glow = CEnvir.Now.Second % 2 == 0 && percent == 1;

                MirImage image = barLibrary.CreateImage(glow ? 59 : 58, ImageType.Image);

                if (image == null) return;

                PresentTexture(image.Image, this, new Rectangle(FocusBar.DisplayArea.X, FocusBar.DisplayArea.Y, (int)(image.Width * percent), image.Height), Color.White, FocusBar);
            };

            DXButton CharacterButton = new DXButton
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 82,
                Parent = this,
                Location = new Point(650, 23),
                Hint = CEnvir.Language.MainPanelCharacterButtonHint
            };
            CharacterButton.MouseClick += (o, e) =>
            {
                GameScene.Game.CharacterBox.Visible = !GameScene.Game.CharacterBox.Visible;
            };

            DXButton InventoryButton = new DXButton
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 87,
                Parent = this,
                Location = new Point(689, 23),
                Hint = CEnvir.Language.MainPanelInventoryButtonHint
            };
            InventoryButton.MouseClick += (o, e) => GameScene.Game.InventoryBox.Visible = !GameScene.Game.InventoryBox.Visible;

            DXButton SpellButton = new DXButton
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 92,
                Parent = this,
                Location = new Point(728, 23),
                Hint = CEnvir.Language.MainPanelSpellButtonHint
            };
            SpellButton.MouseClick += (o, e) => GameScene.Game.MagicBox.Visible = !GameScene.Game.MagicBox.Visible;

            DXButton QuestButton = new DXButton
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 112,
                Parent = this,
                Location = new Point(767, 23),
                Hint = CEnvir.Language.MainPanelQuestButtonHint
            };
            QuestButton.MouseClick += (o, e) => GameScene.Game.QuestBox.Visible = !GameScene.Game.QuestBox.Visible;

            DXButton MailButton = new DXButton
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 97,
                Parent = this,
                Location = new Point(806, 23),
                Hint = CEnvir.Language.MainPanelMailButtonHint
            };
            MailButton.MouseClick += (o, e) =>
            {
                GameScene.Game.CommunicationBox.Visible = !GameScene.Game.CommunicationBox.Visible;
            };

            NewMailIcon = new DXImageControl
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 240,
                Parent = MailButton,
                IsControl = false,
                Location = new Point(2, 2),
                Visible = false,
            };

            AvailableQuestIcon = new DXImageControl
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 240,
                Parent = QuestButton,
                IsControl = false,
                Location = new Point(2, 2),
                Visible = false,
            };
            AvailableQuestIcon.VisibleChanged += (o, e) =>
            {
                if (AvailableQuestIcon.Visible)
                    CompletedQuestIcon.Location = new Point(2, QuestButton.Size.Height - CompletedQuestIcon.Size.Height);
                else
                CompletedQuestIcon.Location = new Point(2, 2);
            };

            CompletedQuestIcon = new DXImageControl
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 241,
                Parent = QuestButton,
                IsControl = false,
                Location = new Point(2, 2),
                Visible = false,
            };

            DXButton BeltButton = new DXButton
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 107,
                Parent = this,
                Location = new Point(845, 23),
                Hint = CEnvir.Language.MainPanelBeltButtonHint
            };
            BeltButton.MouseClick += (o, e) => GameScene.Game.BeltBox.Visible = !GameScene.Game.BeltBox.Visible;

            DXButton GroupButton = new DXButton
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 102,
                Parent = this,
                Location = new Point(884, 23),
                Hint = CEnvir.Language.MainPanelGroupButtonHint
            };
            GroupButton.MouseClick += (o,e) => GameScene.Game.GroupBox.Visible = !GameScene.Game.GroupBox.Visible;

            DXButton ConfigButton = new DXButton
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 117,
                Parent = this,
                Location = new Point(923, 23),
                Hint = CEnvir.Language.MainPanelConfigButtonHint
            };
            ConfigButton.MouseClick += (o, e) => GameScene.Game.ConfigBox.Visible = !GameScene.Game.ConfigBox.Visible;

            DXButton CashShopButton = new DXButton
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 122,
                Parent = this,
                Location = new Point(972, 16),
                Hint = CEnvir.Language.MainPanelCashShopButtonHint
            };
            CashShopButton.MouseClick += (o, e) =>
            {
                if (GameScene.Game.MarketPlaceBox.StoreTab.IsVisible)
                    GameScene.Game.MarketPlaceBox.Visible = false;
                else
                {
                    GameScene.Game.MarketPlaceBox.Visible = true;
                    GameScene.Game.MarketPlaceBox.StoreTab.TabButton.InvokeMouseClick();
                }
            };

            DXLabel label = new DXLabel
            {
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                Text = CEnvir.Language.MainPanelClassLabel,
                Hint = CEnvir.Language.MainPanelClassHint,
            };
            label.Location = new Point(300 - label.Size.Width, 20);

            label = new DXLabel
            {
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                Text = CEnvir.Language.MainPanelLevelLabel,
                Hint = CEnvir.Language.MainPanelLevelHint,
            };
            label.Location = new Point(300 - label.Size.Width, 40);

            label = new DXLabel
            {
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                Text = CEnvir.Language.MainPanelACLabel,
            };
            label.Location = new Point(385 - label.Size.Width, 20);

            label = new DXLabel
            {
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                Text = CEnvir.Language.MainPanelMRLabel,
            };
            label.Location = new Point(470 - label.Size.Width, 20);

            label = new DXLabel
            {
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                Text = CEnvir.Language.MainPanelDCLabel,
            };
            label.Location = new Point(385 - label.Size.Width, 40);

            DXLabel MCLabelLabel = new DXLabel
            {
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                Text = CEnvir.Language.MainPanelMCLabel,
            };
            MCLabelLabel.Location = new Point(470 - MCLabelLabel.Size.Width, 40);

            DXLabel SCLabelLabel = new DXLabel
            {
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                Text = CEnvir.Language.MainPanelSCLabel,
            };
            SCLabelLabel.Location = new Point(470 - SCLabelLabel.Size.Width, 40);

            label = new DXLabel
            {
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                Text = CEnvir.Language.MainPanelAccuracyLabel,
                Hint = CEnvir.Language.MainPanelAccuracyHint,
            };
            label.Location = new Point(567 - label.Size.Width, 20);

            label = new DXLabel
            {
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                Text = CEnvir.Language.MainPanelAgilityLabel,
                Hint = CEnvir.Language.MainPanelAgilityHint,
            };
            label.Location = new Point(567 - label.Size.Width, 40);

            ClassLabel = new DXLabel
            {
                AutoSize = false,
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F)),
                Location = new Point(300, 20),
                Size = new Size(60, 16),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                ForeColour = Color.White
            };

            LevelLabel = new DXLabel
            {
                AutoSize = false,
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F)),
                Location = new Point(300, 40),
                Size = new Size(60, 16),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                ForeColour = Color.White
            };

            ACLabel = new DXLabel
            {
                AutoSize = false,
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F)),
                Location = new Point(385, 20),
                Size = new Size(60, 16),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                ForeColour = Color.White
            };

            MRLabel = new DXLabel
            {
                AutoSize = false,
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F)),
                Location = new Point(470, 20),
                Size = new Size(60, 16),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                ForeColour = Color.White
            };

            DCLabel = new DXLabel
            {
                AutoSize = false,
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F)),
                Location = new Point(385, 40),
                Size = new Size(60, 16),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                ForeColour = Color.White
            };

            MCLabel = new DXLabel
            {
                AutoSize = false,
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F)),
                Location = new Point(470, 40),
                Size = new Size(60, 16),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                ForeColour = Color.White
            };
            MCLabel.VisibleChanged += (o, e) => MCLabelLabel.Visible = MCLabel.Visible;

            SCLabel = new DXLabel
            {
                AutoSize = false,
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F)),
                Location = new Point(470, 40),
                Size = new Size(60, 16),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                ForeColour = Color.White
            };
            SCLabel.VisibleChanged += (o, e) => SCLabelLabel.Visible = SCLabel.Visible;

            AccuracyLabel = new DXLabel
            {
                AutoSize = false,
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F)),
                Location = new Point(567, 20),
                Size = new Size(60, 16),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                ForeColour = Color.White
            };

            AgilityLabel = new DXLabel
            {
                AutoSize = false,
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F)),
                Location = new Point(567, 40),
                Size = new Size(60, 16),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                ForeColour = Color.White
            };

            HealthLabel = new DXLabel
            {
                Parent = this,
                ForeColour = Color.White,
                Outline = true,
                OutlineColour = Color.Black,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter
            };
            HealthLabel.SizeChanged += (o, e) =>
            {
                HealthLabel.Location = new Point(HealthBar.Location.X + (HealthBar.Size.Width - HealthLabel.Size.Width) / 2, HealthBar.Location.Y + (HealthBar.Size.Height - HealthLabel.Size.Height) / 2);
            };

            ManaLabel = new DXLabel
            {
                Parent = this,
                ForeColour = Color.White,
                Outline = true,
                OutlineColour = Color.Black,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter
            };
            ManaLabel.SizeChanged += (o, e) =>
            {
                ManaLabel.Location = new Point(ManaBar.Location.X + (ManaBar.Size.Width - ManaLabel.Size.Width) / 2, ManaBar.Location.Y + (ManaBar.Size.Height - ManaLabel.Size.Height) / 2);
            };

            FocusLabel = new DXLabel
            {
                Parent = this,
                ForeColour = Color.White,
                Outline = true,
                OutlineColour = Color.Black,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Visible = false
            };
            FocusLabel.SizeChanged += (o, e) =>
            {
                FocusLabel.Location = new Point(FocusBar.Location.X + (FocusBar.Size.Width - FocusLabel.Size.Width) / 2, FocusBar.Location.Y + (FocusBar.Size.Height - FocusLabel.Size.Height) / 2);
            };

            ExperienceLabel = new DXLabel
            {
                Parent = this,
                ForeColour = Color.White,
                Outline = true,
                OutlineColour = Color.Black,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter
            };
            ExperienceLabel.SizeChanged += (o, e) =>
            {
                ExperienceLabel.Location = new Point(ExperienceBar.Location.X + (ExperienceBar.Size.Width - ExperienceLabel.Size.Width) / 2, ExperienceBar.Location.Y + (ExperienceBar.Size.Height - ExperienceLabel.Size.Height) / 2);
            };

            AttackModeLabel = new DXLabel
            {
                Parent = this,
                ForeColour = Color.Cyan,
                Outline = true,
                OutlineColour = Color.Black,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Visible = false
            };
            AttackModeLabel.SizeChanged += (o, e) =>
            {
                AttackModeLabel.Location = new Point(FocusBar.Location.X, FocusBar.Location.Y + (FocusBar.Size.Height - AttackModeLabel.Size.Height) / 2 - 2);
            }; 

            PetModeLabel = new DXLabel
            {
                Parent = this,
                ForeColour = Color.Cyan,
                Outline = true,
                OutlineColour = Color.Black,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Visible = false
            };
            PetModeLabel.SizeChanged += (o, e) =>
            {
                PetModeLabel.Location = new Point(FocusBar.Location.X + FocusBar.Size.Width - PetModeLabel.Size.Width, FocusBar.Location.Y + (FocusBar.Size.Height - PetModeLabel.Size.Height)/2 - 2);
            };
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (HealthBar != null)
                {
                    if (!HealthBar.IsDisposed)
                        HealthBar.Dispose();

                    HealthBar = null;
                }

                if (ManaBar != null)
                {
                    if (!ManaBar.IsDisposed)
                        ManaBar.Dispose();

                    ManaBar = null;
                }

                if (ExperienceBar != null)
                {
                    if (!ExperienceBar.IsDisposed)
                        ExperienceBar.Dispose();

                    ExperienceBar = null;
                }

                if (NewMailIcon != null)
                {
                    if (!NewMailIcon.IsDisposed)
                        NewMailIcon.Dispose();

                    NewMailIcon = null;
                }

                if (ClassLabel != null)
                {
                    if (!ClassLabel.IsDisposed)
                        ClassLabel.Dispose();

                    ClassLabel = null;
                }

                if (LevelLabel != null)
                {
                    if (!LevelLabel.IsDisposed)
                        LevelLabel.Dispose();

                    LevelLabel = null;
                }

                if (ACLabel != null)
                {
                    if (!ACLabel.IsDisposed)
                        ACLabel.Dispose();

                    ACLabel = null;
                }

                if (MRLabel != null)
                {
                    if (!MRLabel.IsDisposed)
                        MRLabel.Dispose();

                    MRLabel = null;
                }

                if (DCLabel != null)
                {
                    if (!DCLabel.IsDisposed)
                        DCLabel.Dispose();

                    DCLabel = null;
                }

                if (MCLabel != null)
                {
                    if (!MCLabel.IsDisposed)
                        MCLabel.Dispose();

                    MCLabel = null;
                }

                if (SCLabel != null)
                {
                    if (!SCLabel.IsDisposed)
                        SCLabel.Dispose();

                    SCLabel = null;
                }

                if (AccuracyLabel != null)
                {
                    if (!AccuracyLabel.IsDisposed)
                        AccuracyLabel.Dispose();

                    AccuracyLabel = null;
                }

                if (AgilityLabel != null)
                {
                    if (!AgilityLabel.IsDisposed)
                        AgilityLabel.Dispose();

                    AgilityLabel = null;
                }

                if (HealthLabel != null)
                {
                    if (!HealthLabel.IsDisposed)
                        HealthLabel.Dispose();

                    HealthLabel = null;
                }

                if (ManaLabel != null)
                {
                    if (!ManaLabel.IsDisposed)
                        ManaLabel.Dispose();

                    ManaLabel = null;
                }

                if (FocusLabel != null)
                {
                    if (!FocusLabel.IsDisposed)
                        FocusLabel.Dispose();

                    FocusLabel = null;
                }

                if (ExperienceLabel != null)
                {
                    if (!ExperienceLabel.IsDisposed)
                        ExperienceLabel.Dispose();

                    ExperienceLabel = null;
                }

                if (AttackModeLabel != null)
                {
                    if (!AttackModeLabel.IsDisposed)
                        AttackModeLabel.Dispose();

                    AttackModeLabel = null;
                }

                if (PetModeLabel != null)
                {
                    if (!PetModeLabel.IsDisposed)
                        PetModeLabel.Dispose();

                    PetModeLabel = null;
                }
            }

        }

        #endregion
    }
}
