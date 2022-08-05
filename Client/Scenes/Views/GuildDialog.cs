using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Client.Controls;
using Client.Envir;
using Client.Models;
using Client.UserModels;
using Library;
using Library.SystemModels;
using C = Library.Network.ClientPackets;

//Cleaned
namespace Client.Scenes.Views
{
    public sealed class GuildDialog : DXWindow
    {
        #region Properties

        private DXTabControl GuildTabs;

        #region CreateTab

        private DXTab CreateTab;

        public DXTextBox GuildNameBox;
        public DXNumberTextBox MemberTextBox, StorageTextBox, TotalCostBox;

        public DXCheckBox GoldCheckBox, HornCheckBox;

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
        public DXLabel MemberLimitLabel, GuildFundLabel, DailyGrowthLabel, TotalContributionLabel, DailyContributionLabel;

        public DXVScrollBar NoticeScrollBar;
        public DXTextBox NoticeTextBox;
        public DXButton EditNoticeButton, SaveNoticeButton, CancelNoticeButton;

        #endregion

        #region Member Tab

        private DXTab MemberTab;
        public GuildMemberRow[] MemberRows;
        public DXVScrollBar MemberScrollBar;

        #endregion

        #region Storage Tab

        public DXTab StorageTab;
        public DXTextBox ItemNameTextBox;
        public DXComboBox ItemTypeComboBox;
        public DXItemGrid StorageGrid;
        public DXButton ClearButton;
        public DXVScrollBar StorageScrollBar;
        public ClientUserItem[] GuildStorage = new ClientUserItem[1000];

        #endregion
        
        #region Manage Tab

        private DXTab ManageTab;
        public DXLabel GuildFundLabel1, MemberLimitLabel1, StorageSizeLabel;
        public DXTextBox AddMemberTextBox;
        public DXNumberTextBox MemberTaxBox;
        public DXButton AddMemberButton, EditDefaultMemberButton, SetTaxButton, IncreaseMemberButton, IncreaseStorageButton, StartWarButton;
        public DXNumberTextBox GuildTaxBox;
        public DXControl AddMemberPanel, TreasuryPanel, UpgradePanel, GuildWarPanel;
        public DXTextBox GuildWarTextBox;

        public Dictionary<CastleInfo, GuildCastlePanel> CastlePanels = new Dictionary<CastleInfo, GuildCastlePanel>();

        #region GuildWarNameValid

        public bool GuildWarNameValid
        {
            get { return _GuildWarNameValid; }
            set
            {
                if (_GuildWarNameValid == value) return;

                bool oldValue = _GuildWarNameValid;
                _GuildWarNameValid = value;

                OnGuildWarNameValidChanged(oldValue, value);
            }
        }
        private bool _GuildWarNameValid;
        public event EventHandler<EventArgs> GuildWarNameValidChanged;
        public void OnGuildWarNameValidChanged(bool oValue, bool nValue)
        {
            GuildWarNameValidChanged?.Invoke(this, EventArgs.Empty);

            StartWarButton.Enabled = CanWar;
        }

        #endregion

        #region WarAttempted

        public bool WarAttempted
        {
            get { return _WarAttempted; }
            set
            {
                if (_WarAttempted == value) return;

                bool oldValue = _WarAttempted;
                _WarAttempted = value;

                OnWarAttemptedChanged(oldValue, value);
            }
        }
        private bool _WarAttempted;
        public event EventHandler<EventArgs> WarAttemptedChanged;
        public void OnWarAttemptedChanged(bool oValue, bool nValue)
        {
            WarAttemptedChanged?.Invoke(this, EventArgs.Empty);
            StartWarButton.Enabled = CanWar;
        }

        #endregion
        
        public bool CanWar => !WarAttempted && GuildWarNameValid;




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

        public override WindowType Type => WindowType.GuildBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => true;

        #endregion

        public GuildDialog()
        {
            TitleLabel.Text = "Guild";

            SetClientSize(new Size(516, 419));

            GuildTabs = new DXTabControl
            {
                Parent = this,
                Size = ClientArea.Size,
                Location = ClientArea.Location,
            };

            CreateCreateTab();

            CreateHomeTab();

            CreateMemberTab();

            CreateStorageTab();
            
            CreateManageTab();

            ClearGuild();
        }

