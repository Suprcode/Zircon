using Client.Controls;
using Client.Envir;
using Client.Models;
using Client.UserModels;
using Library;
using Library.SystemModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using C = Library.Network.ClientPackets;

//Cleaned
namespace Client.Scenes.Views
{
    public sealed class GuildDialog : DXImageControl
    {
        #region Properties

        private DXTabControl GuildTabs;

        #region CreateTab

        private DXTab CreateTab;

        public DXTextBox GuildNameBox;
        public DXNumberTextBox MemberTextBox, StorageTextBox, TotalCostBox;

        public DXCheckBox GoldCheckBox, HornCheckBox;

        public DXControl CreatePanel;

        #region MemberLimit

        public int MemberLimit
        {
            get => _MemberLimit;
            set
            {
                if (_MemberLimit == value) return;

                int oldValue = _MemberLimit;
                _MemberLimit = value;

                OnMemberLimitChanged(oldValue, value);
            }
        }
        private int _MemberLimit;
        public event EventHandler<EventArgs> MemberLimitChanged;
        public void OnMemberLimitChanged(int oValue, int nValue)
        {
            MemberLimitChanged?.Invoke(this, EventArgs.Empty);

            TotalCostBox.Value = TotalCost;

            CreateButton.Enabled = CanCreate;
        }

        #endregion

        #region StorageSize

        public int StorageSize
        {
            get => _StorageSize;
            set
            {
                if (_StorageSize == value) return;

                int oldValue = _StorageSize;
                _StorageSize = value;

                OnStorageSizeChanged(oldValue, value);
            }
        }
        private int _StorageSize;
        public event EventHandler<EventArgs> StorageSizeChanged;
        public void OnStorageSizeChanged(int oValue, int nValue)
        {
            StorageSizeChanged?.Invoke(this, EventArgs.Empty);

            TotalCostBox.Value = TotalCost;

            CreateButton.Enabled = CanCreate;
        }

        #endregion

        #region GuildNameValid

        public bool GuildNameValid
        {
            get => _GuildNameValid;
            set
            {
                if (_GuildNameValid == value) return;

                bool oldValue = _GuildNameValid;
                _GuildNameValid = value;

                OnGuildNameValidChanged(oldValue, value);
            }
        }
        private bool _GuildNameValid;
        public event EventHandler<EventArgs> GuildNameValidChanged;
        public void OnGuildNameValidChanged(bool oValue, bool nValue)
        {
            GuildNameValidChanged?.Invoke(this, EventArgs.Empty);

            CreateButton.Enabled = CanCreate;
        }

        #endregion

        #region CreateAttempted

        public bool CreateAttempted
        {
            get => _CreateAttempted;
            set
            {
                if (_CreateAttempted == value) return;

                bool oldValue = _CreateAttempted;
                _CreateAttempted = value;

                OnCreateAttemptedChanged(oldValue, value);
            }
        }
        private bool _CreateAttempted;
        public event EventHandler<EventArgs> CreateAttemptedChanged;
        public void OnCreateAttemptedChanged(bool oValue, bool nValue)
        {
            CreateAttemptedChanged?.Invoke(this, EventArgs.Empty);

            CreateButton.Enabled = CanCreate;
        }

        #endregion

        public bool CanCreate => !CreateAttempted && GuildNameValid && GameScene.Game != null && TotalCost <= GameScene.Game.User.Gold.Amount;
        public int TotalCost => (int) Math.Min(int.MaxValue, (GoldCheckBox.Checked ? Globals.GuildCreationCost : 0) + (MemberLimit * Globals.GuildMemberCost) + (StorageSize * Globals.GuildStorageCost));

        public DXButton CreateButton, StarterGuildButton;

        #endregion

        #region HomeTab

        private DXTab HomeTab;
        public DXLabel MemberLimitLabel, GuildFundLabel, DailyGrowthLabel, TotalContributionLabel, DailyContributionLabel, GuildTaxLabel;

        public DXVScrollBar NoticeScrollBar;
        public DXTextBox NoticeTextBox;
        public DXButton EditNoticeButton, SaveNoticeButton, CancelNoticeButton;
        public DXButton SetTaxButton;
        public DXControl TreasuryPanel;

        #endregion

        #region Member Tab

        private DXTab MemberTab;
        public GuildMemberRow[] MemberRows;
        public DXVScrollBar MemberScrollBar;
        public DXButton AddMemberButton, EditDefaultMemberButton, IncreaseMemberButton;
        public DXControl AddMemberPanel;

        #endregion

        #region Storage Tab

        public DXTab StorageTab;
        public DXTextBox ItemNameTextBox;
        public DXComboBox ItemTypeComboBox;
        public DXItemGrid StorageGrid;
        public DXButton ClearButton;
        public DXVScrollBar StorageScrollBar;
        public ClientUserItem[] GuildStorage = new ClientUserItem[1000];
        public DXButton IncreaseStorageButton;
        public DXControl StoragePanel;
        #endregion

        #region War Tab

        private DXTab WarTab;
        public DXButton StartWarButton;
        public DXControl WarPanel;

        public Dictionary<CastleInfo, GuildCastlePanel> CastlePanels = new Dictionary<CastleInfo, GuildCastlePanel>();

        #endregion

        #region Style Tab

        private DXTab StyleTab;
        private DXControl StyleColourPanel, StyleFlagPanel;
        public DXLabel StyleFlagLabel, StyleColourLabel;
        public DXColourControl StyleColour;
        public DXButton StyleColourButton, StyleFlagPreviousButton, StyleFlagNextButton;

        #endregion

        #region Castle Tab

        private DXTab CastleTab;
        public DXButton ToggleGates, RepairGates, RepairGuards;
        public DXControl CastlePanel;

        //public Dictionary<CastleInfo, GuildCastlePanel> CastlePanels = new Dictionary<CastleInfo, GuildCastlePanel>();

        #endregion

        #region GuildInfo

        public ClientGuildInfo GuildInfo
        {
            get => _GuildInfo;
            set
            {
                if (_GuildInfo == value) return;

                ClientGuildInfo oldValue = _GuildInfo;
                _GuildInfo = value;

                OnGuildInfoChanged(oldValue, value);
            }
        }
        private ClientGuildInfo _GuildInfo;
        public event EventHandler<EventArgs> GuildInfoChanged;
        public void OnGuildInfoChanged(ClientGuildInfo oValue, ClientGuildInfo nValue)
        {
            GuildInfoChanged?.Invoke(this, EventArgs.Empty);

            ClearGuild();

            if (GuildInfo == null)
            {
                CreateTab.TabButton.InvokeMouseClick();
                return;
            }

            for (int i = 0; i < GuildStorage.Length; i++)
                GuildStorage[i] = null;

            HomeTab.TabButton.InvokeMouseClick();

            RefreshStorage();

            RefreshGuildDisplay();
        }

        #endregion

        public DateTime SabukWarDate;

        public DXLabel TitleLabel;
        public DXButton CloseButton;

        public DXImageControl BackgroundImage;

        public WindowSetting Settings;
        public WindowType Type => WindowType.GuildBox;

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

        #endregion

        public GuildDialog()
        {
            Index = 260;
            LibraryFile = LibraryFile.Interface;
            Movable = true;
            Sort = true;
            Size = new Size(456, 556);

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
                Text = CEnvir.Language.GuildDialogTitle,
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
            };
            TitleLabel.Location = new Point((DisplayArea.Width - TitleLabel.Size.Width) / 2, 8);

            GuildTabs = new DXTabControl
            {
                Parent = this,
                Location = new Point(0, 39),
                Size = new Size(456, 464),
                MarginLeft = 15,
                Border = false
            };

            BackgroundImage = new DXImageControl
            {
                Parent = GuildTabs,
                Index = 261,
                LibraryFile = LibraryFile.Interface,
                Location = new Point(0, 23),
                Size = new Size(456, 440),
                Visible = true
            };

            CreateCreateTab();

            CreateHomeTab();

            CreateMemberTab();

            CreateStorageTab();
            
            CreateWarTab();

            CreateStyleTab();

            CreateCastleTab();

            ClearGuild();
        }

        #region Methods

        private void ClearGuild()
        {
            CreateTab.TabButton.Visible = GuildInfo == null;

            HomeTab.TabButton.Visible = GuildInfo != null;
            MemberTab.TabButton.Visible = GuildInfo != null;
            StorageTab.TabButton.Visible = GuildInfo != null;
            WarTab.TabButton.Visible = GuildInfo != null;
            StyleTab.TabButton.Visible = GuildInfo != null;
            CastleTab.TabButton.Visible = GuildInfo != null && GameScene.Game.CastleOwners.Any(x => x.Value == GuildInfo.GuildName);

            if (CreateTab.TabButton.Visible)
                CreateTab.TabButton.InvokeMouseClick();
            else
                HomeTab.TabButton.InvokeMouseClick();

            GuildTabs.TabsChanged();

            for (int i = 0; i < GuildStorage.Length; i++)
                GuildStorage[i] = null;

            TitleLabel.Text = "Guild";

            NoticeTextBox.TextBox.Text = string.Empty;

            MemberLimitLabel.Text = string.Empty;
            GuildFundLabel.Text = string.Empty;
            DailyGrowthLabel.Text = string.Empty;
            TotalContributionLabel.Text = string.Empty;
            DailyContributionLabel.Text = string.Empty;
            GuildTaxLabel.Text = string.Empty;

            MemberScrollBar.MaxValue = 0;

            foreach (GuildMemberRow row in MemberRows)
                row.MemberInfo = null;

            StorageGrid.GridSize = new Size(1, 1);

            StorageScrollBar.MaxValue = 0;

            EditNoticeButton.Visible = false;

            ItemNameTextBox.TextBox.Text = string.Empty;

            ItemTypeComboBox.ListBox.SelectItem(null);
        }

        public override void OnVisibleChanged(bool oValue, bool nValue)
        {
            base.OnVisibleChanged(oValue, nValue);

            if (GuildInfo != null)
                GameScene.Game.GuildBox.RefreshGuildDisplay();
        }

