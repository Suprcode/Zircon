using Client.Controls;
using Client.Envir;
using Client.Scenes.Views.Character;
using Client.UserModels;
using Library;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using C = Library.Network.ClientPackets;
using S = Library.Network.ServerPackets;

//Add that config refresh time

namespace Client.Scenes.Views
{
    public sealed class RankingDialog : DXImageControl
    {
        #region Properties

        #region StartIndex

        public int StartIndex
        {
            get => _StartIndex;
            set
            {
                if (_StartIndex == value) return;

                int oldValue = _StartIndex;
                _StartIndex = value;

                OnStartIndexChanged(oldValue, value);
            }
        }
        private int _StartIndex;
        public event EventHandler<EventArgs> StartIndexChanged;
        public void OnStartIndexChanged(int oValue, int nValue)
        {
            UpdateTime = CEnvir.Now.AddMilliseconds(250);
            
            if (nValue > oValue)
                for (int i = 0; i < Lines.Length; i++)
                {
                    if (nValue - oValue + i < Lines.Length)
                    {
                        if (Lines[i + nValue - oValue].Rank != null)
                        {
                            Lines[i].Rank = Lines[i + nValue - oValue].Rank;
                        }
                        else
                            Lines[i].Loading = true;
                    }
                    else
                        Lines[i].Loading = true;

                }
            else
                for (int i = Lines.Length - 1; i >= 0; i--)
                {
                    if (nValue - oValue + i >= 0)
                    {
                        if (Lines[i + nValue - oValue].Rank != null)
                        Lines[i].Rank = Lines[i + nValue - oValue].Rank;
                        else
                            Lines[i].Loading = true;
                    }
                    else
                        Lines[i].Loading = true;
                }



            StartIndexChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region FilterClass

        public RequiredClass FilterClass
        {
            get => _FilterClass;
            set
            {
                if (_FilterClass == value) return;

                RequiredClass oldValue = _FilterClass;
                _FilterClass = value;

                OnFilterClassChanged(oldValue, value);
            }
        }
        private RequiredClass _FilterClass;
        public event EventHandler<EventArgs> FilterClassChanged;
        public void OnFilterClassChanged(RequiredClass oValue, RequiredClass nValue)
        {
            ScrollBar.Value = 0;
            UpdateTime = CEnvir.Now;

            foreach (RankingLine line in Lines)
                line.Loading = true;

            FilterClassChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region OnlineOnly

        public bool OnlineOnly
        {
            get => _OnlineOnly;
            set
            {
                if (_OnlineOnly == value) return;

                bool oldValue = _OnlineOnly;
                _OnlineOnly = value;

                OnOnlineOnlyChanged(oldValue, value);
            }
        }
        private bool _OnlineOnly;
        public event EventHandler<EventArgs> OnlineOnlyChanged;
        public void OnOnlineOnlyChanged(bool oValue, bool nValue)
        {
            ScrollBar.Value = 0;
            UpdateTime = CEnvir.Now;

            foreach (RankingLine line in Lines)
                line.Loading = true;


            OnlineOnlyChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion


        #region AllowObservation

        public bool AllowObservation
        {
            get => _AllowObservation;
            set
            {
                if (_AllowObservation == value) return;

                bool oldValue = _AllowObservation;
                _AllowObservation = value;

                OnAllowObservationChanged(oldValue, value);
            }
        }
        private bool _AllowObservation;
        public event EventHandler<EventArgs> AllowObservationChanged;
        public void OnAllowObservationChanged(bool oValue, bool nValue)
        {
            ObservableBox.Visible = nValue;
            ObserveButton.Visible = nValue;

            AllowObservationChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Observable

        public bool Observable
        {
            get => _Observable;
            set
            {
                if (_Observable == value) return;

                bool oldValue = _Observable;
                _Observable = value;

                OnObserverableChanged(oldValue, value);
            }
        }
        private bool _Observable;
        public event EventHandler<EventArgs> ObserverableChanged;
        public void OnObserverableChanged(bool oValue, bool nValue)
        {
            ObservableBox.Checked = nValue;

            ObserverableChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region SelectedRow

        public RankingLine SelectedRow
        {
            get => _SelectedRow;
            set
            {
                if (_SelectedRow == value) return;

                RankingLine oldValue = _SelectedRow;
                _SelectedRow = value;

                OnSelectedRowChanged(oldValue, value);
            }
        }
        private RankingLine _SelectedRow;
        public event EventHandler<EventArgs> SelectedRowChanged;
        public void OnSelectedRowChanged(RankingLine oValue, RankingLine nValue)
        {
            if (oValue != null)
                oValue.Selected = false;

            if (nValue != null && nValue.Rank != null)
                nValue.Selected = true;

            ObserveButton.Enabled = SelectedRow != null && SelectedRow.Rank.Online && SelectedRow.Rank.Observable;

            if (GameScene.Game != null && SelectedRow == null)
                ClearInformation();

            SelectedRowChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public DateTime UpdateTime;

        private DXTabControl TabControl;
        private DXTab GlobalTab;

        private DXControl InspectPanel, RankPanel, RankPanelList;
        private DXVScrollBar ScrollBar;

        public DXComboBox RequiredClassBox;
        public DXCheckBox OnlineOnlyBox, ObservableBox;
        public DXButton CloseButton, SearchButton, ObserveButton;

        public DXLabel TitleLabel, LastUpdate;

        public DXTextBox SearchText;

        public RankingLine SearchLine;
        public RankingLine[] Lines;

        #region Inspect

        public DXLabel InspectLabel, CharacterNameLabel, GuildNameLabel, GuildRankLabel;
        public DXItemCell[] Grid;

        private ClientUserItem[] _inspectEquipment = new ClientUserItem[Globals.EquipmentSize];
        public MirClass _inspectClass;
        public MirGender _inspectGender;
        public int _inspectHairType;
        public Color _inspectHairColour;
        public int _inspectLevel;

        public ClientUserItem[] Equipment => _inspectEquipment;
        public MirClass Class => _inspectClass;
        public MirGender Gender => _inspectGender;
        public int HairType => _inspectHairType;
        public Color HairColour => _inspectHairColour;

        #endregion

        public WindowSetting Settings;

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

        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
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

        public WindowType Type => WindowType.RankingBox;

        #endregion

        public RankingDialog(bool fullRanking = false)
        {   
            Index = fullRanking ? 211 : 210;
            LibraryFile = LibraryFile.Interface;
            Size = new Size(fullRanking ? 576 : 330, 456);
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

            TitleLabel = new DXLabel
            {
                Text = CEnvir.Language.RankingDialogTitle,
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
            };
            TitleLabel.Location = new Point((DisplayArea.Width - TitleLabel.Size.Width) / 2, 8);

            RankPanel = new DXControl
            {
                Parent = this,
                Size = new Size(330, 456),
                PassThrough = true
            };
            RankPanel.Location = new Point(DisplayArea.Width - RankPanel.Size.Width, 0);

            #region Inspect

            if (fullRanking)
            {
                TabControl = new DXTabControl
                {
                    Parent = this,
                    Location = new Point(12, 39),
                    Size = new Size(DisplayArea.Width, DisplayArea.Height)
                };

                GlobalTab = new DXTab
                {
                    Parent = TabControl,
                    TabButton = { Label = { Text = CEnvir.Language.RankingDialogGlobalTabLabel } },
                    BackColour = Color.Empty,
                    Location = new Point(0, 25)
                };

                InspectPanel = new DXControl
                {
                    Parent = this,
                    Size = new Size(252, 456),
                    Location = new Point(0, 0),
                    PassThrough = true
                };
                InspectPanel.BeforeChildrenDraw += InspectPanel_BeforeChildrenDraw;

                DXControl namePanel = new DXControl
                {
                    Parent = InspectPanel,
                    Size = new Size(130, 46),
                    Location = new Point(64, 71)
                };
                CharacterNameLabel = new DXLabel
                {
                    AutoSize = false,
                    Size = new Size(130, 20),
                    ForeColour = Color.FromArgb(222, 255, 222),
                    Outline = false,
                    Parent = namePanel,
                    Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                    DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter
                };
                GuildNameLabel = new DXLabel
                {
                    AutoSize = false,
                    Size = new Size(130, 15),
                    ForeColour = Color.FromArgb(255, 255, 181),
                    Outline = false,
                    Parent = namePanel,
                    Location = new Point(0, CharacterNameLabel.Size.Height - 2),
                    DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter
                };
                GuildRankLabel = new DXLabel
                {
                    AutoSize = false,
                    Size = new Size(130, 15),
                    ForeColour = Color.FromArgb(255, 206, 148),
                    Outline = false,
                    Parent = namePanel,
                    Location = new Point(0, CharacterNameLabel.Size.Height + GuildNameLabel.Size.Height - 4),
                    DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter
                };

                #region Grid

                Grid = new DXItemCell[Globals.EquipmentSize];

                DXItemCell cell;
                Grid[(int)EquipmentSlot.Weapon] = cell = new DXItemCell
                {
                    Location = new Point(28, 142),
                    Parent = InspectPanel,
                    ItemGrid = Equipment,
                    Slot = (int)EquipmentSlot.Weapon,
                    GridType = GridType.Inspect,
                    Size = new System.Drawing.Size(65, 90),
                    Hidden = true
                };

                Grid[(int)EquipmentSlot.Armour] = cell = new DXItemCell
                {
                    Location = new Point(90, 143),
                    Parent = InspectPanel,
                    Border = false,
                    ItemGrid = Equipment,
                    Slot = (int)EquipmentSlot.Armour,
                    GridType = GridType.Inspect,
                    Size = new System.Drawing.Size(70, 150),
                    Hidden = true
                };

                Grid[(int)EquipmentSlot.Shield] = cell = new DXItemCell
                {
                    Location = new Point(140, 230),
                    Parent = InspectPanel,
                    FixedBorder = true,
                    Border = true,
                    ItemGrid = Equipment,
                    Slot = (int)EquipmentSlot.Shield,
                    GridType = GridType.Inspect,
                    Hidden = true
                };

                Grid[(int)EquipmentSlot.Helmet] = cell = new DXItemCell
                {
                    Location = new Point(110, 150),
                    Parent = InspectPanel,
                    FixedBorder = true,
                    Border = true,
                    ItemGrid = Equipment,
                    Slot = (int)EquipmentSlot.Helmet,
                    GridType = GridType.Inspect,
                    Size = new Size(35, 35),
                    Hidden = true
                };

                Grid[(int)EquipmentSlot.Emblem] = cell = new DXItemCell
                {
                    Location = new Point(159, 360),
                    Parent = InspectPanel,
                    Border = true,
                    ItemGrid = Equipment,
                    Slot = (int)EquipmentSlot.Emblem,
                    GridType = GridType.Inspect,
                };
                cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 104);

                Grid[(int)EquipmentSlot.HorseArmour] = cell = new DXItemCell
                {
                    Location = new Point(276, 118),
                    Parent = InspectPanel,
                    Border = true,
                    ItemGrid = Equipment,
                    Slot = (int)EquipmentSlot.HorseArmour,
                    GridType = GridType.Inspect,
                    Hidden = true
                };

                Grid[(int)EquipmentSlot.Torch] = cell = new DXItemCell
                {
                    Location = new Point(120, 360),
                    Parent = InspectPanel,
                    Border = true,
                    ItemGrid = Equipment,
                    Slot = (int)EquipmentSlot.Torch,
                    GridType = GridType.Inspect,
                };
                cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 38);

                Grid[(int)EquipmentSlot.Necklace] = cell = new DXItemCell
                {
                    Location = new Point(198, 204),
                    Parent = InspectPanel,
                    Border = true,
                    ItemGrid = Equipment,
                    Slot = (int)EquipmentSlot.Necklace,
                    GridType = GridType.Inspect,
                };
                cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 33);

                Grid[(int)EquipmentSlot.BraceletL] = cell = new DXItemCell
                {
                    Location = new Point(24, 243),
                    Parent = InspectPanel,
                    Border = true,
                    ItemGrid = Equipment,
                    Slot = (int)EquipmentSlot.BraceletL,
                    GridType = GridType.Inspect,
                };
                cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 32);

                Grid[(int)EquipmentSlot.BraceletR] = cell = new DXItemCell
                {
                    Location = new Point(198, 243),
                    Parent = InspectPanel,
                    Border = true,
                    ItemGrid = Equipment,
                    Slot = (int)EquipmentSlot.BraceletR,
                    GridType = GridType.Inspect,
                };
                cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 32);

                Grid[(int)EquipmentSlot.RingL] = cell = new DXItemCell
                {
                    Location = new Point(24, 282),
                    Parent = InspectPanel,
                    Border = true,
                    ItemGrid = Equipment,
                    Slot = (int)EquipmentSlot.RingL,
                    GridType = GridType.Inspect,
                };
                cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 31);

                Grid[(int)EquipmentSlot.RingR] = cell = new DXItemCell
                {
                    Location = new Point(198, 282),
                    Parent = InspectPanel,
                    Border = true,
                    ItemGrid = Equipment,
                    Slot = (int)EquipmentSlot.RingR,
                    GridType = GridType.Inspect,
                };
                cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 31);

                Grid[(int)EquipmentSlot.Flower] = cell = new DXItemCell
                {
                    Parent = InspectPanel,
                    Border = true,
                    ItemGrid = Equipment,
                    Slot = (int)EquipmentSlot.Flower,
                    GridType = GridType.Inspect,
                    Hidden = true
                };

                Grid[(int)EquipmentSlot.Poison] = cell = new DXItemCell
                {
                    Parent = InspectPanel,
                    Border = true,
                    ItemGrid = Equipment,
                    Slot = (int)EquipmentSlot.Poison,
                    GridType = GridType.Inspect,
                    Hidden = true
                };

                Grid[(int)EquipmentSlot.Amulet] = cell = new DXItemCell
                {
                    Location = new Point(198, 321),
                    Parent = InspectPanel,
                    Border = true,
                    ItemGrid = Equipment,
                    Slot = (int)EquipmentSlot.Amulet,
                    GridType = GridType.Inspect,
                    Size = new Size(36, 75)
                };
                cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 39);