        #region Methods

        private void ClearGuild()
        {
            CreateTab.TabButton.Visible = GuildInfo == null;

            HomeTab.TabButton.Visible = GuildInfo != null;
            MemberTab.TabButton.Visible = GuildInfo != null;
            StorageTab.TabButton.Visible = GuildInfo != null;
            ManageTab.TabButton.Visible = GuildInfo != null;

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

            AddMemberTextBox.TextBox.Text = string.Empty;
            GuildTaxBox.TextBox.Text = string.Empty;
            GuildFundLabel1.Text = string.Empty;
            MemberLimitLabel1.Text = string.Empty;
            StorageSizeLabel.Text = string.Empty;

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

            if (!NoticeTextBox.Editable)
                NoticeTextBox.TextBox.Text = GuildInfo.Notice;

            MemberLimitLabel.Text = $"{GuildInfo.Members.Count} / {GuildInfo.MemberLimit}";
            GuildFundLabel.Text = GuildInfo.GuildFunds.ToString("#,##0");
            DailyGrowthLabel.Text = GuildInfo.DailyGrowth.ToString("#,##0");
            TotalContributionLabel.Text = GuildInfo.TotalContribution.ToString("#,##0");
            DailyContributionLabel.Text = GuildInfo.DailyContribution.ToString("#,##0");

            UpdateMemberRows();

            PermissionChanged();

            ApplyStorageFilter();

            GuildFundLabel1.Text = GuildInfo.GuildFunds.ToString("#,##0");
            MemberLimitLabel1.Text = GuildInfo.MemberLimit.ToString();
            StorageSizeLabel.Text = GuildInfo.StorageLimit.ToString(); ;

            GuildTaxBox.Value = GuildInfo.Tax;
        }
        private void RefreshStorage()
        {
            StorageGrid.GridSize = new Size(14, Math.Max(20, (int)Math.Ceiling(GuildInfo.StorageLimit / (float)14)));

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
            UpgradePanel.Enabled = (GuildInfo.Permission & GuildPermission.Leader) == GuildPermission.Leader;
            GuildWarPanel.Enabled = (GuildInfo.Permission & GuildPermission.StartWar) == GuildPermission.StartWar;

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

        #region Create Tab

        public void CreateCreateTab()
        {
            CreateTab = new DXTab
            {
                TabButton = { Label = { Text = "Create" } },
                Parent = GuildTabs,
                Border = true,
            };

            DXLabel stepLabel = new DXLabel
            {
                Text = "Step 1 - Identity",
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
                Text = "Guild Name:",
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
                Text = "Step 2 - Payment",
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
                Label = { Text = $"{Globals.GuildCreationCost:#,##0} Gold", ForeColour = Color.White },
                Parent = CreateTab,
                Checked = true,
                ReadOnly = true,
            };
            GoldCheckBox.Location = new Point((CreateTab.Size.Width - GoldCheckBox.Size.Width) / 2, stepLabel.Location.Y + 30);
            GoldCheckBox.MouseClick += (o, e) => GoldCheckBox.Checked = true;


            HornCheckBox = new DXCheckBox
            {
                Label = { Text = "Uma King's Horn", ForeColour = Color.White },
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
                Text = "Step 3 - Options",
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
                Text = "Extra Members:",
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
                Hint = "Base Limit: 10" +
                       $"Cost per Member Slot: {Globals.GuildMemberCost}."
            };
            label.Location = new Point(MemberTextBox.Location.X + MemberTextBox.Size.Width, MemberTextBox.Location.Y + (MemberTextBox.Size.Height - label.Size.Height) / 2);

            label = new DXLabel
            {
                Text = "Extra Storage:",
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
                Hint = "Base Storage: 10," +
                       $"Cost per Storage Slot: {Globals.GuildStorageCost}."
            };
            label.Location = new Point(StorageTextBox.Location.X + StorageTextBox.Size.Width, StorageTextBox.Location.Y + (StorageTextBox.Size.Height - label.Size.Height) / 2);



            stepLabel = new DXLabel
            {
                Text = "Step 4 - Summary",
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
                Text = "Gold Cost:",
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
                Label = { Text = "Create Guild" },
                Location = new Point(TotalCostBox.Location.X, TotalCostBox.Location.Y + 30)
            };

            CreateButton.MouseClick += CreateButton_MouseClick;

            StarterGuildButton = new DXButton
            {
                Parent = CreateTab,
                ButtonType = ButtonType.SmallButton,
                Size = new Size(120, SmallButtonHeight),
                Label = { Text = "Join Starter Guild" },
                Location = new Point(ClientArea.Left, TotalCostBox.Location.Y + 40)
            };
            StarterGuildButton.MouseClick += StarterGuildButton_MouseClick;

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
                TabButton = { Label = { Text = "Home" } },
                Parent = GuildTabs,
                Border = true,
            };

             new DXLabel
            {
                Text = "Notice",
                Parent = HomeTab,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Size = new Size(HomeTab.Size.Width, 22),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
            };

            NoticeScrollBar = new DXVScrollBar
            {
                Parent = HomeTab,
                Location = new Point(HomeTab.Size.Width - 20, 25),
                Size = new Size(14, 275),
                VisibleSize = 17,
                Change = 1,
            };
            NoticeScrollBar.ValueChanged += (o, e) => SetLineIndex(NoticeScrollBar.Value);

            NoticeTextBox = new DXTextBox
            {
                Parent = HomeTab,
                TextBox = { Multiline = true },
                Location = new Point(5, 25),
                Size = new Size(HomeTab.Size.Width - 25, 275),
                KeepFocus = false,
                Editable = false,
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
                Location = new Point(HomeTab.Size.Width - 66, 4),
                Label = { Text = "Edit" },
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
                Location = new Point(HomeTab.Size.Width - 131, 4),
                Label = { Text = "Save" },
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
                Location = new Point(HomeTab.Size.Width - 66, 4),
                Label = { Text = "Cancel" },
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
                Location = new Point(5, NoticeTextBox.Size.Height + 5 + NoticeTextBox.Location.Y),
                Size = new Size(HomeTab.Size.Width - 11, 88),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99)
            };