        public void RefreshGuildDisplay()
        {
            TitleLabel.Text = GuildInfo.GuildName;
            TitleLabel.Location = new Point((DisplayArea.Width - TitleLabel.Size.Width) / 2, 8);

            if (!NoticeTextBox.Editable)
                NoticeTextBox.TextBox.Text = GuildInfo.Notice;

            MemberLimitLabel.Text = $"{GuildInfo.Members.Count} / {GuildInfo.MemberLimit}";
            GuildFundLabel.Text = GuildInfo.GuildFunds.ToString("#,##0");
            DailyGrowthLabel.Text = GuildInfo.DailyGrowth.ToString("#,##0");
            TotalContributionLabel.Text = GuildInfo.TotalContribution.ToString("#,##0");
            DailyContributionLabel.Text = GuildInfo.DailyContribution.ToString("#,##0");
            GuildTaxLabel.Text = $"{GuildInfo.Tax}%";

            StyleColour.BackColour = GuildInfo.Colour;

            UpdateMemberRows();

            PermissionChanged();

            ApplyStorageFilter();
        }
        private void RefreshStorage()
        {
            StorageGrid.GridSize = new Size(11, Math.Max(20, (int)Math.Ceiling(GuildInfo.StorageLimit / (float)14)));

            StorageScrollBar.MaxValue = StorageGrid.GridSize.Height;

            foreach (DXItemCell cell in StorageGrid.Grid)
                cell.Item = null;

            foreach (ClientUserItem item in GuildInfo.Storage)
            {
                if (item.Slot < 0 || item.Slot >= StorageGrid.Grid.Length) continue;

                StorageGrid.Grid[item.Slot].Item = item;
            }
        }
        public void PermissionChanged()
        {
            if (GuildInfo == null)
            {
                GameScene.Game.NPCGoodsBox.GuildCheckBox.Enabled = false;
                GameScene.Game.NPCGoodsBox.GuildCheckBox.Checked = false;

                GameScene.Game.NPCRepairBox.GuildCheckBox.Checked = false;
                GameScene.Game.NPCRepairBox.GuildCheckBox.Enabled = false;

                GameScene.Game.NPCRepairBox.GuildStorageButton.Enabled = false;
                return;
            }

            EditNoticeButton.Visible = (GuildInfo.Permission & GuildPermission.EditNotice) == GuildPermission.EditNotice;

            StorageGrid.ReadOnly = (GuildInfo.Permission & GuildPermission.Storage) != GuildPermission.Storage;

            AddMemberPanel.Enabled = (GuildInfo.Permission & GuildPermission.AddMember) == GuildPermission.AddMember;
            EditDefaultMemberButton.Enabled = (GuildInfo.Permission & GuildPermission.Leader) == GuildPermission.Leader;

            TreasuryPanel.Enabled = (GuildInfo.Permission & GuildPermission.Leader) == GuildPermission.Leader;
            StoragePanel.Enabled = (GuildInfo.Permission & GuildPermission.Leader) == GuildPermission.Leader;
            WarPanel.Enabled = (GuildInfo.Permission & GuildPermission.StartWar) == GuildPermission.StartWar;

            StyleColourButton.Enabled = (GuildInfo.Permission & GuildPermission.Leader) == GuildPermission.Leader;
            StyleFlagPreviousButton.Enabled = (GuildInfo.Permission & GuildPermission.Leader) == GuildPermission.Leader;
            StyleFlagNextButton.Enabled = (GuildInfo.Permission & GuildPermission.Leader) == GuildPermission.Leader;

            foreach (KeyValuePair<CastleInfo, GuildCastlePanel> pair in CastlePanels)
                pair.Value.RequestButton.Enabled = (GuildInfo.Permission & GuildPermission.Leader) == GuildPermission.Leader;

            //Market, Buy Repair
            GameScene.Game.NPCGoodsBox.GuildCheckBox.Enabled = (GuildInfo.Permission & GuildPermission.FundsMerchant) == GuildPermission.FundsMerchant;
            GameScene.Game.NPCRepairBox.GuildCheckBox.Enabled = (GuildInfo.Permission & GuildPermission.FundsRepair) == GuildPermission.FundsRepair;
            GameScene.Game.MarketPlaceBox.BuyGuildBox.Enabled = (GuildInfo.Permission & GuildPermission.FundsMarket) == GuildPermission.FundsMarket;
            GameScene.Game.MarketPlaceBox.ConsignGuildBox.Enabled = (GuildInfo.Permission & GuildPermission.FundsMarket) == GuildPermission.FundsMarket;
            GameScene.Game.NPCRepairBox.GuildStorageButton.Enabled = (GuildInfo.Permission & GuildPermission.Storage) == GuildPermission.Storage;

            if (!GameScene.Game.NPCGoodsBox.GuildCheckBox.Enabled)
                GameScene.Game.NPCGoodsBox.GuildCheckBox.Checked = false;

            if (!GameScene.Game.NPCRepairBox.GuildCheckBox.Enabled)
                GameScene.Game.NPCRepairBox.GuildCheckBox.Checked = false;

            if (!GameScene.Game.NPCRepairBox.GuildCheckBox.Enabled)
            {
                GameScene.Game.MarketPlaceBox.BuyGuildBox.Checked = false;
                GameScene.Game.MarketPlaceBox.ConsignGuildBox.Checked = false;
            }
        }

        public void RefreshCastleControls()
        {
            if (GuildInfo == null) return;

            ToggleGates.Enabled = false;
            RepairGates.Enabled = false;
            RepairGuards.Enabled = false;

            CastleTab.TabButton.Visible = false;

            foreach (var castle in GameScene.Game.CastleOwners)
            {
                if (castle.Value == GuildInfo.GuildName)
                {
                    CastleTab.TabButton.Visible = true;

                    GuildTabs.TabsChanged();

                    if ((GuildInfo.Permission & GuildPermission.Leader) == GuildPermission.Leader)
                    {
                        if (castle.Key.Gates.Count > 0)
                        {
                            ToggleGates.Enabled = true;
                            RepairGates.Enabled = true;
                        }

                        if (castle.Key.Guards.Count > 0)
                            RepairGuards.Enabled = true;
                    }

                    return;
                }
            }
        }

        #region Create Tab

        public void CreateCreateTab()
        {
            CreateTab = new DXTab
            {
                TabButton = { Label = { Text = CEnvir.Language.GuildDialogCreateTabLabel } },
                Parent = GuildTabs,
                BackColour = Color.Empty,
                Location = new Point(0, 23)
            };
            CreateTab.TabButton.MouseClick += (o, e) =>
            {
                BackgroundImage.Index = 266;
                CreatePanel.Visible = true;
                AddMemberPanel.Visible = false;
                TreasuryPanel.Visible = false;
                StoragePanel.Visible = false;
                WarPanel.Visible = false;
                CastlePanel.Visible = false;
            };

            CreatePanel = new DXControl
            {
                Parent = this,
                Location = new Point(10, 500),
                Size = new Size(436, 50),
                Border = false,
                Visible = true
            };

            StarterGuildButton = new DXButton
            {
                Parent = CreatePanel,
                Location = new Point(10, 10),
                ButtonType = ButtonType.Default,
                Size = new Size(120, DefaultHeight),
                Label = { Text = CEnvir.Language.GuildDialogCreateTabStarterGuildButtonLabel }
            };
            StarterGuildButton.MouseClick += StarterGuildButton_MouseClick;

            DXLabel stepLabel = new DXLabel
            {
                Text = CEnvir.Language.GuildDialogCreateTabStep1Label,
                Parent = CreateTab,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Size = new Size(CreateTab.Size.Width, 22),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Location = new Point(0, 20)
            };


            DXLabel label = new DXLabel
            {
                Text = CEnvir.Language.GuildDialogCreateTabStep1GuildNameLabel,
                Parent = CreateTab,
                Outline = true,
                OutlineColour = Color.Black,
                ForeColour = Color.White,
                IsControl = false,
            };
            label.Location = new Point((CreateTab.Size.Width - label.Size.Width - 5 - 110) / 2, stepLabel.Location.Y + 30);

            GuildNameBox = new DXTextBox
            {
                Size = new Size(110, 18),
                Location = new Point((CreateTab.Size.Width - label.Size.Width - 5 - 110) / 2 + label.Size.Width + 5, label.Location.Y),
                Parent = CreateTab,
            };
            GuildNameBox.TextBox.TextChanged += GuildNameTextBox_TextChanged;

            stepLabel = new DXLabel
            {
                Text = CEnvir.Language.GuildDialogCreateTabStep2Label,
                Parent = CreateTab,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Size = new Size(CreateTab.Size.Width, 22),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Location = new Point(0, label.Location.Y + 50)
            };

            GoldCheckBox = new DXCheckBox
            {
                Label = { Text = string.Format(CEnvir.Language.GuildDialogCreateTabStep2GoldCheckBoxLabel, Globals.GuildCreationCost), ForeColour = Color.White },
                Parent = CreateTab,
                Checked = true,
                ReadOnly = true,
            };
            GoldCheckBox.Location = new Point((CreateTab.Size.Width - GoldCheckBox.Size.Width) / 2, stepLabel.Location.Y + 30);
            GoldCheckBox.MouseClick += (o, e) => GoldCheckBox.Checked = true;


            HornCheckBox = new DXCheckBox
            {
                Label = { Text = CEnvir.Language.GuildDialogCreateTabStep2HornCheckBoxLabel, ForeColour = Color.White },
                Parent = CreateTab,
                ReadOnly = true,
            };
            HornCheckBox.Location = new Point((CreateTab.Size.Width - HornCheckBox.Size.Width) / 2, GoldCheckBox.Location.Y + 20);
            HornCheckBox.MouseClick += (o, e) => HornCheckBox.Checked = true;

            GoldCheckBox.CheckedChanged += (o, e) =>
            {
                TotalCostBox.Value = TotalCost;
                HornCheckBox.Checked = !GoldCheckBox.Checked;
            };
            HornCheckBox.CheckedChanged += (o, e) =>
            {
                TotalCostBox.Value = TotalCost;
                GoldCheckBox.Checked = !HornCheckBox.Checked;
            };

            stepLabel = new DXLabel
            {
                Text = CEnvir.Language.GuildDialogCreateTabStep3Label,
                Parent = CreateTab,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Size = new Size(CreateTab.Size.Width, 22),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Location = new Point(0, HornCheckBox.Location.Y + 50)
            };

            label = new DXLabel
            {
                Text = CEnvir.Language.GuildDialogCreateTabStep3ExtraMembersLabel,
                Parent = CreateTab,
                Outline = true,
                OutlineColour = Color.Black,
                ForeColour = Color.White,
                IsControl = false,
            };
            label.Location = new Point(GuildNameBox.Location.X - label.Size.Width, stepLabel.Location.Y + 30);

            MemberTextBox = new DXNumberTextBox
            {
                Size = new Size(110, 18),
                Location = new Point(GuildNameBox.Location.X, label.Location.Y),
                Parent = CreateTab,
                MinValue = 0,
                MaxValue = 100,
            };
            MemberTextBox.ValueChanged += MemberTextBox_ValueChanged;

            label = new DXLabel
            {
                Text = "[?]",
                Parent = CreateTab,
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Hint = string.Format(CEnvir.Language.GuildDialogCreateTabStep3ExtraMembersHint, Globals.GuildMemberCost)
            };
            label.Location = new Point(MemberTextBox.Location.X + MemberTextBox.Size.Width, MemberTextBox.Location.Y + (MemberTextBox.Size.Height - label.Size.Height) / 2);

            label = new DXLabel
            {
                Text = CEnvir.Language.GuildDialogCreateTabStep3ExtraStorageLabel,
                Parent = CreateTab,
                Outline = true,
                OutlineColour = Color.Black,
                ForeColour = Color.White,
                IsControl = false,
            };
            label.Location = new Point(GuildNameBox.Location.X - label.Size.Width, MemberTextBox.Location.Y + 20);

            StorageTextBox = new DXNumberTextBox
            {
                Size = new Size(110, 18),
                Location = new Point(GuildNameBox.Location.X, label.Location.Y),
                Parent = CreateTab,
                MinValue = 0,
                MaxValue = 500,
            };
            StorageTextBox.ValueChanged += StorageTextBox_ValueChanged;

            label = new DXLabel
            {
                Text = "[?]",
                Parent = CreateTab,
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Hint = string.Format(CEnvir.Language.GuildDialogCreateTabStep3ExtraStorageHint, Globals.GuildStorageCost)
            };
            label.Location = new Point(StorageTextBox.Location.X + StorageTextBox.Size.Width, StorageTextBox.Location.Y + (StorageTextBox.Size.Height - label.Size.Height) / 2);



            stepLabel = new DXLabel
            {
                Text = CEnvir.Language.GuildDialogCreateTabStep4Label,
                Parent = CreateTab,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Size = new Size(CreateTab.Size.Width, 22),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Location = new Point(0, StorageTextBox.Location.Y + 50)
            };

            label = new DXLabel
            {
                Text = CEnvir.Language.GuildDialogCreateTabStep4TotalCostLabel,
                Parent = CreateTab,
                Outline = true,
                OutlineColour = Color.Black,
                ForeColour = Color.White,
                IsControl = false,
            };
            label.Location = new Point(GuildNameBox.Location.X - label.Size.Width, stepLabel.Location.Y + 30);

            TotalCostBox = new DXNumberTextBox
            {
                Size = new Size(110, 18),
                Location = new Point(GuildNameBox.Location.X, label.Location.Y),
                Parent = CreateTab,
                MaxValue = 2000000000,
                Value = TotalCost,
                ReadOnly = true,
            };
            TotalCostBox.ValueChanged += TotalCostBox_ValueChanged;

            CreateButton = new DXButton
            {
                Parent = CreateTab,
                ButtonType = ButtonType.SmallButton,
                Size = new Size(110, SmallButtonHeight),
                Label = { Text = CEnvir.Language.GuildDialogCreateTabCreateButtonLabel },
                Location = new Point(TotalCostBox.Location.X, TotalCostBox.Location.Y + 30)
            };

            CreateButton.MouseClick += CreateButton_MouseClick;

        }

