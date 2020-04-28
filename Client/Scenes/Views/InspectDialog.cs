using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Client.Controls;
using Client.Envir;
using Client.UserModels;
using Library;
using S =  Library.Network.ServerPackets;

//Cleaned
namespace Client.Scenes.Views
{
    public sealed class InspectDialog : DXWindow
    {
        #region Properties
        private DXTabControl TabControl;
        private DXTab CharacterTab, StatsTab, HermitTab;
        public DXLabel CharacterNameLabel, GuildNameLabel, GuildRankLabel;
        public DXImageControl MarriageIcon;


        public DXItemCell[] Grid;

        public DXLabel WearWeightLabel, HandWeightLabel;
        public Dictionary<Stat, DXLabel> DisplayStats = new Dictionary<Stat, DXLabel>();
        public Dictionary<Stat, DXLabel> AttackStats = new Dictionary<Stat, DXLabel>();
        public Dictionary<Stat, DXLabel> AdvantageStats = new Dictionary<Stat, DXLabel>();
        public Dictionary<Stat, DXLabel> DisadvantageStats = new Dictionary<Stat, DXLabel>();

        public Dictionary<Stat, DXLabel> HermitDisplayStats = new Dictionary<Stat, DXLabel>();
        public Dictionary<Stat, DXLabel> HermitAttackStats = new Dictionary<Stat, DXLabel>();
        public DXLabel RemainingLabel;

        public Stats Stats = new Stats();
        public Stats HermitStats = new Stats();
        public int HermitPoints;
        public ClientUserItem[] Equipment = new ClientUserItem[Globals.EquipmentSize];
        public MirClass Class;
        public MirGender Gender;
        public int HairType;
        public Color HairColour;
        public int Level;

        public override WindowType Type => WindowType.InspectBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisiblity => false;

        #endregion

        public InspectDialog()
        {
            HasTitle = false;
            HasFooter = true;
            SetClientSize(new Size(266, 371));




            TabControl = new DXTabControl
            {
                Parent = this,
                Location = ClientArea.Location,
                Size = ClientArea.Size,
            };
            CharacterTab = new DXTab
            {
                Parent = TabControl,
                Border = true,
                TabButton = { Label = { Text = "Character" } },
            };
            CharacterTab.BeforeChildrenDraw += CharacterTab_BeforeChildrenDraw;
            StatsTab = new DXTab
            {
                Parent = TabControl,
                Border = true,
                TabButton = { Label = { Text = "Stats" } },
            };
            HermitTab = new DXTab
            {
                Parent = TabControl,
                Border = true,
                TabButton = { Label = { Text = "Hermit" } },
            };
            DXControl namePanel = new DXControl
            {
                Parent = CharacterTab,
                Size = new Size(150, 45),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                Location = new Point((CharacterTab.Size.Width - 150) / 2, 5),

            };
            CharacterNameLabel = new DXLabel
            {
                AutoSize = false,
                Size = new Size(150, 20),
                ForeColour = Color.FromArgb(222, 255, 222),
                Outline = false,
                Parent = namePanel,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter
            };
            GuildNameLabel = new DXLabel
            {
                AutoSize = false,
                Size = new Size(150, 15),
                ForeColour = Color.FromArgb(255, 255, 181),
                Outline = false,
                Parent = namePanel,
                Location = new Point(0, CharacterNameLabel.Size.Height - 2),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter
            };
            GuildRankLabel = new DXLabel
            {
                AutoSize = false,
                Size = new Size(150, 15),
                ForeColour = Color.FromArgb(255, 206, 148),
                Outline = false,
                Parent = namePanel,
                Location = new Point(0, CharacterNameLabel.Size.Height + GuildNameLabel.Size.Height - 4),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter
            };
            TabControl.SelectedTab = CharacterTab;


            MarriageIcon = new DXImageControl
            {
                Parent = namePanel,
                LibraryFile = LibraryFile.GameInter,
                Index = 1298,
                Location = new Point(5, namePanel.Size.Height - 16),
                Visible = false,
            };

            Grid = new DXItemCell[Globals.EquipmentSize];

            DXItemCell cell;
            Grid[(int)EquipmentSlot.Weapon] = cell = new DXItemCell
            {
                Location = new Point(95, 60),
                Parent = CharacterTab,
                FixedBorder = true,
                Border = true,
                Slot = (int)EquipmentSlot.Weapon,
                ItemGrid = Equipment,
                GridType = GridType.Inspect,
                ReadOnly = true,
            };
            cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 35);

            Grid[(int)EquipmentSlot.Armour] = cell = new DXItemCell
            {
                Location = new Point(135, 60),
                Parent = CharacterTab,
                FixedBorder = true,
                Border = true,
                Slot = (int)EquipmentSlot.Armour,
                ItemGrid = Equipment,
                GridType = GridType.Inspect,
                ReadOnly = true,
            };
            cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 34);