            new DXLabel
            {
                Text = "Stats",
                Parent = panel,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Size = new Size(HomeTab.Size.Width, 22),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
            };

            DXLabel label = new DXLabel
            {
                Text = "Members:",
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
                Text = "Guild Funds:",
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
                Text = "Today's Growth:",
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
                Text = "Total Contibution:",
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
                Text = "Today's Contribution:",
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
                TabButton = { Label = { Text = "Members" } },
                Parent = GuildTabs,
                Border = true,
            };

            new GuildMemberRow
            {
                Location = new Point(5, 5),
                Parent = MemberTab,
                IsHeader = true,
            };


            MemberScrollBar = new DXVScrollBar
            {
                Parent = MemberTab,
                Location = new Point(MemberTab.Size.Width - 20, 29),
                Size = new Size(14, 363),
                VisibleSize = 16,
                Change = 1,
            };
            MemberScrollBar.ValueChanged += MemberScrollBar_ValueChanged;

            MemberRows = new GuildMemberRow[16];
            for (int i = 0; i < MemberRows.Length; i++)
            {
                MemberRows[i] = new GuildMemberRow
                {
                    Parent = MemberTab,
                    Location = new Point(5, 5 + i * 23 + 23),
                    Visible = false
                };

                MemberRows[i].MouseWheel += MemberScrollBar.DoMouseWheel;
            }
            MouseWheel += MemberScrollBar.DoMouseWheel;
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
                TabButton = { Label = { Text = "Storage" } },
                Parent = GuildTabs,
                Border = true,
            };