        private void StarterGuildButton_MouseClick(object sender, MouseEventArgs e)
        {
            CEnvir.Enqueue(new C.JoinStarterGuild
            {
            });
        }

        private void CreateButton_MouseClick(object sender, MouseEventArgs e)
        {
            CreateAttempted = true;

            CEnvir.Enqueue(new C.GuildCreate
            {
                Name = GuildNameBox.TextBox.Text,
                UseGold = GoldCheckBox.Checked,
                Members = MemberLimit,
                Storage = StorageSize,
            });
        }

        private void GuildNameTextBox_TextChanged(object sender, EventArgs e)
        {
            GuildNameValid = Globals.GuildNameRegex.IsMatch(GuildNameBox.TextBox.Text);

            if (string.IsNullOrEmpty(GuildNameBox.TextBox.Text))
                GuildNameBox.BorderColour = Color.FromArgb(198, 166, 99);
            else if (GuildNameValid)
                GuildNameBox.BorderColour = Color.Green;
            else
                GuildNameBox.BorderColour = Color.Red;
        }

        private void StorageTextBox_ValueChanged(object sender, EventArgs e)
        {
            StorageSize = (int)StorageTextBox.Value;
        }

        private void TotalCostBox_ValueChanged(object sender, EventArgs e)
        {
            TotalCostBox.BorderColour = TotalCostBox.Value > GameScene.Game.User.Gold.Amount ? Color.Red : Color.FromArgb(198, 166, 99);
        }

        private void MemberTextBox_ValueChanged(object sender, EventArgs e)
        {
            MemberLimit = (int)MemberTextBox.Value;
        }

        #endregion

        #region Home Tab

        private void CreateHomeTab()
        {
            HomeTab = new DXTab
            {
                TabButton = { Label = { Text = CEnvir.Language.GuildDialogHomeTabLabel } },
                Parent = GuildTabs,
                BackColour = Color.Empty,
                Location = new Point(0, 23)
            };
            HomeTab.TabButton.MouseClick += (o, e) =>
            {
                BackgroundImage.Index = 261;
                CreatePanel.Visible = false;
                AddMemberPanel.Visible = false;
                TreasuryPanel.Visible = true;
                StoragePanel.Visible = false;
                WarPanel.Visible = false;
                CastlePanel.Visible = false;
            };

            new DXLabel
            {
                Text = CEnvir.Language.GuildDialogHomeTabNoticeLabel,
                Parent = HomeTab,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Size = new Size(HomeTab.Size.Width, 21),
                Location = new Point(20, 6),
                DrawFormat = TextFormatFlags.VerticalCenter,
            };

            NoticeScrollBar = new DXVScrollBar
            {
                Parent = HomeTab,
                Location = new Point(HomeTab.Size.Width - 34, 30),
                Size = new Size(22, 262),
                VisibleSize = 17,
                Change = 1,
                Border = false,
                BackColour = Color.Empty,
                UpButton = { Index = 61, LibraryFile = LibraryFile.Interface },
                DownButton = { Index = 62, LibraryFile = LibraryFile.Interface },
                PositionBar = { Index = 60, LibraryFile = LibraryFile.Interface },
            };
            NoticeScrollBar.ValueChanged += (o, e) => SetLineIndex(NoticeScrollBar.Value);

            NoticeTextBox = new DXTextBox
            {
                Parent = HomeTab,
                TextBox = { Multiline = true },
                Location = new Point(16, 33),
                Size = new Size(403, 252),
                KeepFocus = false,
                Editable = false,
                Border = false,
                MaxLength = Globals.MaxGuildNoticeLength
            };
            NoticeTextBox.TextBox.TextChanged += (o, e) => UpdateNoticePosition();
            NoticeTextBox.TextBox.MouseMove += (o, e) => UpdateNoticePosition();
            NoticeTextBox.TextBox.MouseDown += (o, e) => UpdateNoticePosition();
            NoticeTextBox.TextBox.MouseUp += (o, e) => UpdateNoticePosition();
            NoticeTextBox.TextBox.KeyDown += (o, e) => UpdateNoticePosition();
            NoticeTextBox.TextBox.KeyUp += (o, e) => UpdateNoticePosition();
            NoticeTextBox.TextBox.KeyPress += (o, e) =>
            {
                if (e.KeyChar == (char) 1)
                {
                    NoticeTextBox.TextBox.SelectAll();
                    e.Handled = true;
                }

                UpdateNoticePosition();
            };
            NoticeTextBox.TextBox.MouseWheel += (o, e) => NoticeScrollBar.Value -= e.Delta / SystemInformation.MouseWheelScrollDelta;
            NoticeTextBox.MouseWheel += (o, e) => NoticeScrollBar.Value -= e.Delta / SystemInformation.MouseWheelScrollDelta;

            EditNoticeButton = new DXButton
            {
                Parent = HomeTab,
                Size = new Size(60, SmallButtonHeight),
                Location = new Point(HomeTab.Size.Width - 75, 9),
                Label = { Text = CEnvir.Language.GuildDialogHomeTabNoticeEditButtonLabel },
                ButtonType = ButtonType.SmallButton
            };
            EditNoticeButton.MouseClick += (o, e) =>
            {
                EditNoticeButton.Visible = false;
                SaveNoticeButton.Visible = true;
                CancelNoticeButton.Visible = true;
                NoticeTextBox.Editable = true;
                NoticeTextBox.SetFocus();
            };

            SaveNoticeButton = new DXButton
            {
                Parent = HomeTab,
                Size = new Size(60, SmallButtonHeight),
                Location = new Point(HomeTab.Size.Width - 145, 9),
                Label = { Text = CEnvir.Language.GuildDialogHomeTabNoticeSaveButtonLabel },
                ButtonType = ButtonType.SmallButton,
                Visible = false,
            };
            SaveNoticeButton.MouseClick += (o, e) =>
            {
                EditNoticeButton.Visible = true;
                SaveNoticeButton.Visible = false;
                CancelNoticeButton.Visible = false;
                NoticeTextBox.Editable = false;

                CEnvir.Enqueue(new C.GuildEditNotice { Notice = NoticeTextBox.TextBox.Text });
            };

            CancelNoticeButton = new DXButton
            {
                Parent = HomeTab,
                Size = new Size(60, SmallButtonHeight),
                Location = new Point(HomeTab.Size.Width - 75, 9),
                Label = { Text = CEnvir.Language.CommonControlCancel },
                ButtonType = ButtonType.SmallButton,
                Visible = false,
            };
            CancelNoticeButton.MouseClick += (o, e) =>
            {
                EditNoticeButton.Visible = true;
                SaveNoticeButton.Visible = false;
                CancelNoticeButton.Visible = false;
                NoticeTextBox.Editable = false;

                NoticeTextBox.TextBox.Text = GuildInfo.Notice;
            };

            DXControl panel = new DXControl
            {
                Parent = HomeTab,
                Location = new Point(0, NoticeTextBox.Size.Height + 5 + NoticeTextBox.Location.Y),
                Size = new Size(HomeTab.Size.Width, 140),
                Border = false,
                BorderColour = Color.FromArgb(198, 166, 99)
            };

            new DXLabel
            {
                Text = CEnvir.Language.GuildDialogHomeTabNoticeStatsLabel,
                Parent = panel,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Size = new Size(HomeTab.Size.Width, 22),
                Location = new Point(20, 0),
                DrawFormat = TextFormatFlags.VerticalCenter,
            };

            DXLabel label = new DXLabel
            {
                Text = CEnvir.Language.GuildDialogHomeTabNoticeStatsMembersLabel,
                Parent = panel,
                Outline = true,
                IsControl = false,
            };
            label.Location = new Point(120 - label.Size.Width, 30);

            MemberLimitLabel = new DXLabel
            {
                Parent = panel,
                Outline = true,
                ForeColour = Color.White,
                IsControl = false,
                Location = new Point(120, label.Location.Y),
            };

            label = new DXLabel
            {
                Text = CEnvir.Language.GuildDialogHomeTabNoticeStatsGuildFundsLabel,
                Parent = panel,
                Outline = true,
                IsControl = false,
            };
            label.Location = new Point(120 - label.Size.Width, 45);

            GuildFundLabel = new DXLabel
            {
                Parent = panel,
                Outline = true,
                ForeColour = Color.White,
                IsControl = false,
                Location = new Point(120, label.Location.Y),
            };

            label = new DXLabel
            {
                Text = CEnvir.Language.GuildDialogHomeTabNoticeStatsTodaysGrowthLabel,
                Parent = panel,
                Outline = true,
                IsControl = false,
            };
            label.Location = new Point(120 - label.Size.Width, 60);

            DailyGrowthLabel = new DXLabel
            {
                Parent = panel,
                Outline = true,
                ForeColour = Color.White,
                IsControl = false,
                Location = new Point(120, label.Location.Y),
            };

            label = new DXLabel
            {
                Text = CEnvir.Language.GuildDialogHomeTabNoticeStatsTotalContributionLabel,
                Parent = panel,
                Outline = true,
                IsControl = false,
            };
            label.Location = new Point(390 - label.Size.Width, 45);

            TotalContributionLabel = new DXLabel
            {
                Parent = panel,
                Outline = true,
                ForeColour = Color.White,
                IsControl = false,
                Location = new Point(390, label.Location.Y),
            };

            label = new DXLabel
            {
                Text = CEnvir.Language.GuildDialogHomeTabNoticeStatsTodaysContributionLabel,
                Parent = panel,
                Outline = true,
                IsControl = false,
            };
            label.Location = new Point(390 - label.Size.Width, 60);

            DailyContributionLabel = new DXLabel
            {
                Parent = panel,
                Outline = true,
                ForeColour = Color.White,
                IsControl = false,
                Location = new Point(390, label.Location.Y),
            };

            label = new DXLabel
            {
                Text = CEnvir.Language.GuildDialogHomeTabNoticeStatsTaxLabel,
                Parent = panel,
                Outline = true,
                IsControl = false,
            };
            label.Location = new Point(120 - label.Size.Width, 75);

            GuildTaxLabel = new DXLabel
            {
                Parent = panel,
                Outline = true,
                ForeColour = Color.White,
                IsControl = false,
                Location = new Point(120, label.Location.Y),
            };

            TreasuryPanel = new DXControl
            {
                Parent = this,
                Location = new Point(10, 500),
                Size = new Size(436, 50),
                Border = false,
                Visible = true
            };

            SetTaxButton = new DXButton
            {
                Parent = TreasuryPanel,
                Location = new Point(10, 10),
                ButtonType = ButtonType.Default,
                Size = new Size(120, DefaultHeight),
                Label = { Text = CEnvir.Language.GuildDialogManageTabTreasuryChangeButtonLabel },
            };
            SetTaxButton.MouseClick += (o, e) =>
            {
                DXInputWindow window = new DXInputWindow(CEnvir.Language.GuildDialogManageTabTreasuryTaxConfirmMessage, CEnvir.Language.GuildDialogManageTabTreasuryLabel)
                {
                    ConfirmButton = { Enabled = false },
                    Modal = true
                };
                window.ValueTextBox.TextBox.TextChanged += (o1, e1) =>
                {
                    window.ConfirmButton.Enabled = Globals.GuildTaxReg.IsMatch(window.ValueTextBox.TextBox.Text);
                };
                window.ConfirmButton.MouseClick += (o1, e1) =>
                {
                    CEnvir.Enqueue(new C.GuildTax { Tax = long.Parse(window.Value) });
                };
            };
        }

