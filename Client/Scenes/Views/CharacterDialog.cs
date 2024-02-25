using Client.Controls;
using Client.Envir;
using Client.Models;
using Client.Scenes.Views.Character;
using Client.UserModels;
using Library;
using Library.SystemModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using C = Library.Network.ClientPackets;
using S = Library.Network.ServerPackets;

namespace Client.Scenes.Views
{
    public sealed class CharacterDialog : DXImageControl
    {
        #region Properties

        private DXTabControl TabControl;
        private DXTab CharacterTab, HermitTab, DisciplineTab;
        public DXLabel CharacterNameLabel, GuildNameLabel, GuildRankLabel;

        private DXTabControl StatsTabControl;
        private DXTab StatsAttackTab, StatsDefenseTab, StatsWeightTab, StatsOtherTab, StatsElementAttackTab, StatsElementAdvantageTab, StatsElementDisadvantageTab;

        private DXImageControl DisciplineLevel;
        private DXLabel DisciplineLevelLabel, DisciplineUnusedLabel, DisciplineExperienceLabel;
        private DXButton DisciplineButton;

        public Dictionary<MagicInfo, DisciplineMagicCell> DisciplineMagics = new Dictionary<MagicInfo, DisciplineMagicCell>();

        public DXImageControl MarriageIcon;
        public DXLabel MarriageLabel;

        public DXButton CloseButton;

        public DXItemCell[] Grid;

        public DXLabel WearWeightLabel, HandWeightLabel;
        public Dictionary<Stat, DXLabel> DisplayStats = new Dictionary<Stat, DXLabel>();

        public Dictionary<Stat, DXLabel> AttackStats = new Dictionary<Stat, DXLabel>();
        public Dictionary<Stat, DXLabel> AdvantageStats = new Dictionary<Stat, DXLabel>();
        public Dictionary<Stat, DXLabel> DisadvantageStats = new Dictionary<Stat, DXLabel>();

        private DXLabel HermitRemainingLabel;
        private DXCheckBox HermitShowConfirmation;

        public Dictionary<Stat, DXLabel> HermitDisplayStats = new Dictionary<Stat, DXLabel>();
        public Dictionary<Stat, DXLabel> HermitAttackStats = new Dictionary<Stat, DXLabel>();

        public ClientUserItem[] Equipment
        {
            get
            {
                return Inspect ? _inspectEquipment : GameScene.Game.Equipment;
            }
        }
        public Stats Stats
        {
            get
            {
                return Inspect ? _inspectStats : MapObject.User.Stats;
            }
        }
        public Stats HermitStats
        {
            get
            {
                return Inspect ? _inspectHermitStats : MapObject.User.HermitStats;
            }
        }
        public int HermitPoints
        {
            get
            {
                return Inspect ? _inspectHermitPoints : MapObject.User.HermitPoints;
            }
        }
        public MirClass Class
        {
            get
            {
                return Inspect ? _inspectClass : MapObject.User.Class;
            }
        }
        public MirGender Gender
        {
            get
            {
                return Inspect ? _inspectGender : MapObject.User.Gender;
            }
        }
        public int HairType
        {
            get
            {
                return Inspect ? _inspectHairType : MapObject.User.HairType;
            }
        }
        public Color HairColour
        {
            get
            {
                return Inspect ? _inspectHairColour : MapObject.User.HairColour;
            }
        }
        public int Level
        {
            get
            {
                return Inspect ? _inspectLevel : MapObject.User.Level;
            }
        }

        public int Fame
        {
            get
            {
                return Inspect ? _inspectFame : MapObject.User.Stats[Stat.Fame];
            }
        }

        public int GuildFlag
        {
            get
            {
                return Inspect ? _inspectGuildFlag : GameScene.Game.GuildBox.GuildInfo?.Flag ?? -1;
            }
        }

        public Color GuildColour
        {
            get
            {
                return Inspect ? _inspectGuildColour : GameScene.Game.GuildBox.GuildInfo?.Colour ?? Color.Empty;
            }
        }

        private bool HideHead
        {
            get
            {
                return Grid[(int)EquipmentSlot.Costume]?.Item?.Info != null || HasFishingRobe;
            }
        }

        private bool HideBody
        {
            get
            {
                return PlayerObject.CostumeShapeHideBody.Contains(Grid[(int)EquipmentSlot.Costume]?.Item?.Info.Shape ?? -1);
            }
        }

        private bool HasFishingRobe
        {
            get { return Grid != null && Grid[(int)EquipmentSlot.Armour]?.Item?.Info.ItemEffect == ItemEffect.FishingRobe; }
        }

        private bool HasFishingRod
        {
            get { return Grid != null && Grid[(int)EquipmentSlot.Weapon]?.Item?.Info.ItemEffect == ItemEffect.FishingRod; }
        }

        #region Inspect

        private ClientUserItem[] _inspectEquipment = new ClientUserItem[Globals.EquipmentSize];
        private Stats _inspectStats = new Stats();
        public Stats _inspectHermitStats = new Stats();
        public int _inspectHermitPoints;
        public MirClass _inspectClass;
        public MirGender _inspectGender;
        public int _inspectHairType;
        public Color _inspectHairColour;
        public int _inspectLevel;
        public int _inspectFame;
        public int _inspectGuildFlag;
        public Color _inspectGuildColour;

        #endregion

        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            if (IsVisible)
                BringToFront();

            if (Settings != null)
                Settings.Visible = nValue;

            if (!Inspect)
            {
                GameScene.Game.FishingBox.Visible = HasFishingRod && IsVisible;

                HermitTab.TabButton.Visible = GameScene.Game.HermitEnabled;
            }

            base.OnIsVisibleChanged(oValue, nValue);
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

        #endregion

        #region Settings

        public WindowSetting Settings;
        public WindowType Type
        {
            get { return Inspect ? WindowType.InspectBox : WindowType.CharacterBox; }
        }

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

        public bool Inspect { get; private set; }

        #endregion

        public CharacterDialog(bool inspect)
        {
            Inspect = inspect;

            LibraryFile = LibraryFile.Interface;
            Index = Inspect ? 115 : 110;
            Movable = true;
            Sort = true;

            CloseButton = new DXButton
            {
                Parent = this,
                Index = 15,
                LibraryFile = LibraryFile.Interface,
            };
            CloseButton.Location = new Point(DisplayArea.Width - CloseButton.Size.Width - 3, 3);
            CloseButton.MouseClick += (o, e) => Visible = false;

            TabControl = new DXTabControl
            {
                Parent = this,
                Location = new Point(0, 19),
                Size = new Size(DisplayArea.Width, DisplayArea.Height),
                MarginLeft = 18
            };
            CharacterTab = new DXTab
            {
                Parent = TabControl,
                TabButton = { Label = { Text = CEnvir.Language.CharacterCharacterTabLabel } },
                BackColour = Color.Empty,
                Location = new Point(0, 26)
            };
            CharacterTab.BeforeChildrenDraw += CharacterTab_BeforeChildrenDraw;
            CharacterTab.TabButton.MouseClick += (o, e) =>
            {
                Index = Inspect ? 115 : 110;
            };

            DisciplineTab = new DXTab
            {
                Parent = TabControl,
                TabButton = { Label = { Text = CEnvir.Language.CharacterDisciplineTabLabel } },
                BackColour = Color.Empty,
                Location = new Point(0, 26),
            };

            DisciplineTab.TabButton.Visible = !Inspect && Globals.DisciplineInfoList.Binding.Count > 0;
            DisciplineTab.TabButton.MouseClick += (o, e) =>
            {
                Index = 112;
            };

            HermitTab = new DXTab
            {
                Parent = TabControl,
                TabButton = { Label = { Text = CEnvir.Language.CharacterHermitTabLabel } },
                BackColour = Color.Empty,
                Location = new Point(0, 26),
            };

            HermitTab.TabButton.Visible = !Inspect;
            HermitTab.TabButton.MouseClick += (o, e) =>
            {
                Index = 111;
            };

            DXControl namePanel = new DXControl
            {
                Parent = this,
                Size = new Size(137, 68),
                Location = new Point(((CharacterTab.Size.Width - 135) / 2) - 5, 51)
            };
            CharacterNameLabel = new DXLabel
            {
                AutoSize = false,
                Size = new Size(137, 20),
                ForeColour = Color.FromArgb(222, 255, 222),
                Outline = false,
                Parent = namePanel,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter
            };
            GuildNameLabel = new DXLabel
            {
                AutoSize = false,
                Size = new Size(137, 15),
                ForeColour = Color.FromArgb(255, 255, 181),
                Outline = false,
                Parent = namePanel,
                Location = new Point(0, CharacterNameLabel.Size.Height - 2),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter
            };
            GuildRankLabel = new DXLabel
            {
                AutoSize = false,
                Size = new Size(137, 15),
                ForeColour = Color.FromArgb(255, 206, 148),
                Outline = false,
                Parent = namePanel,
                Location = new Point(0, CharacterNameLabel.Size.Height + GuildNameLabel.Size.Height - 4),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter
            };

            MarriageIcon = new DXImageControl
            {
                Parent = CharacterTab,
                LibraryFile = LibraryFile.GameInter,
                Index = 1298,
                Location = new Point(96, 60),
                Visible = false
            };
            MarriageLabel = new DXLabel
            {
                AutoSize = false,
                Parent = CharacterTab,
                Size = new Size(117, 18),
                ForeColour = Color.Pink,
                Location = new Point(112, 55),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Visible = false
            };

            TabControl.SelectedTab = CharacterTab;

            #region Grid

            Grid = new DXItemCell[Globals.EquipmentSize];

            DXItemCell cell;
            Grid[(int)EquipmentSlot.Weapon] = cell = new DXItemCell
            {
                Location = new Point(58, 122),
                Parent = CharacterTab,
                ItemGrid = Equipment,
                Slot = (int)EquipmentSlot.Weapon,
                GridType = Inspect ? GridType.Inspect : GridType.Equipment,
                Size = new System.Drawing.Size(65, 90),
                Hidden = true
            };
            cell.MouseEnter += Cell_MouseEnter;
            cell.MouseLeave += Cell_MouseLeave;
            cell.ItemChanged += (o, e) =>
            {
                GameScene.Game.FishingBox.Visible = Visible && ((DXItemCell)o).Item?.Info.ItemEffect == ItemEffect.FishingRod;
            };

            Grid[(int)EquipmentSlot.Armour] = cell = new DXItemCell
            {
                Location = new Point(120, 123),
                Parent = CharacterTab,
                Border = false,
                ItemGrid = Equipment,
                Slot = (int)EquipmentSlot.Armour,
                GridType = Inspect ? GridType.Inspect : GridType.Equipment,
                Size = new System.Drawing.Size(70, 150),
                Hidden = true
            };
            cell.MouseEnter += Cell_MouseEnter;
            cell.MouseLeave += Cell_MouseLeave;

            Grid[(int)EquipmentSlot.Shield] = cell = new DXItemCell
            {
                Location = new Point(170, 170),
                Parent = CharacterTab,
                FixedBorder = true,
                Border = true,
                ItemGrid = Equipment,
                Slot = (int)EquipmentSlot.Shield,
                GridType = Inspect ? GridType.Inspect : GridType.Equipment,
                Hidden = true
            };
            cell.MouseEnter += Cell_MouseEnter;
            cell.MouseLeave += Cell_MouseLeave;

            Grid[(int)EquipmentSlot.Helmet] = cell = new DXItemCell
            {
                Location = new Point(140, 90),
                Parent = CharacterTab,
                FixedBorder = true,
                Border = true,
                ItemGrid = Equipment,
                Slot = (int)EquipmentSlot.Helmet,
                GridType = Inspect ? GridType.Inspect : GridType.Equipment,
                Size = new Size(35, 35),
                Hidden = true
            };
            cell.MouseEnter += Cell_MouseEnter;
            cell.MouseLeave += Cell_MouseLeave;

            Grid[(int)EquipmentSlot.Emblem] = cell = new DXItemCell
            {
                Location = new Point(244, 118),
                Parent = CharacterTab,
                Border = true,
                ItemGrid = Equipment,
                Slot = (int)EquipmentSlot.Emblem,
                GridType = Inspect ? GridType.Inspect : GridType.Equipment,
            };
            cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 104);
            cell.MouseEnter += Cell_MouseEnter;
            cell.MouseLeave += Cell_MouseLeave;

            Grid[(int)EquipmentSlot.HorseArmour] = cell = new DXItemCell
            {
                Location = new Point(283, 118),
                Parent = CharacterTab,
                Border = true,
                ItemGrid = Equipment,
                Slot = (int)EquipmentSlot.HorseArmour,
                GridType = Inspect ? GridType.Inspect : GridType.Equipment,
            };
            cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 82);
            cell.MouseEnter += Cell_MouseEnter;
            cell.MouseLeave += Cell_MouseLeave;