            DXControl filterPanel = new DXControl
            {
                Parent = StorageTab,
                Size = new Size(StorageTab.Size.Width - 13, 26),
                Location = new Point(6, 6),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99)
            };

            DXLabel label = new DXLabel
            {
                Parent = filterPanel,
                Location = new Point(5, 5),
                Text = "Name:",
            };

            ItemNameTextBox = new DXTextBox
            {
                Parent = filterPanel,
                Size = new Size(180, 20),
                Location = new Point(label.Location.X + label.Size.Width + 5, label.Location.Y),
            };
            ItemNameTextBox.TextBox.TextChanged += (o, e) => ApplyStorageFilter();

            label = new DXLabel
            {
                Parent = filterPanel,
                Location = new Point(ItemNameTextBox.Location.X + ItemNameTextBox.Size.Width + 10, 5),
                Text = "Item:",
            };



            ItemTypeComboBox = new DXComboBox
            {
                Parent = filterPanel,
                Location = new Point(label.Location.X + label.Size.Width + 5, label.Location.Y),
                Size = new Size(95, DXComboBox.DefaultNormalHeight),
                DropDownHeight = 198
            };
            ItemTypeComboBox.SelectedItemChanged += (o, e) => ApplyStorageFilter();

            new DXListBoxItem
            {
                Parent = ItemTypeComboBox.ListBox,
                Label = { Text = $"All" },
                Item = null
            };

            Type itemType = typeof(ItemType);

            for (ItemType i = ItemType.Nothing; i <= ItemType.ItemPart; i++)
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
                Size = new Size(80, SmallButtonHeight),
                Location = new Point(ItemTypeComboBox.Location.X + ItemTypeComboBox.Size.Width + 33, label.Location.Y - 1),
                Parent = filterPanel,
                ButtonType = ButtonType.SmallButton,
                Label = { Text = "Clear" }
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
                Location = new Point(5, 42),
                GridType = GridType.GuildStorage,
                ItemGrid = GuildStorage,
                VisibleHeight = 10,
            };
            StorageGrid.GridSizeChanged += StorageGrid_GridSizeChanged;




            StorageScrollBar = new DXVScrollBar
            {
                Parent = StorageTab,
                Location = new Point(StorageTab.Size.Width - 20, 43),
                Size = new Size(14, 349),
                VisibleSize = 10,
                Change = 1,
            };
            StorageScrollBar.ValueChanged += StorageScrollBar_ValueChanged;

        }

        private void StorageGrid_GridSizeChanged(object sender, EventArgs e)
        {
            foreach (DXItemCell cell in StorageGrid.Grid)
                cell.ItemChanged += (o, e1) => FilterCell(cell);
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
        
        #region Manage Tab

        public void CreateManageTab()
        {
            ManageTab = new DXTab
            {
                TabButton = { Label = { Text = "Manage" } },
                Parent = GuildTabs,
                Border = true,
            };

            AddMemberPanel = new DXControl
            {
                Parent = ManageTab,
                Location = new Point(5, 5),
                Size = new Size((HomeTab.Size.Width - 11 - 5) / 2, 83),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99)
            };

            new DXLabel
            {
                Text = "Membership",
                Parent = AddMemberPanel,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Size = new Size(AddMemberPanel.Size.Width, 22),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
            };


            DXLabel label = new DXLabel
            {
                Parent = AddMemberPanel,
                Text = "Member:",
            };
            label.Location = new Point(ClientArea.X + 55 - label.Size.Width, 32);

            AddMemberTextBox = new DXTextBox
            {
                Parent = AddMemberPanel,
                Location = new Point(ClientArea.X + 55, label.Location.Y),
                Size = new Size(110, 20),
                MaxLength = Globals.MaxCharacterNameLength
            };


            AddMemberButton = new DXButton
            {
                Parent = AddMemberPanel,
                Location = new Point(ClientArea.X + 170, label.Location.Y - 1),
                ButtonType = ButtonType.SmallButton,
                Size = new Size(60, SmallButtonHeight),
                Label = { Text = "Invite" },
            };
            AddMemberButton.MouseClick += (o, e) =>
            {
                CEnvir.Enqueue(new C.GuildInviteMember { Name = AddMemberTextBox.TextBox.Text });
                AddMemberButton.Enabled = false;
                AddMemberTextBox.Enabled = false;
            };

            EditDefaultMemberButton = new DXButton
            {
                Parent = AddMemberPanel,
                Location = new Point(ClientArea.X + 55, label.Location.Y + 25),
                ButtonType = ButtonType.SmallButton,
                Size = new Size(110, SmallButtonHeight),
                Label = { Text = "Default Rank" },
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


            TreasuryPanel = new DXControl
            {
                Parent = ManageTab,
                Location = new Point(5, 93),
                Size = new Size((HomeTab.Size.Width - 11 - 5) / 2, 73),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99)
            };

            new DXLabel
            {
                Text = "Treasury",
                Parent = TreasuryPanel,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Size = new Size(TreasuryPanel.Size.Width, 22),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
            };

            label = new DXLabel
            {
                Parent = TreasuryPanel,
                Text = "Guild Tax:",
            };
            label.Location = new Point(ClientArea.X + 55 - label.Size.Width, 32);

            GuildTaxBox = new DXNumberTextBox
            {
                Parent = TreasuryPanel,
                Location = new Point(ClientArea.X + 55, label.Location.Y),
                Size = new Size(60, 20),
                MaxValue = 100,
                MinValue = 0,
            };
            label = new DXLabel
            {
                Parent = TreasuryPanel,
                Text = "(Percent)",
                Location = new Point(ClientArea.X + 115, 32),
            };


            SetTaxButton = new DXButton
            {
                Parent = TreasuryPanel,
                Location = new Point(ClientArea.X + 170, label.Location.Y - 1),
                ButtonType = ButtonType.SmallButton,
                Size = new Size(60, SmallButtonHeight),
                Label = { Text = "Change" },
            };
            SetTaxButton.MouseClick += (o, e) =>
            {
                CEnvir.Enqueue(new C.GuildTax { Tax = GuildTaxBox.Value });

                GuildTaxBox.Enabled = false;
                SetTaxButton.Enabled = false;
            };

            label = new DXLabel
            {
                Parent = TreasuryPanel,
                Text = "Balance:",
            };
            label.Location = new Point(ClientArea.X + 55 - label.Size.Width, 54);

            GuildFundLabel1 = new DXLabel
            {
                Parent = TreasuryPanel,
                Text = "10,000,000,000",
                Location = new Point(ClientArea.X + 52, label.Location.Y),
                ForeColour = Color.White,
            };


            UpgradePanel = new DXControl
            {
                Parent = ManageTab,
                Location = new Point(5, 171),
                Size = new Size((HomeTab.Size.Width - 11 - 5) / 2, 90),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99)
            };

            new DXLabel
            {
                Text = "Upgrade",
                Parent = UpgradePanel,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Size = new Size(UpgradePanel.Size.Width, 22),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
            };

            label = new DXLabel
            {
                Parent = UpgradePanel,
                Text = "Members:",
            };
            label.Location = new Point(ClientArea.X + 55 - label.Size.Width, 35);

            IncreaseMemberButton = new DXButton
            {
                Parent = UpgradePanel,
                Location = new Point(ClientArea.X + 55, label.Location.Y - 1),
                ButtonType = ButtonType.SmallButton,
                Size = new Size(110, SmallButtonHeight),
                Label = { Text = $"Upgrade ({Globals.GuildMemberCost:#,##0})" },
            };
            IncreaseMemberButton.MouseClick += (o, e) =>
            {
                CEnvir.Enqueue(new C.GuildIncreaseMember());
                IncreaseMemberButton.Enabled = false;
            };

            DXLabel label1 = new DXLabel
            {
                Parent = UpgradePanel,
                Text = "Limit:",
            };
            label1.Location = new Point(ClientArea.X + 205 - label1.Size.Width, label.Location.Y);

            MemberLimitLabel1 = new DXLabel
            {
                Parent = UpgradePanel,
                ForeColour = Color.White,
                Location = new Point(ClientArea.X + 205, label.Location.Y)
            };


            label = new DXLabel
            {
                Parent = UpgradePanel,
                Text = "Storage:",
            };
            label.Location = new Point(ClientArea.X + 55 - label.Size.Width, 60);

            IncreaseStorageButton = new DXButton
            {
                Parent = UpgradePanel,
                Location = new Point(ClientArea.X + 55, label.Location.Y - 1),
                ButtonType = ButtonType.SmallButton,
                Size = new Size(110, SmallButtonHeight),
                Label = { Text = $"Upgrade ({Globals.GuildStorageCost:#,##0})" },
            };
            IncreaseStorageButton.MouseClick += (o, e) =>
            {
                CEnvir.Enqueue(new C.GuildIncreaseStorage());
                IncreaseStorageButton.Enabled = false;
            };

            label1 = new DXLabel
            {
                Parent = UpgradePanel,
                Text = "Limit:",
            };
            label1.Location = new Point(ClientArea.X + 205 - label1.Size.Width, label.Location.Y);

            StorageSizeLabel = new DXLabel
            {
                Parent = UpgradePanel,
                ForeColour = Color.White,
                Location = new Point(ClientArea.X + 205, label.Location.Y)
            };


            GuildWarPanel = new DXControl
            {
                Parent = ManageTab,
                Location = new Point(10 + (HomeTab.Size.Width - 11 - 5) / 2, 5),
                Size = new Size((HomeTab.Size.Width - 11 - 5) / 2, 83),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
            };

            new DXLabel
            {
                Text = "Guild War",
                Parent = GuildWarPanel,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Size = new Size(GuildWarPanel.Size.Width, 22),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
            };


            label = new DXLabel
            {
                Parent = GuildWarPanel,
                Text = "Guild:",
            };
            label.Location = new Point(ClientArea.X + 55 - label.Size.Width, 32);

            GuildWarTextBox = new DXTextBox
            {
                Parent = GuildWarPanel,
                Location = new Point(ClientArea.X + 55, label.Location.Y),
                Size = new Size(110, 20),
                MaxLength = Globals.MaxCharacterNameLength
            };
            GuildWarTextBox.TextBox.TextChanged += GuildWarTextBox_TextChanged;
            GuildWarTextBox.TextBox.KeyPress += GuildWarTextBox_KeyPress;

            StartWarButton = new DXButton
            {
                Parent = GuildWarPanel,
                Location = new Point(ClientArea.X + 170, label.Location.Y - 1),
                ButtonType = ButtonType.SmallButton,
                Size = new Size(60, SmallButtonHeight),
                Label = { Text = "Attack" },
                Enabled =  false,
            };
            StartWarButton.MouseClick += (o, e) => StartWar();

            label = new DXLabel
            {
                Parent = GuildWarPanel,
                Text = $"Cost: {Globals.GuildWarCost:#,##0} Gold",
                Location = new Point(ClientArea.X + 55, label.Location.Y + 25),
            };

            

            int count = 0;
            foreach (CastleInfo castle in CEnvir.CastleInfoList.Binding)
            {
                CastlePanels[castle] = new GuildCastlePanel
                {
                    Parent = ManageTab,
                    Castle = castle,
                    Location =  new Point(5 + count * 255, 275)
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
        
        public void StartWar()
        {
            WarAttempted = true;

            C.GuildWar p = new C.GuildWar
            {
                GuildName = GuildWarTextBox.TextBox.Text,
            };

            CEnvir.Enqueue(p);
        }
        private void GuildWarTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;

            e.Handled = true;

            if (StartWarButton.Enabled)
                StartWar();
        }

        private void GuildWarTextBox_TextChanged(object sender, EventArgs e)
        {
            GuildWarNameValid = Globals.GuildNameRegex.IsMatch(GuildWarTextBox.TextBox.Text);

            if (string.IsNullOrEmpty(GuildWarTextBox.TextBox.Text))
                GuildWarTextBox.BorderColour = Color.FromArgb(198, 166, 99);
            else
                GuildWarTextBox.BorderColour = GuildWarNameValid ? Color.Green : Color.Red;
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

                #region Create Tab

                if (CreateTab != null)
                {
                    if (!CreateTab.IsDisposed)
                        CreateTab.Dispose();

                    CreateTab = null;
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
                
                #region Manage Tab

                if (ManageTab != null)
                {
                    if (!ManageTab.IsDisposed)
                        ManageTab.Dispose();

                    ManageTab = null;
                }

                if (GuildFundLabel1 != null)
                {
                    if (!GuildFundLabel1.IsDisposed)
                        GuildFundLabel1.Dispose();

                    GuildFundLabel1 = null;
                }

                if (MemberLimitLabel1 != null)
                {
                    if (!MemberLimitLabel1.IsDisposed)
                        MemberLimitLabel1.Dispose();

                    MemberLimitLabel1 = null;
                }

                if (StorageSizeLabel != null)
                {
                    if (!StorageSizeLabel.IsDisposed)
                        StorageSizeLabel.Dispose();

                    StorageSizeLabel = null;
                }
                
                if (AddMemberTextBox != null)
                {
                    if (!AddMemberTextBox.IsDisposed)
                        AddMemberTextBox.Dispose();

                    AddMemberTextBox = null;
                }

                if (MemberTaxBox != null)
                {
                    if (!MemberTaxBox.IsDisposed)
                        MemberTaxBox.Dispose();

                    MemberTaxBox = null;
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

                if (SetTaxButton != null)
                {
                    if (!SetTaxButton.IsDisposed)
                        SetTaxButton.Dispose();

                    SetTaxButton = null;
                }

                if (IncreaseMemberButton != null)
                {
                    if (!IncreaseMemberButton.IsDisposed)
                        IncreaseMemberButton.Dispose();

                    IncreaseMemberButton = null;
                }

                if (IncreaseStorageButton != null)
                {
                    if (!IncreaseStorageButton.IsDisposed)
                        IncreaseStorageButton.Dispose();

                    IncreaseStorageButton = null;
                }
                
                if (GuildTaxBox != null)
                {
                    if (!GuildTaxBox.IsDisposed)
                        GuildTaxBox.Dispose();

                    GuildTaxBox = null;
                }

                if (AddMemberPanel != null)
                {
                    if (!AddMemberPanel.IsDisposed)
                        AddMemberPanel.Dispose();

                    AddMemberPanel = null;
                }

                if (TreasuryPanel != null)
                {
                    if (!TreasuryPanel.IsDisposed)
                        TreasuryPanel.Dispose();

                    TreasuryPanel = null;
                }

                if (UpgradePanel != null)
                {
                    if (!UpgradePanel.IsDisposed)
                        UpgradePanel.Dispose();

                    UpgradePanel = null;
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
            NameLabel.Text = "Name";
            NameLabel.ForeColour = Color.FromArgb(198, 166, 99);

            RankLabel.Text = "Rank";
            RankLabel.ForeColour = Color.FromArgb(198, 166, 99);

            TotalLabel.Text = "Total Con.";
            TotalLabel.ForeColour = Color.FromArgb(198, 166, 99);

            DailyLabel.Text = "Daily Con.";
            DailyLabel.ForeColour = Color.FromArgb(198, 166, 99);

            OnlineLabel.Text = "Online";
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
            Size = new Size(488, 20);

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

            TitleLabel.Text = "Edit Member";

            DXLabel label = new DXLabel
            {
                Parent = this,
                Text = "Member:",
            };
            label.Location = new Point(ClientArea.X + 80 - label.Size.Width, ClientArea.Y);


            MemberNameLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(ClientArea.X + 80, label.Location.Y),
                ForeColour = Color.White,
            };


            label = new DXLabel
            {
                Parent = this,
                Text = "Rank:",
            };
            label.Location = new Point(ClientArea.X + 80 - label.Size.Width, MemberNameLabel.Location.Y + 20);

            RankTextBox = new DXTextBox
            {
                Parent = this,
                Location = new Point(ClientArea.X + 80, label.Location.Y), 
                Size = new Size(120, 20),
                MaxLength = Globals.MaxCharacterNameLength
            };

            LeaderBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Is Leader:" },
            };
            LeaderBox.CheckedChanged += (o, e) => UpdatePermission();
            LeaderBox.Location = new Point(ClientArea.X + 94 - LeaderBox.Size.Width, RankTextBox.Location.Y + 24);

            EditNoticeBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Edit Notice:" },
            };
            EditNoticeBox.CheckedChanged += (o, e) => UpdatePermission();
            EditNoticeBox.Location = new Point(ClientArea.X + 94 - EditNoticeBox.Size.Width, LeaderBox.Location.Y + 20);

            AddMemberBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Add Member:" },
            };
            AddMemberBox.CheckedChanged += (o, e) => UpdatePermission();
            AddMemberBox.Location = new Point(ClientArea.X + 94 - AddMemberBox.Size.Width, EditNoticeBox.Location.Y + 20);
            
            StorageBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Use Storage:" },
            };
            StorageBox.CheckedChanged += (o, e) => UpdatePermission();
            StorageBox.Location = new Point(ClientArea.X + 94 - StorageBox.Size.Width, AddMemberBox.Location.Y + 20);

            StartWarBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Start War:" },
            };
            StartWarBox.CheckedChanged += (o, e) => UpdatePermission();
            StartWarBox.Location = new Point(ClientArea.X + 94 - StartWarBox.Size.Width, StorageBox.Location.Y + 20);

            RepairBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Repair Fund:" },
            };
            RepairBox.CheckedChanged += (o, e) => UpdatePermission();
            RepairBox.Location = new Point(ClientArea.X + 200 - RepairBox.Size.Width, EditNoticeBox.Location.Y );


            MerchantBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Merchant Fund:" },
            };
            MerchantBox.CheckedChanged += (o, e) => UpdatePermission();
            MerchantBox.Location = new Point(ClientArea.X + 200 - MerchantBox.Size.Width, RepairBox.Location.Y + 20);

            MarketBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "Market Fund:" },
            };
            MarketBox.CheckedChanged += (o, e) => UpdatePermission();
            MarketBox.Location = new Point(ClientArea.X + 200 - MarketBox.Size.Width, MerchantBox.Location.Y + 20);


            ConfirmButton = new DXButton
            {
                Parent = this,
                Location = new Point(ClientArea.X + 120, StorageBox.Location.Y + 40),
                ButtonType = ButtonType.SmallButton,
                Size = new Size(80, SmallButtonHeight),
                Label = { Text = "Confirm" },
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
                Label = { Text = "Kick" },
            };
            KickButton.MouseClick += (o, e) =>
            {
                DXMessageBox box = new DXMessageBox($"Are you sure you want to remove {MemberNameLabel.Text} from the guild", "Kick Member", DXMessageBoxButtons.YesNo);
                
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
            Size = new Size(250, 118);
            Border = true;
            BorderColour = Color.FromArgb(198, 166, 99);

            CastleNameLabel =  new DXLabel
            {
                AutoSize =  false,
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Size = new Size(Size.Width, 22),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
            };
           // CastleNameLabel.SizeChanged += (o, e) => CastleNameLabel.Location = new Point((Size.Width - CastleNameLabel.Size.Width) / 2, 0);

            DXLabel label = new DXLabel
            {
                Parent = this,
                Text = "Current Owner:",
            };
            label.Location = new Point(80 - label.Size.Width, 25);

            CastleOwnerLabel = new DXLabel
            {
                Parent = this,
                Text = "None",
                Location = new Point(80, 25),
                ForeColour = Color.White
            };

            label = new DXLabel
            {
                Parent = this,
                Text = "Schedule:",
            };
            label.Location = new Point(80 - label.Size.Width, 45);


            CastleDateLabel = new DXLabel
            {
                Parent = this,
                Text = "None",
                Location = new Point(80, 45),
                ForeColour = Color.White
            };

            RequestButton = new DXButton
            {
                Parent = this,
                Location = new Point(80, 75),
                ButtonType = ButtonType.SmallButton,
                Size = new Size(100, SmallButtonHeight),
                Label = { Text = "Submit" },
                Enabled = false,
            };
            RequestButton.MouseClick += (o, e) => CEnvir.Enqueue(new C.GuildRequestConquest { Index = Castle.Index });


            label = new DXLabel
            {
                Parent = this,
                Text = "Cost:",
            };
            label.Location = new Point(80 - label.Size.Width, 95);

            ItemLabel = new DXLabel
            {
                Parent = this,
                Text = "None",
                Location = new Point(80, 95),
                ForeColour = Color.White
            };

        }

        public void Update()
        {
            string owner = GameScene.Game.CastleOwners[Castle];

            CastleOwnerLabel.Text = string.IsNullOrEmpty(owner) ? "None" : owner;
        }

        public override void Process()
        {
            base.Process();

            if (Castle.WarDate == DateTime.MinValue)
                CastleDateLabel.Text = "None";
            else if (Castle.WarDate <= CEnvir.Now)
                CastleDateLabel.Text = "In Progress";
            else
                CastleDateLabel.Text = Functions.ToString(Castle.WarDate - CEnvir.Now, true);
        }
    }
}