        public void UpdateNoticePosition()
        {
            NoticeScrollBar.MaxValue = NoticeTextBox.TextBox.GetLineFromCharIndex(NoticeTextBox.TextBox.TextLength) + 1;
            NoticeScrollBar.Value = GetCurrentLine();
        }

        private int GetCurrentLine()
        {
            return SendMessage(NoticeTextBox.TextBox.Handle, EM_GETFIRSTVISIBLELINE, 0, 0);
        }

        const int EM_GETFIRSTVISIBLELINE = 0x00CE;
        const int EM_LINESCROLL = 0x00B6;

        [DllImport("user32.dll")]
        static extern int SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        private void SetLineIndex(int lineIndex)
        {
            int line = GetCurrentLine();
            if (line == lineIndex) return;

            SendMessage(NoticeTextBox.TextBox.Handle, EM_LINESCROLL, 0, lineIndex - GetCurrentLine());
            NoticeTextBox.DisposeTexture();
        }

        #endregion

        #region Member Tab

        public void CreateMemberTab()
        {
            MemberTab = new DXTab
            {
                TabButton = { Label = { Text = CEnvir.Language.GuildDialogMembersTabLabel } },
                Parent = GuildTabs,
                BackColour = Color.Empty,
                Location = new Point(0, 23)
            };
            MemberTab.TabButton.MouseClick += (o, e) =>
            {
                BackgroundImage.Index = 262;
                CreatePanel.Visible = false;
                AddMemberPanel.Visible = true;
                TreasuryPanel.Visible = false;
                StoragePanel.Visible = false;
                WarPanel.Visible = false;
                CastlePanel.Visible = false;
            };

            new GuildMemberRow
            {
                Location = new Point(7, 9),
                Parent = MemberTab,
                IsHeader = true,
            };

            MemberScrollBar = new DXVScrollBar
            {
                Parent = MemberTab,
                Location = new Point(HomeTab.Size.Width - 34, 30),
                Size = new Size(22, 397),
                VisibleSize = 16,
                Change = 1,
                Border = false,
                BackColour = Color.Empty,
                UpButton = { Index = 61, LibraryFile = LibraryFile.Interface },
                DownButton = { Index = 62, LibraryFile = LibraryFile.Interface },
                PositionBar = { Index = 60, LibraryFile = LibraryFile.Interface },
            };
            MemberScrollBar.ValueChanged += MemberScrollBar_ValueChanged;

            MemberRows = new GuildMemberRow[17];
            for (int i = 0; i < MemberRows.Length; i++)
            {
                MemberRows[i] = new GuildMemberRow
                {
                    Parent = MemberTab,
                    Location = new Point(16, 11 + i * 23 + 23),
                    Visible = false
                };

                MemberRows[i].MouseWheel += MemberScrollBar.DoMouseWheel;
            }
            MouseWheel += MemberScrollBar.DoMouseWheel;


            AddMemberPanel = new DXControl
            {
                Parent = this,
                Location = new Point(10, 500),
                Size = new Size(436, 50),
                Border = false,
                Visible = true
            };

            AddMemberButton = new DXButton
            {
                Parent = AddMemberPanel,
                Location = new Point(10, 10),
                ButtonType = ButtonType.Default,
                Size = new Size(110, DefaultHeight),
                Label = { Text = CEnvir.Language.GuildDialogManageTabMembershipAddButtonLabel },
            };
            AddMemberButton.MouseClick += (o, e) =>
            {
                DXInputWindow window = new DXInputWindow(CEnvir.Language.GuildDialogManageTabMembershipMemberConfirmMessage, CEnvir.Language.GuildDialogManageTabMembershipLabel)
                {
                    ConfirmButton = { Enabled = false },
                    Modal = true
                };
                window.ValueTextBox.TextBox.TextChanged += (o1, e1) =>
                {
                    window.ConfirmButton.Enabled = Globals.CharacterReg.IsMatch(window.ValueTextBox.TextBox.Text);
                };
                window.ConfirmButton.MouseClick += (o1, e1) =>
                {
                    CEnvir.Enqueue(new C.GuildInviteMember { Name = window.Value });
                };
            };

            EditDefaultMemberButton = new DXButton
            {
                Parent = AddMemberPanel,
                Location = new Point(AddMemberButton.DisplayArea.Right + 5, 10),
                ButtonType = ButtonType.Default,
                Size = new Size(110, DefaultHeight),
                Label = { Text = CEnvir.Language.GuildDialogManageTabMembershipEditDefaultButtonLabel },
            };
            EditDefaultMemberButton.MouseClick += (o, e) =>
            {
                GameScene.Game.GuildMemberBox.MemberNameLabel.Text = "Default Member";
                GameScene.Game.GuildMemberBox.RankTextBox.TextBox.Text = GuildInfo.DefaultRank;
                GameScene.Game.GuildMemberBox.Permission = GuildInfo.DefaultPermission;
                GameScene.Game.GuildMemberBox.MemberIndex = 0;

                GameScene.Game.GuildMemberBox.Visible = true;
                GameScene.Game.GuildMemberBox.BringToFront();
            };

            IncreaseMemberButton = new DXButton
            {
                Parent = AddMemberPanel,
                Location = new Point(EditDefaultMemberButton.DisplayArea.Right + 5, 10),
                ButtonType = ButtonType.Default,
                Size = new Size(110, DefaultHeight),
                Label = { Text = CEnvir.Language.GuildDialogManageTabMembershipMembersIncreaseButtonLabel },
            };
            IncreaseMemberButton.MouseClick += (o, e) =>
            {
                DXMessageBox window = new DXMessageBox(string.Format(CEnvir.Language.GuildDialogManageTabMembershipIncreaseMemberConfirmMessage, Globals.GuildMemberCost), CEnvir.Language.GuildDialogManageTabMembershipLabel, DXMessageBoxButtons.YesNo);
                window.YesButton.MouseClick += (o1, e1) =>
                {
                    CEnvir.Enqueue(new C.GuildIncreaseMember());
                };
            };

        }

        private void MemberScrollBar_ValueChanged(object sender, EventArgs e)
        {
            UpdateMemberRows();
        }

        public void UpdateMemberRows()
        {
            if (GuildInfo == null) return;

            MemberScrollBar.MaxValue = GuildInfo.Members.Count;


            for (int i = 0; i < MemberRows.Length; i++)
                MemberRows[i].MemberInfo = i + MemberScrollBar.Value >= GuildInfo.Members.Count ? null : GuildInfo.Members[i + MemberScrollBar.Value];
        }

        #endregion

        #region Storage Tab

        public void CreateStorageTab()
        {
            StorageTab = new DXTab
            {
                TabButton = { Label = { Text = CEnvir.Language.GuildDialogStorageTabLabel } },
                Parent = GuildTabs,
                BackColour = Color.Empty,
                Location = new Point(0, 23)
            };
            StorageTab.TabButton.MouseClick += (o, e) =>
            {
                BackgroundImage.Index = 263;
                CreatePanel.Visible = false;
                AddMemberPanel.Visible = false;
                TreasuryPanel.Visible = false;
                StoragePanel.Visible = true;
                WarPanel.Visible = false;
                CastlePanel.Visible = false;
            };

            DXControl filterPanel = new DXControl
            {
                Parent = StorageTab,
                Size = new Size(StorageTab.Size.Width - 14, 25),
                Location = new Point(6, 11),
                Border = false
            };

            DXLabel label = new DXLabel
            {
                Parent = filterPanel,
                Location = new Point(10, 7),
                Text = CEnvir.Language.GuildDialogStorageTabNameLabel,
            };

            ItemNameTextBox = new DXTextBox
            {
                Parent = filterPanel,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Regular),
                Size = new Size(110, 16),
                Border = false,
                Location = new Point(label.Location.X + label.Size.Width + 2, label.Location.Y),
            };
            ItemNameTextBox.TextBox.TextChanged += (o, e) => ApplyStorageFilter();

            label = new DXLabel
            {
                Parent = filterPanel,
                Location = new Point(ItemNameTextBox.Location.X + ItemNameTextBox.Size.Width + 33, 5),
                Text = CEnvir.Language.GuildDialogStorageTabItemLabel,
            };

            ItemTypeComboBox = new DXComboBox
            {
                Border = false,
                Parent = filterPanel,
                Location = new Point(label.Location.X + label.Size.Width + 1, label.Location.Y + 1),
                Size = new Size(122, DXComboBox.DefaultNormalHeight)
            };
            ItemTypeComboBox.SelectedItemChanged += (o, e) => ApplyStorageFilter();

            new DXListBoxItem
            {
                Parent = ItemTypeComboBox.ListBox,
                Label = { Text = $"All" },
                Item = null
            };

            Type itemType = typeof(ItemType);

            for (ItemType i = ItemType.Nothing; i <= ItemType.Reel; i++)
            {
                MemberInfo[] infos = itemType.GetMember(i.ToString());

                DescriptionAttribute description = infos[0].GetCustomAttribute<DescriptionAttribute>();

                new DXListBoxItem
                {
                    Parent = ItemTypeComboBox.ListBox,
                    Label = { Text = description?.Description ?? i.ToString() },
                    Item = i
                };
            }

            ItemTypeComboBox.ListBox.SelectItem(null);

            ClearButton = new DXButton
            {
                Size = new Size(40, SmallButtonHeight),
                Location = new Point(ItemTypeComboBox.Location.X + ItemTypeComboBox.Size.Width + 30, label.Location.Y - 1),
                Parent = filterPanel,
                ButtonType = ButtonType.SmallButton,
                Label = { Text = CEnvir.Language.GuildDialogStorageTabClearButtonLabel }
            };
            ClearButton.MouseClick += (o, e) =>
            {
                ItemTypeComboBox.ListBox.SelectItem(null);
                ItemNameTextBox.TextBox.Text = string.Empty;
            };