            Grid[(int)EquipmentSlot.Torch] = cell = new DXItemCell
            {
                Location = new Point(10, 196),
                Parent = CharacterTab,
                Border = true,
                ItemGrid = Equipment,
                Slot = (int)EquipmentSlot.Torch,
                GridType = Inspect ? GridType.Inspect : GridType.Equipment,
            };
            cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 38);
            cell.MouseEnter += Cell_MouseEnter;
            cell.MouseLeave += Cell_MouseLeave;

            Grid[(int)EquipmentSlot.Necklace] = cell = new DXItemCell
            {
                Location = new Point(10, 157),
                Parent = CharacterTab,
                Border = true,
                ItemGrid = Equipment,
                Slot = (int)EquipmentSlot.Necklace,
                GridType = Inspect ? GridType.Inspect : GridType.Equipment,
            };
            cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 33);
            cell.MouseEnter += Cell_MouseEnter;
            cell.MouseLeave += Cell_MouseLeave;

            Grid[(int)EquipmentSlot.BraceletL] = cell = new DXItemCell
            {
                Location = new Point(244, 157),
                Parent = CharacterTab,
                Border = true,
                ItemGrid = Equipment,
                Slot = (int)EquipmentSlot.BraceletL,
                GridType = Inspect ? GridType.Inspect : GridType.Equipment,
            };
            cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 32);
            cell.MouseEnter += Cell_MouseEnter;
            cell.MouseLeave += Cell_MouseLeave;

            Grid[(int)EquipmentSlot.BraceletR] = cell = new DXItemCell
            {
                Location = new Point(283, 157),
                Parent = CharacterTab,
                Border = true,
                ItemGrid = Equipment,
                Slot = (int)EquipmentSlot.BraceletR,
                GridType = Inspect ? GridType.Inspect : GridType.Equipment,
            };
            cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 32);
            cell.MouseEnter += Cell_MouseEnter;
            cell.MouseLeave += Cell_MouseLeave;

            Grid[(int)EquipmentSlot.RingL] = cell = new DXItemCell
            {
                Location = new Point(244, 196),
                Parent = CharacterTab,
                Border = true,
                ItemGrid = Equipment,
                Slot = (int)EquipmentSlot.RingL,
                GridType = Inspect ? GridType.Inspect : GridType.Equipment,
            };
            cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 31);
            cell.MouseEnter += Cell_MouseEnter;
            cell.MouseLeave += Cell_MouseLeave;

            Grid[(int)EquipmentSlot.RingR] = cell = new DXItemCell
            {
                Location = new Point(283, 196),
                Parent = CharacterTab,
                Border = true,
                ItemGrid = Equipment,
                Slot = (int)EquipmentSlot.RingR,
                GridType = Inspect ? GridType.Inspect : GridType.Equipment,
            };
            cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 31);
            cell.MouseEnter += Cell_MouseEnter;
            cell.MouseLeave += Cell_MouseLeave;

            Grid[(int)EquipmentSlot.Flower] = cell = new DXItemCell
            {
                Location = new Point(244, 235),
                Parent = CharacterTab,
                Border = true,
                ItemGrid = Equipment,
                Slot = (int)EquipmentSlot.Flower,
                GridType = Inspect ? GridType.Inspect : GridType.Equipment,
            };
            cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 81);
            cell.MouseEnter += Cell_MouseEnter;
            cell.MouseLeave += Cell_MouseLeave;

            Grid[(int)EquipmentSlot.Poison] = cell = new DXItemCell
            {
                Location = new Point(244, 274),
                Parent = CharacterTab,
                Border = true,
                ItemGrid = Equipment,
                Slot = (int)EquipmentSlot.Poison,
                GridType = Inspect ? GridType.Inspect : GridType.Equipment,
            };
            cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 40);
            cell.MouseEnter += Cell_MouseEnter;
            cell.MouseLeave += Cell_MouseLeave;

            Grid[(int)EquipmentSlot.Amulet] = cell = new DXItemCell
            {
                Location = new Point(283, 235),
                Parent = CharacterTab,
                Border = true,
                ItemGrid = Equipment,
                Slot = (int)EquipmentSlot.Amulet,
                GridType = Inspect ? GridType.Inspect : GridType.Equipment,
                Size = new Size(36, 75)
            };
            cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 39);
            cell.MouseEnter += Cell_MouseEnter;
            cell.MouseLeave += Cell_MouseLeave;

            Grid[(int)EquipmentSlot.Shoes] = cell = new DXItemCell
            {
                Location = new Point(10, 235),
                Parent = CharacterTab,
                Border = true,
                ItemGrid = Equipment,
                Slot = (int)EquipmentSlot.Shoes,
                GridType = Inspect ? GridType.Inspect : GridType.Equipment,
                Size = new Size(36, 75)
            };
            cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 36);
            cell.MouseEnter += Cell_MouseEnter;
            cell.MouseLeave += Cell_MouseLeave;

            Grid[(int)EquipmentSlot.Costume] = cell = new DXItemCell
            {
                Location = new Point(10, 118),
                Parent = CharacterTab,
                Border = true,
                ItemGrid = Equipment,
                Slot = (int)EquipmentSlot.Costume,
                GridType = Inspect ? GridType.Inspect : GridType.Equipment,
            };
            cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 34);
            cell.MouseEnter += Cell_MouseEnter;
            cell.MouseLeave += Cell_MouseLeave;

            cell.MouseEnter += Cell_MouseEnter;
            cell.MouseLeave += Cell_MouseLeave;

            #endregion

            #region Stats

            #region Tabs

            StatsTabControl = new DXTabControl
            {
                Parent = CharacterTab,
                Location = new Point(0, 319),
                Size = new Size(DisplayArea.Width, 120),
                MarginLeft = 21,
                Visible = !Inspect
            };

            StatsAttackTab = new DXTab
            {
                Parent = StatsTabControl,
                TabButton =
                { 
                    Label =
                    {
                        Text = CEnvir.Language.CharacterCharacterTabStatsAttackTabLabel
                    },
                    Hint = CEnvir.Language.CharacterCharacterTabStatsAttackTabHint
                },
                BackColour = Color.Empty,
                MinimumTabWidth = 40,
                Location = new Point(0, 22),
            };

            StatsDefenseTab = new DXTab
            {
                Parent = StatsTabControl,
                TabButton =
                { 
                    Label =
                    {
                        Text = CEnvir.Language.CharacterCharacterTabStatsDefenseTabLabel
                    },
                    Hint = CEnvir.Language.CharacterCharacterTabStatsDefenseTabHint
                },
                BackColour = Color.Empty,
                MinimumTabWidth = 40,
                Location = new Point(0, 22),
            };
            StatsWeightTab = new DXTab
            {
                Parent = StatsTabControl,
                TabButton =
                { 
                    Label =
                    {
                        Text = CEnvir.Language.CharacterCharacterTabStatsWeightTabLabel
                    },
                    Hint = CEnvir.Language.CharacterCharacterTabStatsWeightTabHint
                },
                BackColour = Color.Empty,
                MinimumTabWidth = 40,
                Location = new Point(0, 22),
            };
            StatsOtherTab = new DXTab
            {
                Parent = StatsTabControl,
                TabButton =
                { 
                    Label =
                    {
                        Text = CEnvir.Language.CharacterCharacterTabStatsOtherTabLabel            
                    },
                    Hint = CEnvir.Language.CharacterCharacterTabStatsOtherTabHint
                },
                BackColour = Color.Empty,
                MinimumTabWidth = 40,
                Location = new Point(0, 22),
            };
            StatsElementAttackTab = new DXTab
            {
                Parent = StatsTabControl,
                TabButton =
                { 
                    Label =
                    {
                        Text = CEnvir.Language.CharacterCharacterTabStatsElementAttackTabLabel    
                    },
                    Hint = CEnvir.Language.CharacterCharacterTabStatsElementAttackTabHint
                },
                BackColour = Color.Empty,
                MinimumTabWidth = 40,
                Location = new Point(0, 22),
            };
            StatsElementAdvantageTab = new DXTab
            {
                Parent = StatsTabControl,
                TabButton =
                { 
                    Label =
                    {
                        Text = CEnvir.Language.CharacterCharacterTabStatsElementAdvantageTabLabel    
                    },
                    Hint = CEnvir.Language.CharacterCharacterTabStatsElementAdvantageTabHint
                },
                BackColour = Color.Empty,
                MinimumTabWidth = 40,
                Location = new Point(0, 22),
            };
            StatsElementDisadvantageTab = new DXTab
            {
                Parent = StatsTabControl,
                TabButton = 
                { 
                    Label = 
                    { 
                        Text = CEnvir.Language.CharacterCharacterTabStatsElementDisadvantageTabLabel                
                    },
                    Hint = CEnvir.Language.CharacterCharacterTabStatsElementDisadvantageTabHint
                },
                BackColour = Color.Empty,
                MinimumTabWidth = 40,
                Location = new Point(0, 22),
            };

            #endregion

            #region StatsAttackTab

            const int yStart = 6;
            int y = yStart;

            const int left = 15, right = 168, rowSpacing = 22;

            Size labelValueSize = new Size(100, 16);

            DXLabel label = new DXLabel
            {
                Parent = StatsAttackTab,
                Text = CEnvir.Language.CommonStatusDC + ":",
                ForeColour = Color.White
            };
            label.Location = new Point(left, y);

            DisplayStats[Stat.MaxDC] = new DXLabel
            {
                Parent = StatsAttackTab,
                Location = new Point(label.Location.X + 45, y),
                ForeColour = Color.White,
                Text = "0-0",
                Size = labelValueSize,
                AutoSize = false,
                DrawFormat = TextFormatFlags.Right,
                Tag = label
            };

            label = new DXLabel
            {
                Parent = StatsAttackTab,
                Text = CEnvir.Language.CharacterCharacterTabStatsAttackTabMCLabel,
                ForeColour = Color.White
            };
            label.Location = new Point(left, y += rowSpacing);

            DisplayStats[Stat.MaxMC] = new DXLabel
            {
                Parent = StatsAttackTab,
                Location = new Point(label.Location.X + 45, y),
                ForeColour = Color.White,
                Text = "0-0",
                Size = labelValueSize,
                AutoSize = false,
                DrawFormat = TextFormatFlags.Right,
                Tag = label
            };

            label = new DXLabel
            {
                Parent = StatsAttackTab,
                Text = CEnvir.Language.CharacterCharacterTabStatsAttackTabSCLabel,
                ForeColour = Color.White
            };
            label.Location = new Point(left, y += rowSpacing);

            DisplayStats[Stat.MaxSC] = new DXLabel
            {
                Parent = StatsAttackTab,
                Location = new Point(label.Location.X + 45, y),
                ForeColour = Color.White,
                Text = "0-0",
                Size = labelValueSize,
                AutoSize = false,
                DrawFormat = TextFormatFlags.Right,
                Tag = label
            };

            label = new DXLabel
            {
                Parent = StatsAttackTab,
                Text = CEnvir.Language.CharacterCharacterTabStatsAttackTabCritDamageLabel,
                ForeColour = Color.White
            };
            label.Location = new Point(left, y += rowSpacing);

            DisplayStats[Stat.CriticalDamage] = new DXLabel
            {
                Parent = StatsAttackTab,
                Location = new Point(label.Location.X + 45, y),
                ForeColour = Color.White,
                Text = "0",
                Size = labelValueSize,
                AutoSize = false,
                DrawFormat = TextFormatFlags.Right,
                Tag = label
            };

            y = yStart;

            label = new DXLabel
            {
                Parent = StatsAttackTab,
                Text = CEnvir.Language.CharacterCharacterTabStatsAttackTabAccuracyLabel,
                ForeColour = Color.White
            };
            label.Location = new Point(right, y);

            DisplayStats[Stat.Accuracy] = new DXLabel
            {
                Parent = StatsAttackTab,
                Location = new Point(label.Location.X + 45, y),
                ForeColour = Color.White,
                Text = "0",
                Size = labelValueSize,
                AutoSize = false,
                DrawFormat = TextFormatFlags.Right,
                Tag = label
            };

            label = new DXLabel
            {
                Parent = StatsAttackTab,
                Text = CEnvir.Language.CharacterCharacterTabStatsAttackTabASpeedLabel,
                ForeColour = Color.White
            };
            label.Location = new Point(right, y += rowSpacing);

            DisplayStats[Stat.AttackSpeed] = new DXLabel
            {
                Parent = StatsAttackTab,
                Location = new Point(label.Location.X + 45, y),
                ForeColour = Color.White,
                Text = "0",
                Size = labelValueSize,
                AutoSize = false,
                DrawFormat = TextFormatFlags.Right,
                Tag = label
            };

            label = new DXLabel
            {
                Parent = StatsAttackTab,
                Text = CEnvir.Language.CharacterCharacterTabStatsAttackTabLuckLabel,
                ForeColour = Color.White
            };
            label.Location = new Point(right, y += rowSpacing);

            DisplayStats[Stat.Luck] = new DXLabel
            {
                Parent = StatsAttackTab,
                Location = new Point(label.Location.X + 45, y),
                ForeColour = Color.White,
                Text = "0",
                Size = labelValueSize,
                AutoSize = false,
                DrawFormat = TextFormatFlags.Right,
                Tag = label
            };

            label = new DXLabel
            {
                Parent = StatsAttackTab,
                Text = CEnvir.Language.CharacterCharacterTabStatsAttackTabCritChanceLabel,
                ForeColour = Color.White
            };
            label.Location = new Point(right, y += rowSpacing);

            DisplayStats[Stat.CriticalChance] = new DXLabel
            {
                Parent = StatsAttackTab,
                Location = new Point(label.Location.X + 45, y),
                ForeColour = Color.White,
                Text = "0",
                Size = labelValueSize,
                AutoSize = false,
                DrawFormat = TextFormatFlags.Right,
                Tag = label
            };

            #endregion

            #region StatsDefenseTab

            y = yStart;

            label = new DXLabel
            {
                Parent = StatsDefenseTab,
                Text = CEnvir.Language.CommonStatusAC + ":",
                ForeColour = Color.White
            };
            label.Location = new Point(left, y);

            DisplayStats[Stat.MaxAC] = new DXLabel
            {
                Parent = StatsDefenseTab,
                Location = new Point(label.Location.X + 45, y),
                ForeColour = Color.White,
                Text = "0-0",
                Size = labelValueSize,
                AutoSize = false,
                DrawFormat = TextFormatFlags.Right,
                Tag = label
            };

            label = new DXLabel
            {
                Parent = StatsDefenseTab,
                Text = CEnvir.Language.CommonStatusMR + ":",
                ForeColour = Color.White
            };
            label.Location = new Point(left, y += rowSpacing);

            DisplayStats[Stat.MaxMR] = new DXLabel
            {
                Parent = StatsDefenseTab,
                Location = new Point(label.Location.X + 45, y),
                ForeColour = Color.White,
                Text = "0-0",
                Size = labelValueSize,
                AutoSize = false,
                DrawFormat = TextFormatFlags.Right,
                Tag = label
            };

            y = yStart;

            label = new DXLabel
            {
                Parent = StatsDefenseTab,
                Text = CEnvir.Language.CharacterCharacterTabStatsDefenseTabAgilityLabel,
                ForeColour = Color.White
            };
            label.Location = new Point(right, y);

            DisplayStats[Stat.Agility] = new DXLabel
            {
                Parent = StatsDefenseTab,
                Location = new Point(label.Location.X + 45, y),
                ForeColour = Color.White,
                Text = "0",
                Size = labelValueSize,
                AutoSize = false,
                DrawFormat = TextFormatFlags.Right,
                Tag = label
            };

            label = new DXLabel
            {
                Parent = StatsDefenseTab,
                Text = CEnvir.Language.CharacterCharacterTabStatsDefenseTabLifeStealLabel,
                ForeColour = Color.White
            };
            label.Location = new Point(right, y += rowSpacing);

            DisplayStats[Stat.LifeSteal] = new DXLabel
            {
                Parent = StatsDefenseTab,
                Location = new Point(label.Location.X + 45, y),
                ForeColour = Color.White,
                Text = "0",
                Size = labelValueSize,
                AutoSize = false,
                DrawFormat = TextFormatFlags.Right,
                Tag = label
            };

            #endregion

            #region StatsWeightTab

            y = yStart;

            label = new DXLabel
            {
                Parent = StatsWeightTab,
                Text = CEnvir.Language.CharacterCharacterTabStatsWeightTabBodyWLabel,
                ForeColour = Color.White
            };
            label.Location = new Point(left, y);

            WearWeightLabel = new DXLabel
            {
                Parent = StatsWeightTab,
                Location = new Point(label.Location.X + 45, y),
                ForeColour = Color.White,
                Text = "0 / 0",
                Size = labelValueSize,
                AutoSize = false,
                DrawFormat = TextFormatFlags.Right,
                Tag = label
            };

            label = new DXLabel
            {
                Parent = StatsWeightTab,
                Text = CEnvir.Language.CharacterCharacterTabStatsWeightTabHandWLabel,
                ForeColour = Color.White
            };
            label.Location = new Point(left, y += rowSpacing);

            HandWeightLabel = new DXLabel
            {
                Parent = StatsWeightTab,
                Location = new Point(label.Location.X + 45, y),
                ForeColour = Color.White,
                Text = "0 / 0",
                Size = labelValueSize,
                AutoSize = false,
                DrawFormat = TextFormatFlags.Right,
                Tag = label
            };

            #endregion

            #region StatsAdditionalTab

            y = yStart;

            label = new DXLabel
            {
                Parent = StatsOtherTab,
                Text = CEnvir.Language.CharacterCharacterTabStatsAdditionalTabComfortLabel,
                ForeColour = Color.White
            };
            label.Location = new Point(left, y);

            DisplayStats[Stat.Comfort] = new DXLabel
            {
                Parent = StatsOtherTab,
                Location = new Point(label.Location.X + 45, y),
                ForeColour = Color.White,
                Text = "0",
                Size = labelValueSize,
                AutoSize = false,
                DrawFormat = TextFormatFlags.Right,
                Tag = label
            };

            label = new DXLabel
            {
                Parent = StatsOtherTab,
                Text = CEnvir.Language.CharacterCharacterTabStatsAdditionalTabPickupRadiusLabel,
                ForeColour = Color.White
            };
            label.Location = new Point(left, y += rowSpacing);

            DisplayStats[Stat.PickUpRadius] = new DXLabel
            {
                Parent = StatsOtherTab,
                Location = new Point(label.Location.X + 45, y),
                ForeColour = Color.White,
                Text = "0",
                Size = labelValueSize,
                AutoSize = false,
                DrawFormat = TextFormatFlags.Right,
                Tag = label
            };

            y = yStart;

            label = new DXLabel
            {
                Parent = StatsOtherTab,
                Text = CEnvir.Language.CharacterCharacterTabStatsAdditionalTabGoldRateLabel,
                ForeColour = Color.White
            };
            label.Location = new Point(right, y);

            DisplayStats[Stat.GoldRate] = new DXLabel
            {
                Parent = StatsOtherTab,
                Location = new Point(label.Location.X + 45, y),
                ForeColour = Color.White,
                Text = "0",
                Size = labelValueSize,
                AutoSize = false,
                DrawFormat = TextFormatFlags.Right,
                Tag = label
            };

            label = new DXLabel
            {
                Parent = StatsOtherTab,
                Text = CEnvir.Language.CharacterCharacterTabStatsAdditionalTabDropRateLabel,
                ForeColour = Color.White
            };
            label.Location = new Point(right, y += rowSpacing);

            DisplayStats[Stat.DropRate] = new DXLabel
            {
                Parent = StatsOtherTab,
                Location = new Point(label.Location.X + 45, y),
                ForeColour = Color.White,
                Text = "0",
                Size = labelValueSize,
                AutoSize = false,
                DrawFormat = TextFormatFlags.Right,
                Tag = label
            };

            label = new DXLabel
            {
                Parent = StatsOtherTab,
                Text = CEnvir.Language.CharacterCharacterTabStatsAdditionalTabExpRateLabel,
                ForeColour = Color.White
            };
            label.Location = new Point(right, y += rowSpacing);

            DisplayStats[Stat.ExperienceRate] = new DXLabel
            {
                Parent = StatsOtherTab,
                Location = new Point(label.Location.X + 45, y),
                ForeColour = Color.White,
                Text = "0",
                Size = labelValueSize,
                AutoSize = false,
                DrawFormat = TextFormatFlags.Right,
                Tag = label
            };

            #endregion

            #region StatsElementAttackTab

            y = yStart;

            label = new DXLabel
            {
                Parent = StatsElementAttackTab,
                Text = CEnvir.Language.CommonStatusFire + ":",
                ForeColour = Color.White
            };
            label.Location = new Point(left, y);

            DXImageControl icon = new DXImageControl
            {
                Parent = StatsElementAttackTab,
                LibraryFile = LibraryFile.ProgUse,
                Index = 600,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = CEnvir.Language.CommonStatusFire,
                Visible = false
            };
            icon.Location = new Point(label.Location.X + 75, y - 3);

            AttackStats[Stat.FireAttack] = new DXLabel
            {
                Parent = StatsElementAttackTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            label = new DXLabel
            {
                Parent = StatsElementAttackTab,
                Text = CEnvir.Language.CommonStatusIce + ":",
                ForeColour = Color.White
            };
            label.Location = new Point(left, y += rowSpacing);

            icon = new DXImageControl
            {
                Parent = StatsElementAttackTab,
                LibraryFile = LibraryFile.ProgUse,
                Index = 601,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = CEnvir.Language.CommonStatusIce,
                Visible = false
            };
            icon.Location = new Point(label.Location.X + 75, y - 3);

            AttackStats[Stat.IceAttack] = new DXLabel
            {
                Parent = StatsElementAttackTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            label = new DXLabel
            {
                Parent = StatsElementAttackTab,
                Text = CEnvir.Language.CommonStatusLightning + ":",
                ForeColour = Color.White
            };
            label.Location = new Point(left, y += rowSpacing);

            icon = new DXImageControl
            {
                Parent = StatsElementAttackTab,
                LibraryFile = LibraryFile.ProgUse,
                Index = 602,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = CEnvir.Language.CommonStatusLightning,
                Visible = false
            };
            icon.Location = new Point(label.Location.X + 75, y - 3);

            AttackStats[Stat.LightningAttack] = new DXLabel
            {
                Parent = StatsElementAttackTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            label = new DXLabel
            {
                Parent = StatsElementAttackTab,
                Text = CEnvir.Language.CommonStatusWind + ":",
                ForeColour = Color.White
            };
            label.Location = new Point(left, y += rowSpacing);

            icon = new DXImageControl
            {
                Parent = StatsElementAttackTab,
                LibraryFile = LibraryFile.ProgUse,
                Index = 603,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = CEnvir.Language.CommonStatusWind,
                Visible = false
            };
            icon.Location = new Point(label.Location.X + 75, y - 3);

            AttackStats[Stat.WindAttack] = new DXLabel
            {
                Parent = StatsElementAttackTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            y = yStart;

            label = new DXLabel
            {
                Parent = StatsElementAttackTab,
                Text = CEnvir.Language.CommonStatusHoly + ":",
                ForeColour = Color.White
            };
            label.Location = new Point(right, y);

            icon = new DXImageControl
            {
                Parent = StatsElementAttackTab,
                LibraryFile = LibraryFile.ProgUse,
                Index = 604,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = CEnvir.Language.CommonStatusHoly,
                Visible = false
            };
            icon.Location = new Point(label.Location.X + 75, y - 3);

            AttackStats[Stat.HolyAttack] = new DXLabel
            {
                Parent = StatsElementAttackTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            label = new DXLabel
            {
                Parent = StatsElementAttackTab,
                Text = CEnvir.Language.CommonStatusDark + ":",
                ForeColour = Color.White
            };
            label.Location = new Point(right, y += rowSpacing);

            icon = new DXImageControl
            {
                Parent = StatsElementAttackTab,
                LibraryFile = LibraryFile.ProgUse,
                Index = 605,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = CEnvir.Language.CommonStatusDark,
                Visible = false
            };
            icon.Location = new Point(label.Location.X + 75, y - 3);

            AttackStats[Stat.DarkAttack] = new DXLabel
            {
                Parent = StatsElementAttackTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            label = new DXLabel
            {
                Parent = StatsElementAttackTab,
                Text = CEnvir.Language.CommonStatusPhantom + ":",
                ForeColour = Color.White
            };
            label.Location = new Point(right, y += rowSpacing);

            icon = new DXImageControl
            {
                Parent = StatsElementAttackTab,
                LibraryFile = LibraryFile.ProgUse,
                Index = 606,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = CEnvir.Language.CommonStatusPhantom,
                Visible = false
            };
            icon.Location = new Point(label.Location.X + 75, y - 3);

            AttackStats[Stat.PhantomAttack] = new DXLabel
            {
                Parent = StatsElementAttackTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            #endregion

            #region StatsElementAdvantageTab

            y = yStart;

            label = new DXLabel
            {
                Parent = StatsElementAdvantageTab,
                Text = CEnvir.Language.CommonStatusFire + ":",
                ForeColour = Color.White
            };
            label.Location = new Point(left, y);

            icon = new DXImageControl
            {
                Parent = StatsElementAdvantageTab,
                LibraryFile = LibraryFile.ProgUse,
                Index = 600,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = CEnvir.Language.CommonStatusFire,
                Visible = false
            };
            icon.Location = new Point(label.Location.X + 75, y - 3);

            AdvantageStats[Stat.FireResistance] = new DXLabel
            {
                Parent = StatsElementAdvantageTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            label = new DXLabel
            {
                Parent = StatsElementAdvantageTab,
                Text = CEnvir.Language.CommonStatusIce + ":",
                ForeColour = Color.White
            };
            label.Location = new Point(left, y += rowSpacing);

            icon = new DXImageControl
            {
                Parent = StatsElementAdvantageTab,
                LibraryFile = LibraryFile.ProgUse,
                Index = 601,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = CEnvir.Language.CommonStatusIce,
                Visible = false
            };
            icon.Location = new Point(label.Location.X + 75, y - 3);

            AdvantageStats[Stat.IceResistance] = new DXLabel
            {
                Parent = StatsElementAdvantageTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            label = new DXLabel
            {
                Parent = StatsElementAdvantageTab,
                Text = CEnvir.Language.CommonStatusLightning + ":",
                ForeColour = Color.White
            };
            label.Location = new Point(left, y += rowSpacing);

            icon = new DXImageControl
            {
                Parent = StatsElementAdvantageTab,
                LibraryFile = LibraryFile.ProgUse,
                Index = 602,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = CEnvir.Language.CommonStatusLightning,
                Visible = false
            };
            icon.Location = new Point(label.Location.X + 75, y - 3);

            AdvantageStats[Stat.LightningResistance] = new DXLabel
            {
                Parent = StatsElementAdvantageTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            label = new DXLabel
            {
                Parent = StatsElementAdvantageTab,
                Text = CEnvir.Language.CommonStatusWind + ":",
                ForeColour = Color.White
            };
            label.Location = new Point(left, y += rowSpacing);

            icon = new DXImageControl
            {
                Parent = StatsElementAdvantageTab,
                LibraryFile = LibraryFile.ProgUse,
                Index = 603,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = CEnvir.Language.CommonStatusWind,
                Visible = false
            };
            icon.Location = new Point(label.Location.X + 75, y - 3);

            AdvantageStats[Stat.WindResistance] = new DXLabel
            {
                Parent = StatsElementAdvantageTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            y = yStart;

            label = new DXLabel
            {
                Parent = StatsElementAdvantageTab,
                Text = CEnvir.Language.CommonStatusHoly + ":",
                ForeColour = Color.White
            };
            label.Location = new Point(right, y);

            icon = new DXImageControl
            {
                Parent = StatsElementAdvantageTab,
                LibraryFile = LibraryFile.ProgUse,
                Index = 604,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = CEnvir.Language.CommonStatusHoly,
                Visible = false
            };
            icon.Location = new Point(label.Location.X + 75, y - 3);

            AdvantageStats[Stat.HolyResistance] = new DXLabel
            {
                Parent = StatsElementAdvantageTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            label = new DXLabel
            {
                Parent = StatsElementAdvantageTab,
                Text = CEnvir.Language.CommonStatusDark + ":",
                ForeColour = Color.White
            };
            label.Location = new Point(right, y += rowSpacing);

            icon = new DXImageControl
            {
                Parent = StatsElementAdvantageTab,
                LibraryFile = LibraryFile.ProgUse,
                Index = 605,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = CEnvir.Language.CommonStatusDark,
                Visible = false
            };
            icon.Location = new Point(label.Location.X + 75, y - 3);

            AdvantageStats[Stat.DarkResistance] = new DXLabel
            {
                Parent = StatsElementAdvantageTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            label = new DXLabel
            {
                Parent = StatsElementAdvantageTab,
                Text = CEnvir.Language.CommonStatusPhantom + ":",
                ForeColour = Color.White
            };
            label.Location = new Point(right, y += rowSpacing);

            icon = new DXImageControl
            {
                Parent = StatsElementAdvantageTab,
                LibraryFile = LibraryFile.ProgUse,
                Index = 606,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = CEnvir.Language.CommonStatusPhantom,
                Visible = false
            };
            icon.Location = new Point(label.Location.X + 75, y - 3);

            AdvantageStats[Stat.PhantomResistance] = new DXLabel
            {
                Parent = StatsElementAdvantageTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            label = new DXLabel
            {
                Parent = StatsElementAdvantageTab,
                Text = CEnvir.Language.CommonStatusPhysical + ":",
                ForeColour = Color.White
            };
            label.Location = new Point(right, y += rowSpacing);

            icon = new DXImageControl
            {
                Parent = StatsElementAdvantageTab,
                LibraryFile = LibraryFile.GameInter,
                Index = 1517,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = CEnvir.Language.CommonStatusPhysical,
                Visible = false
            };
            icon.Location = new Point(label.Location.X + 75, y - 3);

            AdvantageStats[Stat.PhysicalResistance] = new DXLabel
            {
                Parent = StatsElementAdvantageTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            #endregion

            #region StatsElementDisadvantageTab

            y = yStart;

            label = new DXLabel
            {
                Parent = StatsElementDisadvantageTab,
                Text = CEnvir.Language.CommonStatusFire + ":",
                ForeColour = Color.White
            };
            label.Location = new Point(left, y);

            icon = new DXImageControl
            {
                Parent = StatsElementDisadvantageTab,
                LibraryFile = LibraryFile.ProgUse,
                Index = 600,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = CEnvir.Language.CommonStatusFire,
                Visible = false
            };
            icon.Location = new Point(label.Location.X + 75, y - 3);

            DisadvantageStats[Stat.FireResistance] = new DXLabel
            {
                Parent = StatsElementDisadvantageTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            label = new DXLabel
            {
                Parent = StatsElementDisadvantageTab,
                Text = CEnvir.Language.CommonStatusIce + ":",
                ForeColour = Color.White
            };
            label.Location = new Point(left, y += rowSpacing);

            icon = new DXImageControl
            {
                Parent = StatsElementDisadvantageTab,
                LibraryFile = LibraryFile.ProgUse,
                Index = 601,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = CEnvir.Language.CommonStatusIce,
                Visible = false
            };
            icon.Location = new Point(label.Location.X + 75, y - 3);

            DisadvantageStats[Stat.IceResistance] = new DXLabel
            {
                Parent = StatsElementDisadvantageTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            label = new DXLabel
            {
                Parent = StatsElementDisadvantageTab,
                Text = CEnvir.Language.CommonStatusLightning + ":",
                ForeColour = Color.White
            };
            label.Location = new Point(left, y += rowSpacing);

            icon = new DXImageControl
            {
                Parent = StatsElementDisadvantageTab,
                LibraryFile = LibraryFile.ProgUse,
                Index = 602,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = CEnvir.Language.CommonStatusLightning,
                Visible = false
            };
            icon.Location = new Point(label.Location.X + 75, y - 3);

            DisadvantageStats[Stat.LightningResistance] = new DXLabel
            {
                Parent = StatsElementDisadvantageTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            label = new DXLabel
            {
                Parent = StatsElementDisadvantageTab,
                Text = CEnvir.Language.CommonStatusWind + ":",
                ForeColour = Color.White
            };
            label.Location = new Point(left, y += rowSpacing);

            icon = new DXImageControl
            {
                Parent = StatsElementDisadvantageTab,
                LibraryFile = LibraryFile.ProgUse,
                Index = 603,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = CEnvir.Language.CommonStatusWind,
                Visible = false
            };
            icon.Location = new Point(label.Location.X + 75, y - 3);

            DisadvantageStats[Stat.WindResistance] = new DXLabel
            {
                Parent = StatsElementDisadvantageTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            y = yStart;

            label = new DXLabel
            {
                Parent = StatsElementDisadvantageTab,
                Text = CEnvir.Language.CommonStatusHoly + ":",
                ForeColour = Color.White
            };
            label.Location = new Point(right, y);

            icon = new DXImageControl
            {
                Parent = StatsElementDisadvantageTab,
                LibraryFile = LibraryFile.ProgUse,
                Index = 604,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = CEnvir.Language.CommonStatusHoly,
                Visible = false
            };
            icon.Location = new Point(label.Location.X + 75, y - 3);

            DisadvantageStats[Stat.HolyResistance] = new DXLabel
            {
                Parent = StatsElementDisadvantageTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            label = new DXLabel
            {
                Parent = StatsElementDisadvantageTab,
                Text = CEnvir.Language.CommonStatusDark + ":",
                ForeColour = Color.White
            };
            label.Location = new Point(right, y += rowSpacing);

            icon = new DXImageControl
            {
                Parent = StatsElementDisadvantageTab,
                LibraryFile = LibraryFile.ProgUse,
                Index = 605,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = CEnvir.Language.CommonStatusDark,
                Visible = false
            };
            icon.Location = new Point(label.Location.X + 75, y - 3);

            DisadvantageStats[Stat.DarkResistance] = new DXLabel
            {
                Parent = StatsElementDisadvantageTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            label = new DXLabel
            {
                Parent = StatsElementDisadvantageTab,
                Text = CEnvir.Language.CommonStatusPhantom + ":",
                ForeColour = Color.White
            };
            label.Location = new Point(right, y += rowSpacing);

            icon = new DXImageControl
            {
                Parent = StatsElementDisadvantageTab,
                LibraryFile = LibraryFile.ProgUse,
                Index = 606,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = CEnvir.Language.CommonStatusPhantom,
                Visible = false,
            };
            icon.Location = new Point(label.Location.X + 75, y - 3);

            DisadvantageStats[Stat.PhantomResistance] = new DXLabel
            {
                Parent = StatsElementDisadvantageTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            label = new DXLabel
            {
                Parent = StatsElementDisadvantageTab,
                Text = CEnvir.Language.CommonStatusPhysical + ":",
                ForeColour = Color.White
            };
            label.Location = new Point(right, y += rowSpacing);

            icon = new DXImageControl
            {
                Parent = StatsElementDisadvantageTab,
                LibraryFile = LibraryFile.GameInter,
                Index = 1517,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = CEnvir.Language.CommonStatusPhysical,
                Visible = false,
            };
            icon.Location = new Point(label.Location.X + 75, y - 3);

            DisadvantageStats[Stat.PhysicalResistance] = new DXLabel
            {
                Parent = StatsElementDisadvantageTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            #endregion

            #endregion

            #region Hermit

            HermitShowConfirmation = new DXCheckBox
            {
                Parent = HermitTab,
                Label = { Text = CEnvir.Language.CharacterHermitTabShowConfirmationLabel },
                Checked = true,
                Location = new Point(175, 78)
            };

            label = new DXLabel
            {
                Parent = HermitTab,
                Text = CEnvir.Language.CharacterHermitTabUnspentPointsLabel
            };
            label.Location = new Point(HermitTab.Size.Width / 4 * 2 - label.Size.Width + 28, 151);

            HermitRemainingLabel = new DXLabel
            {
                Parent = HermitTab,
                Location = new Point(label.Location.X + label.Size.Width - 5, label.Location.Y),
                ForeColour = Color.White,
                Text = "0"
            };

            int x = 26;

            #region Hermit Buttons

            var button = new DXButton
            {
                Parent = HermitTab,
                Location = new Point(x, 98),
                Label = { Text = CEnvir.Language.CommonStatusAC },
                ButtonType = ButtonType.SmallButton,
                Size = new Size(65, SmallButtonHeight)
            };
            button.MouseClick += (o, e) =>
            {
                if (HermitPoints == 0) return;

                if (HermitShowConfirmation.Checked)
                {
                    DXMessageBox box = new DXMessageBox(string.Format(CEnvir.Language.CharacterHermitTabButtonsConfirmationMessage, CEnvir.Language.CommonStatusAC), CEnvir.Language.CharacterHermitTabButtonsConfirmationCaption, DXMessageBoxButtons.YesNo);

                    box.YesButton.MouseClick += (o1, e1) =>
                    {
                        CEnvir.Enqueue(new C.Hermit { Stat = Stat.MaxAC });
                    };
                }
                else
                {
                    CEnvir.Enqueue(new C.Hermit { Stat = Stat.MaxAC });
                }
            };

            button = new DXButton
            {
                Parent = HermitTab,
                Location = new Point(x, 123),
                Label = { Text = CEnvir.Language.CommonStatusMR },
                ButtonType = ButtonType.SmallButton,
                Size = new Size(65, SmallButtonHeight),
            };
            button.MouseClick += (o, e) =>
            {
                if (HermitPoints == 0) return;

                if (HermitShowConfirmation.Checked)
                {
                    DXMessageBox box = new DXMessageBox(string.Format(CEnvir.Language.CharacterHermitTabButtonsConfirmationMessage, CEnvir.Language.CommonStatusMR), CEnvir.Language.CharacterHermitTabButtonsConfirmationCaption, DXMessageBoxButtons.YesNo);

                    box.YesButton.MouseClick += (o1, e1) =>
                    {
                        CEnvir.Enqueue(new C.Hermit { Stat = Stat.MaxMR });
                    };
                }
                else
                {
                    CEnvir.Enqueue(new C.Hermit { Stat = Stat.MaxMR });
                }
            };

            x += button.Size.Width + 5;

            button = new DXButton
            {
                Parent = HermitTab,
                Location = new Point(x, 98),
                Label = { Text = CEnvir.Language.CommonStatusHealth },
                ButtonType = ButtonType.SmallButton,
                Size = new Size(65, SmallButtonHeight)
            };
            button.MouseClick += (o, e) =>
            {
                if (HermitPoints == 0) return;

                if (HermitShowConfirmation.Checked)
                {
                    DXMessageBox box = new DXMessageBox(string.Format(CEnvir.Language.CharacterHermitTabButtonsConfirmationMessage, CEnvir.Language.CommonStatusHealth), CEnvir.Language.CharacterHermitTabButtonsConfirmationCaption, DXMessageBoxButtons.YesNo);

                    box.YesButton.MouseClick += (o1, e1) =>
                    {
                        CEnvir.Enqueue(new C.Hermit { Stat = Stat.Health });
                    };
                }
                else
                {
                    CEnvir.Enqueue(new C.Hermit { Stat = Stat.Health });
                }
            };

            button = new DXButton
            {
                Parent = HermitTab,
                Location = new Point(x, 123),
                Label = { Text = CEnvir.Language.CommonStatusMana },
                ButtonType = ButtonType.SmallButton,
                Size = new Size(65, SmallButtonHeight)
            };
            button.MouseClick += (o, e) =>
            {
                if (HermitPoints == 0) return;

                if (HermitShowConfirmation.Checked)
                {
                    DXMessageBox box = new DXMessageBox(string.Format(CEnvir.Language.CharacterHermitTabButtonsConfirmationMessage, CEnvir.Language.CommonStatusMana), CEnvir.Language.CharacterHermitTabButtonsConfirmationCaption, DXMessageBoxButtons.YesNo);

                    box.YesButton.MouseClick += (o1, e1) =>
                    {
                        CEnvir.Enqueue(new C.Hermit { Stat = Stat.Mana });
                    };
                }
                else
                {
                    CEnvir.Enqueue(new C.Hermit { Stat = Stat.Mana });
                }
            };

            x += button.Size.Width + 5;

            button = new DXButton
            {
                Parent = HermitTab,
                Location = new Point(x, 98),
                Label = { Text = CEnvir.Language.CommonStatusDC },
                ButtonType = ButtonType.SmallButton,
                Size = new Size(65, SmallButtonHeight)
            };
            button.MouseClick += (o, e) =>
            {
                if (HermitPoints == 0) return;

                if (HermitShowConfirmation.Checked)
                {
                    DXMessageBox box = new DXMessageBox(string.Format(CEnvir.Language.CharacterHermitTabButtonsConfirmationMessage, CEnvir.Language.CommonStatusDC), CEnvir.Language.CharacterHermitTabButtonsConfirmationCaption, DXMessageBoxButtons.YesNo);

                    box.YesButton.MouseClick += (o1, e1) =>
                    {
                        CEnvir.Enqueue(new C.Hermit { Stat = Stat.MaxDC });
                    };
                }
                else
                {
                    CEnvir.Enqueue(new C.Hermit { Stat = Stat.MaxDC });
                }
            };

            button = new DXButton
            {
                Parent = HermitTab,
                Location = new Point(x, 123),
                Label = { Text = CEnvir.Language.CharacterHermitTabButtonsMCLabel },
                ButtonType = ButtonType.SmallButton,
                Size = new Size(65, SmallButtonHeight)
            };
            button.MouseClick += (o, e) =>
            {
                if (HermitPoints == 0) return;

                if (HermitShowConfirmation.Checked)
                {
                    DXMessageBox box = new DXMessageBox(CEnvir.Language.CharacterHermitTabButtonsConfirmationMCMessage, CEnvir.Language.CharacterHermitTabButtonsConfirmationCaption, DXMessageBoxButtons.YesNo);

                    box.YesButton.MouseClick += (o1, e1) =>
                    {
                        CEnvir.Enqueue(new C.Hermit { Stat = Stat.MaxMC });
                    };
                }
                else
                {
                    CEnvir.Enqueue(new C.Hermit { Stat = Stat.MaxMC });
                }
            };

            x += button.Size.Width + 5;

            button = new DXButton
            {
                Parent = HermitTab,
                Location = new Point(x, 98),
                Label = { Text = CEnvir.Language.CharacterHermitTabButtonsSCLabel },
                ButtonType = ButtonType.SmallButton,
                Size = new Size(65, SmallButtonHeight)
            };
            button.MouseClick += (o, e) =>
            {
                if (HermitPoints == 0) return;

                if (HermitShowConfirmation.Checked)
                {
                    DXMessageBox box = new DXMessageBox(CEnvir.Language.CharacterHermitTabButtonsConfirmationSCMessage, CEnvir.Language.CharacterHermitTabButtonsConfirmationCaption, DXMessageBoxButtons.YesNo);

                    box.YesButton.MouseClick += (o1, e1) =>
                    {
                        CEnvir.Enqueue(new C.Hermit { Stat = Stat.MaxSC });
                    };
                }
                else
                {
                    CEnvir.Enqueue(new C.Hermit { Stat = Stat.MaxSC });
                }
            };

            button = new DXButton
            {
                Parent = HermitTab,
                Location = new Point(x, 123),
                Label = { Text = CEnvir.Language.CharacterHermitTabButtonsElementLabel },
                ButtonType = ButtonType.SmallButton,
                Size = new Size(65, SmallButtonHeight)
            };
            button.MouseClick += (o, e) =>
            {
                if (HermitPoints == 0) return;

                if (HermitShowConfirmation.Checked)
                {
                    DXMessageBox box = new DXMessageBox(CEnvir.Language.CharacterHermitTabButtonsConfirmationElementMessage, CEnvir.Language.CharacterHermitTabButtonsConfirmationCaption, DXMessageBoxButtons.YesNo);

                    box.YesButton.MouseClick += (o1, e1) =>
                    {
                        CEnvir.Enqueue(new C.Hermit { Stat = Stat.WeaponElement });
                    };
                }
                else
                {
                    CEnvir.Enqueue(new C.Hermit { Stat = Stat.WeaponElement });
                }
            };

            #endregion

            #region Hermit Stats
            int xOffset = 40;

            label = new DXLabel
            {
                Parent = HermitTab,
                Text = CEnvir.Language.CommonStatusAC + ":"
            };
            label.Location = new Point(HermitTab.Size.Width / 4 - label.Size.Width + xOffset, 176);

            HermitDisplayStats[Stat.MaxAC] = new DXLabel
            {
                Parent = HermitTab,
                Location = new Point(label.Location.X + label.Size.Width - 5, label.Location.Y),
                ForeColour = Color.White,
                Text = "0-0"
            };

            label = new DXLabel
            {
                Parent = HermitTab,
                Text = CEnvir.Language.CommonStatusMR + ":"
            };
            label.Location = new Point(HermitTab.Size.Width / 4 * 2 - label.Size.Width + xOffset, 176);

            HermitDisplayStats[Stat.MaxMR] = new DXLabel
            {
                Parent = HermitTab,
                Location = new Point(label.Location.X + label.Size.Width - 5, label.Location.Y),
                ForeColour = Color.White,
                Text = "0-0"
            };

            label = new DXLabel
            {
                Parent = HermitTab,
                Text = CEnvir.Language.CommonStatusDC + ":"
            };
            label.Location = new Point(HermitTab.Size.Width / 5 - label.Size.Width + xOffset, 206);

            HermitDisplayStats[Stat.MaxDC] = new DXLabel
            {
                Parent = HermitTab,
                Location = new Point(label.Location.X + label.Size.Width - 5, label.Location.Y),
                ForeColour = Color.White,
                Text = "0-0"
            };

            label = new DXLabel
            {
                Parent = HermitTab,
                Text = CEnvir.Language.CommonStatusMC + ":"
            };
            label.Location = new Point(HermitTab.Size.Width / 5 * 2 - label.Size.Width + xOffset, 206);

            HermitDisplayStats[Stat.MaxMC] = new DXLabel
            {
                Parent = HermitTab,
                Location = new Point(label.Location.X + label.Size.Width - 5, label.Location.Y),
                ForeColour = Color.White,
                Text = "0-0"
            };

            label = new DXLabel
            {
                Parent = HermitTab,
                Text = CEnvir.Language.CommonStatusSC + ":"
            };
            label.Location = new Point(HermitTab.Size.Width / 5 * 3 - label.Size.Width + xOffset, 206);

            HermitDisplayStats[Stat.MaxSC] = new DXLabel
            {
                Parent = HermitTab,
                Location = new Point(label.Location.X + label.Size.Width - 5, label.Location.Y),
                ForeColour = Color.White,
                Text = "0-0"
            };

            label = new DXLabel
            {
                Parent = HermitTab,
                Text = CEnvir.Language.CommonStatusHealth + ":"
            };
            label.Location = new Point(HermitTab.Size.Width / 4 - label.Size.Width + xOffset, 236);

            HermitDisplayStats[Stat.Health] = new DXLabel
            {
                Parent = HermitTab,
                Location = new Point(label.Location.X + label.Size.Width - 5, label.Location.Y),
                ForeColour = Color.White,
                Text = "0"
            };

            label = new DXLabel
            {
                Parent = HermitTab,
                Text = CEnvir.Language.CommonStatusMana + ":"
            };
            label.Location = new Point(HermitTab.Size.Width / 4 * 2 - label.Size.Width + xOffset, 236);

            HermitDisplayStats[Stat.Mana] = new DXLabel
            {
                Parent = HermitTab,
                Location = new Point(label.Location.X + label.Size.Width - 5, label.Location.Y),
                ForeColour = Color.White,
                Text = "0"
            };


            #region Attack

            label = new DXLabel
            {
                Parent = HermitTab,
                Text = CEnvir.Language.CommonStatusElementAttack + ":"
            };
            label.Location = new Point(65 - label.Size.Width, 296);

            icon = new DXImageControl
            {
                Parent = HermitTab,
                LibraryFile = LibraryFile.ProgUse,
                Index = 600,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = CEnvir.Language.CommonStatusFire,
            };
            icon.Location = new Point(label.Location.X + label.Size.Width, label.Location.Y + (label.Size.Height - icon.Size.Height) / 2);

            HermitAttackStats[Stat.FireAttack] = new DXLabel
            {
                Parent = HermitTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            icon = new DXImageControl
            {
                Parent = HermitTab,
                LibraryFile = LibraryFile.ProgUse,
                Index = 601,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = CEnvir.Language.CommonStatusIce,
            };
            icon.Location = new Point(label.Location.X + label.Size.Width + 50, label.Location.Y + (label.Size.Height - icon.Size.Height) / 2);

            HermitAttackStats[Stat.IceAttack] = new DXLabel
            {
                Parent = HermitTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            icon = new DXImageControl
            {
                Parent = HermitTab,
                LibraryFile = LibraryFile.ProgUse,
                Index = 602,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = CEnvir.Language.CommonStatusLightning,
            };
            icon.Location = new Point(label.Location.X + label.Size.Width + 100, label.Location.Y + (label.Size.Height - icon.Size.Height) / 2);

            HermitAttackStats[Stat.LightningAttack] = new DXLabel
            {
                Parent = HermitTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            icon = new DXImageControl
            {
                Parent = HermitTab,
                LibraryFile = LibraryFile.ProgUse,
                Index = 603,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = CEnvir.Language.CommonStatusWind,
            };
            icon.Location = new Point(label.Location.X + label.Size.Width + 150, label.Location.Y + (label.Size.Height - icon.Size.Height) / 2);

            HermitAttackStats[Stat.WindAttack] = new DXLabel
            {
                Parent = HermitTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            icon = new DXImageControl
            {
                Parent = HermitTab,
                LibraryFile = LibraryFile.ProgUse,
                Index = 604,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = CEnvir.Language.CommonStatusHoly,
            };
            icon.Location = new Point(label.Location.X + label.Size.Width, label.Location.Y + (label.Size.Height - icon.Size.Height) / 2 + 30);

            HermitAttackStats[Stat.HolyAttack] = new DXLabel
            {
                Parent = HermitTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y + 31),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            icon = new DXImageControl
            {
                Parent = HermitTab,
                LibraryFile = LibraryFile.ProgUse,
                Index = 605,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = CEnvir.Language.CommonStatusDark,
            };
            icon.Location = new Point(label.Location.X + label.Size.Width + 50, label.Location.Y + (label.Size.Height - icon.Size.Height) / 2 + 31);

            HermitAttackStats[Stat.DarkAttack] = new DXLabel
            {
                Parent = HermitTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y + 31),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            icon = new DXImageControl
            {
                Parent = HermitTab,
                LibraryFile = LibraryFile.ProgUse,
                Index = 606,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = CEnvir.Language.CommonStatusPhantom,
            };
            icon.Location = new Point(label.Location.X + label.Size.Width + 100, label.Location.Y + (label.Size.Height - icon.Size.Height) / 2 + 31);

            HermitAttackStats[Stat.PhantomAttack] = new DXLabel
            {
                Parent = HermitTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y + 31),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            #endregion


            #endregion

            #endregion

            #region Discipline

            DisciplineLevel = new DXImageControl
            {
                Parent = DisciplineTab,
                Index = 215,
                LibraryFile = LibraryFile.Interface,
                Size = new Size(256, 192)
            };
            DisciplineLevel.Location = new Point((Size.Width - DisciplineLevel.Size.Width) / 2, 64);

            label = new DXLabel
            {
                Parent = DisciplineTab,
                Text = CEnvir.Language.CharacterDisciplineTabLvLabel,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.Left,
            };
            label.Location = new Point(13, 313);

            DisciplineLevelLabel = new DXLabel
            {
                Parent = DisciplineTab,
                AutoSize = false,
                Size = new Size(46, 18),
                Location = new Point(116, 314),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Text = "0",
                ForeColour = Color.White
            };

            label = new DXLabel
            {
                Parent = DisciplineTab,
                Text = CEnvir.Language.CharacterDisciplineTabUnusedLabel,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.Left,
                Visible = false
            };
            label.Location = new Point(166, 314);

            DisciplineUnusedLabel = new DXLabel
            {
                Parent = DisciplineTab,
                AutoSize = false,
                Size = new Size(49, 18),
                Location = new Point(266, 314),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Text = "???",
                ForeColour = Color.White,
                Visible = false
            };

            label = new DXLabel
            {
                Parent = DisciplineTab,
                Text = CEnvir.Language.CharacterDisciplineTabExpLabel,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.Left
            };
            label.Location = new Point(13, 336);

            DisciplineExperienceLabel = new DXLabel
            {
                Parent = DisciplineTab,
                AutoSize = false,
                Size = new Size(303, 18),
                Location = new Point(14, 336),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Text = "0/0",
                ForeColour = Color.White
            };

            DisciplineButton = new DXButton
            {
                Parent = DisciplineTab,
                Label = { Text = CEnvir.Language.CharacterDisciplineTabButtonGainLabel },
                Location = new Point(182, 266),
                Size = new Size(120, DefaultHeight),
                Enabled = false
            };
            DisciplineButton.MouseClick += (o, e) =>
            {
                var nextLevel = GetNextDisciplineLevel();

                if (nextLevel != null)
                {
                    DXMessageBox box = new DXMessageBox(string.Format(CEnvir.Language.CharacterDisciplineTabConfirmationMessage, nextLevel.RequiredGold), CEnvir.Language.CharacterDisciplineTabConfirmationCaption, DXMessageBoxButtons.YesNo);

                    box.YesButton.MouseClick += (o, e1) => CEnvir.Enqueue(new C.IncreaseDiscipline());
                }
            };

            #endregion
        }

        private void Cell_MouseLeave(object sender, EventArgs e)
        {
            foreach (var stat in DisplayStats.Keys)
                DisplayStats[stat].ForeColour = Color.White;

            foreach (var stat in AttackStats.Keys)
                AttackStats[stat].ForeColour = Color.White;

            foreach (var stat in AdvantageStats.Keys)
                AdvantageStats[stat].ForeColour = Color.White;

            foreach (var stat in DisadvantageStats.Keys)
                DisadvantageStats[stat].ForeColour = Color.White;

            WearWeightLabel.ForeColour = Color.White;

            HandWeightLabel.ForeColour = Color.White;
        }

        private void Cell_MouseEnter(object sender, EventArgs e)
        {
            DXItemCell cell = sender as DXItemCell;

            if (cell.Item != null)
            {
                foreach (var stat in DisplayStats.Keys)
                    DisplayStats[stat].ForeColour = HasStat(cell.Item, stat) ? Color.Orange : Color.White;

                foreach (var stat in AttackStats.Keys)
                    AttackStats[stat].ForeColour = HasStat(cell.Item, stat) ? Color.Orange : Color.White;

                foreach (var stat in AdvantageStats.Keys)
                    AdvantageStats[stat].ForeColour = HasStat(cell.Item, stat) ? Color.Orange : Color.White;

                foreach (var stat in DisadvantageStats.Keys)
                    DisadvantageStats[stat].ForeColour = HasStat(cell.Item, stat) ? Color.Orange : Color.White;

                WearWeightLabel.ForeColour = cell.Item.Weight > 0 && (cell.Item.Info.ItemType != ItemType.Weapon && cell.Item.Info.ItemType != ItemType.Torch) ? Color.Orange : Color.White;

                HandWeightLabel.ForeColour = cell.Item.Weight > 0 && (cell.Item.Info.ItemType == ItemType.Weapon || cell.Item.Info.ItemType == ItemType.Torch) ? Color.Orange : Color.White;
            }
        }

        private bool HasStat(ClientUserItem item, Stat stat)
        {
            return item.Info.Stats[stat] != 0 || item.AddedStats[stat] != 0;
        }

        private void CharacterTab_BeforeChildrenDraw(object sender, EventArgs e)
        {
            int x = 130;
            int y = 270;

            ClientUserItem weapon = Grid[(int)EquipmentSlot.Weapon]?.Item;
            ClientUserItem armour = Grid[(int)EquipmentSlot.Armour]?.Item;
            ClientUserItem helmet = Grid[(int)EquipmentSlot.Helmet]?.Item;
            ClientUserItem shield = Grid[(int)EquipmentSlot.Shield]?.Item;
            ClientUserItem costume = Grid[(int)EquipmentSlot.Costume]?.Item;

            if (Fame > 0)
            {
                MirImage image = FameEffectDecider.GetFameEffectImageOrNull(Fame, out int offSetX, out int offSetY);
                if (image != null)
                {
                    bool oldBlend = DXManager.Blending;
                    float oldRate = DXManager.BlendRate;

                    int x1 = 257 + offSetX;
                    int y1 = 76 + offSetY;

                    DXManager.SetBlend(true, 0.8F);
                    PresentTexture(image.Image, CharacterTab, new Rectangle(DisplayArea.X + x1 + image.OffSetX, DisplayArea.Y + y1 + image.OffSetY, image.Width, image.Height), ForeColour, this);
                    DXManager.SetBlend(oldBlend, oldRate);
                }
            }

            if (armour != null && costume == null)
            {
                MirImage image = EquipEffectDecider.GetEffectImageOrNull(armour, Gender);
                if (image != null)
                {
                    bool oldBlend = DXManager.Blending;
                    float oldRate = DXManager.BlendRate;

                    DXManager.SetBlend(true, 0.8F);
                    PresentTexture(image.Image, CharacterTab, new Rectangle(DisplayArea.X + x + image.OffSetX, DisplayArea.Y + y + image.OffSetY, image.Width, image.Height), ForeColour, this);
                    DXManager.SetBlend(oldBlend, oldRate);
                }
            }

            if (!CEnvir.LibraryList.TryGetValue(LibraryFile.ProgUse, out MirLibrary library)) return;

            if (!HideBody)
            {
                if (Class == MirClass.Assassin && Gender == MirGender.Female && HairType == 1 && Grid[(int)EquipmentSlot.Helmet].Item == null)
                    library.Draw(1160, DisplayArea.X + x, DisplayArea.Y + y, HairColour, true, 1F, ImageType.Image);

                switch (Gender)
                {
                    case MirGender.Male:
                        library.Draw(0, DisplayArea.X + x, DisplayArea.Y + y, Color.White, true, 1F, ImageType.Image);
                        break;
                    case MirGender.Female:
                        library.Draw(1, DisplayArea.X + x, DisplayArea.Y + y, Color.White, true, 1F, ImageType.Image);
                        break;
                }
            }

            if (CEnvir.LibraryList.TryGetValue(LibraryFile.Equip, out library))
            {
                if (costume != null)
                {
                    int costumeIndex = costume.Info.Image;
                    library.Draw(costumeIndex, DisplayArea.X + x, DisplayArea.Y + y, Color.White, true, 1F, ImageType.Image);
                }
                else if (armour != null)
                {
                    int armourIndex = armour.Info.Image;
                    library.Draw(armourIndex, DisplayArea.X + x, DisplayArea.Y + y, Color.White, true, 1F, ImageType.Image);
                    library.Draw(armourIndex, DisplayArea.X + x, DisplayArea.Y + y, armour.Colour, true, 1F, ImageType.Overlay);
                }

                if (!HideBody)
                {
                    if (weapon != null)
                    {
                        int weaponIndex = weapon.Info.Image;
                        library.Draw(weaponIndex, DisplayArea.X + x, DisplayArea.Y + y, Color.White, true, 1F, ImageType.Image);
                        library.Draw(weaponIndex, DisplayArea.X + x, DisplayArea.Y + y, weapon.Colour, true, 1F, ImageType.Overlay);

                        MirImage image = EquipEffectDecider.GetEffectImageOrNull(weapon, Gender);
                        if (image != null)
                        {
                            bool oldBlend = DXManager.Blending;
                            float oldRate = DXManager.BlendRate;

                            DXManager.SetBlend(true, 0.8F);
                            PresentTexture(image.Image, CharacterTab, new Rectangle(DisplayArea.X + x + image.OffSetX, DisplayArea.Y + y + image.OffSetY, image.Width, image.Height), ForeColour, this);
                            DXManager.SetBlend(oldBlend, oldRate);
                        }
                    }

                    if (shield != null)
                    {
                        int shieldIndex = shield.Info.Image;
                        library.Draw(shieldIndex, DisplayArea.X + x, DisplayArea.Y + y, Color.White, true, 1F, ImageType.Image);
                        library.Draw(shieldIndex, DisplayArea.X + x, DisplayArea.Y + y, shield.Colour, true, 1F, ImageType.Overlay);

                        MirImage image = EquipEffectDecider.GetEffectImageOrNull(shield, Gender);
                        if (image != null)
                        {
                            bool oldBlend = DXManager.Blending;
                            float oldRate = DXManager.BlendRate;

                            DXManager.SetBlend(true, 0.8F);
                            PresentTexture(image.Image, CharacterTab, new Rectangle(DisplayArea.X + x + image.OffSetX, DisplayArea.Y + y + image.OffSetY, image.Width, image.Height), ForeColour, this);
                            DXManager.SetBlend(oldBlend, oldRate);
                        }
                    }
                }
            }
            if (Inspect && GuildFlag > -1)
            {
                if (CEnvir.LibraryList.TryGetValue(LibraryFile.GameInter, out MirLibrary gameLibrary))
                {
                    var guildFlag = 1690 + GuildFlag;

                    var flagImage = gameLibrary.CreateImage(guildFlag, ImageType.Image);
                    var flagOverlay = gameLibrary.CreateImage(guildFlag, ImageType.Overlay);

                    var flagX = x - 100;
                    var flagY = 105;

                    if (flagImage != null)
                    {
                        PresentTexture(flagImage.Image, CharacterTab, new Rectangle(DisplayArea.X + flagX + flagImage.OffSetX, DisplayArea.Y + flagY + flagImage.OffSetY, flagImage.Width, flagImage.Height), ForeColour, this);
                    }

                    if (flagOverlay != null)
                    {
                        PresentTexture(flagOverlay.Overlay, CharacterTab, new Rectangle(DisplayArea.X + flagX + flagOverlay.OffSetX, DisplayArea.Y + flagY + flagOverlay.OffSetY, flagOverlay.Width, flagOverlay.Height), GuildColour, this);
                    }
                }
            }

            if (HideHead) return;
            if (helmet != null && library != null)
            {
                int index = helmet.Info.Image;
                library.Draw(index, DisplayArea.X + x, DisplayArea.Y + y, Color.White, true, 1F, ImageType.Image);
                library.Draw(index, DisplayArea.X + x, DisplayArea.Y + y, helmet.Colour, true, 1F, ImageType.Overlay);
            }
            else if (HairType > 0)
            {
                library = CEnvir.LibraryList[LibraryFile.ProgUse];

                switch (Class)
                {
                    case MirClass.Warrior:
                    case MirClass.Wizard:
                    case MirClass.Taoist:
                        switch (Gender)
                        {
                            case MirGender.Male:
                                library.Draw(60 + HairType - 1, DisplayArea.X + x, DisplayArea.Y + y, HairColour, true, 1F, ImageType.Image);
                                break;
                            case MirGender.Female:
                                library.Draw(80 + HairType - 1, DisplayArea.X + x, DisplayArea.Y + y, HairColour, true, 1F, ImageType.Image);
                                break;
                        }
                        break;
                    case MirClass.Assassin:
                        switch (Gender)
                        {
                            case MirGender.Male:
                                library.Draw(1100 + HairType - 1, DisplayArea.X + x, DisplayArea.Y + y, HairColour, true, 1F, ImageType.Image);
                                break;
                            case MirGender.Female:
                                library.Draw(1120 + HairType - 1, DisplayArea.X + x, DisplayArea.Y + y, HairColour, true, 1F, ImageType.Image);
                                break;
                        }
                        break;
                }
            }
        }

        #region Methods

        public void Draw(DXItemCell cell, int index, bool backgroundCell = false, bool visible = true)
        {
            if (InterfaceLibrary == null) return;

            Size s;
            int x, y;

            if (backgroundCell)
            {
                s = InterfaceLibrary.GetSize(65);
                x = (cell.Size.Width - s.Width) / 2 + cell.DisplayArea.X;
                y = (cell.Size.Height - s.Height) / 2 + cell.DisplayArea.Y;

                InterfaceLibrary.Draw(65, x, y, Color.White, false, 1F, ImageType.Image);
            }

            if (cell.Item != null) return;

            s = InterfaceLibrary.GetSize(index);
            x = (cell.Size.Width - s.Width) / 2 + cell.DisplayArea.X;
            y = (cell.Size.Height - s.Height) / 2 + cell.DisplayArea.Y;

            InterfaceLibrary.Draw(index, x, y, Color.White, false, 0.2F, ImageType.Image);
        }

        public void UpdateStats()
        {
            foreach (KeyValuePair<Stat, DXLabel> pair in DisplayStats)
            {
                pair.Value.Text = Stats.GetFormat(pair.Key);

                if (pair.Value.Tag is DXLabel title && string.IsNullOrEmpty(title.Text))
                {
                    title.Text = Stats.GetTitle(pair.Key, false) + ": ";
                }
            }

            foreach (KeyValuePair<Stat, DXLabel> pair in AttackStats)
            {
                if (Stats[pair.Key] > 0)
                {
                    pair.Value.Text = $"+{Stats[pair.Key]}";
                    pair.Value.ForeColour = Color.DeepSkyBlue;
                    ((DXImageControl)pair.Value.Tag).ForeColour = Color.White;
                }
                else
                {
                    pair.Value.Text = "0";
                    pair.Value.ForeColour = Color.FromArgb(60, 60, 60);
                    ((DXImageControl)pair.Value.Tag).ForeColour = Color.FromArgb(60, 60, 60);
                }

                //if (pair.Value.Tag is DXImageControl icon)
                //{
                //    icon.Hint = Stats.GetTitle(pair.Key)?.TrimEnd(':', ' ');
                //}
            }

            foreach (KeyValuePair<Stat, DXLabel> pair in AdvantageStats)
            {
                if (Stats[pair.Key] > 0)
                {
                    pair.Value.Text = $"x{Stats[pair.Key]}";
                    pair.Value.ForeColour = Color.Lime;
                    ((DXImageControl)pair.Value.Tag).ForeColour = Color.White;
                }
                else
                {
                    pair.Value.Text = "0";
                    pair.Value.ForeColour = Color.FromArgb(60, 60, 60);
                    ((DXImageControl)pair.Value.Tag).ForeColour = Color.FromArgb(60, 60, 60);
                }
            }

            foreach (KeyValuePair<Stat, DXLabel> pair in DisadvantageStats)
            {
                pair.Value.Text = Stats.GetFormat(pair.Key);

                if (Stats[pair.Key] < 0)
                {
                    pair.Value.Text = $"x{Math.Abs(Stats[pair.Key])}";
                    pair.Value.ForeColour = Color.IndianRed;
                    ((DXImageControl)pair.Value.Tag).ForeColour = Color.White;
                }
                else
                {
                    pair.Value.Text = "0";
                    pair.Value.ForeColour = Color.FromArgb(60, 60, 60);
                    ((DXImageControl)pair.Value.Tag).ForeColour = Color.FromArgb(60, 60, 60);
                }
            }


            foreach (KeyValuePair<Stat, DXLabel> pair in HermitDisplayStats)
                pair.Value.Text = HermitStats.GetFormat(pair.Key);


            foreach (KeyValuePair<Stat, DXLabel> pair in HermitAttackStats)
            {
                if (HermitStats[pair.Key] > 0)
                {
                    pair.Value.Text = $"+{HermitStats[pair.Key]}";
                    pair.Value.ForeColour = Color.White;
                    ((DXImageControl)pair.Value.Tag).ForeColour = Color.White;
                }
                else
                {
                    pair.Value.Text = "0";
                    pair.Value.ForeColour = Color.FromArgb(60, 60, 60);
                    ((DXImageControl)pair.Value.Tag).ForeColour = Color.FromArgb(60, 60, 60);
                }
            }

            HermitRemainingLabel.Text = HermitPoints.ToString();
        }

        public void NewInformation(S.Inspect p)
        {
            if (!Inspect) return;

            Visible = true;
            CharacterNameLabel.Text = p.Name;
            GuildNameLabel.Text = p.GuildName;
            GuildRankLabel.Text = p.GuildRank;

            _inspectGuildFlag = p.GuildFlag;
            _inspectGuildColour = p.GuildColour;

            //_inspectStats.Clear();
            //_inspectStats.Add(p.Stats);

            //_inspectHermitStats.Clear();
            //_inspectHermitStats.Add(p.HermitStats);
            //_inspectHermitPoints = p.HermitPoints;

            _inspectGender = p.Gender;
            _inspectClass = p.Class;
            _inspectLevel = p.Level;
            _inspectFame = p.Fame;

            MarriageIcon.Visible = !string.IsNullOrEmpty(p.Partner);
            MarriageLabel.Visible = !string.IsNullOrEmpty(p.Partner);
            MarriageLabel.Text = p.Partner;

            _inspectHairColour = p.HairColour;
            _inspectHairType = p.Hair;

            foreach (DXItemCell cell in Grid)
            {
                if (cell == null) continue;

                cell.Item = null;
            }

            foreach (ClientUserItem item in p.Items)
            {
                if (Grid[item.Slot] == null) continue;

                Grid[item.Slot].Item = item;
            }

            //WearWeightLabel.Text = $"{p.WearWeight}/{p.Stats[Stat.WearWeight]}";
            //HandWeightLabel.Text = $"{p.HandWeight}/{p.Stats[Stat.HandWeight]}";

            //WearWeightLabel.ForeColour = p.WearWeight > Stats[Stat.WearWeight] ? Color.Red : Color.White;
            //HandWeightLabel.ForeColour = p.HandWeight > Stats[Stat.HandWeight] ? Color.Red : Color.White;

            UpdateStats();
        }

        public void UpdateDiscipline()
        {
            if (Globals.DisciplineInfoList.Binding.Count == 0)
            {
                DisciplineButton.Enabled = false;
                return;
            }

            var nextLevel = GetNextDisciplineLevel();

            DisciplineButton.Enabled = nextLevel != null;

            var userDiscipline = GameScene.Game.User.Discipline;

            if (userDiscipline == null)
            {
                DisciplineLevel.Index = 215;
                DisciplineLevelLabel.Text = "0";
                DisciplineExperienceLabel.Text = $"0/0";

                List<MagicInfo> keys = new(DisciplineMagics.Keys);

                foreach (var key in keys)
                {
                    DisciplineMagics[key].Dispose();
                    DisciplineMagics[key] = null;
                }

                DisciplineMagics.Clear();
            }
            else
            {
                DisciplineLevel.Index = 215 + userDiscipline.Level;
                DisciplineLevelLabel.Text = userDiscipline.Level.ToString();

                if (nextLevel != null)
                    DisciplineExperienceLabel.Text = $"{userDiscipline.Experience}/{nextLevel.RequiredExperience}";
                else
                    DisciplineExperienceLabel.Text = $"{userDiscipline.Experience}/Max";

                int x = 51;

                foreach (var magic in userDiscipline.Magics)
                {
                    if (!DisciplineMagics.ContainsKey(magic.Info))
                    {
                        DisciplineMagicCell cell = new DisciplineMagicCell
                        {
                            Parent = DisciplineTab,
                            Info = magic.Info,
                            BackColour = Color.Empty,
                            Location = new Point(x, 380)
                        };
                        DisciplineMagics[magic.Info] = cell;
                    }

                    x += 62;
                }
            }
        }

        public void RefreshDisciplineMagic(MagicInfo info)
        {
            if (DisciplineMagics.ContainsKey(info))
                DisciplineMagics[info].Refresh();
        }

        private DisciplineInfo GetNextDisciplineLevel()
        {
            int currentLevel = 0;

            if (GameScene.Game.User.Discipline != null)
                currentLevel = GameScene.Game.User.Discipline.Level;

            var nextLevel = Globals.DisciplineInfoList.Binding.FirstOrDefault(x => x.Level == (currentLevel + 1));

            return nextLevel;
        }

        #endregion


        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (TabControl != null)
                {
                    if (!TabControl.IsDisposed)
                        TabControl.Dispose();

                    TabControl = null;
                }

                if (CloseButton != null)
                {
                    if (!CloseButton.IsDisposed)
                        CloseButton.Dispose();

                    CloseButton = null;
                }

                if (CharacterTab != null)
                {
                    if (!CharacterTab.IsDisposed)
                        CharacterTab.Dispose();

                    CharacterTab = null;
                }

                if (HermitTab != null)
                {
                    if (!HermitTab.IsDisposed)
                        HermitTab.Dispose();

                    HermitTab = null;
                }

                if (DisciplineTab != null)
                {
                    if (!DisciplineTab.IsDisposed)
                        DisciplineTab.Dispose();

                    DisciplineTab = null;
                }

                if (CharacterNameLabel != null)
                {
                    if (!CharacterNameLabel.IsDisposed)
                        CharacterNameLabel.Dispose();

                    CharacterNameLabel = null;
                }

                if (GuildNameLabel != null)
                {
                    if (!GuildNameLabel.IsDisposed)
                        GuildNameLabel.Dispose();

                    GuildNameLabel = null;
                }

                if (GuildRankLabel != null)
                {
                    if (!GuildRankLabel.IsDisposed)
                        GuildRankLabel.Dispose();

                    GuildRankLabel = null;
                }

                if (MarriageIcon != null)
                {
                    if (!MarriageIcon.IsDisposed)
                        MarriageIcon.Dispose();

                    MarriageIcon = null;
                }

                if (MarriageLabel != null)
                {
                    if (!MarriageLabel.IsDisposed)
                        MarriageLabel.Dispose();

                    MarriageLabel = null;
                }


                if (StatsAttackTab != null)
                {
                    if (!StatsAttackTab.IsDisposed)
                        StatsAttackTab.Dispose();

                    StatsAttackTab = null;
                }

                if (StatsAttackTab != null)
                {
                    if (!StatsAttackTab.IsDisposed)
                        StatsAttackTab.Dispose();

                    StatsAttackTab = null;
                }

                if (StatsDefenseTab != null)
                {
                    if (!StatsDefenseTab.IsDisposed)
                        StatsDefenseTab.Dispose();

                    StatsDefenseTab = null;
                }

                if (StatsWeightTab != null)
                {
                    if (!StatsWeightTab.IsDisposed)
                        StatsWeightTab.Dispose();

                    StatsWeightTab = null;
                }

                if (StatsOtherTab != null)
                {
                    if (!StatsOtherTab.IsDisposed)
                        StatsOtherTab.Dispose();

                    StatsOtherTab = null;
                }

                if (StatsElementAttackTab != null)
                {
                    if (!StatsElementAttackTab.IsDisposed)
                        StatsElementAttackTab.Dispose();

                    StatsElementAttackTab = null;
                }

                if (StatsElementAdvantageTab != null)
                {
                    if (!StatsElementAdvantageTab.IsDisposed)
                        StatsElementAdvantageTab.Dispose();

                    StatsElementAdvantageTab = null;
                }

                if (StatsElementDisadvantageTab != null)
                {
                    if (!StatsElementDisadvantageTab.IsDisposed)
                        StatsElementDisadvantageTab.Dispose();

                    StatsElementDisadvantageTab = null;
                }

                if (StatsTabControl != null)
                {
                    if (!StatsTabControl.IsDisposed)
                        StatsTabControl.Dispose();

                    StatsTabControl = null;
                }

                if (Grid != null)
                {
                    for (int i = 0; i < Grid.Length; i++)
                    {
                        if (Grid[i] != null)
                        {
                            if (!Grid[i].IsDisposed)
                                Grid[i].Dispose();

                            Grid[i] = null;
                        }
                    }

                    Grid = null;
                }

                if (WearWeightLabel != null)
                {
                    if (!WearWeightLabel.IsDisposed)
                        WearWeightLabel.Dispose();

                    WearWeightLabel = null;
                }

                if (HandWeightLabel != null)
                {
                    if (!HandWeightLabel.IsDisposed)
                        HandWeightLabel.Dispose();

                    HandWeightLabel = null;
                }

                foreach (KeyValuePair<Stat, DXLabel> pair in DisplayStats)
                {
                    if (pair.Value == null) continue;
                    if (pair.Value.IsDisposed) continue;

                    pair.Value.Dispose();
                }
                DisplayStats.Clear();
                DisplayStats = null;

                foreach (KeyValuePair<Stat, DXLabel> pair in AttackStats)
                {
                    if (pair.Value == null) continue;
                    if (pair.Value.IsDisposed) continue;

                    pair.Value.Dispose();
                }
                AttackStats.Clear();
                AttackStats = null;

                foreach (KeyValuePair<Stat, DXLabel> pair in AdvantageStats)
                {
                    if (pair.Value == null) continue;
                    if (pair.Value.IsDisposed) continue;

                    pair.Value.Dispose();
                }
                AdvantageStats.Clear();
                AdvantageStats = null;

                foreach (KeyValuePair<Stat, DXLabel> pair in DisadvantageStats)
                {
                    if (pair.Value == null) continue;
                    if (pair.Value.IsDisposed) continue;

                    pair.Value.Dispose();
                }
                DisadvantageStats.Clear();
                DisadvantageStats = null;

                if (HermitRemainingLabel != null)
                {
                    if (!HermitRemainingLabel.IsDisposed)
                        HermitRemainingLabel.Dispose();

                    HermitRemainingLabel = null;
                }

                if (HermitShowConfirmation != null)
                {
                    if (!HermitShowConfirmation.IsDisposed)
                        HermitShowConfirmation.Dispose();

                    HermitShowConfirmation = null;
                }

                if (DisciplineLevel != null)
                {
                    if (!DisciplineLevel.IsDisposed)
                        DisciplineLevel.Dispose();

                    DisciplineLevel = null;
                }

                if (DisciplineLevelLabel != null)
                {
                    if (!DisciplineLevelLabel.IsDisposed)
                        DisciplineLevelLabel.Dispose();

                    DisciplineLevelLabel = null;
                }

                if (DisciplineUnusedLabel != null)
                {
                    if (!DisciplineUnusedLabel.IsDisposed)
                        DisciplineUnusedLabel.Dispose();

                    DisciplineUnusedLabel = null;
                }

                if (DisciplineExperienceLabel != null)
                {
                    if (!DisciplineExperienceLabel.IsDisposed)
                        DisciplineExperienceLabel.Dispose();

                    DisciplineExperienceLabel = null;
                }

                if (DisciplineButton != null)
                {
                    if (!DisciplineButton.IsDisposed)
                        DisciplineButton.Dispose();

                    DisciplineButton = null;
                }

                foreach (KeyValuePair<Stat, DXLabel> pair in HermitDisplayStats)
                {
                    if (pair.Value == null) continue;
                    if (pair.Value.IsDisposed) continue;

                    pair.Value.Dispose();
                }
                HermitDisplayStats.Clear();
                HermitDisplayStats = null;

                foreach (KeyValuePair<Stat, DXLabel> pair in HermitAttackStats)
                {
                    if (pair.Value == null) continue;
                    if (pair.Value.IsDisposed) continue;

                    pair.Value.Dispose();
                }
                HermitAttackStats.Clear();
                HermitAttackStats = null;

                List<MagicInfo> keys = new(DisciplineMagics.Keys);

                foreach (var key in keys)
                {
                    DisciplineMagics[key].Dispose();
                    DisciplineMagics[key] = null;
                }

                DisciplineMagics.Clear();

                _inspectEquipment = null;
                _inspectStats = null;
                _inspectHermitStats = null;

                Settings = null;
            }
        }

        #endregion
    }

    public sealed class DisciplineMagicCell : DXControl
    {
        #region Properties

        #region Info
        public MagicInfo Info
        {
            get => _Info;
            set
            {
                if (_Info == value) return;

                MagicInfo oldValue = _Info;
                _Info = value;

                OnInfoChanged(oldValue, value);
            }
        }
        private MagicInfo _Info;
        public event EventHandler<EventArgs> InfoChanged;
        public void OnInfoChanged(MagicInfo oValue, MagicInfo nValue)
        {
            Image.Index = Info.Icon;
            Refresh();
            InfoChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        public DXImageControl Image;
        public DXLabel KeyLabel;

        #endregion

        public DisciplineMagicCell()
        {
            Size = new Size(36, 36);
            DrawTexture = true;

            Image = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.MagicIcon,
                Location = new Point(0, 0)
            };
            Image.MouseClick += Image_MouseClick;
            Image.KeyDown += Image_KeyDown;
            Image.MouseEnter += (o, e) => OnMouseEnter();
            Image.MouseLeave += (o, e) => OnMouseLeave();

            KeyLabel = new DXLabel
            {
                Parent = Image,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                IsControl = false,
                ForeColour = Color.Aquamarine,
                AutoSize = false,
                Size = new Size(36, 36),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter
            };
            KeyLabel.SizeChanged += (o, e) => KeyLabel.Location = new Point(Image.Size.Width - KeyLabel.Size.Width, Image.Size.Height - KeyLabel.Size.Height);
            KeyLabel.MouseEnter += (o, e) => OnMouseEnter();
            KeyLabel.MouseLeave += (o, e) => OnMouseLeave();
        }

        #region Methods
        private void Image_MouseClick(object sender, MouseEventArgs e)
        {
            if (GameScene.Game.Observer) return;

            ClientUserMagic magic;

            if (!MapObject.User.Magics.TryGetValue(Info, out magic)) return;

            switch (GameScene.Game.MagicBarBox.SpellSet)
            {
                case 1:
                    magic.Set1Key = SpellKey.None;
                    break;
                case 2:
                    magic.Set2Key = SpellKey.None;
                    break;
                case 3:
                    magic.Set3Key = SpellKey.None;
                    break;
                case 4:
                    magic.Set4Key = SpellKey.None;
                    break;

            }

            CEnvir.Enqueue(new C.MagicKey { Magic = magic.Info.Magic, Set1Key = magic.Set1Key, Set2Key = magic.Set2Key, Set3Key = magic.Set3Key, Set4Key = magic.Set4Key });
            Refresh();
            GameScene.Game.MagicBarBox.UpdateIcons();
        }

        private void Image_KeyDown(object sender, KeyEventArgs e)
        {
            if (GameScene.Game.Observer) return;

            if (e.Handled) return;
            if (MouseControl != Image) return;

            SpellKey key = SpellKey.None;

            foreach (KeyBindAction action in CEnvir.GetKeyAction(e.KeyCode))
            {
                switch (action)
                {
                    case KeyBindAction.SpellUse01:
                        key = SpellKey.Spell01;
                        break;
                    case KeyBindAction.SpellUse02:
                        key = SpellKey.Spell02;
                        break;
                    case KeyBindAction.SpellUse03:
                        key = SpellKey.Spell03;
                        break;
                    case KeyBindAction.SpellUse04:
                        key = SpellKey.Spell04;
                        break;
                    case KeyBindAction.SpellUse05:
                        key = SpellKey.Spell05;
                        break;
                    case KeyBindAction.SpellUse06:
                        key = SpellKey.Spell06;
                        break;
                    case KeyBindAction.SpellUse07:
                        key = SpellKey.Spell07;
                        break;
                    case KeyBindAction.SpellUse08:
                        key = SpellKey.Spell08;
                        break;
                    case KeyBindAction.SpellUse09:
                        key = SpellKey.Spell09;
                        break;
                    case KeyBindAction.SpellUse10:
                        key = SpellKey.Spell10;
                        break;
                    case KeyBindAction.SpellUse11:
                        key = SpellKey.Spell11;
                        break;
                    case KeyBindAction.SpellUse12:
                        key = SpellKey.Spell12;
                        break;
                    case KeyBindAction.SpellUse13:
                        key = SpellKey.Spell13;
                        break;
                    case KeyBindAction.SpellUse14:
                        key = SpellKey.Spell14;
                        break;
                    case KeyBindAction.SpellUse15:
                        key = SpellKey.Spell15;
                        break;
                    case KeyBindAction.SpellUse16:
                        key = SpellKey.Spell16;
                        break;
                    case KeyBindAction.SpellUse17:
                        key = SpellKey.Spell17;
                        break;
                    case KeyBindAction.SpellUse18:
                        key = SpellKey.Spell18;
                        break;
                    case KeyBindAction.SpellUse19:
                        key = SpellKey.Spell19;
                        break;
                    case KeyBindAction.SpellUse20:
                        key = SpellKey.Spell20;
                        break;
                    case KeyBindAction.SpellUse21:
                        key = SpellKey.Spell21;
                        break;
                    case KeyBindAction.SpellUse22:
                        key = SpellKey.Spell22;
                        break;
                    case KeyBindAction.SpellUse23:
                        key = SpellKey.Spell23;
                        break;
                    case KeyBindAction.SpellUse24:
                        key = SpellKey.Spell24;
                        break;
                    default:
                        continue;
                }

                e.Handled = true;
            }

            if (key == SpellKey.None) return;

            ClientUserMagic magic;

            if (!MapObject.User.Magics.TryGetValue(Info, out magic)) return;

            switch (GameScene.Game.MagicBarBox.SpellSet)
            {
                case 1:
                    magic.Set1Key = key;
                    break;
                case 2:
                    magic.Set2Key = key;
                    break;
                case 3:
                    magic.Set3Key = key;
                    break;
                case 4:
                    magic.Set4Key = key;
                    break;
            }

            foreach (KeyValuePair<MagicInfo, ClientUserMagic> pair in MapObject.User.Magics)
            {
                if (pair.Key == magic.Info) continue;

                if (pair.Value.Set1Key == magic.Set1Key && magic.Set1Key != SpellKey.None)
                {
                    pair.Value.Set1Key = SpellKey.None;

                    GameScene.Game.MagicBox.RefreshMagic(pair.Key);
                    GameScene.Game.CharacterBox.RefreshDisciplineMagic(pair.Key);
                }

                if (pair.Value.Set2Key == magic.Set2Key && magic.Set2Key != SpellKey.None)
                {
                    pair.Value.Set2Key = SpellKey.None;

                    GameScene.Game.MagicBox.RefreshMagic(pair.Key);
                    GameScene.Game.CharacterBox.RefreshDisciplineMagic(pair.Key);
                }

                if (pair.Value.Set3Key == magic.Set3Key && magic.Set3Key != SpellKey.None)
                {
                    pair.Value.Set3Key = SpellKey.None;

                    GameScene.Game.MagicBox.RefreshMagic(pair.Key);
                    GameScene.Game.CharacterBox.RefreshDisciplineMagic(pair.Key);
                }

                if (pair.Value.Set4Key == magic.Set4Key && magic.Set4Key != SpellKey.None)
                {
                    pair.Value.Set4Key = SpellKey.None;

                    GameScene.Game.MagicBox.RefreshMagic(pair.Key);
                    GameScene.Game.CharacterBox.RefreshDisciplineMagic(pair.Key);
                }
            }

            CEnvir.Enqueue(new C.MagicKey { Magic = magic.Info.Magic, Set1Key = magic.Set1Key, Set2Key = magic.Set2Key, Set3Key = magic.Set3Key, Set4Key = magic.Set4Key });
            Refresh();
            GameScene.Game.MagicBarBox.UpdateIcons();
        }

        public override void OnMouseEnter()
        {
            GameScene.Game.MouseMagic = Info;
        }
        public override void OnMouseLeave()
        {
            GameScene.Game.MouseMagic = null;
        }

        public void Refresh()
        {
            if (MapObject.User == null) return;

            if (MapObject.User.Magics.TryGetValue(Info, out ClientUserMagic magic))
            {
                SpellKey key = SpellKey.None;
                switch (GameScene.Game.MagicBarBox.SpellSet)
                {
                    case 1:
                        key = magic.Set1Key;
                        break;
                    case 2:
                        key = magic.Set2Key;
                        break;
                    case 3:
                        key = magic.Set3Key;
                        break;
                    case 4:
                        key = magic.Set4Key;
                        break;
                }

                Type type = typeof(SpellKey);

                MemberInfo[] infos = type.GetMember(key.ToString());

                DescriptionAttribute description = infos[0].GetCustomAttribute<DescriptionAttribute>();
                KeyLabel.Text = description?.Description;
            }

            if (this == MouseControl)
            {
                GameScene.Game.MouseMagic = null;
                GameScene.Game.MouseMagic = Info;
            }
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _Info = null;
                InfoChanged = null;

                if (Image != null)
                {
                    if (!Image.IsDisposed)
                        Image.Dispose();

                    Image = null;
                }

                if (KeyLabel != null)
                {
                    if (!KeyLabel.IsDisposed)
                        KeyLabel.Dispose();

                    KeyLabel = null;
                }
            }
        }

        #endregion
    }
}