                Grid[(int)EquipmentSlot.Shoes] = cell = new DXItemCell
                {
                    Location = new Point(24, 321),
                    Parent = InspectPanel,
                    Border = true,
                    ItemGrid = Equipment,
                    Slot = (int)EquipmentSlot.Shoes,
                    GridType = GridType.Inspect,
                    Size = new Size(36, 75)
                };
                cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 36);

                Grid[(int)EquipmentSlot.Costume] = cell = new DXItemCell
                {
                    Location = new Point(24, 204),
                    Parent = InspectPanel,
                    Border = true,
                    ItemGrid = Equipment,
                    Slot = (int)EquipmentSlot.Costume,
                    GridType = GridType.Inspect,
                };
                cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 34);

                Grid[(int)EquipmentSlot.Hook] = cell = new DXItemCell
                {
                    Location = new Point(0, 0),
                    Parent = InspectPanel,
                    FixedBorder = true,
                    Border = true,
                    ItemGrid = Equipment,
                    Slot = (int)EquipmentSlot.Hook,
                    GridType = GridType.Inspect,
                    Hidden = true
                };

                Grid[(int)EquipmentSlot.Float] = cell = new DXItemCell
                {
                    Location = new Point(0, 0),
                    Parent = InspectPanel,
                    FixedBorder = true,
                    Border = true,
                    ItemGrid = Equipment,
                    Slot = (int)EquipmentSlot.Float,
                    GridType = GridType.Inspect,
                    Hidden = true
                };

                Grid[(int)EquipmentSlot.Bait] = cell = new DXItemCell
                {
                    Location = new Point(0, 0),
                    Parent = InspectPanel,
                    FixedBorder = true,
                    Border = true,
                    ItemGrid = Equipment,
                    Slot = (int)EquipmentSlot.Bait,
                    GridType = GridType.Inspect,
                    Hidden = true
                };

                Grid[(int)EquipmentSlot.Finder] = cell = new DXItemCell
                {
                    Location = new Point(0, 0),
                    Parent = InspectPanel,
                    FixedBorder = true,
                    Border = true,
                    ItemGrid = Equipment,
                    Slot = (int)EquipmentSlot.Finder,
                    GridType = GridType.Inspect,
                    Hidden = true
                };

                Grid[(int)EquipmentSlot.Reel] = cell = new DXItemCell
                {
                    Location = new Point(0, 0),
                    Parent = InspectPanel,
                    FixedBorder = true,
                    Border = true,
                    ItemGrid = Equipment,
                    Slot = (int)EquipmentSlot.Reel,
                    GridType = GridType.Inspect,
                    Hidden = true
                };

                #endregion

                InspectLabel = new DXLabel
                {
                    Parent = InspectPanel,
                    AutoSize = false,
                    ForeColour = Color.White,
                    DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                    Size = new Size(148, 16),
                    Location = new Point(77, 419),
                };
            }


            #endregion

            #region Ranking

            SearchText = new DXTextBox
            {
                Border = false,
                Parent = RankPanel,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Regular),
                Location = new Point(13, 68),
                Size = new Size(147, 16),
                MaxLength = Globals.MaxCharacterNameLength
            };
            SearchText.TextBox.KeyPress += (o, e) =>
            {
                switch (e.KeyChar)
                {
                    case (char)Keys.Enter:
                        if (SearchButton != null && !SearchButton.IsDisposed)
                            SearchButton.InvokeMouseClick();
                        break;
                    default: return;
                }
                e.Handled = true;
            };
            SearchText.TextBox.TextChanged += (o, e) =>
            {
                SearchButton.Enabled = !string.IsNullOrEmpty(SearchText.TextBox.Text);
            };

            SearchButton = new DXButton
            {
                ButtonType = ButtonType.SmallButton,
                Size = new Size(60, SmallButtonHeight),
                Parent = RankPanel,
                Label = { Text = CEnvir.Language.RankingDialogSearchButtonLabel },
                Visible = true,
                Location = new Point(164, 66),
                Enabled = false
            };
            SearchButton.MouseClick += (o, e) =>
            {
                CEnvir.Enqueue(new C.RankSearch { Name = SearchText.TextBox.Text });
            };

            ObserveButton = new DXButton
            {
                ButtonType = ButtonType.SmallButton,
                Size = new Size(60, SmallButtonHeight),
                Parent = RankPanel,
                Label = { Text = CEnvir.Language.RankingDialogObserveButtonLabel },
                Visible = false,
                Enabled = false,
                Location = new Point(SearchButton.Location.X + SearchButton.Size.Width + 5, 66)
            };
            ObserveButton.MouseClick += (o, e) =>
            {
                if (SelectedRow == null)
                    return;

                if (GameScene.Game != null && CEnvir.Now < GameScene.Game.User.CombatTime.AddSeconds(10) && !GameScene.Game.Observer)
                {
                    GameScene.Game.ReceiveChat(CEnvir.Language.SpectatorModeWarningInCombat, MessageType.System);
                    return;
                }

                CEnvir.Enqueue(new C.ObserverRequest { Name = SelectedRow.Rank.Name });
            };

            RankPanelList = new DXControl
            {
                Location = new Point(0, 122),
                Size = new Size(330, 286),
                Parent = RankPanel,
                Border = true,
                BackColour = Color.Lime
            };

            Lines = new RankingLine[11];
            ScrollBar = new DXVScrollBar
            {
                Parent = RankPanelList,
                Size = new Size(20, RankPanelList.Size.Height),
                Location = new Point(RankPanelList.Size.Width - 26, 0),
                VisibleSize = Lines.Length,
                Change = 5,
                BackColour = Color.Empty,
                Border = false,
                UpButton = { Index = 61, LibraryFile = LibraryFile.Interface },
                DownButton = { Index = 62, LibraryFile = LibraryFile.Interface },
                PositionBar = { Index = 60, LibraryFile = LibraryFile.Interface }
            };
            ScrollBar.ValueChanged += (o, e) =>
            {
                StartIndex = ScrollBar.Value;
                SelectedRow = null;
            };
            MouseWheel += ScrollBar.DoMouseWheel;

            SearchLine = new RankingLine
            {
                Parent = RankPanel,
                Location = new Point(12, 90),
                Visible = true,
            };
            SearchLine.MouseClick += (o, e) => SelectedRow = (RankingLine)o;

            for (int i = 0; i < Lines.Length; i++)
            {
                Lines[i] = new RankingLine
                {
                    Parent = RankPanelList,
                    Location = new Point(12, 16 + (23 * i)),
                    Visible = false,
                };
                Lines[i].MouseWheel += ScrollBar.DoMouseWheel;
                Lines[i].MouseClick += (o, e) => SelectedRow = (RankingLine)o;
            }

            RequiredClassBox = new DXComboBox
            {
                Parent = RankPanel,
                Size = new Size(122, DXComboBox.DefaultNormalHeight),
                Location = new Point(12, 39),
                Border = false
            };
            RequiredClassBox.SelectedItemChanged += (o, e) =>
            {
                FilterClass = (RequiredClass?) RequiredClassBox.SelectedItem ?? RequiredClass.All;
                Config.RankingClass = (int)FilterClass;
                SelectedRow = null;
            };

            new DXListBoxItem
            {
                Parent = RequiredClassBox.ListBox,
                Label = { Text = $"{RequiredClass.All}" },
                Item = RequiredClass.All
            };

            new DXListBoxItem
            {
                Parent = RequiredClassBox.ListBox,
                Label = { Text = $"{RequiredClass.Warrior}" },
                Item = RequiredClass.Warrior
            };
            new DXListBoxItem
            {
                Parent = RequiredClassBox.ListBox,
                Label = { Text = $"{RequiredClass.Wizard}" },
                Item = RequiredClass.Wizard
            };
            new DXListBoxItem
            {
                Parent = RequiredClassBox.ListBox,
                Label = { Text = $"{RequiredClass.Taoist}" },
                Item = RequiredClass.Taoist
            };

            new DXListBoxItem
            {
                Parent = RequiredClassBox.ListBox,
                Label = { Text = $"{RequiredClass.Assassin}" },
                Item = RequiredClass.Assassin
            };

            RequiredClassBox.ListBox.SelectItem((RequiredClass)Config.RankingClass);

            OnlineOnlyBox = new DXCheckBox
            {
                Parent = RankPanel,
                Label = { Text = CEnvir.Language.RankingDialogOnlineOnlyLabel }
            };
            OnlineOnlyBox.CheckedChanged += (o, e) =>
            {
                OnlineOnly = OnlineOnlyBox.Checked;
                Config.RankingOnline = OnlineOnly;
                SelectedRow = null;
            };
            OnlineOnlyBox.Location = new Point(RequiredClassBox.Location.X + RequiredClassBox.Size.Width + 5, 38);
            OnlineOnlyBox.Checked = Config.RankingOnline;

            ObservableBox = new DXCheckBox
            {
                Parent = RankPanel,
                Visible = false,
                Label = { Text = CEnvir.Language.RankingDialogObservableLabel }
            };
            ObservableBox.CheckedChanged += (o, e) =>
            {
                if (ObservableBox.Checked == Observable) return;

                if (GameScene.Game == null) return;
                if (GameScene.Game.Observer) return;
                if (!GameScene.Game.User.InSafeZone)
                {
                    GameScene.Game.ReceiveChat(CEnvir.Language.SpectatorModeWarningInSafezone, MessageType.System);
                    return;
                }

                CEnvir.Enqueue(new C.ObservableSwitch { Allow = !Observable });
            };
            ObservableBox.Location = new Point(OnlineOnlyBox.Location.X + OnlineOnlyBox.Size.Width + 5, 38);
            
            LastUpdate = new DXLabel
            {
                Parent = this,
                AutoSize = false,
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Size = new Size(fullRanking ? 278 : 176, 16),
                Location = new Point(fullRanking ? 229 : 77, 419),
            };

            #endregion
        }

        private void InspectPanel_BeforeChildrenDraw(object sender, EventArgs e)
        {
            if (GameScene.Game == null || SelectedRow == null) return;

            int x = 100;
            int y = 290;

            if (!CEnvir.LibraryList.TryGetValue(LibraryFile.Equip, out MirLibrary library)) return;

            ClientUserItem weapon = Grid[(int)EquipmentSlot.Weapon]?.Item;
            ClientUserItem armour = Grid[(int)EquipmentSlot.Armour]?.Item;
            ClientUserItem helmet = Grid[(int)EquipmentSlot.Helmet]?.Item;
            ClientUserItem shield = Grid[(int)EquipmentSlot.Shield]?.Item;
            ClientUserItem costume = Grid[(int)EquipmentSlot.Costume]?.Item;

            if (armour != null && costume == null)
            {
                MirImage image = EquipEffectDecider.GetEffectImageOrNull(armour, Gender);
                if (image != null)
                {
                    bool oldBlend = DXManager.Blending;
                    float oldRate = DXManager.BlendRate;

                    DXManager.SetBlend(true, 0.8F);
                    PresentTexture(image.Image, InspectPanel, new Rectangle(InspectPanel.DisplayArea.X + x + image.OffSetX, InspectPanel.DisplayArea.Y + y + image.OffSetY, image.Width, image.Height), ForeColour, this);
                    DXManager.SetBlend(oldBlend, oldRate);
                }
            }

            if (!CEnvir.LibraryList.TryGetValue(LibraryFile.ProgUse, out library)) return;

            if (Class == MirClass.Assassin && Gender == MirGender.Female && HairType == 1 && helmet == null)
                library.Draw(1160, InspectPanel.DisplayArea.X + x, InspectPanel.DisplayArea.Y + y, HairColour, true, 1F, ImageType.Image);

            switch (Gender)
            {
                case MirGender.Male:
                    library.Draw(0, InspectPanel.DisplayArea.X + x, InspectPanel.DisplayArea.Y + y, Color.White, true, 1F, ImageType.Image);
                    break;
                case MirGender.Female:
                    library.Draw(1, InspectPanel.DisplayArea.X + x, InspectPanel.DisplayArea.Y + y, Color.White, true, 1F, ImageType.Image);
                    break;
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
                    library.Draw(armourIndex, InspectPanel.DisplayArea.X + x, InspectPanel.DisplayArea.Y + y, Color.White, true, 1F, ImageType.Image);
                    library.Draw(armourIndex, InspectPanel.DisplayArea.X + x, InspectPanel.DisplayArea.Y + y, armour.Colour, true, 1F, ImageType.Overlay);
                }

                if (weapon != null)
                {
                    int weaponIndex = weapon.Info.Image;
                    library.Draw(weaponIndex, InspectPanel.DisplayArea.X + x, InspectPanel.DisplayArea.Y + y, Color.White, true, 1F, ImageType.Image);
                    library.Draw(weaponIndex, InspectPanel.DisplayArea.X + x, InspectPanel.DisplayArea.Y + y, weapon.Colour, true, 1F, ImageType.Overlay);

                    MirImage image = EquipEffectDecider.GetEffectImageOrNull(weapon, Gender);
                    if (image != null)
                    {
                        bool oldBlend = DXManager.Blending;
                        float oldRate = DXManager.BlendRate;

                        DXManager.SetBlend(true, 0.8F);
                        PresentTexture(image.Image, InspectPanel, new Rectangle(DisplayArea.X + x + image.OffSetX, DisplayArea.Y + y + image.OffSetY, image.Width, image.Height), ForeColour, this);
                        DXManager.SetBlend(oldBlend, oldRate);
                    }
                }

                if (shield != null)
                {
                    int shieldIndex = shield.Info.Image;
                    library.Draw(shieldIndex, InspectPanel.DisplayArea.X + x, InspectPanel.DisplayArea.Y + y, Color.White, true, 1F, ImageType.Image);
                    library.Draw(shieldIndex, InspectPanel.DisplayArea.X + x, InspectPanel.DisplayArea.Y + y, shield.Colour, true, 1F, ImageType.Overlay);

                    MirImage image = EquipEffectDecider.GetEffectImageOrNull(shield, Gender);
                    if (image != null)
                    {
                        bool oldBlend = DXManager.Blending;
                        float oldRate = DXManager.BlendRate;

                        DXManager.SetBlend(true, 0.8F);
                        PresentTexture(image.Image, InspectPanel, new Rectangle(DisplayArea.X + x + image.OffSetX, DisplayArea.Y + y + image.OffSetY, image.Width, image.Height), ForeColour, this);
                        DXManager.SetBlend(oldBlend, oldRate);
                    }
                }
            }

            var hasFishingRobe = armour?.Info.ItemEffect == ItemEffect.FishingRobe;
            if (hasFishingRobe) return;

            if (helmet != null && library != null)
            {
                int index = helmet.Info.Image;

                library.Draw(index, InspectPanel.DisplayArea.X + x, InspectPanel.DisplayArea.Y + y, Color.White, true, 1F, ImageType.Image);
                library.Draw(index, InspectPanel.DisplayArea.X + x, InspectPanel.DisplayArea.Y + y, helmet.Colour, true, 1F, ImageType.Overlay);
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
                                library.Draw(60 + HairType - 1, InspectPanel.DisplayArea.X + x, InspectPanel.DisplayArea.Y + y, HairColour, true, 1F, ImageType.Image);
                                break;
                            case MirGender.Female:
                                library.Draw(80 + HairType - 1, InspectPanel.DisplayArea.X + x, InspectPanel.DisplayArea.Y + y, HairColour, true, 1F, ImageType.Image);
                                break;
                        }
                        break;
                    case MirClass.Assassin:
                        switch (Gender)
                        {
                            case MirGender.Male:
                                library.Draw(1100 + HairType - 1, InspectPanel.DisplayArea.X + x, InspectPanel.DisplayArea.Y + y, HairColour, true, 1F, ImageType.Image);
                                break;
                            case MirGender.Female:
                                library.Draw(1120 + HairType - 1, InspectPanel.DisplayArea.X + x, InspectPanel.DisplayArea.Y + y, HairColour, true, 1F, ImageType.Image);
                                break;
                        }
                        break;
                }
            }
        }

        #region Methods

        public override void Process()
        {
            base.Process();

            if (CEnvir.Now < UpdateTime) return;

            LastUpdate.Text = CEnvir.Now.ToString("F");

            UpdateTime = CEnvir.Now.AddSeconds(10);

            CEnvir.Enqueue(new C.RankRequest
            {
                Class = FilterClass,
                OnlineOnly = OnlineOnly,
                StartIndex = StartIndex,
            });
        }

        public void Update(S.RankSearch p)
        {
            SearchLine.Rank = p.Rank;
        }

        public void Update(S.Rankings p)
        {
            AllowObservation = p.AllowObservation;

            if (p.Class != FilterClass || p.OnlineOnly != OnlineOnly) return;

            ScrollBar.MaxValue = p.Total;

            for (int i = 0; i < Lines.Length; i++)
            {
                Lines[i].Loading = false;
                Lines[i].Rank = i >= p.Ranks.Count ? null : p.Ranks[i];
            }
        }

        public void NewInformation(S.Inspect p)
        {
            InspectLabel.Text = $"Lv. {p.Level} - Cl. {p.Class}";

            CharacterNameLabel.Text = p.Name;
            GuildNameLabel.Text = p.GuildName;
            GuildRankLabel.Text = p.GuildRank;

            _inspectGender = p.Gender;
            _inspectClass = p.Class;
            _inspectLevel = p.Level;

            _inspectHairColour = p.HairColour;
            _inspectHairType = p.Hair;

            foreach (DXItemCell cell in Grid)
                cell.Item = null;

            foreach (ClientUserItem item in p.Items)
                Grid[item.Slot].Item = item;
        }

        private void ClearInformation()
        {
            InspectLabel.Text = string.Empty;

            CharacterNameLabel.Text = string.Empty;
            GuildNameLabel.Text = string.Empty;
            GuildRankLabel.Text = string.Empty;

            _inspectGender = MirGender.Male;
            _inspectClass = MirClass.Warrior;
            _inspectLevel = 0;

            _inspectHairColour = Color.Empty;
            _inspectHairType = 0;

            foreach (DXItemCell cell in Grid)
                cell.Item = null;
        }

        public void Draw(DXItemCell cell, int index)
        {
            if (InterfaceLibrary == null) return;

            if (cell.Item != null) return;

            Size s = InterfaceLibrary.GetSize(index);
            int x = (cell.Size.Width - s.Width) / 2 + cell.DisplayArea.X;
            int y = (cell.Size.Height - s.Height) / 2 + cell.DisplayArea.Y;

            InterfaceLibrary.Draw(index, x, y, Color.White, false, 0.2F, ImageType.Image);
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _StartIndex = 0;
                StartIndexChanged = null;

                _FilterClass = 0;
                FilterClassChanged = null;

                _OnlineOnly = false;
                OnlineOnlyChanged = null;

                _Observable = false;
                ObserverableChanged = null;

                _SelectedRow = null;
                SelectedRowChanged = null;

                UpdateTime = DateTime.MinValue;

                if (TabControl != null)
                {
                    if (!TabControl.IsDisposed)
                        TabControl.Dispose();

                    TabControl = null;
                }

                if (GlobalTab != null)
                {
                    if (!GlobalTab.IsDisposed)
                        GlobalTab.Dispose();

                    GlobalTab = null;
                }

                if (InspectPanel != null)
                {
                    if (!InspectPanel.IsDisposed)
                        InspectPanel.Dispose();

                    InspectPanel = null;
                }

                if (RankPanel != null)
                {
                    if (!RankPanel.IsDisposed)
                        RankPanel.Dispose();

                    RankPanel = null;
                }

                if (RankPanelList != null)
                {
                    if (!RankPanelList.IsDisposed)
                        RankPanelList.Dispose();

                    RankPanelList = null;
                }

                if (ScrollBar != null)
                {
                    if (!ScrollBar.IsDisposed)
                        ScrollBar.Dispose();

                    ScrollBar = null;
                }

                if (RequiredClassBox != null)
                {
                    if (!RequiredClassBox.IsDisposed)
                        RequiredClassBox.Dispose();

                    RequiredClassBox = null;
                }

                if (OnlineOnlyBox != null)
                {
                    if (!OnlineOnlyBox.IsDisposed)
                        OnlineOnlyBox.Dispose();

                    OnlineOnlyBox = null;
                }

                if (ObservableBox != null)
                {
                    if (!ObservableBox.IsDisposed)
                        ObservableBox.Dispose();

                    ObservableBox = null;
                }

                if (CloseButton != null)
                {
                    if (!CloseButton.IsDisposed)
                        CloseButton.Dispose();

                    CloseButton = null;
                }

                if (SearchButton != null)
                {
                    if (!SearchButton.IsDisposed)
                        SearchButton.Dispose();

                    SearchButton = null;
                }

                if (ObserveButton != null)
                {
                    if (!ObserveButton.IsDisposed)
                        ObserveButton.Dispose();

                    ObserveButton = null;
                }

                if (TitleLabel != null)
                {
                    if (!TitleLabel.IsDisposed)
                        TitleLabel.Dispose();

                    TitleLabel = null;
                }

                if (LastUpdate != null)
                {
                    if (!LastUpdate.IsDisposed)
                        LastUpdate.Dispose();

                    LastUpdate = null;
                }

                if (SearchText != null)
                {
                    if (!SearchText.IsDisposed)
                        SearchText.Dispose();

                    SearchText = null;
                }

                if (SearchLine != null)
                {
                    if (!SearchLine.IsDisposed)
                        SearchLine.Dispose();

                    SearchLine = null;
                }

                if (Lines != null)
                {
                    for (int i = 0; i < Lines.Length; i++)
                    {
                        if (Lines[i] != null)
                        {
                            if (!Lines[i].IsDisposed)
                                Lines[i].Dispose();

                            Lines[i] = null;
                        }
                    }

                    Lines = null;
                }

                if (InspectLabel != null)
                {
                    if (!InspectLabel.IsDisposed)
                        InspectLabel.Dispose();

                    InspectLabel = null;
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

                _inspectGender = MirGender.Male;
                _inspectClass = MirClass.Warrior;
                _inspectLevel = 0;

                _inspectHairColour = Color.Empty;
                _inspectHairType = 0;

                _inspectEquipment = null;

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
            }

        }

        #endregion
    }

    public sealed class RankingLine : DXControl
    {
        #region Properties

        #region Rank

        public RankInfo Rank
        {
            get => _Rank;
            set
            {
                if (_Rank == value) return;

                RankInfo oldValue = _Rank;
                _Rank = value;

                OnRankChanged(oldValue, value);
            }
        }
        private RankInfo _Rank;
        public event EventHandler<EventArgs> RankChanged;
        public void OnRankChanged(RankInfo oValue, RankInfo nValue)
        {
            if (Rank == null)
            {
                RankLabel.Text = "";
                NameLabel.Text = "";
                LevelLabel.Text = "";
                ChangeLabel.Text = "";
                Visible = !Loading;
                OnlineImage.Visible = false;
            }
            else
            {
                Visible = true;
                RankLabel.ForeColour = Color.White;
                RankLabel.Text = Rank.Rank.ToString();
                NameLabel.Text = $"{Rank.Name}{(Rank.Rebirth > 0 ? $" [Rebirth: {Rank.Rebirth}]" : "")}";

                //decimal percent = 0;
                //percent = Math.Min(1, Math.Max(0, Rank.MaxExperience > 0 ? Rank.Experience / Rank.MaxExperience : 0));

                LevelLabel.Text = $"Lv. {Rank.Level}";

                var change = Rank.RankChange;

                if (change == 0)
                {
                    ChangeLabel.Text = " - ";
                    ChangeLabel.ForeColour = Color.White;
                }
                else {
                    ChangeLabel.Text = $"{(change > 0 ? "▲" : "▼")}{Math.Abs(Rank.RankChange)}";
                    ChangeLabel.ForeColour = change > 0 ? Color.OrangeRed : Color.DodgerBlue;
                }

                NameLabel.ForeColour = Color.White;

                LevelLabel.ForeColour = Color.White;

                OnlineImage.Visible = true;
                OnlineImage.Index = Rank.Online ? 3625 : 3624;
            }

            RankChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Loading

        public bool Loading
        {
            get => _Loading;
            set
            {
                if (_Loading == value) return;

                bool oldValue = _Loading;
                _Loading = value;

                OnLoadingChanged(oldValue, value);
            }
        }
        private bool _Loading;
        public event EventHandler<EventArgs> LoadingChanged;
        public void OnLoadingChanged(bool oValue, bool nValue)
        {
            if (!Loading)
            {
                RankLabel.Text = "";
                NameLabel.Text = "";
                LevelLabel.Text = "";

                Visible = false;
                return;
            }

            Rank = null;
            NameLabel.Text = "Updating...";
            NameLabel.ForeColour = Color.Orange;
            Visible = true;
            OnlineImage.Visible = false;

            LoadingChanged?.Invoke(this, EventArgs.Empty);
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
            BackColour = Selected ? Color.FromArgb(50, 255, 16, 16) : Color.Empty;

            SelectedChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public DXLabel RankLabel, LevelLabel, NameLabel, ChangeLabel;
        public DXImageControl OnlineImage;

        #endregion

        public RankingLine()
        {
            Size = new Size(288, 22);
            DrawTexture = true;
            BackColour = Color.Empty;

            OnlineImage = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter,
                Index = 3624,
                Location = new Point(2, 6),
                IsControl = false,
                Visible = false
            };

            RankLabel = new DXLabel
            {
                Parent = this,
                AutoSize = false,
                Location = new Point(10, 0),
                Size = new Size(31, 22),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                IsControl = false,
            };

            LevelLabel = new DXLabel
            {
                Parent = this,
                AutoSize = false,
                Location = new Point(RankLabel.Location.X + RankLabel.Size.Width - 1, 0),
                Size = new Size(43, 22),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                IsControl = false,
            };

            NameLabel = new DXLabel
            {
                Parent = this,
                AutoSize = false,
                Location = new Point(LevelLabel.Location.X + LevelLabel.Size.Width - 1, 0),
                Size = new Size(168, 22),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                IsControl = false,
            };

            ChangeLabel = new DXLabel
            {
                Parent = this,
                AutoSize = false,
                Location = new Point(NameLabel.Location.X + NameLabel.Size.Width - 1, 0),
                Size = new Size(40, 22),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                IsControl = false,
            };
        }

        #region Methods

        public override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (GameScene.Game == null || Rank == null) return;

            CEnvir.Enqueue(new C.Inspect { Index = Rank.Index, Ranking = true });
        }

        public override void OnMouseEnter()
        {
            base.OnMouseEnter();

            if (Rank != null)
                BackColour = Color.FromArgb(50, 255, 16, 16);
        }

        public override void OnMouseLeave()
        {
            base.OnMouseLeave();

            if (!Selected)
                BackColour = Color.Empty;
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _Rank = null;
                RankChanged = null;

                _Loading = false;
                LoadingChanged = null;

                _Selected = false;
                SelectedChanged = null;

                if (RankLabel != null)
                {
                    if (!RankLabel.IsDisposed)
                        RankLabel.Dispose();

                    RankLabel = null;
                }

                if (NameLabel != null)
                {
                    if (!NameLabel.IsDisposed)
                        NameLabel.Dispose();

                    NameLabel = null;
                }

                if (ChangeLabel != null)
                {
                    if (!ChangeLabel.IsDisposed)
                        ChangeLabel.Dispose();

                    ChangeLabel = null;
                }

                if (LevelLabel != null)
                {
                    if (!LevelLabel.IsDisposed)
                        LevelLabel.Dispose();

                    LevelLabel = null;
                }

                if (OnlineImage != null)
                {
                    if (!OnlineImage.IsDisposed)
                        OnlineImage.Dispose();

                    OnlineImage = null;
                }
            }
        }

        #endregion
    }

}