            StorageGrid = new DXItemGrid
            {
                Parent = StorageTab,
                GridSize = new Size(1, 1),
                Location = new Point(24, 51),
                GridType = GridType.GuildStorage,
                ItemGrid = GuildStorage,
                VisibleHeight = 10,
                Border = false,
                GridPadding = 1,
                BackColour = Color.Empty
            };
            StorageGrid.GridSizeChanged += StorageGrid_GridSizeChanged;

            StorageScrollBar = new DXVScrollBar
            {
                Parent = StorageTab,
                Location = new Point(StorageTab.Size.Width - 20, 43),
                Size = new Size(14, 349),
                VisibleSize = 10,
                Change = 1,
                Visible = false
            };
            StorageScrollBar.ValueChanged += StorageScrollBar_ValueChanged;


            StoragePanel = new DXControl
            {
                Parent = this,
                Location = new Point(10, 500),
                Size = new Size(436, 50),
                Border = false,
                Visible = true
            };

            IncreaseStorageButton = new DXButton
            {
                Parent = StoragePanel,
                Location = new Point(10, 10),
                ButtonType = ButtonType.Default,
                Size = new Size(110, DefaultHeight),
                Label = { Text = string.Format(CEnvir.Language.GuildDialogManageTabUpgradeStorageIncreaseButtonLabel, Globals.GuildStorageCost) },
            };
            IncreaseStorageButton.MouseClick += (o, e) =>
            {
                DXMessageBox window = new DXMessageBox(string.Format(CEnvir.Language.GuildDialogManageTabUpgradeStorageConfirmMessage, Globals.GuildStorageCost), CEnvir.Language.GuildDialogManageTabUpgradeStorageLabel, DXMessageBoxButtons.YesNo);
                window.YesButton.MouseClick += (o1, e1) =>
                {
                    CEnvir.Enqueue(new C.GuildIncreaseStorage());
                };
            };

        }

        private void StorageGrid_GridSizeChanged(object sender, EventArgs e)
        {
            foreach (DXItemCell cell in StorageGrid.Grid)
                cell.ItemChanged += (o, e1) => FilterCell(cell);

            if (StorageScrollBar != null)
            {
                foreach (DXItemCell cell in StorageGrid.Grid)
                    cell.MouseWheel += StorageScrollBar.DoMouseWheel;
            }
        }

        public void ApplyStorageFilter()
        {
            foreach (DXItemCell cell in StorageGrid.Grid)
                FilterCell(cell);
        }
        public void FilterCell(DXItemCell cell)
        {
            if (cell.Slot >= GuildInfo.StorageLimit)
            {
                cell.Enabled = false;
                return;
            }

            if (cell.Item == null && (ItemTypeComboBox.SelectedItem != null || !string.IsNullOrEmpty(ItemNameTextBox.TextBox.Text)))
            {
                cell.Enabled = false;
                return;
            }

            if (ItemTypeComboBox.SelectedItem != null && cell.Item != null && cell.Item.Info.ItemType != (ItemType) ItemTypeComboBox.SelectedItem)
            {
                cell.Enabled = false;
                return;
            }

            if (!string.IsNullOrEmpty(ItemNameTextBox.TextBox.Text) && cell.Item != null && cell.Item.Info.ItemName.IndexOf(ItemNameTextBox.TextBox.Text, StringComparison.OrdinalIgnoreCase) < 0)
            {
                cell.Enabled = false;
                return;
            }

            cell.Enabled = true;
        }


        private void StorageScrollBar_ValueChanged(object sender, EventArgs e)
        {
            StorageGrid.ScrollValue = StorageScrollBar.Value;
        }


        #endregion
        
        #region War Tab

        public void CreateWarTab()
        {
            WarTab = new DXTab
            {
                TabButton = { Label = { Text = CEnvir.Language.GuildDialogWarTabLabel } },
                Parent = GuildTabs,
                BackColour = Color.Empty,
                Location = new Point(0, 23)
            };
            WarTab.TabButton.MouseClick += (o, e) =>
            {
                BackgroundImage.Index = 264;
                CreatePanel.Visible = false;
                AddMemberPanel.Visible = false;
                TreasuryPanel.Visible = false;
                StoragePanel.Visible = false;
                WarPanel.Visible = true;
                CastlePanel.Visible = false;
            };
         
            WarPanel = new DXControl
            {
                Parent = this,
                Location = new Point(10, 500),
                Size = new Size(436, 50),
                Border = false,
                Visible = true
            };

            StartWarButton = new DXButton
            {
                Parent = WarPanel,
                Location = new Point(10, 10),
                ButtonType = ButtonType.Default,
                Size = new Size(110, DefaultHeight),
                Label = { Text = CEnvir.Language.GuildDialogWarTabGuildWarStartWarButtonLabel }
            };
            StartWarButton.MouseClick += (o, e) =>
            {
                DXInputWindow window = new DXInputWindow(CEnvir.Language.GuildDialogWarTabGuildWarConfirmMessage, CEnvir.Language.GuildDialogWarTabGuildWarStartWarButtonLabel)
                {
                    ConfirmButton = { Enabled = false },
                    Modal = true
                };
                window.ValueTextBox.TextBox.TextChanged += (o1, e1) =>
                {
                    window.ConfirmButton.Enabled = Globals.GuildNameRegex.IsMatch(window.ValueTextBox.TextBox.Text);
                };
                window.ConfirmButton.MouseClick += (o1, e1) =>
                {
                    CEnvir.Enqueue(new C.GuildWar { GuildName = window.Value });
                };
            };

            int count = 0;
            foreach (CastleInfo castle in CEnvir.CastleInfoList.Binding)
            {
                CastlePanels[castle] = new GuildCastlePanel
                {
                    Parent = WarTab,
                    Castle = castle,
                    Location =  new Point(14, (142 * count) + 7),
                    Visible = true
                };
                count++;
            }

            /*

            new DXLabel
            {
                Text = "Desert Conquest",
                Parent = panel,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Size = new Size(panel.Size.Width, 22),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Enabled = false
            };*/
            

        }

        #endregion

        #region Style Tab

        public void CreateStyleTab()
        {
            StyleTab = new DXTab
            {
                TabButton = { Label = { Text = "Style" } },
                Parent = GuildTabs,
                BackColour = Color.Empty,
                Location = new Point(0, 23)
            };
            StyleTab.TabButton.MouseClick += (o, e) =>
            {
                BackgroundImage.Index = 265;
                CreatePanel.Visible = false;
                AddMemberPanel.Visible = false;
                TreasuryPanel.Visible = false;
                StoragePanel.Visible = false;
                WarPanel.Visible = false;
                CastlePanel.Visible = false;
            };
            StyleTab.BeforeChildrenDraw += StyleTab_BeforeChildrenDraw;

            StyleColourPanel = new DXControl
            {
                Parent = StyleTab,
                Location = new Point(231, 7),
                Size = new Size(212, 89),
                Border = false
            };

            new DXLabel
            {
                Text = "Colour",
                Parent = StyleColourPanel,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Size = new Size(StyleColourPanel.Size.Width, 22),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
            };

            StyleColour = new DXColourControl
            {
                Parent = StyleColourPanel,
                Size = new Size(110, 20),
                Location = new Point(9, 34),
                BackColour = Color.FromArgb(CEnvir.Random.Next(256), CEnvir.Random.Next(256), CEnvir.Random.Next(256))
            };
            StyleColourButton = new DXButton
            {
                Parent = StyleColourPanel,
                Location = new Point(125, 34),
                ButtonType = ButtonType.SmallButton,
                Size = new Size(80, SmallButtonHeight),
                Label = { Text = "Save" },
            };
            StyleColourButton.MouseClick += (o, e) =>
            {
                CEnvir.Enqueue(new C.GuildColour { Colour = StyleColour.BackColour });
            };

            StyleFlagPanel = new DXControl
            {
                Parent = StyleTab,
                Location = new Point(14, 7),
                Size = new Size(212, 292),
                Border = false
            };

            new DXLabel
            {
                Text = "Flag",
                Parent = StyleFlagPanel,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Size = new Size(StyleFlagPanel.Size.Width, 22),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
            };

            StyleFlagPreviousButton = new DXButton
            {
                Parent = StyleFlagPanel,
                Location = new Point(5, StyleFlagPanel.Size.Height - 23),
                ButtonType = ButtonType.SmallButton,
                Size = new Size(60, SmallButtonHeight),
                Label = { Text = "Previous" },
            };
            StyleFlagPreviousButton.MouseClick += (o, e) =>
            {
                int newFlag = GuildInfo.Flag - 1;

                if (newFlag < 0) newFlag = 9;

                CEnvir.Enqueue(new C.GuildFlag { Flag = newFlag });
            };

            StyleFlagNextButton = new DXButton
            {
                Parent = StyleFlagPanel,
                Location = new Point(StyleFlagPanel.Size.Width - 65, StyleFlagPanel.Size.Height - 23),
                ButtonType = ButtonType.SmallButton,
                Size = new Size(60, SmallButtonHeight),
                Label = { Text = "Next" },
            };
            StyleFlagNextButton.MouseClick += (o, e) =>
            {
                int newFlag = GuildInfo.Flag + 1;

                if (newFlag > 9) newFlag = 0;

                CEnvir.Enqueue(new C.GuildFlag { Flag = newFlag });
            };
        }

        private void StyleTab_BeforeChildrenDraw(object sender, EventArgs e)
        {
            MirLibrary library;

            int x = 100;
            int y = 270;

            if (!CEnvir.LibraryList.TryGetValue(LibraryFile.CastleFlag, out library)) return;

            library.Draw(GuildInfo.Flag * 100, DisplayArea.X + x, DisplayArea.Y + y, Color.White, true, 1F, ImageType.Image);
            library.Draw(GuildInfo.Flag * 100, DisplayArea.X + x, DisplayArea.Y + y, GuildInfo.Colour, true, 1F, ImageType.Overlay);
        }

        #endregion

        #region Castle Tab

        public void CreateCastleTab()
        {
            CastleTab = new DXTab
            {
                TabButton = { Label = { Text = CEnvir.Language.GuildDialogCastleTabLabel } },
                Parent = GuildTabs,
                BackColour = Color.Empty,
                Location = new Point(0, 23)
            };
            CastleTab.TabButton.MouseClick += (o, e) =>
            {
                BackgroundImage.Index = 264;
                CreatePanel.Visible = false;
                AddMemberPanel.Visible = false;
                TreasuryPanel.Visible = false;
                StoragePanel.Visible = false;
                WarPanel.Visible = false;
                CastlePanel.Visible = true;
            };

            CastlePanel = new DXControl
            {
                Parent = this,
                Location = new Point(10, 500),
                Size = new Size(436, 50),
                Border = false,
                Visible = true
            };

            ToggleGates = new DXButton
            {
                Parent = CastlePanel,
                Location = new Point(10, 10),
                ButtonType = ButtonType.Default,
                Size = new Size(120, DefaultHeight),
                Label = { Text = "Open/Close Gates" },
                Enabled = false,
                Visible = true
            };
            ToggleGates.MouseClick += (o, e) =>
            {
                CEnvir.Enqueue(new C.GuildToggleCastleGates());
            };

            RepairGates = new DXButton
            {
                Parent = CastlePanel,
                Location = new Point(220, 10),
                ButtonType = ButtonType.Default,
                Size = new Size(100, DefaultHeight),
                Label = { Text = "Repair Gates" },
                Enabled = false,
                Visible = true
            };
            RepairGates.MouseClick += (o, e) =>
            {
                var castle = GameScene.Game.CastleOwners.Single(x => x.Value == GuildInfo.GuildName).Key;
                var cost = castle.Gates.Sum(x => x.RepairCost);

                DXMessageBox box = new DXMessageBox(string.Format(CEnvir.Language.GuildRepairGatesConfirmMsg, cost), CEnvir.Language.GuildRepairGatesConfirmCaption, DXMessageBoxButtons.YesNo);

                box.YesButton.MouseClick += (o1, e1) =>
                {
                    CEnvir.Enqueue(new C.GuildRepairCastleGates());
                };
            };

            RepairGuards = new DXButton
            {
                Parent = CastlePanel,
                Location = new Point(330, 10),
                ButtonType = ButtonType.Default,
                Size = new Size(100, DefaultHeight),
                Label = { Text = "Repair Guards" },
                Enabled = false,
                Visible = true
            };
            RepairGuards.MouseClick += (o, e) =>
            {
                var castle = GameScene.Game.CastleOwners.Single(x => x.Value == GuildInfo.GuildName).Key;
                var cost = castle.Guards.Sum(x => x.RepairCost);

                DXMessageBox box = new DXMessageBox(string.Format(CEnvir.Language.GuildRepairGuardsConfirmMsg, cost), CEnvir.Language.GuildRepairGuardsConfirmCaption, DXMessageBoxButtons.YesNo);

                box.YesButton.MouseClick += (o1, e1) =>
                {
                    CEnvir.Enqueue(new C.GuildRepairCastleGuards());
                };
            };

            //int count = 0;
            //foreach (CastleInfo castle in CEnvir.CastleInfoList.Binding)
            //{
            //    CastlePanels[castle] = new GuildCastlePanel
            //    {
            //        Parent = WarTab,
            //        Castle = castle,
            //        Location = new Point(14, (142 * count) + 7),
            //        Visible = true
            //    };
            //    count++;
            //}

        }

        #endregion

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (GuildTabs != null)
                {
                    if (!GuildTabs.IsDisposed)
                        GuildTabs.Dispose();

                    GuildTabs = null;
                }

                if (BackgroundImage != null)
                {
                    if (!BackgroundImage.IsDisposed)
                        BackgroundImage.Dispose();

                    BackgroundImage = null;
                }

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

                #region Create Tab

                if (CreateTab != null)
                {
                    if (!CreateTab.IsDisposed)
                        CreateTab.Dispose();

                    CreateTab = null;
                }

                if (TreasuryPanel != null)
                {
                    if (!TreasuryPanel.IsDisposed)
                        TreasuryPanel.Dispose();

                    TreasuryPanel = null;
                }

                if (GuildNameBox != null)
                {
                    if (!GuildNameBox.IsDisposed)
                        GuildNameBox.Dispose();

                    GuildNameBox = null;
                }

                if (MemberTextBox != null)
                {
                    if (!MemberTextBox.IsDisposed)
                        MemberTextBox.Dispose();

                    MemberTextBox = null;
                }

                if (StorageTextBox != null)
                {
                    if (!StorageTextBox.IsDisposed)
                        StorageTextBox.Dispose();

                    StorageTextBox = null;
                }

                if (TotalCostBox != null)
                {
                    if (!TotalCostBox.IsDisposed)
                        TotalCostBox.Dispose();

                    TotalCostBox = null;
                }

                if (GoldCheckBox != null)
                {
                    if (!GoldCheckBox.IsDisposed)
                        GoldCheckBox.Dispose();

                    GoldCheckBox = null;
                }

                if (HornCheckBox != null)
                {
                    if (!HornCheckBox.IsDisposed)
                        HornCheckBox.Dispose();

                    HornCheckBox = null;
                }

                _MemberLimit = 0;
                MemberLimitChanged = null;

                _StorageSize = 0;
                StorageSizeChanged = null;

                _GuildNameValid = false;
                GuildNameValidChanged = null;

                _CreateAttempted = false;
                CreateAttemptedChanged = null;

                if (CreateButton != null)
                {
                    if (!CreateButton.IsDisposed)
                        CreateButton.Dispose();

                    CreateButton = null;
                }

                #endregion

                #region Home Tab

                if (HomeTab != null)
                {
                    if (!HomeTab.IsDisposed)
                        HomeTab.Dispose();

                    HomeTab = null;
                }

                if (SetTaxButton != null)
                {
                    if (!SetTaxButton.IsDisposed)
                        SetTaxButton.Dispose();

                    SetTaxButton = null;
                }

                if (MemberLimitLabel != null)
                {
                    if (!MemberLimitLabel.IsDisposed)
                        MemberLimitLabel.Dispose();

                    MemberLimitLabel = null;
                }

                if (GuildFundLabel != null)
                {
                    if (!GuildFundLabel.IsDisposed)
                        GuildFundLabel.Dispose();

                    GuildFundLabel = null;
                }

                if (DailyGrowthLabel != null)
                {
                    if (!DailyGrowthLabel.IsDisposed)
                        DailyGrowthLabel.Dispose();

                    DailyGrowthLabel = null;
                }
                
                if (TotalContributionLabel != null)
                {
                    if (!TotalContributionLabel.IsDisposed)
                        TotalContributionLabel.Dispose();

                    TotalContributionLabel = null;
                }

                if (DailyContributionLabel != null)
                {
                    if (!DailyContributionLabel.IsDisposed)
                        DailyContributionLabel.Dispose();

                    DailyContributionLabel = null;
                }

                if (GuildTaxLabel != null)
                {
                    if (!GuildTaxLabel.IsDisposed)
                        GuildTaxLabel.Dispose();

                    GuildTaxLabel = null;
                }

                if (NoticeScrollBar != null)
                {
                    if (!NoticeScrollBar.IsDisposed)
                        NoticeScrollBar.Dispose();

                    NoticeScrollBar = null;
                }

                if (NoticeTextBox != null)
                {
                    if (!NoticeTextBox.IsDisposed)
                        NoticeTextBox.Dispose();

                    NoticeTextBox = null;
                }

                if (EditNoticeButton != null)
                {
                    if (!EditNoticeButton.IsDisposed)
                        EditNoticeButton.Dispose();

                    EditNoticeButton = null;
                }

                if (SaveNoticeButton != null)
                {
                    if (!SaveNoticeButton.IsDisposed)
                        SaveNoticeButton.Dispose();

                    SaveNoticeButton = null;
                }

                if (CancelNoticeButton != null)
                {
                    if (!CancelNoticeButton.IsDisposed)
                        CancelNoticeButton.Dispose();

                    CancelNoticeButton = null;
                }

                #endregion

                #region Member Tab

                if (MemberTab != null)
                {
                    if (!MemberTab.IsDisposed)
                        MemberTab.Dispose();

                    MemberTab = null;
                }

                if (AddMemberPanel != null)
                {
                    if (!AddMemberPanel.IsDisposed)
                        AddMemberPanel.Dispose();

                    AddMemberPanel = null;
                }

                if (AddMemberButton != null)
                {
                    if (!AddMemberButton.IsDisposed)
                        AddMemberButton.Dispose();

                    AddMemberButton = null;
                }

                if (EditDefaultMemberButton != null)
                {
                    if (!EditDefaultMemberButton.IsDisposed)
                        EditDefaultMemberButton.Dispose();

                    EditDefaultMemberButton = null;
                }

                if (IncreaseMemberButton != null)
                {
                    if (!IncreaseMemberButton.IsDisposed)
                        IncreaseMemberButton.Dispose();

                    IncreaseMemberButton = null;
                }

                if (MemberRows != null)
                {
                    for (int i = 0; i < MemberRows.Length; i++)
                    {
                        if (MemberRows[i] != null)
                        {
                            if (!MemberRows[i].IsDisposed)
                                MemberRows[i].Dispose();

                            MemberRows[i] = null;
                        }
                    }
                    MemberRows = null;
                }

                if (MemberScrollBar != null)
                {
                    if (!MemberScrollBar.IsDisposed)
                        MemberScrollBar.Dispose();

                    MemberScrollBar = null;
                }

                #endregion

                #region Storage Tab

                if (StorageTab != null)
                {
                    if (!StorageTab.IsDisposed)
                        StorageTab.Dispose();

                    StorageTab = null;
                }

                if (StoragePanel != null)
                {
                    if (!StoragePanel.IsDisposed)
                        StoragePanel.Dispose();

                    StoragePanel = null;
                }

                if (IncreaseStorageButton != null)
                {
                    if (!IncreaseStorageButton.IsDisposed)
                        IncreaseStorageButton.Dispose();

                    IncreaseStorageButton = null;
                }

                if (ItemNameTextBox != null)
                {
                    if (!ItemNameTextBox.IsDisposed)
                        ItemNameTextBox.Dispose();

                    ItemNameTextBox = null;
                }

                if (ItemTypeComboBox != null)
                {
                    if (!ItemTypeComboBox.IsDisposed)
                        ItemTypeComboBox.Dispose();

                    ItemTypeComboBox = null;
                }

                if (StorageGrid != null)
                {
                    if (!StorageGrid.IsDisposed)
                        StorageGrid.Dispose();

                    StorageGrid = null;
                }

                if (ClearButton != null)
                {
                    if (!ClearButton.IsDisposed)
                        ClearButton.Dispose();

                    ClearButton = null;
                }

                if (StorageScrollBar != null)
                {
                    if (!StorageScrollBar.IsDisposed)
                        StorageScrollBar.Dispose();

                    StorageScrollBar = null;
                }

                if (GuildStorage != null)
                {
                    for (int i = 0; i < GuildStorage.Length; i++)
                    {
                        GuildStorage[i] = null;
                    }

                    StorageTab = null;
                }

                #endregion
                
                #region War Tab

                if (WarTab != null)
                {
                    if (!WarTab.IsDisposed)
                        WarTab.Dispose();

                    WarTab = null;
                }

                #endregion

                #region StyleTab

                if (StyleTab != null)
                {
                    if (!StyleTab.IsDisposed)
                        StyleTab.Dispose();

                    StyleTab = null;
                }

                if (StyleColourPanel != null)
                {
                    if (!StyleColourPanel.IsDisposed)
                        StyleColourPanel.Dispose();

                    StyleColourPanel = null;
                }

                if (StyleFlagPanel != null)
                {
                    if (!StyleFlagPanel.IsDisposed)
                        StyleFlagPanel.Dispose();

                    StyleFlagPanel = null;
                }

                if (StyleColourButton != null)
                {
                    if (!StyleColourButton.IsDisposed)
                        StyleColourButton.Dispose();

                    StyleColourButton = null;
                }

                if (StyleFlagPreviousButton != null)
                {
                    if (!StyleFlagPreviousButton.IsDisposed)
                        StyleFlagPreviousButton.Dispose();

                    StyleFlagPreviousButton = null;
                }

                if (StyleFlagNextButton != null)
                {
                    if (!StyleFlagNextButton.IsDisposed)
                        StyleFlagNextButton.Dispose();

                    StyleFlagNextButton = null;
                }

                if (StyleFlagLabel != null)
                {
                    if (!StyleFlagLabel.IsDisposed)
                        StyleFlagLabel.Dispose();

                    StyleFlagLabel = null;
                }

                if (StyleColourLabel != null)
                {
                    if (!StyleColourLabel.IsDisposed)
                        StyleColourLabel.Dispose();

                    StyleColourLabel = null;
                }

                if (StyleColour != null)
                {
                    if (!StyleColour.IsDisposed)
                        StyleColour.Dispose();

                    StyleColour = null;
                }

                #endregion

                #region Castle Tab

                if (CastleTab != null)
                {
                    if (!CastleTab.IsDisposed)
                        CastleTab.Dispose();

                    CastleTab = null;
                }

                if (ToggleGates != null)
                {
                    if (!ToggleGates.IsDisposed)
                        ToggleGates.Dispose();

                    ToggleGates = null;
                }

                if (RepairGates != null)
                {
                    if (!RepairGates.IsDisposed)
                        RepairGates.Dispose();

                    RepairGates = null;
                }

                if (RepairGuards != null)
                {
                    if (!RepairGuards.IsDisposed)
                        RepairGuards.Dispose();

                    RepairGuards = null;
                }

                #endregion

                _GuildInfo = null;
                GuildInfoChanged = null;
            }
        }

        #endregion

    }

    public sealed class GuildMemberRow : DXControl
    {
        #region Properties

        public DXLabel NameLabel, RankLabel, TotalLabel, DailyLabel, OnlineLabel;

        #region IsHeader

        public bool IsHeader
        {
            get => _IsHeader;
            set
            {
                if (_IsHeader == value) return;

                bool oldValue = _IsHeader;
                _IsHeader = value;

                OnIsHeaderChanged(oldValue, value);
            }
        }
        private bool _IsHeader;
        public event EventHandler<EventArgs> IsHeaderChanged;
        public void OnIsHeaderChanged(bool oValue, bool nValue)
        {
            NameLabel.Text = CEnvir.Language.GuildMemberRowNameLabel;
            NameLabel.ForeColour = Color.FromArgb(198, 166, 99);

            RankLabel.Text = CEnvir.Language.GuildMemberRowRankLabel;
            RankLabel.ForeColour = Color.FromArgb(198, 166, 99);

            TotalLabel.Text = CEnvir.Language.GuildMemberRowTotalLabel;
            TotalLabel.ForeColour = Color.FromArgb(198, 166, 99);

            DailyLabel.Text = CEnvir.Language.GuildMemberRowDailyLabel;
            DailyLabel.ForeColour = Color.FromArgb(198, 166, 99);

            OnlineLabel.Text = CEnvir.Language.GuildMemberRowOnlineLabel;
            OnlineLabel.ForeColour = Color.FromArgb(198, 166, 99);

            DrawTexture = false;

            IsHeaderChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region MemberInfo

        public ClientGuildMemberInfo MemberInfo
        {
            get => _MemberInfo;
            set
            {
                ClientGuildMemberInfo oldValue = _MemberInfo;
                _MemberInfo = value;

                OnMemberInfoChanged(oldValue, value);
            }
        }
        private ClientGuildMemberInfo _MemberInfo;
        public event EventHandler<EventArgs> MemberInfoChanged;
        public void OnMemberInfoChanged(ClientGuildMemberInfo oValue, ClientGuildMemberInfo nValue)
        {
            Visible = MemberInfo != null;

            if (MemberInfo == null) return;

            NameLabel.Text = MemberInfo.Name;
            RankLabel.Text = MemberInfo.Rank;
            TotalLabel.Text = MemberInfo.TotalContribution.ToString("#,##0");
            DailyLabel.Text = MemberInfo.DailyContribution.ToString("#,##0");

            OnlineLabel.Text = MemberInfo.Rank;


            MemberInfoChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #endregion

        public GuildMemberRow()
        {
            Size = new Size(402, 20);

            DrawTexture = true;
            BackColour = /*Selected ? Color.FromArgb(80, 80, 125) :*/ Color.FromArgb(25, 20, 0);

            NameLabel = new DXLabel
            {
                IsControl = false,
                Parent = this,
                Location = new Point(10, 2),
                ForeColour = Color.White,
            };

            RankLabel = new DXLabel
            {
                IsControl = false,
                Parent = this,
                Location = new Point(110, 2),
                ForeColour = Color.White,
            };


            TotalLabel = new DXLabel
            {
                IsControl = false,
                Parent = this,
                Location = new Point(210, 2),
                ForeColour = Color.White,
            };


            DailyLabel = new DXLabel
            {
                IsControl = false,
                Parent = this,
                Location = new Point(310, 2),
                ForeColour = Color.White,
            };


            OnlineLabel = new DXLabel
            {
                IsControl = false,
                Parent = this,
                Location = new Point(400, 2),
                ForeColour = Color.White,
            };
        }

        #region Methods

        public override void Process()
        {
            base.Process();

            if (MemberInfo == null) return;

            if (MemberInfo.LastOnline == DateTime.MaxValue)
            {
                OnlineLabel.Text = "Online";
                OnlineLabel.ForeColour = Color.Green;
            }
            else
            {
                OnlineLabel.Text = Functions.ToString(CEnvir.Now - MemberInfo.LastOnline, true, true);
                OnlineLabel.ForeColour = Color.White;
            }

        }

        public override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            switch (e.Button)
            {
                case MouseButtons.Left:

                    if (!Enabled || MemberInfo == null || (GameScene.Game.GuildBox.GuildInfo.Permission & GuildPermission.Leader) != GuildPermission.Leader) return;

                    GameScene.Game.GuildMemberBox.MemberNameLabel.Text = MemberInfo.Name;
                    GameScene.Game.GuildMemberBox.RankTextBox.TextBox.Text = MemberInfo.Rank;
                    GameScene.Game.GuildMemberBox.Permission = MemberInfo.Permission;
                    GameScene.Game.GuildMemberBox.MemberIndex = MemberInfo.Index;

                    GameScene.Game.GuildMemberBox.Visible = true;
                    GameScene.Game.GuildMemberBox.BringToFront();
                    break;
                case MouseButtons.Right:
                    if (MemberInfo == null || MemberInfo.ObjectID == MapObject.User.ObjectID) return;

                    GameScene.Game.BigMapBox.Visible = true;
                    GameScene.Game.BigMapBox.Opacity = 1F;
                    
                    if (!GameScene.Game.DataDictionary.TryGetValue(MemberInfo.ObjectID, out ClientObjectData data)) return;

                    GameScene.Game.BigMapBox.SelectedInfo = Globals.MapInfoList.Binding.FirstOrDefault(x => x.Index == data.MapIndex);
                    break;
                case MouseButtons.Middle:
                    if (MemberInfo == null) return;

                    CEnvir.Enqueue(new C.GroupInvite { Name = MemberInfo.Name });
                    break;
            }
        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (NameLabel != null)
                {
                    if (!NameLabel.IsDisposed)
                        NameLabel.Dispose();

                    NameLabel = null;
                }

                if (RankLabel != null)
                {
                    if (!RankLabel.IsDisposed)
                        RankLabel.Dispose();

                    RankLabel = null;
                }

                if (TotalLabel != null)
                {
                    if (!TotalLabel.IsDisposed)
                        TotalLabel.Dispose();

                    TotalLabel = null;
                }

                if (DailyLabel != null)
                {
                    if (!DailyLabel.IsDisposed)
                        DailyLabel.Dispose();

                    DailyLabel = null;
                }

                if (OnlineLabel != null)
                {
                    if (!OnlineLabel.IsDisposed)
                        OnlineLabel.Dispose();

                    OnlineLabel = null;
                }
            }

        }

        #endregion
    }
    
    public sealed class GuildMemberDialog : DXWindow
    {
        #region Properties

        public DXLabel MemberNameLabel;

        public DXTextBox RankTextBox;

        public DXCheckBox LeaderBox, EditNoticeBox, AddMemberBox, StorageBox, RepairBox, MerchantBox, MarketBox, StartWarBox;
        
        public DXButton ConfirmButton, KickButton;

        #region MemberIndex

        public int MemberIndex
        {
            get => _MemberIndex;
            set
            {
                if (_MemberIndex == value) return;

                int oldValue = _MemberIndex;
                _MemberIndex = value;

                OnMemberIndexChanged(oldValue, value);
            }
        }
        private int _MemberIndex;
        public event EventHandler<EventArgs> MemberIndexChanged;
        public void OnMemberIndexChanged(int oValue, int nValue)
        {
            MemberIndexChanged?.Invoke(this, EventArgs.Empty);

            KickButton.Visible = MemberIndex > 0;

            LeaderBox.Enabled = MemberIndex != GameScene.Game.GuildBox.GuildInfo.UserIndex;
            EditNoticeBox.Enabled = MemberIndex != GameScene.Game.GuildBox.GuildInfo.UserIndex;
            AddMemberBox.Enabled = MemberIndex != GameScene.Game.GuildBox.GuildInfo.UserIndex;
            StorageBox.Enabled = MemberIndex != GameScene.Game.GuildBox.GuildInfo.UserIndex;
            RepairBox.Enabled = MemberIndex != GameScene.Game.GuildBox.GuildInfo.UserIndex;
            MerchantBox.Enabled = MemberIndex != GameScene.Game.GuildBox.GuildInfo.UserIndex;
            MarketBox.Enabled = MemberIndex != GameScene.Game.GuildBox.GuildInfo.UserIndex;


            KickButton.Enabled = MemberIndex != GameScene.Game.GuildBox.GuildInfo.UserIndex;
        }

        #endregion

        #region Permission

        private bool Updating;

        public GuildPermission Permission
        {
            get => _Permission;
            set
            {

                GuildPermission oldValue = _Permission;
                _Permission = value;

                OnPermissionChanged(oldValue, value);
            }
        }
        private GuildPermission _Permission;
        public event EventHandler<EventArgs> PermissionChanged;
        public void OnPermissionChanged(GuildPermission oValue, GuildPermission nValue)
        {
            if (Updating) return;

            Updating = true;
            LeaderBox.Checked = (Permission & GuildPermission.Leader) == GuildPermission.Leader;
            EditNoticeBox.Checked = (Permission & GuildPermission.EditNotice) == GuildPermission.EditNotice;
            AddMemberBox.Checked = (Permission & GuildPermission.AddMember) == GuildPermission.AddMember;
            StorageBox.Checked = (Permission & GuildPermission.Storage) == GuildPermission.Storage;
            RepairBox.Checked = (Permission & GuildPermission.FundsRepair) == GuildPermission.FundsRepair;
            MerchantBox.Checked = (Permission & GuildPermission.FundsMerchant) == GuildPermission.FundsMerchant;
            MarketBox.Checked = (Permission & GuildPermission.FundsMarket) == GuildPermission.FundsMarket;
            StartWarBox.Checked = (Permission & GuildPermission.StartWar) == GuildPermission.StartWar;

            Updating = false;
            PermissionChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion


        public override WindowType Type => WindowType.GuildMemberBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        public GuildMemberDialog()
        {
            SetClientSize(new Size(200, 160));

            TitleLabel.Text = CEnvir.Language.GuildMemberDialogTitle;

            DXLabel label = new DXLabel
            {
                Parent = this,
                Text = CEnvir.Language.GuildMemberDialogMemberLabel,
            };
            label.Location = new Point(080 - label.Size.Width, ClientArea.Y);


            MemberNameLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(080, label.Location.Y),
                ForeColour = Color.White,
            };


            label = new DXLabel
            {
                Parent = this,
                Text = CEnvir.Language.GuildMemberDialogRankLabel,
            };
            label.Location = new Point(080 - label.Size.Width, MemberNameLabel.Location.Y + 20);

            RankTextBox = new DXTextBox
            {
                Parent = this,
                Location = new Point(080, label.Location.Y), 
                Size = new Size(120, 20),
                MaxLength = Globals.MaxCharacterNameLength
            };

            LeaderBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = CEnvir.Language.GuildMemberDialogLeaderLabel },
            };
            LeaderBox.CheckedChanged += (o, e) => UpdatePermission();
            LeaderBox.Location = new Point(094 - LeaderBox.Size.Width, RankTextBox.Location.Y + 24);

            EditNoticeBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = CEnvir.Language.GuildMemberDialogEditNoticeLabel },
            };
            EditNoticeBox.CheckedChanged += (o, e) => UpdatePermission();
            EditNoticeBox.Location = new Point(094 - EditNoticeBox.Size.Width, LeaderBox.Location.Y + 20);

            AddMemberBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = CEnvir.Language.GuildMemberDialogAddMemberLabel },
            };
            AddMemberBox.CheckedChanged += (o, e) => UpdatePermission();
            AddMemberBox.Location = new Point(094 - AddMemberBox.Size.Width, EditNoticeBox.Location.Y + 20);
            
            StorageBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = CEnvir.Language.GuildMemberDialogUseStorageLabel },
            };
            StorageBox.CheckedChanged += (o, e) => UpdatePermission();
            StorageBox.Location = new Point(094 - StorageBox.Size.Width, AddMemberBox.Location.Y + 20);

            StartWarBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = CEnvir.Language.GuildMemberDialogStartWarLabel },
            };
            StartWarBox.CheckedChanged += (o, e) => UpdatePermission();
            StartWarBox.Location = new Point(094 - StartWarBox.Size.Width, StorageBox.Location.Y + 20);

            RepairBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = CEnvir.Language.GuildMemberDialogRepairLabel },
            };
            RepairBox.CheckedChanged += (o, e) => UpdatePermission();
            RepairBox.Location = new Point(0200 - RepairBox.Size.Width, EditNoticeBox.Location.Y );


            MerchantBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = CEnvir.Language.GuildMemberDialogMerchantLabel },
            };
            MerchantBox.CheckedChanged += (o, e) => UpdatePermission();
            MerchantBox.Location = new Point(0200 - MerchantBox.Size.Width, RepairBox.Location.Y + 20);

            MarketBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = CEnvir.Language.GuildMemberDialogMarketLabel },
            };
            MarketBox.CheckedChanged += (o, e) => UpdatePermission();
            MarketBox.Location = new Point(0200 - MarketBox.Size.Width, MerchantBox.Location.Y + 20);


            ConfirmButton = new DXButton
            {
                Parent = this,
                Location = new Point(0120, StorageBox.Location.Y + 40),
                ButtonType = ButtonType.SmallButton,
                Size = new Size(80, SmallButtonHeight),
                Label = { Text = CEnvir.Language.CommonControlConfirm },
            };

            ConfirmButton.MouseClick += (o, e) =>
            {
                CEnvir.Enqueue(new C.GuildEditMember { Index = MemberIndex, Permission = Permission, Rank = RankTextBox.TextBox.Text });

                Visible = false;
            };
            KickButton = new DXButton
            {
                Parent = this,
                Location = new Point(ClientArea.X, StorageBox.Location.Y + 40),
                ButtonType = ButtonType.SmallButton,
                Size = new Size(40, SmallButtonHeight),
                Label = { Text = CEnvir.Language.GuildMemberDialogKickButtonLabel },
            };
            KickButton.MouseClick += (o, e) =>
            {
                DXMessageBox box = new DXMessageBox(string.Format(CEnvir.Language.GuildMemberDialogKickButtonConfirmMsg, MemberNameLabel.Text), CEnvir.Language.GuildMemberDialogKickButtonConfirmCaption, DXMessageBoxButtons.YesNo);
                
                box.YesButton.MouseClick += (o1, e1) =>
                {
                    CEnvir.Enqueue(new C.GuildKickMember { Index = MemberIndex });
                    Visible = false;
                };
            };

        }

        #region Methods

        private void UpdatePermission()
        {
            if (Updating) return;

            GuildPermission permission = GuildPermission.None;

            if (LeaderBox.Checked)
                permission |= GuildPermission.Leader;

            if (EditNoticeBox.Checked)
                permission |= GuildPermission.EditNotice;

            if (AddMemberBox.Checked)
                permission |= GuildPermission.AddMember;

            if (StorageBox.Checked)
                permission |= GuildPermission.Storage;

            if (RepairBox.Checked)
                permission |= GuildPermission.FundsRepair;

            if (MerchantBox.Checked)
                permission |= GuildPermission.FundsMerchant;

            if (MarketBox.Checked)
                permission |= GuildPermission.FundsMarket;

            if (StartWarBox.Checked)
                permission |= GuildPermission.StartWar;


            Permission = permission;
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _MemberIndex = 0;
                MemberIndexChanged = null;

                _Permission = 0;
                PermissionChanged = null;

                if (MemberNameLabel != null)
                {
                    if (!MemberNameLabel.IsDisposed)
                        MemberNameLabel.Dispose();

                    MemberNameLabel = null;
                }

                if (RankTextBox != null)
                {
                    if (!RankTextBox.IsDisposed)
                        RankTextBox.Dispose();

                    RankTextBox = null;
                }

                if (LeaderBox != null)
                {
                    if (!LeaderBox.IsDisposed)
                        LeaderBox.Dispose();

                    LeaderBox = null;
                }

                if (EditNoticeBox != null)
                {
                    if (!EditNoticeBox.IsDisposed)
                        EditNoticeBox.Dispose();

                    EditNoticeBox = null;
                }

                if (AddMemberBox != null)
                {
                    if (!AddMemberBox.IsDisposed)
                        AddMemberBox.Dispose();

                    AddMemberBox = null;
                }

                if (StorageBox != null)
                {
                    if (!StorageBox.IsDisposed)
                        StorageBox.Dispose();

                    StorageBox = null;
                }

                if (RepairBox != null)
                {
                    if (!RepairBox.IsDisposed)
                        RepairBox.Dispose();

                    RepairBox = null;
                }

                if (MerchantBox != null)
                {
                    if (!MerchantBox.IsDisposed)
                        MerchantBox.Dispose();

                    MerchantBox = null;
                }

                if (MarketBox != null)
                {
                    if (!MarketBox.IsDisposed)
                        MarketBox.Dispose();

                    MarketBox = null;
                }

                if (ConfirmButton != null)
                {
                    if (!ConfirmButton.IsDisposed)
                        ConfirmButton.Dispose();

                    ConfirmButton = null;
                }

                if (KickButton != null)
                {
                    if (!KickButton.IsDisposed)
                        KickButton.Dispose();

                    KickButton = null;
                }
            }

        }

        #endregion
    }

    public sealed class GuildCastlePanel : DXControl
    {
        #region Castle

        public CastleInfo Castle
        {
            get { return _Castle; }
            set
            {
                if (_Castle == value) return;

                CastleInfo oldValue = _Castle;
                _Castle = value;

                OnCastleChanged(oldValue, value);
            }
        }
        private CastleInfo _Castle;
        public event EventHandler<EventArgs> CastleChanged;
        public void OnCastleChanged(CastleInfo oValue, CastleInfo nValue)
        {
            CastleChanged?.Invoke(this, EventArgs.Empty);

            if (Castle == null) return;

            CastleNameLabel.Text = Castle.Name;
            ItemLabel.Text = Castle.Item?.ItemName ?? "None";
        }

        #endregion

        public DXLabel CastleNameLabel, CastleOwnerLabel, CastleDateLabel, ItemLabel;

        public DXButton RequestButton;

        public GuildCastlePanel()
        {
            Size = new Size(428, 133);

            Border = false;

            AfterDraw += GuildCastlePanel_AfterDraw;

            CastleNameLabel =  new DXLabel
            {
                AutoSize =  false,
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Size = new Size(348, 30),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
            };

            DXLabel label = new DXLabel
            {
                Parent = this,
                Text = CEnvir.Language.GuildCastlePanelOwnerLabel,
            };
            label.Location = new Point(110 - label.Size.Width, 35);

            CastleOwnerLabel = new DXLabel
            {
                Parent = this,
                Text = CEnvir.Language.GuildCastlePanelNoneText,
                Location = new Point(110, 35),
                ForeColour = Color.White
            };

            label = new DXLabel
            {
                Parent = this,
                Text = CEnvir.Language.GuildCastlePanelScheduleLabel,
            };
            label.Location = new Point(110 - label.Size.Width, 60);

            CastleDateLabel = new DXLabel
            {
                Parent = this,
                Text = CEnvir.Language.GuildCastlePanelNoneText,
                Location = new Point(110, 60),
                ForeColour = Color.White
            };

            RequestButton = new DXButton
            {
                Parent = this,
                Location = new Point(359, 85),
                ButtonType = ButtonType.Default,
                Size = new Size(60, DefaultHeight),
                Label = { Text = CEnvir.Language.GuildCastlePanelRequestButtonLabel },
                Enabled = false,
            };
            RequestButton.MouseClick += (o, e) => CEnvir.Enqueue(new C.GuildRequestConquest { Index = Castle.Index });

            label = new DXLabel
            {
                Parent = this,
                Text = CEnvir.Language.GuildCastlePanelCostLabel,
            };
            label.Location = new Point(110 - label.Size.Width, 110);

            ItemLabel = new DXLabel
            {
                Parent = this,
                Text = CEnvir.Language.GuildCastlePanelNoneText,
                Location = new Point(110, 110),
                ForeColour = Color.White
            };
        }

        private void GuildCastlePanel_AfterDraw(object sender, EventArgs e)
        {
            if (CEnvir.LibraryList.TryGetValue(LibraryFile.Inventory, out MirLibrary library))
            {
                if (library != null && Castle.Item != null)
                {
                    var s = library.GetSize(Castle.Item.Image);
                    var x = (36 - s.Width) / 2 + DisplayArea.X;
                    var y = (36 - s.Height) / 2 + DisplayArea.Y;

                    library.Draw(Castle.Item.Image, x + 372, y + 18, Color.White, false, 1F, ImageType.Image);
                }
            }
        }

        public void Update()
        {
            string owner = GameScene.Game.CastleOwners[Castle];

            CastleOwnerLabel.Text = string.IsNullOrEmpty(owner) ? CEnvir.Language.GuildCastlePanelNoneText : owner;
        }

        public override void Process()
        {
            base.Process();

            if (Castle.WarDate == DateTime.MinValue)
                CastleDateLabel.Text = CEnvir.Language.GuildCastlePanelNoneText;
            else if (Castle.WarDate <= CEnvir.Now)
                CastleDateLabel.Text = CEnvir.Language.GuildCastlePanelInProgressText;
            else
                CastleDateLabel.Text = Functions.ToString(Castle.WarDate - CEnvir.Now, true);
        }
    }
}
