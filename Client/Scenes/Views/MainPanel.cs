using System;
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

        public DXImageControl ClassImage, LevelImage, ACImage, MACImage, DCImage, MCImage, SCImage, FPImage, CPImage;
        public DXLabel ClassLabel, LevelLabel, FPLabel, CPLabel, ACLabel, DCLabel, SCLabel, MACLabel, MCLabel, HealthLabel, ManaLabel, FocusLabel, AttackModeLabel, PetModeLabel;

        DXButton CharacterButton, InventoryButton, SpellButton, QuestButton, MailButton, BeltButton, GroupButton, MenuButton, CashShopButton;
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
                HintPosition = HintPosition.FixedY
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

            CharacterButton = new DXButton
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 82,
                Parent = this,
                Location = new Point(650, 23),
                Hint = CEnvir.Language.MainPanelCharacterButtonHint,
                HintPosition = HintPosition.TopLeft
            };
            CharacterButton.MouseClick += (o, e) =>
            {
                GameScene.Game.CharacterBox.Visible = !GameScene.Game.CharacterBox.Visible;
            };

            InventoryButton = new DXButton
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 87,
                Parent = this,
                Location = new Point(689, 23),
                Hint = CEnvir.Language.MainPanelInventoryButtonHint,
                HintPosition = HintPosition.TopLeft
            };
            InventoryButton.MouseClick += (o, e) => GameScene.Game.InventoryBox.Visible = !GameScene.Game.InventoryBox.Visible;

            SpellButton = new DXButton
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 92,
                Parent = this,
                Location = new Point(728, 23),
                Hint = CEnvir.Language.MainPanelSpellButtonHint,
                HintPosition = HintPosition.TopLeft
            };
            SpellButton.MouseClick += (o, e) => GameScene.Game.MagicBox.Visible = !GameScene.Game.MagicBox.Visible;

            QuestButton = new DXButton
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 112,
                Parent = this,
                Location = new Point(767, 23),
                Hint = CEnvir.Language.MainPanelQuestButtonHint,
                HintPosition = HintPosition.TopLeft
            };
            QuestButton.MouseClick += (o, e) => GameScene.Game.QuestBox.Visible = !GameScene.Game.QuestBox.Visible;

            MailButton = new DXButton
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 97,
                Parent = this,
                Location = new Point(806, 23),
                Hint = CEnvir.Language.MainPanelMailButtonHint,
                HintPosition = HintPosition.TopLeft
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

            BeltButton = new DXButton
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 107,
                Parent = this,
                Location = new Point(845, 23),
                Hint = CEnvir.Language.MainPanelBeltButtonHint,
                HintPosition = HintPosition.TopLeft
            };
            BeltButton.MouseClick += (o, e) => GameScene.Game.BeltBox.Visible = !GameScene.Game.BeltBox.Visible;

            GroupButton = new DXButton
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 102,
                Parent = this,
                Location = new Point(884, 23),
                Hint = CEnvir.Language.MainPanelGroupButtonHint,
                HintPosition = HintPosition.TopLeft
            };
            GroupButton.MouseClick += (o,e) => GameScene.Game.GroupBox.Visible = !GameScene.Game.GroupBox.Visible;

            MenuButton = new DXButton
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 117,
                Parent = this,
                Location = new Point(923, 23),
                Hint = CEnvir.Language.MainPanelMenuButtonHint,
                HintPosition = HintPosition.TopLeft
            };
            MenuButton.MouseClick += (o, e) => GameScene.Game.MenuBox.Visible = !GameScene.Game.MenuBox.Visible;

            DXButton CashShopButton = new DXButton
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 122,
                Parent = this,
                Location = new Point(972, 16),
                Hint = CEnvir.Language.MainPanelCashShopButtonHint,
                HintPosition = HintPosition.TopLeft
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

            #region Image Stat Labels
            ClassImage = new DXImageControl
            {
                Parent = this,
                Index = 70,
                LibraryFile = LibraryFile.GameInter,
                Location = new Point(277, 25),
                Hint = CEnvir.Language.MainPanelClassHint
            };
            LevelImage = new DXImageControl
            {
                Parent = this,
                Index = 71,
                LibraryFile = LibraryFile.GameInter,
                Location = new Point(277, 45),
                Hint = CEnvir.Language.MainPanelLevelHint
            };

            FPImage = new DXImageControl
            {
                Parent = this,
                Index = 72,
                LibraryFile = LibraryFile.GameInter,
                Location = new Point(362, 25)
            };
            CPImage = new DXImageControl
            {
                Parent = this,
                Index = 73,
                LibraryFile = LibraryFile.GameInter,
                Location = new Point(362, 45)
            };

            ACImage = new DXImageControl
            {
                Parent = this,
                Index = 66,
                LibraryFile = LibraryFile.GameInter,
                Location = new Point(445, 25)
            };
            DCImage = new DXImageControl
            {
                Parent = this,
                Index = 65,
                LibraryFile = LibraryFile.GameInter,
                Location = new Point(445, 45)
            };

            MACImage = new DXImageControl
            {
                Parent = this,
                Index = 63,
                LibraryFile = LibraryFile.GameInter,
                Location = new Point(531, 25)
            };
            MCImage = new DXImageControl
            {
                Parent = this,
                Index = 62,
                LibraryFile = LibraryFile.GameInter,
                Location = new Point(541, 45)
            };
            SCImage = new DXImageControl
            {
                Parent = this,
                Index = 64,
                LibraryFile = LibraryFile.GameInter,
                Location = new Point(547, 45)
            };

            #endregion

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

            //FP
            FPLabel = new DXLabel
            {
                AutoSize = false,
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F)),
                Location = new Point(385, 20),
                Size = new Size(60, 16),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                ForeColour = Color.White
            };

            CPLabel = new DXLabel
            {
                AutoSize = false,
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F)),
                
                Location = new Point(385, 40),
                Size = new Size(60, 16),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                ForeColour = Color.White
            };

            ACLabel = new DXLabel
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
                Location = new Point(470, 40),
                Size = new Size(60, 16),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                ForeColour = Color.White
            };

            MACLabel = new DXLabel
            {
                AutoSize = false,
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F)),
                Location = new Point(567, 20),
                Size = new Size(60, 16),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                ForeColour = Color.White
            };

            MCLabel = new DXLabel
            {
                AutoSize = false,
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F)),
                Location = new Point(567, 40),
                Size = new Size(60, 16),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                ForeColour = Color.White
            };
            MCLabel.VisibleChanged += (o, e) => MCImage.Visible = MCLabel.Visible;

            SCLabel = new DXLabel
            {
                AutoSize = false,
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F)),
                Location = new Point(567, 40),
                Size = new Size(60, 16),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                ForeColour = Color.White
            };
            SCLabel.VisibleChanged += (o, e) => SCImage.Visible = SCLabel.Visible;

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
                if (CharacterButton != null)
                {
                    if (!CharacterButton.IsDisposed)
                        CharacterButton.Dispose();

                    CharacterButton = null;
                }

                if (InventoryButton != null)
                {
                    if (!InventoryButton.IsDisposed)
                        InventoryButton.Dispose();

                    InventoryButton = null;
                }

                if (SpellButton != null)
                {
                    if (!SpellButton.IsDisposed)
                        SpellButton.Dispose();

                    SpellButton = null;
                }

                if (QuestButton != null)
                {
                    if (!QuestButton.IsDisposed)
                        QuestButton.Dispose();

                    QuestButton = null;
                }

                if (MailButton != null)
                {
                    if (!MailButton.IsDisposed)
                        MailButton.Dispose();

                    MailButton = null;
                }

                if (BeltButton != null)
                {
                    if (!BeltButton.IsDisposed)
                        BeltButton.Dispose();

                    BeltButton = null;
                }

                if (GroupButton != null)
                {
                    if (!GroupButton.IsDisposed)
                        GroupButton.Dispose();

                    GroupButton = null;
                }

                if (MenuButton != null)
                {
                    if (!MenuButton.IsDisposed)
                        MenuButton.Dispose();

                    MenuButton = null;
                }

                if (CashShopButton != null)
                {
                    if (!CashShopButton.IsDisposed)
                        CashShopButton.Dispose();

                    CashShopButton = null;
                }

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

                if (FPLabel != null)
                {
                    if (!FPLabel.IsDisposed)
                        FPLabel.Dispose();

                    FPLabel = null;
                }

                if (CPLabel != null)
                {
                    if (!CPLabel.IsDisposed)
                        CPLabel.Dispose();

                    CPLabel = null;
                }

                if (ACLabel != null)
                {
                    if (!ACLabel.IsDisposed)
                        ACLabel.Dispose();

                    ACLabel = null;
                }

                if (DCLabel != null)
                {
                    if (!DCLabel.IsDisposed)
                        DCLabel.Dispose();

                    DCLabel = null;
                }

                if (SCLabel != null)
                {
                    if (!SCLabel.IsDisposed)
                        SCLabel.Dispose();

                    SCLabel = null;
                }

                if (MACLabel != null)
                {
                    if (!MACLabel.IsDisposed)
                        MACLabel.Dispose();

                    MACLabel = null;
                }

                if (MCLabel != null)
                {
                    if (!MCLabel.IsDisposed)
                        MCLabel.Dispose();

                    MCLabel = null;
                }

                if (ClassImage != null)
                {
                    if (!ClassImage.IsDisposed)
                        ClassImage.Dispose();

                    ClassImage = null;
                }

                if (LevelImage != null)
                {
                    if (!LevelImage.IsDisposed)
                        LevelImage.Dispose();

                    LevelImage = null;
                }

                if (FPImage != null)
                {
                    if (!FPImage.IsDisposed)
                        FPImage.Dispose();

                    FPImage = null;
                }

                if (CPImage != null)
                {
                    if (!CPImage.IsDisposed)
                        CPImage.Dispose();

                    CPImage = null;
                }

                if (ACImage != null)
                {
                    if (!ACImage.IsDisposed)
                        ACImage.Dispose();

                    ACImage = null;
                }

                if (DCImage != null)
                {
                    if (!DCImage.IsDisposed)
                        DCImage.Dispose();

                    DCImage = null;
                }

                if (SCImage != null)
                {
                    if (!SCImage.IsDisposed)
                        SCImage.Dispose();

                    SCImage = null;
                }

                if (MACImage != null)
                {
                    if (!MACImage.IsDisposed)
                        MACImage.Dispose();

                    MACImage = null;
                }

                if (MCImage != null)
                {
                    if (!MCImage.IsDisposed)
                        MCImage.Dispose();

                    MCImage = null;
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