            Grid[(int)EquipmentSlot.Shield] = cell = new DXItemCell
            {
                Location = new Point(175, 60),
                Parent = CharacterTab,
                FixedBorder = true,
                Border = true,
                Slot = (int)EquipmentSlot.Shield,
                ItemGrid = Equipment,
                GridType = GridType.Inspect,
                ReadOnly = true,
            };
            cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 105);

            Grid[(int)EquipmentSlot.Helmet] = cell = new DXItemCell
            {
                Location = new Point(215, 60),
                Parent = CharacterTab,
                FixedBorder = true,
                Border = true,
                Slot = (int)EquipmentSlot.Helmet,
                ItemGrid = Equipment,
                GridType = GridType.Inspect,
                ReadOnly = true,
            };
            cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 37);

            Grid[(int)EquipmentSlot.Torch] = cell = new DXItemCell
            {
                Location = new Point(215, 140),
                Parent = CharacterTab,
                FixedBorder = true,
                Border = true,
                Slot = (int)EquipmentSlot.Torch,
                ItemGrid = Equipment,
                GridType = GridType.Inspect,
                ReadOnly = true,
            };
            cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 38);

            Grid[(int)EquipmentSlot.Necklace] = cell = new DXItemCell
            {
                Location = new Point(215, 180),
                Parent = CharacterTab,
                FixedBorder = true,
                Border = true,
                Slot = (int)EquipmentSlot.Necklace,
                ItemGrid = Equipment,
                GridType = GridType.Inspect,
                ReadOnly = true,
            };
            cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 33);

            Grid[(int)EquipmentSlot.BraceletL] = cell = new DXItemCell
            {
                Location = new Point(15, 220),
                Parent = CharacterTab,
                FixedBorder = true,
                Border = true,
                Slot = (int)EquipmentSlot.BraceletL,
                ItemGrid = Equipment,
                GridType = GridType.Inspect,
                ReadOnly = true,
            };
            cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 32);

            Grid[(int)EquipmentSlot.BraceletR] = cell = new DXItemCell
            {
                Location = new Point(215, 220),
                Parent = CharacterTab,
                FixedBorder = true,
                Border = true,
                Slot = (int)EquipmentSlot.BraceletR,
                ItemGrid = Equipment,
                GridType = GridType.Inspect,
                ReadOnly = true,
            };
            cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 32);

            Grid[(int)EquipmentSlot.RingL] = cell = new DXItemCell
            {
                Location = new Point(15, 260),
                Parent = CharacterTab,
                FixedBorder = true,
                Border = true,
                Slot = (int)EquipmentSlot.RingL,
                ItemGrid = Equipment,
                GridType = GridType.Inspect,
                ReadOnly = true,
            };
            cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 31);

            Grid[(int)EquipmentSlot.RingR] = cell = new DXItemCell
            {
                Location = new Point(215, 260),
                Parent = CharacterTab,
                FixedBorder = true,
                Border = true,
                Slot = (int)EquipmentSlot.RingR,
                ItemGrid = Equipment,
                GridType = GridType.Inspect,
                ReadOnly = true,
            };
            cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 31);

            Grid[(int)EquipmentSlot.Emblem] = cell = new DXItemCell
            {
                Location = new Point(55, 300),
                Parent = CharacterTab,
                FixedBorder = true,
                Border = true,
                Slot = (int)EquipmentSlot.Emblem,
                ItemGrid = Equipment,
                GridType = GridType.Inspect,
                ReadOnly = true,
            };
            cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 104);

            Grid[(int)EquipmentSlot.Emblem] = cell = new DXItemCell
            {
                Location = new Point(15, 300),
                Parent = CharacterTab,
                FixedBorder = true,
                Border = true,
                Slot = (int)EquipmentSlot.Wings,
                ItemGrid = Equipment,
                GridType = GridType.Inspect,
                ReadOnly = true,
            };
            cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 190);

            Grid[(int)EquipmentSlot.Shoes] = cell = new DXItemCell
            {
                Location = new Point(95, 300),
                Parent = CharacterTab,
                FixedBorder = true,
                Border = true,
                Slot = (int)EquipmentSlot.Shoes,
                ItemGrid = Equipment,
                GridType = GridType.Inspect,
                ReadOnly = true,
            };
            cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 36);

            Grid[(int)EquipmentSlot.Poison] = cell = new DXItemCell
            {
                Location = new Point(135, 300),
                Parent = CharacterTab,
                FixedBorder = true,
                Border = true,
                Slot = (int)EquipmentSlot.Poison,
                ItemGrid = Equipment,
                GridType = GridType.Inspect,
                ReadOnly = true,
            };
            cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 40);

            Grid[(int)EquipmentSlot.Amulet] = cell = new DXItemCell
            {
                Location = new Point(175, 300),
                Parent = CharacterTab,
                FixedBorder = true,
                Border = true,
                Slot = (int)EquipmentSlot.Amulet,
                ItemGrid = Equipment,
                GridType = GridType.Inspect,
                ReadOnly = true,
            };
            cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 39);

            Grid[(int)EquipmentSlot.Flower] = cell = new DXItemCell
            {
                Location = new Point(215, 300),
                Parent = CharacterTab,
                FixedBorder = true,
                Border = true,
                Slot = (int)EquipmentSlot.Flower,
                ItemGrid = Equipment,
                GridType = GridType.Inspect,
                ReadOnly = true,
            };
            cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 81);

            Grid[(int)EquipmentSlot.HorseArmour] = cell = new DXItemCell
            {
                Location = new Point(215, 100),
                Parent = CharacterTab,
                FixedBorder = true,
                Border = true,
                Slot = (int)EquipmentSlot.HorseArmour,
                ItemGrid = Equipment,
                GridType = GridType.Inspect,
                ReadOnly = true,
            };
            cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 82);



            int y = 0;
            DXLabel label = new DXLabel
            {
                Parent = StatsTab,
                Text = "AC:"
            };
            label.Location = new Point(StatsTab.Size.Width / 4 - label.Size.Width + 25, y += 10);

            DisplayStats[Stat.MaxAC] = new DXLabel
            {
                Parent = StatsTab,
                Location = new Point(label.Location.X + label.Size.Width - 5, y),
                ForeColour = Color.White,
                Text = "0-0"
            };

            label = new DXLabel
            {
                Parent = StatsTab,
                Text = "MR:"
            };
            label.Location = new Point(StatsTab.Size.Width / 4 * 2 - label.Size.Width + 25, y);

            DisplayStats[Stat.MaxMR] = new DXLabel
            {
                Parent = StatsTab,
                Location = new Point(label.Location.X + label.Size.Width - 5, y),
                ForeColour = Color.White,
                Text = "0-0"
            };

            label = new DXLabel
            {
                Parent = StatsTab,
                Text = "DC:"
            };
            label.Location = new Point(StatsTab.Size.Width / 4 - label.Size.Width + 25, y += 20);

            DisplayStats[Stat.MaxDC] = new DXLabel
            {
                Parent = StatsTab,
                Location = new Point(label.Location.X + label.Size.Width - 5, y),
                ForeColour = Color.White,
                Text = "0-0"
            };

            label = new DXLabel
            {
                Parent = StatsTab,
                Text = "MC:"
            };
            label.Location = new Point(StatsTab.Size.Width / 4 * 2 - label.Size.Width + 25, y);

            DisplayStats[Stat.MaxMC] = new DXLabel
            {
                Parent = StatsTab,
                Location = new Point(label.Location.X + label.Size.Width - 5, y),
                ForeColour = Color.White,
                Text = "0-0"
            };

            label = new DXLabel
            {
                Parent = StatsTab,
                Text = "SC:"
            };
            label.Location = new Point(StatsTab.Size.Width / 4 * 3 - label.Size.Width + 25, y);

            DisplayStats[Stat.MaxSC] = new DXLabel
            {
                Parent = StatsTab,
                Location = new Point(label.Location.X + label.Size.Width - 5, y),
                ForeColour = Color.White,
                Text = "0-0"
            };


            label = new DXLabel
            {
                Parent = StatsTab,
                Text = "Accuracy:"
            };
            label.Location = new Point(StatsTab.Size.Width / 4 - label.Size.Width + 25, y += 20);

            DisplayStats[Stat.Accuracy] = new DXLabel
            {
                Parent = StatsTab,
                Location = new Point(label.Location.X + label.Size.Width - 5, y),
                ForeColour = Color.White,
                Text = "0"
            };


            label = new DXLabel
            {
                Parent = StatsTab,
                Text = "Agility:"
            };
            label.Location = new Point(StatsTab.Size.Width / 4 * 2 - label.Size.Width + 25, y);

            DisplayStats[Stat.Agility] = new DXLabel
            {
                Parent = StatsTab,
                Location = new Point(label.Location.X + label.Size.Width - 5, y),
                ForeColour = Color.White,
                Text = "0"
            };

            label = new DXLabel
            {
                Parent = StatsTab,
                Text = "Body W:"
            };
            label.Location = new Point(StatsTab.Size.Width / 4 - label.Size.Width + 25, y += 20);

            WearWeightLabel = new DXLabel
            {
                Parent = StatsTab,
                Location = new Point(label.Location.X + label.Size.Width - 5, y),
                ForeColour = Color.White,
                Text = "0"
            };

            label = new DXLabel
            {
                Parent = StatsTab,
                Text = "Hand W:"
            };
            label.Location = new Point(StatsTab.Size.Width / 4 * 3 - label.Size.Width + 25, y);

            HandWeightLabel = new DXLabel
            {
                Parent = StatsTab,
                Location = new Point(label.Location.X + label.Size.Width - 5, y),
                ForeColour = Color.White,
                Text = "0"
            };



            label = new DXLabel
            {
                Parent = StatsTab,
                Text = "A. Speed:"
            };
            label.Location = new Point(StatsTab.Size.Width / 4 - label.Size.Width + 25, y += 20);

            DisplayStats[Stat.AttackSpeed] = new DXLabel
            {
                Parent = StatsTab,
                Location = new Point(label.Location.X + label.Size.Width - 5, y),
                ForeColour = Color.White,
                Text = "0"
            };


            label = new DXLabel
            {
                Parent = StatsTab,
                Text = "Luck:"
            };
            label.Location = new Point(StatsTab.Size.Width / 4 * 2 - label.Size.Width + 25, y);

            DisplayStats[Stat.Luck] = new DXLabel
            {
                Parent = StatsTab,
                Location = new Point(label.Location.X + label.Size.Width - 5, y),
                ForeColour = Color.White,
                Text = "0"
            };

            label = new DXLabel
            {
                Parent = StatsTab,
                Text = "Comfort:"
            };
            label.Location = new Point(StatsTab.Size.Width / 4 * 3 - label.Size.Width + 25, y);

            DisplayStats[Stat.Comfort] = new DXLabel
            {
                Parent = StatsTab,
                Location = new Point(label.Location.X + label.Size.Width - 5, y),
                ForeColour = Color.White,
                Text = "0"
            };



            label = new DXLabel
            {
                Parent = StatsTab,
                Text = "Life Steal:"
            };
            label.Location = new Point(StatsTab.Size.Width / 4 - label.Size.Width + 25, y += 20);

            DisplayStats[Stat.LifeSteal] = new DXLabel
            {
                Parent = StatsTab,
                Location = new Point(label.Location.X + label.Size.Width - 5, y),
                ForeColour = Color.White,
                Text = "0"
            };

            label = new DXLabel
            {
                Parent = StatsTab,
                Text = "Gold Rate:"
            };
            label.Location = new Point(StatsTab.Size.Width / 4 * 3 - label.Size.Width + 25, y);

            DisplayStats[Stat.GoldRate] = new DXLabel
            {
                Parent = StatsTab,
                Location = new Point(label.Location.X + label.Size.Width - 5, y),
                ForeColour = Color.White,
                Text = "0"
            };

            label = new DXLabel
            {
                Parent = StatsTab,
                Text = "Critical Chance:"
            };
            label.Location = new Point(StatsTab.Size.Width / 4 - label.Size.Width + 25, y += 20);

            DisplayStats[Stat.CriticalChance] = new DXLabel
            {
                Parent = StatsTab,
                Location = new Point(label.Location.X + label.Size.Width - 5, y),
                ForeColour = Color.White,
                Text = "0"
            };

            label = new DXLabel
            {
                Parent = StatsTab,
                Text = "Drop Rate:"
            };
            label.Location = new Point(StatsTab.Size.Width / 4 * 3 - label.Size.Width + 25, y);

            DisplayStats[Stat.DropRate] = new DXLabel
            {
                Parent = StatsTab,
                Location = new Point(label.Location.X + label.Size.Width - 5, y),
                ForeColour = Color.White,
                Text = "0"
            };

            label = new DXLabel
            {
                Parent = StatsTab,
                Text = "Pick Up Radius:"
            };
            label.Location = new Point(StatsTab.Size.Width / 4 - label.Size.Width + 25, y += 20);

            DisplayStats[Stat.PickUpRadius] = new DXLabel
            {
                Parent = StatsTab,
                Location = new Point(label.Location.X + label.Size.Width - 5, y),
                ForeColour = Color.White,
                Text = "0"
            };


            label = new DXLabel
            {
                Parent = StatsTab,
                Text = "Exp. Rate:"
            };
            label.Location = new Point(StatsTab.Size.Width / 4 * 3 - label.Size.Width + 25, y);

            DisplayStats[Stat.ExperienceRate] = new DXLabel
            {
                Parent = StatsTab,
                Location = new Point(label.Location.X + label.Size.Width - 5, y),
                ForeColour = Color.White,
                Text = "0"
            };

            #region Attack


            label = new DXLabel
            {
                Parent = StatsTab,
                Text = "E. Att:"
            };
            label.Location = new Point(50 - label.Size.Width, 175);

            DXImageControl icon = new DXImageControl
            {
                Parent = StatsTab,
                LibraryFile = LibraryFile.ProgUse,
                Index = 600,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = "Fire",
            };
            icon.Location = new Point(label.Location.X + label.Size.Width, label.Location.Y + (label.Size.Height - icon.Size.Height) / 2);

            AttackStats[Stat.FireAttack] = new DXLabel
            {
                Parent = StatsTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            icon = new DXImageControl
            {
                Parent = StatsTab,
                LibraryFile = LibraryFile.ProgUse,
                Index = 601,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = "Ice",
            };
            icon.Location = new Point(label.Location.X + label.Size.Width + 50, label.Location.Y + (label.Size.Height - icon.Size.Height) / 2);

            AttackStats[Stat.IceAttack] = new DXLabel
            {
                Parent = StatsTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            icon = new DXImageControl
            {
                Parent = StatsTab,
                LibraryFile = LibraryFile.ProgUse,
                Index = 602,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = "Lightning",
            };
            icon.Location = new Point(label.Location.X + label.Size.Width + 100, label.Location.Y + (label.Size.Height - icon.Size.Height) / 2);

            AttackStats[Stat.LightningAttack] = new DXLabel
            {
                Parent = StatsTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            icon = new DXImageControl
            {
                Parent = StatsTab,
                LibraryFile = LibraryFile.ProgUse,
                Index = 603,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = "Wind",
            };
            icon.Location = new Point(label.Location.X + label.Size.Width + 150, label.Location.Y + (label.Size.Height - icon.Size.Height) / 2);

            AttackStats[Stat.WindAttack] = new DXLabel
            {
                Parent = StatsTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            icon = new DXImageControl
            {
                Parent = StatsTab,
                LibraryFile = LibraryFile.ProgUse,
                Index = 604,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = "Holy",
            };
            icon.Location = new Point(label.Location.X + label.Size.Width, label.Location.Y + (label.Size.Height - icon.Size.Height) / 2 + 25);

            AttackStats[Stat.HolyAttack] = new DXLabel
            {
                Parent = StatsTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y + 25),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            icon = new DXImageControl
            {
                Parent = StatsTab,
                LibraryFile = LibraryFile.ProgUse,
                Index = 605,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = "Dark",
            };
            icon.Location = new Point(label.Location.X + label.Size.Width + 50, label.Location.Y + (label.Size.Height - icon.Size.Height) / 2 + 25);

            AttackStats[Stat.DarkAttack] = new DXLabel
            {
                Parent = StatsTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y + 25),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            icon = new DXImageControl
            {
                Parent = StatsTab,
                LibraryFile = LibraryFile.ProgUse,
                Index = 606,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = "Phantom",
            };
            icon.Location = new Point(label.Location.X + label.Size.Width + 100, label.Location.Y + (label.Size.Height - icon.Size.Height) / 2 + 25);

            AttackStats[Stat.PhantomAttack] = new DXLabel
            {
                Parent = StatsTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y + 25),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            #endregion

            #region Resistance


            label = new DXLabel
            {
                Parent = StatsTab,
                Text = "E. Adv:"
            };
            label.Location = new Point(50 - label.Size.Width, 235);


            icon = new DXImageControl
            {
                Parent = StatsTab,
                LibraryFile = LibraryFile.ProgUse,
                Index = 600,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = "Fire",
            };
            icon.Location = new Point(label.Location.X + label.Size.Width, label.Location.Y + (label.Size.Height - icon.Size.Height) / 2);

            AdvantageStats[Stat.FireResistance] = new DXLabel
            {
                Parent = StatsTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            icon = new DXImageControl
            {
                Parent = StatsTab,
                LibraryFile = LibraryFile.ProgUse,
                Index = 601,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = "Ice",
            };
            icon.Location = new Point(label.Location.X + label.Size.Width + 50, label.Location.Y + (label.Size.Height - icon.Size.Height) / 2);

            AdvantageStats[Stat.IceResistance] = new DXLabel
            {
                Parent = StatsTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            icon = new DXImageControl
            {
                Parent = StatsTab,
                LibraryFile = LibraryFile.ProgUse,
                Index = 602,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = "Lightning",
            };
            icon.Location = new Point(label.Location.X + label.Size.Width + 100, label.Location.Y + (label.Size.Height - icon.Size.Height) / 2);

            AdvantageStats[Stat.LightningResistance] = new DXLabel
            {
                Parent = StatsTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            icon = new DXImageControl
            {
                Parent = StatsTab,
                LibraryFile = LibraryFile.ProgUse,
                Index = 603,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = "Wind",
            };
            icon.Location = new Point(label.Location.X + label.Size.Width + 150, label.Location.Y + (label.Size.Height - icon.Size.Height) / 2);

            AdvantageStats[Stat.WindResistance] = new DXLabel
            {
                Parent = StatsTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            icon = new DXImageControl
            {
                Parent = StatsTab,
                LibraryFile = LibraryFile.ProgUse,
                Index = 604,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = "Holy",
            };
            icon.Location = new Point(label.Location.X + label.Size.Width, label.Location.Y + (label.Size.Height - icon.Size.Height) / 2 + 25);

            AdvantageStats[Stat.HolyResistance] = new DXLabel
            {
                Parent = StatsTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y + 25),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            icon = new DXImageControl
            {
                Parent = StatsTab,
                LibraryFile = LibraryFile.ProgUse,
                Index = 605,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = "Dark",
            };
            icon.Location = new Point(label.Location.X + label.Size.Width + 50, label.Location.Y + (label.Size.Height - icon.Size.Height) / 2 + 25);

            AdvantageStats[Stat.DarkResistance] = new DXLabel
            {
                Parent = StatsTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y + 25),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            icon = new DXImageControl
            {
                Parent = StatsTab,
                LibraryFile = LibraryFile.ProgUse,
                Index = 606,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = "Phantom",
            };
            icon.Location = new Point(label.Location.X + label.Size.Width + 100, label.Location.Y + (label.Size.Height - icon.Size.Height) / 2 + 25);

            AdvantageStats[Stat.PhantomResistance] = new DXLabel
            {
                Parent = StatsTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y + 25),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            icon = new DXImageControl
            {
                Parent = StatsTab,
                LibraryFile = LibraryFile.GameInter,
                Index = 1517,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = "Physical",
            };
            icon.Location = new Point(label.Location.X + label.Size.Width + 150, label.Location.Y + (label.Size.Height - icon.Size.Height) / 2 + 25);

            AdvantageStats[Stat.PhysicalResistance] = new DXLabel
            {
                Parent = StatsTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y + 25),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            #endregion

            #region Resistance


            label = new DXLabel
            {
                Parent = StatsTab,
                Text = "E. Dis:"
            };
            label.Location = new Point(50 - label.Size.Width, 295);

            icon = new DXImageControl
            {
                Parent = StatsTab,
                LibraryFile = LibraryFile.ProgUse,
                Index = 600,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = "Fire",
            };
            icon.Location = new Point(label.Location.X + label.Size.Width, label.Location.Y + (label.Size.Height - icon.Size.Height) / 2);

            DisadvantageStats[Stat.FireResistance] = new DXLabel
            {
                Parent = StatsTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            icon = new DXImageControl
            {
                Parent = StatsTab,
                LibraryFile = LibraryFile.ProgUse,
                Index = 601,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = "Ice",
            };
            icon.Location = new Point(label.Location.X + label.Size.Width + 50, label.Location.Y + (label.Size.Height - icon.Size.Height) / 2);

            DisadvantageStats[Stat.IceResistance] = new DXLabel
            {
                Parent = StatsTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            icon = new DXImageControl
            {
                Parent = StatsTab,
                LibraryFile = LibraryFile.ProgUse,
                Index = 602,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = "Lightning",
            };
            icon.Location = new Point(label.Location.X + label.Size.Width + 100, label.Location.Y + (label.Size.Height - icon.Size.Height) / 2);

            DisadvantageStats[Stat.LightningResistance] = new DXLabel
            {
                Parent = StatsTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            icon = new DXImageControl
            {
                Parent = StatsTab,
                LibraryFile = LibraryFile.ProgUse,
                Index = 603,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = "Wind",
            };
            icon.Location = new Point(label.Location.X + label.Size.Width + 150, label.Location.Y + (label.Size.Height - icon.Size.Height) / 2);

            DisadvantageStats[Stat.WindResistance] = new DXLabel
            {
                Parent = StatsTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            icon = new DXImageControl
            {
                Parent = StatsTab,
                LibraryFile = LibraryFile.ProgUse,
                Index = 604,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = "Holy",
            };
            icon.Location = new Point(label.Location.X + label.Size.Width, label.Location.Y + (label.Size.Height - icon.Size.Height) / 2 + 25);

            DisadvantageStats[Stat.HolyResistance] = new DXLabel
            {
                Parent = StatsTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y + 25),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            icon = new DXImageControl
            {
                Parent = StatsTab,
                LibraryFile = LibraryFile.ProgUse,
                Index = 605,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = "Dark",
            };
            icon.Location = new Point(label.Location.X + label.Size.Width + 50, label.Location.Y + (label.Size.Height - icon.Size.Height) / 2 + 25);

            DisadvantageStats[Stat.DarkResistance] = new DXLabel
            {
                Parent = StatsTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y + 25),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            icon = new DXImageControl
            {
                Parent = StatsTab,
                LibraryFile = LibraryFile.ProgUse,
                Index = 606,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = "Phantom",
            };
            icon.Location = new Point(label.Location.X + label.Size.Width + 100, label.Location.Y + (label.Size.Height - icon.Size.Height) / 2 + 25);

            DisadvantageStats[Stat.PhantomResistance] = new DXLabel
            {
                Parent = StatsTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y + 25),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            icon = new DXImageControl
            {
                Parent = StatsTab,
                LibraryFile = LibraryFile.GameInter,
                Index = 1517,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = "Physical",
            };
            icon.Location = new Point(label.Location.X + label.Size.Width + 150, label.Location.Y + (label.Size.Height - icon.Size.Height) / 2 + 25);

            DisadvantageStats[Stat.PhysicalResistance] = new DXLabel
            {
                Parent = StatsTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y +25),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };
            #endregion



            label = new DXLabel
            {
                Parent = HermitTab,
                Text = "AC:"
            };
            label.Location = new Point(HermitTab.Size.Width / 4 - label.Size.Width + 25, 15);

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
                Text = "MR:"
            };
            label.Location = new Point(HermitTab.Size.Width / 4 * 2 - label.Size.Width + 25, 15);

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
                Text = "DC:"
            };
            label.Location = new Point(HermitTab.Size.Width / 4 - label.Size.Width + 25, 35);

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
                Text = "MC:"
            };
            label.Location = new Point(HermitTab.Size.Width / 4 * 2 - label.Size.Width + 25, 35);

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
                Text = "SC:"
            };
            label.Location = new Point(HermitTab.Size.Width / 4 * 3 - label.Size.Width + 25, 35);

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
                Text = "Health:"
            };
            label.Location = new Point(HermitTab.Size.Width / 4 - label.Size.Width + 25, 55);

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
                Text = "Mana:"
            };
            label.Location = new Point(HermitTab.Size.Width / 4 * 2 - label.Size.Width + 25, 55);

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
                Text = "E. Att:"
            };
            label.Location = new Point(50 - label.Size.Width, 90);

            icon = new DXImageControl
            {
                Parent = HermitTab,
                LibraryFile = LibraryFile.ProgUse,
                Index = 600,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = "Fire",
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
                Hint = "Ice",
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
                Hint = "Lightning",
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
                Hint = "Wind",
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
                Hint = "Holy",
            };
            icon.Location = new Point(label.Location.X + label.Size.Width, label.Location.Y + (label.Size.Height - icon.Size.Height) / 2 + 25);

            HermitAttackStats[Stat.HolyAttack] = new DXLabel
            {
                Parent = HermitTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y + 25),
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
                Hint = "Dark",
            };
            icon.Location = new Point(label.Location.X + label.Size.Width + 50, label.Location.Y + (label.Size.Height - icon.Size.Height) / 2 + 25);

            HermitAttackStats[Stat.DarkAttack] = new DXLabel
            {
                Parent = HermitTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y + 25),
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
                Hint = "Phantom",
            };
            icon.Location = new Point(label.Location.X + label.Size.Width + 100, label.Location.Y + (label.Size.Height - icon.Size.Height) / 2 + 25);

            HermitAttackStats[Stat.PhantomAttack] = new DXLabel
            {
                Parent = HermitTab,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y + 25),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            #endregion

            label = new DXLabel
            {
                Parent = HermitTab,
                Text = "Unspent Points:"
            };
            label.Location = new Point(HermitTab.Size.Width / 4 * 2 - label.Size.Width + 25, 150);

            RemainingLabel = new DXLabel
            {
                Parent = HermitTab,
                Location = new Point(label.Location.X + label.Size.Width - 5, label.Location.Y),
                ForeColour = Color.White,
                Text = "0"
            };

        }

        #region Methods

        public void Draw(DXItemCell cell, int index)
        {
            if (InterfaceLibrary == null) return;

            Size s = InterfaceLibrary.GetSize(index);
            int x = (cell.Size.Width - s.Width) / 2 + cell.DisplayArea.X;
            int y = (cell.Size.Height - s.Height) / 2 + cell.DisplayArea.Y;

            InterfaceLibrary.Draw(index, x, y, Color.White, false, 0.2F, ImageType.Image);
        }
        private void CharacterTab_BeforeChildrenDraw(object sender, EventArgs e)
        {

            MirLibrary library;

            if (!CEnvir.LibraryList.TryGetValue(LibraryFile.ProgUse, out library)) return;

            int x = 130;
            int y = 270;

            if (Grid[(int)EquipmentSlot.Armour].Item != null)
            {
                int index = Grid[(int)EquipmentSlot.Armour].Item.Info.Image;

                MirLibrary effectLibrary;

                if (CEnvir.LibraryList.TryGetValue(LibraryFile.EquipEffect_UI, out effectLibrary))
                {
                    MirImage image = null;
                    switch (index)
                    {
                        //All
                        case 962:
                            image = effectLibrary.CreateImage(1700 + (GameScene.Game.MapControl.Animation % 10), ImageType.Image);
                            break;
                        case 972:
                            image = effectLibrary.CreateImage(1720 + (GameScene.Game.MapControl.Animation % 10), ImageType.Image);
                            break;

                        //War
                        case 963:
                            image = effectLibrary.CreateImage(400 + (GameScene.Game.MapControl.Animation % 15), ImageType.Image);
                            break;
                        case 973:
                            image = effectLibrary.CreateImage(420 + (GameScene.Game.MapControl.Animation % 15), ImageType.Image);
                            break;

                        //Wiz
                        case 964:
                            image = effectLibrary.CreateImage(300 + (GameScene.Game.MapControl.Animation % 15), ImageType.Image);
                            break;
                        case 974:
                            image = effectLibrary.CreateImage(320 + (GameScene.Game.MapControl.Animation % 15), ImageType.Image);
                            break;

                        //Tao
                        case 965:
                            image = effectLibrary.CreateImage(200 + (GameScene.Game.MapControl.Animation % 15), ImageType.Image);
                            break;
                        case 975:
                            image = effectLibrary.CreateImage(220 + (GameScene.Game.MapControl.Animation % 15), ImageType.Image);
                            break;
                        //Ass
                        case 2007:
                            image = effectLibrary.CreateImage(500 + (GameScene.Game.MapControl.Animation % 13), ImageType.Image);
                            break;
                        case 2017:
                            image = effectLibrary.CreateImage(520 + (GameScene.Game.MapControl.Animation % 13), ImageType.Image);
                            break;


                        case 942:
                            image = effectLibrary.CreateImage(700, ImageType.Image);
                            break;
                        case 961:
                            image = effectLibrary.CreateImage(1600, ImageType.Image);
                            break;
                        case 982:
                            image = effectLibrary.CreateImage(800, ImageType.Image);
                            break;
                        case 983:
                            image = effectLibrary.CreateImage(1200, ImageType.Image);
                            break;
                        case 984:
                            image = effectLibrary.CreateImage(1100, ImageType.Image);
                            break;
                        case 1022:
                            image = effectLibrary.CreateImage(900, ImageType.Image);
                            break;
                        case 1023:
                            image = effectLibrary.CreateImage(1300, ImageType.Image);
                            break;
                        case 1002:
                            image = effectLibrary.CreateImage(1000, ImageType.Image);
                            break;
                        case 1003:
                            image = effectLibrary.CreateImage(1400, ImageType.Image);
                            break;

                        case 952:
                            image = effectLibrary.CreateImage(720, ImageType.Image);
                            break;
                        case 971:
                            image = effectLibrary.CreateImage(1620, ImageType.Image);
                            break;
                        case 992:
                            image = effectLibrary.CreateImage(820, ImageType.Image);
                            break;
                        case 993:
                            image = effectLibrary.CreateImage(1220, ImageType.Image);
                            break;
                        case 994:
                            image = effectLibrary.CreateImage(1120, ImageType.Image);
                            break;
                        case 1032:
                            image = effectLibrary.CreateImage(920, ImageType.Image);
                            break;
                        case 1033:
                            image = effectLibrary.CreateImage(1320, ImageType.Image);
                            break;
                        case 1012:
                            image = effectLibrary.CreateImage(1020, ImageType.Image);
                            break;
                        case 1013:
                            image = effectLibrary.CreateImage(1420, ImageType.Image);
                            break;
                    }
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


            if (CEnvir.LibraryList.TryGetValue(LibraryFile.Equip, out library))
            {
                if (Grid[(int)EquipmentSlot.Armour].Item != null)
                {
                    int index = Grid[(int)EquipmentSlot.Armour].Item.Info.Image;

                    library.Draw(index, DisplayArea.X + x, DisplayArea.Y + y, Color.White, true, 1F, ImageType.Image);
                    library.Draw(index, DisplayArea.X + x, DisplayArea.Y + y, Grid[(int)EquipmentSlot.Armour].Item.Colour, true, 1F, ImageType.Overlay);
                }

                if (Grid[(int)EquipmentSlot.Weapon].Item != null)
                {
                    int index = Grid[(int)EquipmentSlot.Weapon].Item.Info.Image;

                    library.Draw(index, DisplayArea.X + x, DisplayArea.Y + y, Color.White, true, 1F, ImageType.Image);
                    library.Draw(index, DisplayArea.X + x, DisplayArea.Y + y, Grid[(int)EquipmentSlot.Weapon].Item.Colour, true, 1F, ImageType.Overlay);
                }

                if (Grid[(int)EquipmentSlot.Shield].Item != null)
                {
                    int index = Grid[(int)EquipmentSlot.Shield].Item.Info.Image;

                    library.Draw(index, DisplayArea.X + x, DisplayArea.Y + y, Color.White, true, 1F, ImageType.Image);
                    library.Draw(index, DisplayArea.X + x, DisplayArea.Y + y, Grid[(int)EquipmentSlot.Shield].Item.Colour, true, 1F, ImageType.Overlay);
                }
            }


            if (Grid[(int)EquipmentSlot.Helmet].Item != null && library != null)
            {
                int index = Grid[(int)EquipmentSlot.Helmet].Item.Info.Image;

                library.Draw(index, DisplayArea.X + x, DisplayArea.Y + y, Color.White, true, 1F, ImageType.Image);
                library.Draw(index, DisplayArea.X + x, DisplayArea.Y + y, Grid[(int)EquipmentSlot.Helmet].Item.Colour, true, 1F, ImageType.Overlay);
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
        public void UpdateStats()
        {
            foreach (KeyValuePair<Stat, DXLabel> pair in DisplayStats)
                pair.Value.Text = Stats.GetFormat(pair.Key);


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

            RemainingLabel.Text = HermitPoints.ToString();

        }

        public void NewInformation(S.Inspect p)
        {
            Visible = true;
            CharacterNameLabel.Text = p.Name;
            GuildNameLabel.Text = p.GuildName;
            GuildRankLabel.Text = p.GuildRank;

            Stats.Clear();

            Stats.Add(p.Stats);

            HermitStats.Clear();
            HermitStats.Add(p.HermitStats);
            HermitPoints = p.HermitPoints;
            
            Gender = p.Gender;
            Class = p.Class;
            Level = p.Level;

            MarriageIcon.Visible = !string.IsNullOrEmpty(p.Partner);
            MarriageIcon.Hint = p.Partner;

            HairColour = p.HairColour;
            HairType = p.Hair;

            foreach (DXItemCell cell in Grid)
                cell.Item = null;

            foreach (ClientUserItem item in p.Items)
                Grid[item.Slot].Item = item;

            WearWeightLabel.Text = $"{p.WearWeight}/{p.Stats[Stat.WearWeight]}";
            HandWeightLabel.Text = $"{p.HandWeight}/{p.Stats[Stat.HandWeight]}";

            WearWeightLabel.ForeColour = p.WearWeight > Stats[Stat.WearWeight] ? Color.Red : Color.White;
            HandWeightLabel.ForeColour = p.HandWeight > Stats[Stat.HandWeight] ? Color.Red : Color.White;


            UpdateStats();
        }
        #endregion


        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                Stats = null;
                HermitStats = null;
                HermitPoints = 0;

                if (Equipment != null)
                {
                    for (int i = 0; i < Equipment.Length; i++)
                    {
                        Equipment[i] = null;
                    }

                    Equipment = null;
                }

                Class = 0;
                Gender = 0;
                HairType = 0;
                HairColour = Color.Empty;
                Level = 0;



                if (TabControl != null)
                {
                    if (!TabControl.IsDisposed)
                        TabControl.Dispose();

                    TabControl = null;
                }

                if (CharacterTab != null)
                {
                    if (!CharacterTab.IsDisposed)
                        CharacterTab.Dispose();

                    CharacterTab = null;
                }

                if (StatsTab != null)
                {
                    if (!StatsTab.IsDisposed)
                        StatsTab.Dispose();

                    StatsTab = null;
                }

                if (HermitTab != null)
                {
                    if (!HermitTab.IsDisposed)
                        HermitTab.Dispose();

                    HermitTab = null;
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

                if (RemainingLabel != null)
                {
                    if (!RemainingLabel.IsDisposed)
                        RemainingLabel.Dispose();

                    RemainingLabel = null;
                }

            }

        }

        #endregion

    }
}
