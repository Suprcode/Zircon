using Client.Controls;
using Client.Envir;
using Client.Models;
using Client.Scenes.Views;
using Client.UserModels;
using Library;
using Library.SystemModels;
using MirDB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using C = Library.Network.ClientPackets;

//Cleaned
namespace Client.Scenes
{
    public sealed class GameScene : DXScene
    {
        #region Properties
        public static GameScene Game;

        public DXItemCell SelectedCell
        {
            get => _SelectedCell;
            set
            {
                if (_SelectedCell == value) return;

                if (_SelectedCell != null) _SelectedCell.Selected = false;

                _SelectedCell = value;

                if (_SelectedCell != null) _SelectedCell.Selected = true;
            }
        }
        private DXItemCell _SelectedCell;

        #region User

        public UserObject User
        {
            get => _User;
            set
            {
                if (_User == value) return;

                _User = value;

                UserChanged();
            }
        }
        private UserObject _User;


        #endregion

        #region Observer

        public bool Observer
        {
            get => _Observer;
            set
            {
                if (_Observer == value) return;

                bool oldValue = _Observer;
                _Observer = value;

                OnObserverChanged(oldValue, value);
            }
        }
        private bool _Observer;
        public event EventHandler<EventArgs> ObserverChanged;
        public void OnObserverChanged(bool oValue, bool nValue)
        {
            ObserverChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public ClientUserCurrency CurrencyPickedUp = null;

        public MapObject MagicObject, MouseObject, TargetObject, FocusObject;
        public DXControl ItemLabel, MagicLabel, FameLabel;

        #region MouseItem

        public ClientUserItem MouseItem
        {
            get => _MouseItem;
            set
            {
                if (_MouseItem == value) return;

                ClientUserItem oldValue = _MouseItem;
                _MouseItem = value;

                OnMouseItemChanged(oldValue, value);
            }
        }
        private ClientUserItem _MouseItem;
        public event EventHandler<EventArgs> MouseItemChanged;
        public void OnMouseItemChanged(ClientUserItem oValue, ClientUserItem nValue)
        {
            MouseItemChanged?.Invoke(this, EventArgs.Empty);

            CreateItemLabel();
        }

        #endregion

        #region MouseMagic

        public MagicInfo MouseMagic
        {
            get => _MouseMagic;
            set
            {
                if (_MouseMagic == value) return;

                MagicInfo oldValue = _MouseMagic;
                _MouseMagic = value;

                OnMouseMagicChanged(oldValue, value);
            }
        }
        private MagicInfo _MouseMagic;
        public event EventHandler<EventArgs> MouseMagicChanged;
        public void OnMouseMagicChanged(MagicInfo oValue, MagicInfo nValue)
        {
            MouseMagicChanged?.Invoke(this, EventArgs.Empty);

            if (MagicLabel != null && !MagicLabel.IsDisposed) MagicLabel.Dispose();
            MagicLabel = null;
            CreateMagicLabel();
        }

        #endregion

        #region MouseFame

        public FameInfo MouseFame
        {
            get => _MouseFame;
            set
            {
                if (_MouseFame == value) return;

                FameInfo oldValue = _MouseFame;
                _MouseFame = value;

                OnMouseFameChanged(oldValue, value);
            }
        }
        private FameInfo _MouseFame;
        public event EventHandler<EventArgs> MouseFameChanged;
        public void OnMouseFameChanged(FameInfo oValue, FameInfo nValue)
        {
            MouseFameChanged?.Invoke(this, EventArgs.Empty);

            if (FameLabel != null && !FameLabel.IsDisposed) FameLabel.Dispose();
            FameLabel = null;
            CreateFameLabel();
        }

        #endregion

        public MapControl MapControl;
        public MainPanel MainPanel;

        public MenuDialog MenuBox;
        public DXConfigWindow ConfigBox;
        public HelpDialog HelpBox;
        public CaptionDialog CaptionBox;
        public InventoryDialog InventoryBox;
        public CharacterDialog CharacterBox;
        public FilterDropDialog FilterDropBox;
        public ExitDialog ExitBox;
        public ChatTextBox ChatTextBox;
        public BeltDialog BeltBox;
        public ChatOptionsDialog ChatOptionsBox;
        public NPCDialog NPCBox;
        public NPCGoodsDialog NPCGoodsBox;
        public NPCRepairDialog NPCRepairBox;
        public NPCRefinementStoneDialog NPCRefinementStoneBox;
        public NPCRefineDialog NPCRefineBox;
        public NPCRefineRetrieveDialog NPCRefineRetrieveBox;
        public NPCQuestListDialog NPCQuestListBox;
        public NPCQuestDialog NPCQuestBox;
        public NPCAdoptCompanionDialog NPCAdoptCompanionBox;
        public NPCCompanionStorageDialog NPCCompanionStorageBox;
        public NPCWeddingRingDialog NPCWeddingRingBox;
        public NPCItemFragmentDialog NPCItemFragmentBox;
        public NPCAccessoryUpgradeDialog NPCAccessoryUpgradeBox;
        public NPCAccessoryLevelDialog NPCAccessoryLevelBox;
        public NPCAccessoryResetDialog NPCAccessoryResetBox;
        public NPCMasterRefineDialog NPCMasterRefineBox;
        public NPCRollDialog NPCRollBox;
        public MiniMapDialog MiniMapBox;
        public BigMapDialog BigMapBox;
        public MagicDialog MagicBox;
        public GroupDialog GroupBox;
        public GroupHealthDialog GroupHealthBox;
        public BuffDialog BuffBox;
        public StorageDialog StorageBox;
        public AutoPotionDialog AutoPotionBox;
        public CharacterDialog InspectBox;
        public RankingDialog RankingBox;
        public GameStoreDialog GameStoreBox;
        public ConsignmentDialog ConsignmentBox;
        public DungeonFinderDialog DungeonFinderBox;
        public CommunicationDialog CommunicationBox;
        public TradeDialog TradeBox;
        public GuildDialog GuildBox;
        public GuildMemberDialog GuildMemberBox;
        public QuestDialog QuestBox;
        public QuestTrackerDialog QuestTrackerBox;
        public MilestoneAchievedDialog MilestoneAchievedBox;
        public CompanionDialog CompanionBox;
        public MonsterDialog MonsterBox;
        public MagicBarDialog MagicBarBox;
        public EditCharacterDialog EditCharacterBox;
        public FortuneCheckerDialog FortuneCheckerBox;
        public NPCWeaponCraftWindow NPCWeaponCraftBox;
        public NPCAccessoryRefineDialog NPCAccessoryRefineBox;
        public NPCSocketDialog NPCSocketBox;
        public NPCSocketCombineDialog NPCSocketCombineBox;
        public CurrencyDialog CurrencyBox;
        public TimerDialog TimerBox;
        public BundleDialog BundleBox;
        public LootBoxDialog LootBoxBox;

        public FishingDialog FishingBox;
        public FishingCatchDialog FishingCatchBox;

        public ClientUserItem[] Inventory = new ClientUserItem[Globals.InventorySize];
        public ClientUserItem[] Equipment = new ClientUserItem[Globals.EquipmentSize];

        public List<ClientUserQuest> QuestLog = new List<ClientUserQuest>();

        public HashSet<string> GuildWars = new HashSet<string>();
        public HashSet<CastleInfo> ConquestWars = new HashSet<CastleInfo>();

        public SortedDictionary<uint, ClientObjectData> DataDictionary = new SortedDictionary<uint, ClientObjectData>();

        public Dictionary<ItemInfo, ClientFortuneInfo> FortuneDictionary = new Dictionary<ItemInfo, ClientFortuneInfo>();

        public Dictionary<CastleInfo, string> CastleOwners = new Dictionary<CastleInfo, string>();

        public bool MoveFrame { get; set; }
        public DateTime MoveTime, OutputTime, ItemRefreshTime;

        public bool CanRun;

        public bool AutoRun
        {
            get => _AutoRun;
            set
            {
                if (_AutoRun == value) return;
                _AutoRun = value;

                ReceiveChat(value ? CEnvir.Language.GameSceneAutoRunOn : CEnvir.Language.GameSceneAutoRunOff, MessageType.Hint);
            }
        }
        private bool _AutoRun;

        #region StorageSize

        public int StorageSize
        {
            get { return _StorageSize; }
            set
            {
                if (_StorageSize == value) return;

                int oldValue = _StorageSize;
                _StorageSize = value;

                OnStorageSizeChanged(oldValue, value);
            }
        }
        private int _StorageSize;
        public void OnStorageSizeChanged(int oValue, int nValue)
        {
            StorageBox.RefreshStorage();
        }

        #endregion

        #region NPCID

        public uint NPCID
        {
            get => _NPCID;
            set
            {
                if (_NPCID == value) return;

                uint oldValue = _NPCID;
                _NPCID = value;

                OnNPCIDChanged(oldValue, value);
            }
        }
        private uint _NPCID;
        public void OnNPCIDChanged(uint oValue, uint nValue)
        {

        }

        #endregion

        #region Companion

        public ClientUserCompanion Companion
        {
            get => _Companion;
            set
            {
                if (_Companion == value) return;

                _Companion = value;

                CompanionChanged();
            }
        }
        private ClientUserCompanion _Companion;

        #endregion

        public ClientPlayerInfo Partner
        {
            get => _Partner;
            set
            {
                if (_Partner == value) return;

                _Partner = value;

                MarriageChanged();
            }
        }
        private ClientPlayerInfo _Partner;


        public uint InspectID;
        public DateTime PickUpTime, UseItemTime, NPCTime, ToggleTime, InspectTime, ItemTime = CEnvir.Now, ReincarnationPillTime, ItemReviveTime;

        public bool StruckEnabled;

        public bool HermitEnabled
        {
            get => _HermitEnabled;
            set
            {
                if (_HermitEnabled == value) return;

                _HermitEnabled = value;
                CharacterBox.OnHermitChanged(_HermitEnabled);
            }
        }
        private bool _HermitEnabled;

        public float DayTime
        {
            get => _DayTime;
            set
            {
                if (_DayTime == value) return;

                _DayTime = value;
                MapControl.LLayer.UpdateLights();
            }
        }
        private float _DayTime;

        public TimeOfDay TimeOfDay
        {
            get => _TimeOfDay;
            set
            {
                if (_TimeOfDay == value) return;

                _TimeOfDay = value;
            }
        }
        private TimeOfDay _TimeOfDay;

        public string TimeOfDayLabel
        {
            get => _TimeOfDayLabel;
            set
            {
                if (_TimeOfDayLabel == value) return;

                _TimeOfDayLabel = value;
            }
        }
        private string _TimeOfDayLabel = string.Empty;

        public override void OnSizeChanged(Size oValue, Size nValue)
        {
            base.OnSizeChanged(oValue, nValue);

            SetDefaultLocations();

            foreach (DXWindow window in DXWindow.Windows)
                window.LoadSettings();

            CharacterBox?.LoadSettings();
            InventoryBox?.LoadSettings();
            MagicBox?.LoadSettings();
            StorageBox?.LoadSettings();
            TradeBox?.LoadSettings();
            CompanionBox?.LoadSettings();
            CommunicationBox?.LoadSettings();
            RankingBox?.LoadSettings();
            QuestBox?.LoadSettings();
            FishingBox?.LoadSettings();
            GroupBox?.LoadSettings();
            GuildBox?.LoadSettings();
            ConfigBox?.LoadSettings();
            MenuBox?.LoadSettings();          
            HelpBox?.LoadSettings();
            GameStoreBox?.LoadSettings();
            ConsignmentBox?.LoadSettings();

            LoadChatTabs();
        }

        #endregion

        public GameScene(Size size) : base(size)
        {
            DrawTexture = false;
            Game = this;

            foreach (NPCInfo info in Globals.NPCInfoList.Binding)
                info.CurrentQuest = null;

            MapControl = new MapControl
            {
                Parent = this,
                Size = Size,
            };
            MapControl.MouseWheel += (o, e) =>
            {
                foreach (ChatTab tab in ChatTab.Tabs)
                {
                    if (!tab.DisplayArea.Contains(e.Location) || !tab.Visible) continue;

                    tab.ScrollBar.DoMouseWheel(tab.ScrollBar, e);
                }
            };

            MainPanel = new MainPanel { Parent = this };

            MenuBox = new MenuDialog
            {
                Parent = this,
                Visible = false
            };

            ConfigBox = new DXConfigWindow
            {
                Parent = this,
                Visible = false,
                NetworkTab = { Enabled = false, TabButton = { Visible = false } },
                UITab = { TabButton = { Visible = true } },
            };

            HelpBox = new HelpDialog
            {
                Parent = this,
                Visible = false
            };

            ExitBox = new ExitDialog
            {
                Parent = this,
                Visible = false,
            };

            CaptionBox = new CaptionDialog
            {
                Parent = this,
                Visible = false,
            };

            InventoryBox = new InventoryDialog
            {
                Parent = this,
                Visible = false,
            };

            CharacterBox = new CharacterDialog(false)
            {
                Parent = this,
                Visible = false,
            };

            FilterDropBox = new FilterDropDialog
            {
                Parent = this,
                Visible = false,
            };

            ChatTextBox = new ChatTextBox
            {
                Parent = this,
                Visible = false
            };
            ChatOptionsBox = new ChatOptionsDialog
            {
                Parent = this,
                Visible = false,
            };
            BeltBox = new BeltDialog
            {
                Parent = this,
            };
            NPCBox = new NPCDialog
            {
                Parent = this,
                Visible = false
            };
            NPCGoodsBox = new NPCGoodsDialog
            {
                Parent = this,
                Visible = false
            };

            NPCRepairBox = new NPCRepairDialog
            {
                Parent = this,
                Visible = false,
            };
            NPCQuestListBox = new NPCQuestListDialog
            {
                Parent = this,
                Visible = false,
            };
            NPCQuestBox = new NPCQuestDialog
            {
                Parent = this,
                Visible = false,
            };
            NPCAdoptCompanionBox = new NPCAdoptCompanionDialog
            {
                Parent = this,
                Visible = false,
            };
            NPCCompanionStorageBox = new NPCCompanionStorageDialog
            {
                Parent = this,
                Visible = false,
            };
            NPCWeddingRingBox = new NPCWeddingRingDialog
            {
                Parent = this,
                Visible = false,
            };

            MiniMapBox = new MiniMapDialog
            {
                Parent = this,
            };
            MagicBox = new MagicDialog()
            {
                Parent = this,
                Visible = false,
            };
            GroupBox = new GroupDialog()
            {
                Parent = this,
                Visible = false,
            };
            GroupHealthBox = new GroupHealthDialog()
            {
                Parent = this,
                Visible = true,
            };

            BigMapBox = new BigMapDialog
            {
                Parent = this,
                Visible = false,
            };
            BuffBox = new BuffDialog
            {
                Parent = this,
            };
            StorageBox = new StorageDialog
            {
                Parent = this,
                Visible = false
            };
            AutoPotionBox = new AutoPotionDialog
            {
                Parent = this,
                Visible = false
            };
            NPCRefinementStoneBox = new NPCRefinementStoneDialog
            {
                Parent = this,
                Visible = false,
            };
            NPCItemFragmentBox = new NPCItemFragmentDialog()
            {
                Parent = this,
                Visible = false,
            };
            NPCAccessoryUpgradeBox = new NPCAccessoryUpgradeDialog
            {
                Parent = this,
                Visible = false,
            };
            NPCAccessoryLevelBox = new NPCAccessoryLevelDialog
            {
                Parent = this,
                Visible = false,
            };
            NPCAccessoryResetBox = new NPCAccessoryResetDialog
            {
                Parent = this,
                Visible = false,
            };
            NPCRefineBox = new NPCRefineDialog
            {
                Parent = this,
                Visible = false
            };
            NPCRefineRetrieveBox = new NPCRefineRetrieveDialog
            {
                Parent = this,
                Visible = false
            };
            NPCMasterRefineBox = new NPCMasterRefineDialog
            {
                Parent = this,
                Visible = false
            };
            NPCRollBox = new NPCRollDialog
            {
                Parent = this,
                Visible = false
            };

            InspectBox = new CharacterDialog(true)
            {
                Parent = this,
                Visible = false
            };
            RankingBox = new RankingDialog(true)
            {
                Parent = this,
                Visible = false
            };
            GameStoreBox = new GameStoreDialog
            {
                Parent = this,
                Visible = false,
            };
            ConsignmentBox = new ConsignmentDialog
            {
                Parent = this,
                Visible = false,
            };
            DungeonFinderBox = new DungeonFinderDialog
            {
                Parent = this,
                Visible = false,
            };
            EditCharacterBox = new EditCharacterDialog
            {
                Parent = this,
                Visible = false
            };
            CommunicationBox = new CommunicationDialog
            {
                Parent = this,
                Visible = false
            };
            TradeBox = new TradeDialog
            {
                Parent = this,
                Visible = false
            };
            GuildBox = new GuildDialog
            {
                Parent = this,
                Visible = false
            };
            GuildMemberBox = new GuildMemberDialog
            {
                Parent = this,
                Visible = false
            };

            QuestBox = new QuestDialog
            {
                Parent = this,
                Visible = false
            };

            QuestTrackerBox = new QuestTrackerDialog
            {
                Parent = this,
                Visible = false
            };

            MilestoneAchievedBox = new MilestoneAchievedDialog
            {
                Parent = this,
                Visible = false,
            };

            CompanionBox = new CompanionDialog
            {
                Parent = this,
                Visible = false,
            };

            MonsterBox = new MonsterDialog
            {
                Parent = this,
                Visible = false,
            };
            MagicBarBox = new MagicBarDialog
            {
                Parent = this,
                Visible = false,
            };

            FortuneCheckerBox = new FortuneCheckerDialog
            {
                Parent = this,
                Visible = false,
            };

            CurrencyBox = new CurrencyDialog
            {
                Parent = this,
                Visible = false,
            };

            TimerBox = new TimerDialog
            {
                Parent = this,
                Visible = true,
            };

            NPCWeaponCraftBox = new NPCWeaponCraftWindow
            {
                Visible = false,
                Parent = this,
            };
            NPCAccessoryRefineBox = new NPCAccessoryRefineDialog
            {
                Parent = this,
                Visible = false,
            };
            NPCSocketBox = new NPCSocketDialog
            {
                Parent = this,
                Visible = false,
            };
            NPCSocketCombineBox = new NPCSocketCombineDialog
            {
                Parent = this,
                Visible = false,
            };

            FishingBox = new FishingDialog(CharacterBox)
            {
                Parent = this,
                Visible = false,
            };

            FishingCatchBox = new FishingCatchDialog
            {
                Parent = this,
                Visible = false,
            };

            BundleBox = new BundleDialog
            {
                Parent = this,
                Visible = false
            };

            LootBoxBox = new LootBoxDialog
            {
                Parent = this,
                Visible = false
            };

            SetDefaultLocations();

            LoadChatTabs();

            foreach (DXWindow window in DXWindow.Windows)
                window.LoadSettings();

            CharacterBox.LoadSettings();
            MagicBox.LoadSettings();
            InventoryBox.LoadSettings();
            StorageBox.LoadSettings();
            TradeBox.LoadSettings();
            CompanionBox.LoadSettings();
            CommunicationBox.LoadSettings();
            RankingBox.LoadSettings();
            QuestBox.LoadSettings();
            FishingBox.LoadSettings();
            GroupBox.LoadSettings();
            GuildBox.LoadSettings();
            MenuBox.LoadSettings();
            HelpBox.LoadSettings();
            GameStoreBox.LoadSettings();
            ConsignmentBox.LoadSettings();
        }

        #region Methods
        private void SetDefaultLocations()
        {
            if (ConfigBox == null) return;

            MenuBox.Location = new Point(Size.Width - MenuBox.Size.Width, Size.Height - MenuBox.Size.Height - MainPanel.Size.Height);

            ConfigBox.Location = new Point((Size.Width - ConfigBox.Size.Width) / 2, (Size.Height - ConfigBox.Size.Height) / 2);

            CaptionBox.Location = Point.Empty;

            ChatOptionsBox.Location = new Point((Size.Width - ChatOptionsBox.Size.Width) / 2, (Size.Height - ChatOptionsBox.Size.Height) / 2);

            ExitBox.Location = new Point((Size.Width - ExitBox.Size.Width) / 2, (Size.Height - ExitBox.Size.Height) / 2);

            TradeBox.Location = new Point((Size.Width - TradeBox.Size.Width) / 2, (Size.Height - TradeBox.Size.Height) / 2);

            GuildBox.Location = new Point((Size.Width - GuildBox.Size.Width) / 2, (Size.Height - GuildBox.Size.Height) / 2);

            GuildMemberBox.Location = new Point((Size.Width - GuildMemberBox.Size.Width) / 2, (Size.Height - GuildMemberBox.Size.Height) / 2);

            InventoryBox.Location = new Point(Size.Width - InventoryBox.Size.Width, MiniMapBox.Size.Height);

            CharacterBox.Location = Point.Empty;

            MapControl.Size = Size;

            MainPanel.Location = new Point((Size.Width - MainPanel.Size.Width) / 2, Size.Height - MainPanel.Size.Height);

            ChatTextBox.Location = new Point((Size.Width - ChatTextBox.Size.Width) / 2, (Size.Height - ChatTextBox.Size.Height) / 2);

            BeltBox.Location = new Point(MainPanel.Location.X + MainPanel.Size.Width - BeltBox.Size.Width, MainPanel.Location.Y - BeltBox.Size.Height);

            NPCBox.Location = Point.Empty;

            NPCGoodsBox.Location = new Point(0, NPCBox.Size.Height);

            NPCRollBox.Location = new Point((Size.Width - NPCRollBox.Size.Width) / 2, (Size.Height - NPCRollBox.Size.Height) / 2);

            NPCRepairBox.Location = new Point(0, NPCBox.Size.Height);

            MiniMapBox.Location = new Point(Size.Width - MiniMapBox.Size.Width, 0);

            QuestTrackerBox.Location = new Point(Size.Width - QuestTrackerBox.Size.Width, MiniMapBox.Size.Height + 5);

            MilestoneAchievedBox.Location = new Point((Size.Width - MilestoneAchievedBox.Size.Width) / 2, ((Size.Height - MilestoneAchievedBox.Size.Height) / 2) + 100);

            BuffBox.Location = new Point(Size.Width - MiniMapBox.Size.Width - BuffBox.Size.Width - 5, 0);

            MagicBox.Location = new Point(Size.Width - MagicBox.Size.Width, 0);

            GroupBox.Location = new Point((Size.Width - GroupBox.Size.Width) / 2, (Size.Height - GroupBox.Size.Height) / 2);

            StorageBox.Location = new Point(Size.Width - StorageBox.Size.Width - InventoryBox.Size.Width, 0);

            AutoPotionBox.Location = new Point((Size.Width - AutoPotionBox.Size.Width) / 2, (Size.Height - AutoPotionBox.Size.Height) / 2);

            InspectBox.Location = new Point(CharacterBox.Size.Width, 0);

            RankingBox.Location = new Point((Size.Width - RankingBox.Size.Width) / 2, (Size.Height - RankingBox.Size.Height) / 2);

            GameStoreBox.Location = new Point((Size.Width - GameStoreBox.Size.Width) / 2, (Size.Height - GameStoreBox.Size.Height) / 2);

            ConsignmentBox.Location = new Point((Size.Width - ConsignmentBox.Size.Width) / 2, (Size.Height - ConsignmentBox.Size.Height) / 2);

            CommunicationBox.Location = new Point((Size.Width - CommunicationBox.Size.Width) / 2, (Size.Height - CommunicationBox.Size.Height) / 2);

            CompanionBox.Location = new Point((Size.Width - CompanionBox.Size.Width) / 2, (Size.Height - CompanionBox.Size.Height) / 2);

            MonsterBox.Location = new Point((Size.Width - MonsterBox.Size.Width) / 2, 50);

            EditCharacterBox.Location = new Point((Size.Width - EditCharacterBox.Size.Width) / 2, (Size.Height - EditCharacterBox.Size.Height) / 2);

            FortuneCheckerBox.Location = new Point((Size.Width - FortuneCheckerBox.Size.Width) / 2, (Size.Height - FortuneCheckerBox.Size.Height) / 2);

            NPCWeaponCraftBox.Location = new Point((Size.Width - NPCWeaponCraftBox.Size.Width) / 2, (Size.Height - NPCWeaponCraftBox.Size.Height) / 2);

            NPCSocketBox.Location = new Point((Size.Width - NPCSocketBox.Size.Width) / 2, (Size.Height - NPCSocketBox.Size.Height) / 2);
            NPCSocketCombineBox.Location = new Point((Size.Width - NPCSocketCombineBox.Size.Width) / 2, (Size.Height - NPCSocketCombineBox.Size.Height) / 2);

            CurrencyBox.Location = new Point((Size.Width - CurrencyBox.Size.Width) / 2, (Size.Height - CurrencyBox.Size.Height) / 2);

            FishingBox.Location = new Point(CharacterBox.Location.X + CharacterBox.Size.Width, CharacterBox.Location.Y);

            FishingCatchBox.Location = new Point(((Size.Width - FishingCatchBox.Size.Width) / 2), ((Size.Height - FishingCatchBox.Size.Height) / 2) + 200);

            TimerBox.Location = new Point(MainPanel.DisplayArea.Right - 115, Size.Height - 170);

            BundleBox.Location = new Point((Size.Width - BundleBox.Size.Width) / 2, (Size.Height - BundleBox.Size.Height) / 2);

            LootBoxBox.Location = new Point((Size.Width - LootBoxBox.Size.Width) / 2, (Size.Height - LootBoxBox.Size.Height) / 2);
        }

        public void SaveChatTabs()
        {
            DBCollection<ChatTabControlSetting> controlSettings = CEnvir.Session.GetCollection<ChatTabControlSetting>();
            DBCollection<ChatTabPageSetting> pageSettings = CEnvir.Session.GetCollection<ChatTabPageSetting>();

            for (int i = controlSettings.Binding.Count - 1; i >= 0; i--)
                controlSettings.Binding[i].Delete();

            foreach (DXControl temp1 in Controls)
            {
                DXTabControl tabControl = temp1 as DXTabControl;

                if (tabControl == null) continue;

                ChatTabControlSetting cSetting = controlSettings.CreateNewObject();

                cSetting.Resolution = Config.GameSize;
                cSetting.Location = tabControl.Location;
                cSetting.Size = tabControl.Size;

                foreach (DXControl tempC in tabControl.Controls)
                {
                    ChatTab tab = tempC as ChatTab;
                    if (tab == null) continue;

                    ChatTabPageSetting pSetting = pageSettings.CreateNewObject();

                    pSetting.Parent = cSetting;

                    if (tabControl.SelectedTab == tab)
                        cSetting.SelectedPage = pSetting;

                    pSetting.Name = tab.Panel.NameTextBox.TextBox.Text;
                    pSetting.Transparent = tab.Panel.TransparentCheckBox.Checked;
                    pSetting.Alert = tab.Panel.AlertCheckBox.Checked;
                    pSetting.HideTab = tab.Panel.HideTabCheckBox.Checked;
                    pSetting.ReverseList = tab.Panel.ReverseListCheckBox.Checked;
                    pSetting.CleanUp = tab.Panel.CleanUpCheckBox.Checked;
                    pSetting.FadeOut = tab.Panel.FadeOutCheckBox.Checked;

                    pSetting.LocalChat = tab.Panel.LocalCheckBox.Checked;
                    pSetting.WhisperChat = tab.Panel.WhisperCheckBox.Checked;
                    pSetting.GroupChat = tab.Panel.GroupCheckBox.Checked;
                    pSetting.GuildChat = tab.Panel.GuildCheckBox.Checked;
                    pSetting.ShoutChat = tab.Panel.ShoutCheckBox.Checked;
                    pSetting.GlobalChat = tab.Panel.GlobalCheckBox.Checked;
                    pSetting.ObserverChat = tab.Panel.ObserverCheckBox.Checked;
                    pSetting.HintChat = tab.Panel.HintCheckBox.Checked;
                    pSetting.SystemChat = tab.Panel.SystemCheckBox.Checked;
                    pSetting.GainsChat = tab.Panel.GainsCheckBox.Checked;
                }
            }
        }

        public void LoadChatTabs()
        {
            if (ConfigBox == null) return;

            for (int i = ChatTab.Tabs.Count - 1; i >= 0; i--)
                ChatTab.Tabs[i].Panel.RemoveButton.InvokeMouseClick();

            DBCollection<ChatTabControlSetting> controlSettings = CEnvir.Session.GetCollection<ChatTabControlSetting>();

            bool result = false;
            foreach (ChatTabControlSetting cSetting in controlSettings.Binding)
            {
                if (cSetting.Resolution != Config.GameSize) continue;

                result = true;

                DXTabControl tabControl = new DXTabControl
                {
                    Location = cSetting.Location,
                    Size = cSetting.Size,
                    Parent = this,
                };

                ChatTab selected = null;
                foreach (ChatTabPageSetting pSetting in cSetting.Controls)
                {
                    ChatTab tab = ChatOptionsBox.AddNewTab(pSetting);

                    tab.Parent = tabControl;

                    tab.Panel.NameTextBox.TextBox.Text = tab.Settings.Name;
                    tab.Panel.AlertCheckBox.Checked = tab.Settings.Alert;
                    tab.Panel.HideTabCheckBox.Checked = tab.Settings.HideTab;
                    tab.Panel.ReverseListCheckBox.Checked = tab.Settings.ReverseList;
                    tab.Panel.CleanUpCheckBox.Checked = tab.Settings.CleanUp;
                    tab.Panel.FadeOutCheckBox.Checked = tab.Settings.FadeOut;

                    tab.Panel.LocalCheckBox.Checked = tab.Settings.LocalChat;
                    tab.Panel.WhisperCheckBox.Checked = tab.Settings.WhisperChat;
                    tab.Panel.GroupCheckBox.Checked = tab.Settings.GroupChat;
                    tab.Panel.GuildCheckBox.Checked = tab.Settings.GuildChat;
                    tab.Panel.ShoutCheckBox.Checked = tab.Settings.ShoutChat;
                    tab.Panel.GlobalCheckBox.Checked = tab.Settings.GlobalChat;
                    tab.Panel.ObserverCheckBox.Checked = tab.Settings.ObserverChat;

                    tab.Panel.HintCheckBox.Checked = tab.Settings.HintChat;
                    tab.Panel.SystemCheckBox.Checked = tab.Settings.SystemChat;
                    tab.Panel.GainsCheckBox.Checked = tab.Settings.GainsChat;
                }

                foreach (ChatTab tab in ChatTab.Tabs)
                {
                    tab.Panel.TransparentCheckBox.Checked = tab.Settings.Transparent;

                    if (tab.Settings == cSetting.SelectedPage)
                        selected = tab;
                }

                tabControl.SelectedTab = selected;
            }

            if (result)
                Game.ReceiveChat(CEnvir.Language.ChatLayoutLoaded, MessageType.Announcement);
            else
                ChatOptionsBox.CreateDefaultWindows();
        }

        public override void Process()
        {
            base.Process();

            if (CEnvir.Now >= MoveTime)
            {
                MoveTime = CEnvir.Now.AddMilliseconds(100);
                MapControl.Animation++;
                MoveFrame = true;
            }
            else if (!Config.SmoothMove)
            {
                MoveFrame = false;
            }

            if (MouseControl == MapControl)
                MapControl.CheckCursor();

            if (MouseControl == MapControl)
            {
                if (CEnvir.Ctrl && MapObject.MouseObject?.Race == ObjectType.Item)
                    MouseItem = ((ItemObject)MapObject.MouseObject).Item;
                else
                    MouseItem = null;
            }

            TimeSpan ticks = CEnvir.Now - ItemTime;
            ItemTime = CEnvir.Now;

            if (!User.InSafeZone)
            {
                foreach (ClientUserItem item in Equipment)
                {
                    if ((item?.Flags & UserItemFlags.Expirable) != UserItemFlags.Expirable) continue;

                    item.ExpireTime -= ticks;
                }

                foreach (ClientUserItem item in Inventory)
                {
                    if ((item?.Flags & UserItemFlags.Expirable) != UserItemFlags.Expirable) continue;

                    item.ExpireTime -= ticks;
                }

                if (Companion != null)
                {
                    foreach (ClientUserItem item in Companion.InventoryArray)
                    {
                        if ((item?.Flags & UserItemFlags.Expirable) != UserItemFlags.Expirable) continue;

                        item.ExpireTime -= ticks;
                    }
                    foreach (ClientUserItem item in Companion.EquipmentArray)
                    {
                        if ((item?.Flags & UserItemFlags.Expirable) != UserItemFlags.Expirable) continue;

                        item.ExpireTime -= ticks;
                    }
                }
            }

            if (MouseItem != null && CEnvir.Now > ItemRefreshTime)
            {
                CreateItemLabel();
            }

            MapControl.ProcessInput();

            foreach (MapObject ob in MapControl.Objects)
                ob.Process();

            for (int i = MapControl.Effects.Count - 1; i >= 0; i--)
                MapControl.Effects[i].Process();

            for (int i = MapControl.ParticleEffects.Count - 1; i >= 0; i--)
                MapControl.ParticleEffects[i].Process();

            if (ItemLabel != null && !ItemLabel.IsDisposed)
            {
                int x = CEnvir.MouseLocation.X + 15, y = CEnvir.MouseLocation.Y;

                if (x + ItemLabel.Size.Width > Size.Width + Location.X)
                    x = Size.Width - ItemLabel.Size.Width + Location.X;

                if (y + ItemLabel.Size.Height > Size.Height + Location.Y)
                    y = Size.Height - ItemLabel.Size.Height + Location.Y;

                if (x < Location.X)
                    x = Location.X;

                if (y <= Location.Y)
                    y = Location.Y;

                ItemLabel.Location = new Point(x, y);
            }

            if (MagicLabel != null && !MagicLabel.IsDisposed)
            {
                int x = CEnvir.MouseLocation.X + 15, y = CEnvir.MouseLocation.Y;

                if (x + MagicLabel.Size.Width > Size.Width + Location.X)
                    x = Size.Width - MagicLabel.Size.Width + Location.X;

                if (y + MagicLabel.Size.Height > Size.Height + Location.Y)
                    y = Size.Height - MagicLabel.Size.Height + Location.Y;

                if (x < Location.X)
                    x = Location.X;

                if (y <= Location.Y)
                    y = Location.Y;

                MagicLabel.Location = new Point(x, y);
            }

            if (FameLabel != null && !FameLabel.IsDisposed)
            {
                int x = CEnvir.MouseLocation.X + 15, y = CEnvir.MouseLocation.Y;

                if (x + FameLabel.Size.Width > Size.Width + Location.X)
                    x = Size.Width - FameLabel.Size.Width + Location.X;

                if (y + FameLabel.Size.Height > Size.Height + Location.Y)
                    y = Size.Height - FameLabel.Size.Height + Location.Y;

                if (x < Location.X)
                    x = Location.X;

                if (y <= Location.Y)
                    y = Location.Y;

                FameLabel.Location = new Point(x, y);
            }

            MonsterObject mob = MouseObject as MonsterObject;

            if (mob != null && mob.CompanionObject == null)
                MonsterBox.Monster = mob;
            else
            {
                mob = FocusObject as MonsterObject;
                if (mob != null && mob.CompanionObject == null && !FocusObject.Dead)
                    MonsterBox.Monster = mob;
            }
        }

        public override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            if (e.Handled) return;

            switch ((Keys)e.KeyChar)
            {
                case Keys.Enter:
                    ChatTextBox.ToggleVisibility(e, false);
                    break;
            }
        }

        public override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Handled) return;

            switch (e.KeyCode)
            {
                case Keys.Escape:
                    MonsterBox.Monster = null;
                    e.Handled = true;
                    break;
            }

            foreach (KeyBindAction action in CEnvir.GetKeyAction(e.KeyCode))
            {
                switch (action)
                {
                    case KeyBindAction.MenuWindow:
                        MenuBox.Visible = !MenuBox.Visible;
                        break;
                    case KeyBindAction.HelpWindow:
                        HelpBox.Visible = !HelpBox.Visible;
                        break;
                    case KeyBindAction.ConfigWindow:
                        ConfigBox.Visible = !ConfigBox.Visible;
                        break;
                    case KeyBindAction.RankingWindow:
                        RankingBox.Visible = !RankingBox.Visible && CEnvir.Connection != null;
                        break;
                    case KeyBindAction.CharacterWindow:
                        CharacterBox.Visible = !CharacterBox.Visible;
                        break;
                    case KeyBindAction.InventoryWindow:
                        InventoryBox.Visible = !InventoryBox.Visible;
                        break;
                    case KeyBindAction.FortuneWindow:
                        FortuneCheckerBox.Visible = !FortuneCheckerBox.Visible;
                        break;
                    case KeyBindAction.CurrencyWindow:
                        CurrencyBox.Visible = !CurrencyBox.Visible;
                        break;
                    case KeyBindAction.MagicWindow:
                        MagicBox.Visible = !MagicBox.Visible;
                        break;
                    case KeyBindAction.MagicBarWindow:
                        MagicBarBox.Visible = !MagicBarBox.Visible;
                        break;
                    case KeyBindAction.GameStoreWindow:
                        GameStoreBox.Visible = !GameStoreBox.Visible;
                        break;
                    case KeyBindAction.DungeonFinderWindow:
                        DungeonFinderBox.Visible = !DungeonFinderBox.Visible;
                        break;
                    case KeyBindAction.CompanionWindow:
                        CompanionBox.Visible = !CompanionBox.Visible;
                        break;
                    case KeyBindAction.FilterDropWindow:
                        FilterDropBox.Visible = !FilterDropBox.Visible;
                        break;
                    case KeyBindAction.GroupWindow:
                        GroupBox.Visible = !GroupBox.Visible;
                        break;
                    case KeyBindAction.AutoPotionWindow:
                        AutoPotionBox.Visible = !AutoPotionBox.Visible;
                        break;
                    case KeyBindAction.StorageWindow:
                        StorageBox.Visible = !StorageBox.Visible;
                        break;
                    case KeyBindAction.BlockListWindow:
                        CommunicationBox.Visible = !CommunicationBox.Visible;
                        if (CommunicationBox.Visible)
                            CommunicationBox.BlockTab.TabButton.InvokeMouseClick();
                        break;
                    case KeyBindAction.MailSendWindow:
                        CommunicationBox.Visible = !CommunicationBox.Visible;
                        if (CommunicationBox.Visible)
                            CommunicationBox.SendTab.TabButton.InvokeMouseClick();
                        break;
                    case KeyBindAction.GuildWindow:
                        GuildBox.Visible = !GuildBox.Visible;
                        break;
                    case KeyBindAction.QuestLogWindow:
                        QuestBox.Visible = !QuestBox.Visible;
                        break;
                    case KeyBindAction.QuestTrackerWindow:
                        QuestBox.CurrentTab.ShowTrackerBox.Checked = !QuestBox.CurrentTab.ShowTrackerBox.Checked;
                        break;
                    case KeyBindAction.BeltWindow:
                        BeltBox.Visible = !BeltBox.Visible;
                        break;
                    case KeyBindAction.MapMiniWindow:
                        if (!MiniMapBox.Visible)
                        {
                            MiniMapBox.Opacity = 1F;
                            MiniMapBox.Visible = true;
                            return;
                        }

                        if (MiniMapBox.Opacity == 1F)
                        {
                            MiniMapBox.Opacity = 0.5F;
                            return;
                        }

                        MiniMapBox.Visible = false;
                        break;
                    case KeyBindAction.MapBigWindow:
                        BigMapBox.ToggleOpen(!BigMapBox.Visible);
                        break;
                    case KeyBindAction.MailBoxWindow:
                        if (Observer) continue;
                        CommunicationBox.Visible = !CommunicationBox.Visible;
                        break;
                    case KeyBindAction.ChatOptionsWindow:
                        ChatOptionsBox.Visible = !ChatOptionsBox.Visible;
                        break;
                    case KeyBindAction.ExitGameWindow:
                        ExitBox.Visible = true;
                        ExitBox.BringToFront();
                        break;
                    case KeyBindAction.ChangeAttackMode:
                        if (Observer) continue;
                        User.AttackMode = (AttackMode)(((int)User.AttackMode + 1) % 5);
                        CEnvir.Enqueue(new C.ChangeAttackMode { Mode = User.AttackMode });
                        break;
                    case KeyBindAction.ChangePetMode:
                        if (Observer) continue;

                        User.PetMode = (PetMode)(((int)User.PetMode + 1) % 5);
                        CEnvir.Enqueue(new C.ChangePetMode { Mode = User.PetMode });
                        break;
                    case KeyBindAction.GroupAllowSwitch:
                        if (Observer) continue;

                        GroupBox.AllowGroupBox.InvokeMouseClick();
                        break;
                    case KeyBindAction.GroupTarget:
                        if (Observer) continue;

                        if (MouseObject == null || MouseObject.Race != ObjectType.Player) continue;

                        CEnvir.Enqueue(new C.GroupInvite { Name = MouseObject.Name });
                        break;
                    case KeyBindAction.TradeRequest:
                        if (Observer) continue;

                        CEnvir.Enqueue(new C.TradeRequest());
                        break;
                    case KeyBindAction.TradeAllowSwitch:
                        if (Observer) continue;

                        CEnvir.Enqueue(new C.Chat { Text = "@AllowTrade" });
                        break;
                    case KeyBindAction.ChangeChatMode:
                        ChatTextBox.ChatModeButton.InvokeMouseClick();
                        break;
                    case KeyBindAction.ItemPickUp:
                        if (Observer) continue;

                        if (CEnvir.Now > PickUpTime)
                        {
                            CEnvir.Enqueue(new C.PickUp());
                            PickUpTime = CEnvir.Now.AddMilliseconds(250);
                        }
                        break;
                    case KeyBindAction.PartnerTeleport:
                        if (Observer) continue;

                        CEnvir.Enqueue(new C.MarriageTeleport());
                        break;
                    case KeyBindAction.MountToggle:
                        if (Observer) continue;

                        if (CEnvir.Now < User.NextActionTime || User.ActionQueue.Count > 0) return;
                        if (CEnvir.Now < User.ServerTime) return; //Next Server response Time.

                        User.ServerTime = CEnvir.Now.AddSeconds(5);
                        CEnvir.Enqueue(new C.Mount());
                        break;
                    case KeyBindAction.AutoRunToggle:
                        if (Observer) continue;

                        AutoRun = !AutoRun;
                        break;
                    case KeyBindAction.UseBelt01:
                        if (Observer) continue;
                        if (e.Shift && Config.ShiftOpenChat) return;

                        if (BeltBox.Grid.Grid.Length > 0)
                        {
                            if (SelectedCell != null)
                                SelectedCell.MoveItem(BeltBox.Grid.Grid[0]);
                            else
                                BeltBox.Grid.Grid[0].UseItem();
                        }
                        break;
                    case KeyBindAction.UseBelt02:
                        if (Observer) continue;

                        if (BeltBox.Grid.Grid.Length > 1)
                        {
                            if (SelectedCell != null)
                                SelectedCell.MoveItem(BeltBox.Grid.Grid[1]);
                            else
                                BeltBox.Grid.Grid[1].UseItem();
                        }
                        break;
                    case KeyBindAction.UseBelt03:
                        if (Observer) continue;

                        if (BeltBox.Grid.Grid.Length > 2)
                        {
                            if (SelectedCell != null)
                                SelectedCell.MoveItem(BeltBox.Grid.Grid[2]);
                            else
                                BeltBox.Grid.Grid[2].UseItem();
                        }
                        break;
                    case KeyBindAction.UseBelt04:
                        if (Observer) continue;


                        if (BeltBox.Grid.Grid.Length > 3)
                        {
                            if (SelectedCell != null)
                                SelectedCell.MoveItem(BeltBox.Grid.Grid[3]);
                            else
                                BeltBox.Grid.Grid[3].UseItem();
                        }
                        break;
                    case KeyBindAction.UseBelt05:
                        if (Observer) continue;


                        if (BeltBox.Grid.Grid.Length > 4)
                        {
                            if (SelectedCell != null)
                                SelectedCell.MoveItem(BeltBox.Grid.Grid[4]);
                            else
                                BeltBox.Grid.Grid[4].UseItem();
                        }
                        break;
                    case KeyBindAction.UseBelt06:
                        if (Observer) continue;


                        if (BeltBox.Grid.Grid.Length > 5)
                        {
                            if (SelectedCell != null)
                                SelectedCell.MoveItem(BeltBox.Grid.Grid[5]);
                            else
                                BeltBox.Grid.Grid[5].UseItem();
                        }
                        break;
                    case KeyBindAction.UseBelt07:
                        if (Observer) continue;


                        if (BeltBox.Grid.Grid.Length > 6)
                        {
                            if (SelectedCell != null)
                                SelectedCell.MoveItem(BeltBox.Grid.Grid[6]);
                            else
                                BeltBox.Grid.Grid[6].UseItem();
                        }
                        break;
                    case KeyBindAction.UseBelt08:
                        if (Observer) continue;


                        if (BeltBox.Grid.Grid.Length > 7)
                        {
                            if (SelectedCell != null)
                                SelectedCell.MoveItem(BeltBox.Grid.Grid[7]);
                            else
                                BeltBox.Grid.Grid[7].UseItem();
                        }
                        break;
                    case KeyBindAction.UseBelt09:
                        if (Observer) continue;


                        if (BeltBox.Grid.Grid.Length > 8)
                        {
                            if (SelectedCell != null)
                                SelectedCell.MoveItem(BeltBox.Grid.Grid[8]);
                            else
                                BeltBox.Grid.Grid[8].UseItem();
                        }
                        break;
                    case KeyBindAction.UseBelt10:
                        if (Observer) continue;


                        if (BeltBox.Grid.Grid.Length > 9)
                        {
                            if (SelectedCell != null)
                                SelectedCell.MoveItem(BeltBox.Grid.Grid[9]);
                            else
                                BeltBox.Grid.Grid[9].UseItem();
                        }
                        break;

                    case KeyBindAction.SpellSet01:
                        MagicBarBox.SpellSet = 1;
                        break;
                    case KeyBindAction.SpellSet02:
                        MagicBarBox.SpellSet = 2;
                        break;
                    case KeyBindAction.SpellSet03:
                        MagicBarBox.SpellSet = 3;
                        break;
                    case KeyBindAction.SpellSet04:
                        MagicBarBox.SpellSet = 4;
                        break;

                    case KeyBindAction.SpellUse01:
                        if (Observer) continue;

                        UseMagic(SpellKey.Spell01);
                        break;
                    case KeyBindAction.SpellUse02:
                        if (Observer) continue;

                        UseMagic(SpellKey.Spell02);
                        break;
                    case KeyBindAction.SpellUse03:
                        if (Observer) continue;

                        UseMagic(SpellKey.Spell03);
                        break;
                    case KeyBindAction.SpellUse04:
                        if (Observer) continue;

                        UseMagic(SpellKey.Spell04);
                        break;
                    case KeyBindAction.SpellUse05:
                        if (Observer) continue;

                        UseMagic(SpellKey.Spell05);
                        break;
                    case KeyBindAction.SpellUse06:
                        if (Observer) continue;

                        UseMagic(SpellKey.Spell06);
                        break;
                    case KeyBindAction.SpellUse07:
                        if (Observer) continue;

                        UseMagic(SpellKey.Spell07);
                        break;
                    case KeyBindAction.SpellUse08:
                        if (Observer) continue;

                        UseMagic(SpellKey.Spell08);
                        break;
                    case KeyBindAction.SpellUse09:
                        if (Observer) continue;

                        UseMagic(SpellKey.Spell09);
                        break;
                    case KeyBindAction.SpellUse10:
                        if (Observer) continue;

                        UseMagic(SpellKey.Spell10);
                        break;
                    case KeyBindAction.SpellUse11:
                        if (Observer) continue;

                        UseMagic(SpellKey.Spell11);
                        break;
                    case KeyBindAction.SpellUse12:
                        if (Observer) continue;

                        UseMagic(SpellKey.Spell12);
                        break;
                    case KeyBindAction.SpellUse13:
                        if (Observer) continue;

                        UseMagic(SpellKey.Spell13);
                        break;
                    case KeyBindAction.SpellUse14:
                        if (Observer) continue;

                        UseMagic(SpellKey.Spell14);
                        break;
                    case KeyBindAction.SpellUse15:
                        if (Observer) continue;

                        UseMagic(SpellKey.Spell15);
                        break;
                    case KeyBindAction.SpellUse16:
                        if (Observer) continue;

                        UseMagic(SpellKey.Spell16);
                        break;
                    case KeyBindAction.SpellUse17:
                        if (Observer) continue;

                        UseMagic(SpellKey.Spell17);
                        break;
                    case KeyBindAction.SpellUse18:
                        if (Observer) continue;

                        UseMagic(SpellKey.Spell18);
                        break;
                    case KeyBindAction.SpellUse19:
                        if (Observer) continue;

                        UseMagic(SpellKey.Spell19);
                        break;
                    case KeyBindAction.SpellUse20:
                        if (Observer) continue;

                        UseMagic(SpellKey.Spell20);
                        break;
                    case KeyBindAction.SpellUse21:
                        if (Observer) continue;

                        UseMagic(SpellKey.Spell21);
                        break;
                    case KeyBindAction.SpellUse22:
                        if (Observer) continue;

                        UseMagic(SpellKey.Spell22);
                        break;
                    case KeyBindAction.SpellUse23:
                        if (Observer) continue;

                        UseMagic(SpellKey.Spell23);
                        break;
                    case KeyBindAction.SpellUse24:
                        if (Observer) continue;

                        UseMagic(SpellKey.Spell24);
                        break;
                    default:
                        continue;
                }

                e.Handled = true;
            }
        }

        private void CreateItemLabel()
        {
            if (ItemLabel != null && !ItemLabel.IsDisposed) ItemLabel.Dispose();
            if (MouseItem == null) return;

            ItemRefreshTime = CEnvir.Now.AddSeconds(1);

            ItemInfo displayInfo = GetItemLabelDisplayInfo(MouseItem);
            Stats itemStats = new Stats();
            itemStats.Add(MouseItem.Info.Stats);
            itemStats.Add(MouseItem.AddedStats);

            ItemLabelBuilder builder = new ItemLabelBuilder(MouseItem);

            string itemName = displayInfo.ItemName;
            if (MouseItem.Info.ItemEffect == ItemEffect.ItemPart)
                itemName += " - [Part]";

            builder.AddHeader(itemName, Color.Yellow, displayInfo.Rarity.ToString(), GetItemLabelRarityColour(displayInfo.Rarity));

            builder.StartSection();
            AddItemLabelMetadata(builder, displayInfo);

            if (CEnvir.IsCurrencyItem(MouseItem.Info) || MouseItem.Info.ItemEffect == ItemEffect.Experience)
            {
                AddItemLabelDescription(builder, displayInfo);
                builder.Complete();
                ItemLabel = builder.Label;
                return;
            }

            builder.StartSection();
            switch (displayInfo.ItemType)
            {
                case ItemType.Consumable:
                case ItemType.Scroll:
                    if (MouseItem.Info.ItemEffect == ItemEffect.StatExtractor || MouseItem.Info.ItemEffect == ItemEffect.RefineExtractor)
                        AddEquipmentItemInfo(builder, displayInfo);
                    else
                        AddPotionItemInfo(builder);
                    break;
                case ItemType.Book:
                case ItemType.Bundle:
                case ItemType.LootBox:
                    break;
                default:
                    AddEquipmentItemInfo(builder, displayInfo);
                    break;
            }

            AddItemLabelRequirements(builder, displayInfo);

            AddItemLabelSocketInfo(builder, displayInfo);

            AddItemLabelTradeState(builder, displayInfo);

            if (MouseItem.Info.Durability > 0 && MouseItem.Info.CanRepair && MouseItem.Info.StackSize == 1)
            {
                switch (MouseItem.Info.ItemType)
                {
                    case ItemType.Weapon:
                    case ItemType.Armour:
                    case ItemType.Helmet:
                    case ItemType.Necklace:
                    case ItemType.Bracelet:
                    case ItemType.Ring:
                    case ItemType.Shoes:
                    case ItemType.Shield:
                        builder.StartSection();

                        if (CEnvir.Now >= MouseItem.NextSpecialRepair)
                            builder.AddLine("Can Special Repair", Color.LimeGreen);
                        else
                            builder.AddLine($"Special Repair in {Functions.ToString(MouseItem.NextSpecialRepair - CEnvir.Now, true)}", Color.Red);
                        break;
                }
            }

            if ((MouseItem.Flags & UserItemFlags.Expirable) == UserItemFlags.Expirable)
            {
                builder.StartSection();
                builder.AddLine($"Expires in {Functions.ToString(MouseItem.ExpireTime, true)}", Color.Chocolate);
            }

            if (itemStats[Stat.ItemReviveTime] > 0)
            {
                DateTime value = MouseItem.Info.ItemEffect == ItemEffect.PillOfReincarnation ? ReincarnationPillTime : ItemReviveTime;

                builder.StartSection();
                if (CEnvir.Now >= value)
                    builder.AddLine("Revival ready", Color.LimeGreen);
                else
                    builder.AddLine($"Revival ready in {Functions.ToString(value - CEnvir.Now, true)}", Color.Red);
            }

            if (MouseItem.Info.Set != null)
            {
                builder.StartSection();
                AddSetItemInfo(builder, MouseItem.Info.Set);
            }

            if ((MouseItem.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage)
            {
                builder.StartSection();
                builder.AddLine("Wedding Ring.", Color.MediumOrchid);
            }

            if ((MouseItem.Flags & UserItemFlags.GameMaster) == UserItemFlags.GameMaster)
            {
                builder.StartSection();
                builder.AddLine("Created by a Game Master.", Color.LightSeaGreen);
            }

            if (NPCItemFragmentBox.IsVisible && MouseItem.CanFragment())
            {
                builder.StartSection();
                builder.AddLine($"Fragment Cost: {MouseItem.FragmentCost():#,##0}", Color.MediumAquamarine);
                builder.AddLine($"Fragments: {(MouseItem.Info.Rarity == Rarity.Common ? "Fragment" : "Fragment (II)")} x{MouseItem.FragmentCount():#,##0}", Color.MediumAquamarine);
            }

            if (CEnvir.Now < MouseItem.NextReset)
            {
                builder.StartSection();
                builder.AddLine($"Reset Available in {Functions.ToString(MouseItem.NextReset - CEnvir.Now, true)}", Color.Red);
            }

            if ((MouseItem.Flags & UserItemFlags.Locked) == UserItemFlags.Locked)
            {
                builder.StartSection();
                builder.AddIconLine("Locked: Prevents accidentally selling or throwing away\n[Middle Mouse Button] or [Scroll Lock] to Unlock.", Color.FromArgb(150, 135, 105), LibraryFile.GameInter, 370);
            }

            builder.Complete();
            ItemLabel = builder.Label;
        }

        private void CreateFameLabel()
        {
            if (MouseFame == null) return;

            ItemLabelBuilder builder = new ItemLabelBuilder();
            builder.AddHeader(MouseFame.Name, Color.Yellow, null, Color.Empty);

            if (!string.IsNullOrEmpty(MouseFame.Description))
            {
                builder.StartSection();
                builder.AddLine(Functions.BreakStringIntoLines(MouseFame.Description, 45), Color.Wheat);
            }

            if (MouseFame.BuffStats.Count > 0)
            {
                Stats stats = new Stats();
                foreach (FameInfoStat stat in MouseFame.BuffStats)
                    stats[stat.Stat] = stat.Amount;

                string statsText = string.Empty;
                foreach (KeyValuePair<Stat, int> pair in stats.Values)
                {
                    if (pair.Key == Stat.Duration) continue;

                    string temp = stats.GetDisplay(pair.Key);

                    if (temp == null) continue;
                    statsText += $"\n{temp}";
                }

                if (!string.IsNullOrEmpty(statsText))
                {
                    builder.StartSection();
                    builder.AddLine(statsText.Trim(), Color.White);
                }
            }

            builder.Complete();
            FameLabel = builder.Label;
        }

        private void CreateMagicLabel()
        {
            if (MouseMagic == null) return;

            ItemLabelBuilder builder = new ItemLabelBuilder();
            builder.AddHeader(MouseMagic.Name, Color.Yellow, null, Color.Empty);
            builder.AddLine($"<{MouseMagic.Property}>", Color.Yellow);

            ClientUserMagic magic;
            bool disciplineSkill = false;

            builder.StartSection();

            if (User.Magics.TryGetValue(MouseMagic, out magic))
            {
                int level = magic.Level;
                disciplineSkill = magic.Info.School == MagicSchool.Discipline;

                string text;
                if (magic.Level < Globals.MagicMaxLevel)
                {
                    text = magic.Level switch
                    {
                        0 => $"{magic.Experience}/{magic.Info.Experience1}",
                        1 => $"{magic.Experience}/{magic.Info.Experience2}",
                        2 => $"{magic.Experience}/{magic.Info.Experience3}",
                        _ => $"{magic.Experience}/{(magic.Level - 2) * 500}",
                    };
                }
                else
                {
                    text = $"Max Level";
                }

                builder.AddLine($"Current Level: {level}    Experience: {text}", Color.LimeGreen);
            }
            else
            {
                builder.AddLine($"Not learned", Color.Red);
            }

            builder.StartSection();
            builder.AddLine($"Rank 1 Requirement: Level {MouseMagic.NeedLevel1}    Experience: {MouseMagic.Experience1:#,##0}", User.Level < MouseMagic.NeedLevel1 ? Color.Red : Color.White);
            builder.AddLine($"Rank 2 Requirement: Level {MouseMagic.NeedLevel2}    Experience: {MouseMagic.Experience2:#,##0}", User.Level < MouseMagic.NeedLevel2 ? Color.Red : Color.White);
            builder.AddLine($"Rank 3 Requirement: Level {MouseMagic.NeedLevel3}    Experience: {MouseMagic.Experience3:#,##0}", User.Level < MouseMagic.NeedLevel3 ? Color.Red : Color.White);

            if (!disciplineSkill)
                builder.AddLine($"Rank 4+ Requirement: Books", magic?.Level < 3 ? Color.Red : Color.White);

            if (!string.IsNullOrEmpty(MouseMagic.Description))
            {
                builder.StartSection();
                builder.AddLine(Functions.BreakStringIntoLines(MouseMagic.Description, 45), Color.Wheat);
            }

            builder.Complete();
            MagicLabel = builder.Label;

            if (disciplineSkill)
                MagicLabel.BorderColour = Color.LimeGreen;
        }

        private static Size GetItemLabelImageSize(ClientUserItem item)
        {
            if (item == null) return new Size(DXItemCell.CellWidth, DXItemCell.CellHeight);

            MirLibrary library;
            if (!CEnvir.LibraryList.TryGetValue(LibraryFile.Inventory, out library)) return new Size(DXItemCell.CellWidth, DXItemCell.CellHeight);

            int image = ItemLabelImageControl.GetImageIndex(item);
            if (image < 0) return new Size(DXItemCell.CellWidth, DXItemCell.CellHeight);

            Size size = library.GetSize(image);

            return size.IsEmpty ? new Size(DXItemCell.CellWidth, DXItemCell.CellHeight) : size;
        }

        private sealed class ItemLabelImageControl : DXControl
        {
            public ClientUserItem Item;

            public static int GetImageIndex(ClientUserItem item)
            {
                if (item == null) return -1;

                if (CEnvir.IsCurrencyItem(item.Info))
                    return CEnvir.CurrencyImage(item.Info, item.Count);

                ItemInfo info = item.Info;

                if (info.ItemEffect == ItemEffect.ItemPart && item.AddedStats[Stat.ItemIndex] > 0)
                    info = Globals.ItemInfoList.Binding.First(x => x.Index == item.AddedStats[Stat.ItemIndex]);

                return info.Image;
            }

            protected override void DrawControl()
            {
                base.DrawControl();

                MirLibrary library;
                int image = GetImageIndex(Item);

                if (image < 0 || !CEnvir.LibraryList.TryGetValue(LibraryFile.Inventory, out library)) return;

                library.Draw(image, DisplayArea.X, DisplayArea.Y, Color.White, false, 1F, ImageType.Image);

                if (Item?.Colour != Color.Empty)
                    library.Draw(image, DisplayArea.X, DisplayArea.Y, Item.Colour, false, 1F, ImageType.Overlay);
            }
        }

        private sealed class ScaledItemLabelIcon : DXImageControl
        {
            public float RenderScale { get; set; } = 1F;

            public void SetRenderScale(float scale)
            {
                RenderScale = scale;
                if (Library == null || !Library.TryGetTexture(Index, ImageType.Image, out MirImage image, out _, out _)) return;

                FixedSize = true;
                Size = new Size(
                    Math.Max(1, (int)Math.Ceiling(image.Width * scale)),
                    Math.Max(1, (int)Math.Ceiling(image.Height * scale)));
            }

            protected override void DrawMirTexture()
            {
                if (Library == null || !Library.TryGetTexture(Index, ImageType.Image, out MirImage image, out RenderTexture texture, out Rectangle? sourceRectangle)) return;

                Rectangle drawArea = new Rectangle(DisplayArea.Location, new Size(image.Width, image.Height));
                PresentTexture(texture, sourceRectangle, Parent, drawArea, IsEnabled ? ForeColour : Color.FromArgb(75, 75, 75), this, 0, 0, RenderScale, false);
                image.ExpireTime = Time.Now + Config.CacheDuration;
            }
        }

        private sealed class ItemLabelBuilder
        {
            private const int Padding = 6;
            private const int MinimumIconAreaWidth = 46;
            private const int MinimumTextWidth = 180;
            private const int DividerGap = 3;
            private const int LabelPadding = 4;

            private static readonly Color LabelBackColour = Color.FromArgb(230, 18, 15, 8);
            private static readonly Color LabelBorderColour = Color.FromArgb(105, 95, 62);
            private static readonly Color DividerColour = Color.FromArgb(125, 99, 83, 50);
            private static readonly Color MutedUnavailableColour = Color.FromArgb(150, 135, 105);

            private readonly Size _imageSize;
            private readonly int _iconAreaWidth;
            private readonly ItemLabelImageControl _imageControl;
            private readonly List<DXControl> _sections = new List<DXControl>();
            private readonly List<DXLabel> _rightAlignedHeaderLabels = new List<DXLabel>();

            private DXControl _currentSection;
            private int _currentSectionHeight;
            private int _textWidth = MinimumTextWidth;
            private bool _currentSectionHasLines;

            public DXControl Label { get; }

            public ItemLabelBuilder()
            {
                _imageSize = Size.Empty;
                _iconAreaWidth = 0;
                Label = CreateLabel();
            }

            public ItemLabelBuilder(ClientUserItem item)
            {
                _imageSize = GetItemLabelImageSize(item);
                _iconAreaWidth = Math.Max(MinimumIconAreaWidth, _imageSize.Width + Padding * 2);
                Label = CreateLabel();

                _imageControl = new ItemLabelImageControl
                {
                    IsControl = false,
                    Item = item,
                    Parent = Label,
                    Size = _imageSize,
                };
            }

            public void AddHeader(string text, Color colour, string rightText, Color rightColour)
            {
                DXLabel title = AddLine(text, colour);

                if (string.IsNullOrEmpty(rightText)) return;

                DXLabel rightLabel = new DXLabel
                {
                    ForeColour = rightColour,
                    Location = new Point(title.Location.X + title.Size.Width + 10, title.Location.Y),
                    Parent = _currentSection,
                    Text = rightText,
                };

                _rightAlignedHeaderLabels.Add(rightLabel);
                IncludeLabel(rightLabel);
            }

            public DXLabel AddLine(string text, Color colour, int indent = 0)
            {
                EnsureSection();

                DXLabel label = new DXLabel
                {
                    ForeColour = colour == Color.Gray ? MutedUnavailableColour : colour,
                    Location = new Point(LabelPadding + indent, _currentSectionHeight),
                    Parent = _currentSection,
                    Text = text,
                };

                _currentSectionHasLines = true;
                _currentSectionHeight = label.DisplayArea.Bottom;
                _currentSection.Size = new Size(_currentSection.Size.Width, _currentSectionHeight);
                IncludeLabel(label);

                return label;
            }

            public DXLabel AddIconLine(string text, Color colour, LibraryFile iconLibrary, int iconIndex, int indent = 0, float iconScale = 1F, int iconAreaWidth = 0)
            {
                EnsureSection();

                ScaledItemLabelIcon icon = new ScaledItemLabelIcon
                {
                    IsControl = false,
                    LibraryFile = iconLibrary,
                    Index = iconIndex,
                    Parent = _currentSection,
                };

                if (iconScale != 1F && icon.Size.Width > 0 && icon.Size.Height > 0)
                    icon.SetRenderScale(iconScale);

                int effectiveIconAreaWidth = iconAreaWidth > 0 ? iconAreaWidth : icon.Size.Width;
                int iconGap = effectiveIconAreaWidth > 0 ? 3 : 0;
                int labelX = LabelPadding + indent + effectiveIconAreaWidth + iconGap;
                DXLabel label = new DXLabel
                {
                    ForeColour = colour == Color.Gray ? MutedUnavailableColour : colour,
                    Location = new Point(labelX, _currentSectionHeight),
                    Parent = _currentSection,
                    Text = text,
                };

                int rowHeight = Math.Max(label.Size.Height, icon.Size.Height);

                icon.Location = new Point(LabelPadding + indent + Math.Max(0, (effectiveIconAreaWidth - icon.Size.Width) / 2), _currentSectionHeight + Math.Max(0, (rowHeight - icon.Size.Height) / 2));
                label.Location = new Point(labelX, _currentSectionHeight + Math.Max(0, (rowHeight - label.Size.Height) / 2));

                _currentSectionHasLines = true;
                _currentSectionHeight += rowHeight;
                IncludeControl(icon);
                IncludeLabel(label);
                _currentSection.Size = new Size(_currentSection.Size.Width, Math.Max(_currentSection.Size.Height, _currentSectionHeight));

                return label;
            }

            public void StartSection()
            {
                if (_currentSection == null || !_currentSectionHasLines) return;

                _currentSection = null;
                _currentSectionHeight = 0;
                _currentSectionHasLines = false;
            }

            public void AddSectionBottomPadding(int padding)
            {
                if (_currentSection == null || !_currentSectionHasLines || padding <= 0) return;

                _currentSectionHeight += padding;
                _currentSection.Size = new Size(_currentSection.Size.Width, Math.Max(_currentSection.Size.Height, _currentSectionHeight));
            }

            public void Complete()
            {
                AlignHeaderRightLabels();

                int textX = _iconAreaWidth > 0 ? _iconAreaWidth + Padding : Padding;
                int y = Padding;
                bool drewSection = false;

                foreach (DXControl section in _sections)
                {
                    if (section.Size.Height == 0) continue;

                    if (drewSection)
                    {
                        y += DividerGap;

                        new DXControl
                        {
                            BackColour = DividerColour,
                            DrawTexture = true,
                            IsControl = false,
                            Location = new Point(textX + 3, y),
                            Parent = Label,
                            Size = new Size(Math.Max(1, _textWidth - 6), 1),
                        };

                        y += DividerGap + 1;
                    }

                    section.Location = new Point(textX, y);
                    section.Size = new Size(_textWidth, section.Size.Height);
                    y += section.Size.Height;
                    drewSection = true;
                }

                int minimumHeight = _imageControl == null ? 0 : _imageSize.Height + Padding * 2;

                Label.Size = new Size(textX + _textWidth + Padding, Math.Max(y + Padding, minimumHeight));

                if (_imageControl != null)
                    _imageControl.Location = new Point(Padding + (_iconAreaWidth - _imageControl.Size.Width) / 2, Math.Max(Padding, (Label.Size.Height - _imageControl.Size.Height) / 2));
            }

            private static DXControl CreateLabel()
            {
                return new DXControl
                {
                    BackColour = LabelBackColour,
                    Border = true,
                    BorderColour = LabelBorderColour,
                    DrawTexture = true,
                    IsControl = false,
                    IsVisible = true,
                };
            }

            private void EnsureSection()
            {
                if (_currentSection != null) return;

                _currentSection = new DXControl
                {
                    IsControl = false,
                    Parent = Label,
                };

                _sections.Add(_currentSection);
            }

            private void IncludeLabel(DXLabel label)
            {
                _textWidth = Math.Max(_textWidth, label.Location.X + label.Size.Width + LabelPadding);
                _currentSection.Size = new Size(Math.Max(_currentSection.Size.Width, label.Location.X + label.Size.Width + LabelPadding), Math.Max(_currentSection.Size.Height, label.DisplayArea.Bottom));
            }

            private void IncludeControl(DXControl control)
            {
                _textWidth = Math.Max(_textWidth, control.Location.X + control.Size.Width + LabelPadding);
                _currentSection.Size = new Size(Math.Max(_currentSection.Size.Width, control.Location.X + control.Size.Width + LabelPadding), Math.Max(_currentSection.Size.Height, control.DisplayArea.Bottom));
            }

            private void AlignHeaderRightLabels()
            {
                foreach (DXLabel label in _rightAlignedHeaderLabels)
                {
                    int x = Math.Max(label.Location.X, _textWidth - label.Size.Width);
                    label.Location = new Point(x, label.Location.Y);
                    _textWidth = Math.Max(_textWidth, label.Location.X + label.Size.Width + LabelPadding);
                }
            }
        }

        private static ItemInfo GetItemLabelDisplayInfo(ClientUserItem item)
        {
            if (item.Info.ItemEffect == ItemEffect.ItemPart && item.AddedStats[Stat.ItemIndex] > 0)
                return Globals.ItemInfoList.Binding.First(x => x.Index == item.AddedStats[Stat.ItemIndex]);

            return item.Info;
        }

        private static Color GetItemLabelRarityColour(Rarity rarity)
        {
            switch (rarity)
            {
                case Rarity.Superior:
                    return Color.PaleGreen;
                case Rarity.Elite:
                    return Color.MediumPurple;
                default:
                    return Color.White;
            }
        }

        private void AddItemLabelMetadata(ItemLabelBuilder builder, ItemInfo displayInfo)
        {
            if (displayInfo.ItemType != ItemType.Nothing)
            {
                MemberInfo member = typeof(ItemType).GetMember(displayInfo.ItemType.ToString()).FirstOrDefault();
                DescriptionAttribute description = member?.GetCustomAttribute<DescriptionAttribute>();

                builder.AddLine($"Type: {description?.Description ?? displayInfo.ItemType.ToString()}", Color.White);
            }

            if (MouseItem.Info.Durability > 0)
            {
                Color durabilityColour = MouseItem.CurrentDurability == 0 ? Color.Red : Color.FromArgb(132, 255, 255);

                switch (displayInfo.ItemType)
                {
                    case ItemType.Book:
                        builder.AddLine($"Pages: {MouseItem.CurrentDurability}/{MouseItem.MaxDurability}", durabilityColour);
                        break;
                    case ItemType.Meat:
                        builder.AddLine($"Quality: {Math.Round(MouseItem.CurrentDurability / 1000M)}/{Math.Round(MouseItem.MaxDurability / 1000M)}", durabilityColour);
                        break;
                    case ItemType.Ore:
                        builder.AddLine($"Purity: {Math.Round(MouseItem.CurrentDurability / 1000M)}", MouseItem.CurrentDurability == 0 ? Color.Red : Color.White);
                        break;
                    case ItemType.SocketGem:

                        string gemType = MouseItem.Info.Shape switch
                        {
                            0 => "Piercing",
                            1 => "Weapon",
                            2 => "Armour",
                            3 => "Curse",
                            4 => "Reset",
                            _ => "Unknown"
                        };

                        builder.AddLine($"Gem Type: {gemType}", Color.Aquamarine);

                        decimal socketPurity = MouseItem.CurrentDurability / 1000M;
                        decimal maximumSocketPurity = Math.Max(0, Globals.MaxGemPurity);
                        string socketPurityText;

                        if (socketPurity <= maximumSocketPurity * 0.2M)
                            socketPurityText = "Lowest";
                        else if (socketPurity <= maximumSocketPurity * 0.4M)
                            socketPurityText = "Low";
                        else if (socketPurity <= maximumSocketPurity * 0.6M)
                            socketPurityText = "Medium";
                        else if (socketPurity <= maximumSocketPurity * 0.8M)
                            socketPurityText = "High";
                        else
                            socketPurityText = "Supreme";

                        builder.AddLine($"Purity: {socketPurityText}", MouseItem.CurrentDurability == 0 ? Color.Red : Color.White);
                        break;
                    default:
                        if (MouseItem.Info.StackSize == 1)
                            builder.AddLine($"Durability: {Math.Round(MouseItem.CurrentDurability / 1000M)}/{Math.Round(MouseItem.MaxDurability / 1000M)}", durabilityColour);
                        break;
                }
            }

            if (CEnvir.IsCurrencyItem(MouseItem.Info) || MouseItem.Info.ItemEffect == ItemEffect.Experience)
                builder.AddLine($"Amount: {MouseItem.Count:#,##0}", Color.White);
            else if (MouseItem.Info.ItemEffect == ItemEffect.ItemPart)
                builder.AddLine($"Parts: {MouseItem.Count}/{displayInfo.PartCount}.", Color.White);
            else if (MouseItem.Info.StackSize > 1)
                builder.AddLine($"Count: {MouseItem.Count}/{MouseItem.Info.StackSize}", Color.White);

            if (displayInfo.ItemType == ItemType.LootBox)
            {
                int remainingRerolls = MouseItem.AddedStats[Stat.Counter1];
                int lootBoxState = MouseItem.AddedStats[Stat.Counter2];

                if (lootBoxState > 1)
                {
                    int openCount = 0;

                    for (int i = 0; i < LootBoxInfo.SlotSize; i++)
                    {
                        if ((MouseItem.CurrentDurability & (1 << i)) != 0)
                            openCount++;
                    }

                    builder.AddLine($"Open Count: {openCount}/{LootBoxInfo.SlotSize}", Color.White);
                }
                else
                {
                    builder.AddLine($"Reroll Count: {remainingRerolls}/{Globals.LootBoxRerollCount}", Color.White);
                }
            }

            if (MouseItem.Info.Weight > 0)
                builder.AddLine($"Weight: {MouseItem.Info.Weight}", GetItemLabelWeightColour());
        }

        private void AddItemLabelSocketInfo(ItemLabelBuilder builder, ItemInfo displayInfo)
        {
            if (displayInfo.ItemType != ItemType.Weapon && displayInfo.ItemType != ItemType.Armour) return;

            IEnumerable<ClientUserItemSocket> sockets = MouseItem.Sockets ?? Enumerable.Empty<ClientUserItemSocket>();
            bool sectionStarted = false;

            foreach (ClientUserItemSocket socket in sockets.OrderBy(x => x.Slot))
            {
                ClientUserItem gemItem = socket.Gem;
                if (gemItem == null)
                {
                    if (!sectionStarted)
                    {
                        builder.StartSection();
                        sectionStarted = true;
                    }

                    builder.AddLine("Empty Socket", Color.Gray);
                    continue;
                }

                ItemInfo gem = gemItem.Info;
                if (gem == null) continue;

                Stats gemStats = new Stats(gem.Stats);
                gemStats.Add(gemItem.AddedStats);
                bool iconAdded = false;
                int statIndent = 0;

                foreach (KeyValuePair<Stat, int> pair in gemStats.Values)
                {
                    string text = gemStats.GetDisplay(pair.Key);
                    if (text == null) continue;

                    if (!sectionStarted)
                    {
                        builder.StartSection();
                        sectionStarted = true;
                    }

                    Color colour = Color.FromArgb(140, 220, 89);
                    if (!iconAdded)
                    {
                        DXLabel firstStatLabel = builder.AddIconLine(text, colour, LibraryFile.StoreItem, gem.Image, 0, 0.5F, 16);
                        statIndent = Math.Max(0, firstStatLabel.Location.X - 3);
                        iconAdded = true;
                    }
                    else
                        builder.AddLine(text, colour, statIndent);
                }
            }

            if (sectionStarted)
                builder.AddSectionBottomPadding(3);
        }

        private Color GetItemLabelWeightColour()
        {
            switch (MouseItem.Info.ItemType)
            {
                case ItemType.Weapon:
                case ItemType.Shield:
                case ItemType.Torch:
                case ItemType.Hook:
                case ItemType.Float:
                case ItemType.Reel:
                    if (User.HandWeight - (Equipment[(int)EquipmentSlot.Weapon]?.Info.Weight ?? 0) + MouseItem.Info.Weight > User.Stats[Stat.HandWeight])
                        return Color.Red;
                    break;
                case ItemType.Armour:
                case ItemType.Helmet:
                case ItemType.Necklace:
                case ItemType.Bracelet:
                case ItemType.Ring:
                case ItemType.Shoes:
                case ItemType.Poison:
                case ItemType.Amulet:
                case ItemType.Bait:
                case ItemType.Finder:
                    if (User.WearWeight - (Equipment[(int)EquipmentSlot.Armour]?.Info.Weight ?? 0) + MouseItem.Info.Weight > User.Stats[Stat.WearWeight])
                        return Color.Red;
                    break;
            }

            return Color.White;
        }

        private void AddEquipmentItemInfo(ItemLabelBuilder builder, ItemInfo displayInfo)
        {
            Stats stats = new Stats();
            stats.Add(displayInfo.Stats, displayInfo.ItemType != ItemType.Weapon);
            stats.Add(MouseItem.AddedStats, MouseItem.Info.ItemType != ItemType.Weapon);

            if (displayInfo.ItemType == ItemType.Weapon)
            {
                Stat element = MouseItem.AddedStats.GetWeaponElement();

                if (element == Stat.None)
                    element = displayInfo.Stats.GetWeaponElement();

                if (element != Stat.None)
                    stats[element] += MouseItem.AddedStats.GetWeaponElementValue() + displayInfo.Stats.GetWeaponElementValue();
            }

            bool firstElement = stats.HasElementalWeakness();
            foreach (KeyValuePair<Stat, int> pair in stats.Values)
            {
                string text = stats.GetDisplay(pair.Key);
                if (text == null) continue;

                string added = MouseItem.AddedStats.GetFormat(pair.Key);
                Color colour = Color.White;

                switch (pair.Key)
                {
                    case Stat.Luck:
                        colour = Color.Yellow;
                        break;
                    case Stat.Strength:
                        colour = Color.FromArgb(148, 255, 206);
                        break;
                    case Stat.DropRate:
                    case Stat.ExperienceRate:
                    case Stat.SkillRate:
                    case Stat.GoldRate:
                        colour = Color.Yellow;
                        if (added != null)
                            text += $" ({added})";
                        break;
                    case Stat.FireAttack:
                    case Stat.IceAttack:
                    case Stat.LightningAttack:
                    case Stat.WindAttack:
                    case Stat.HolyAttack:
                    case Stat.DarkAttack:
                    case Stat.PhantomAttack:
                        colour = Color.DeepSkyBlue;
                        break;
                    case Stat.FireResistance:
                    case Stat.IceResistance:
                    case Stat.LightningResistance:
                    case Stat.WindResistance:
                    case Stat.HolyResistance:
                    case Stat.DarkResistance:
                    case Stat.PhantomResistance:
                    case Stat.PhysicalResistance:
                        colour = !firstElement ? Color.Lime : Color.IndianRed;
                        firstElement = true;
                        break;
                    default:
                        if (MouseItem.AddedStats[pair.Key] != 0)
                        {
                            text += $"   ({added})";
                            colour = Color.FromArgb(148, 255, 206);
                        }
                        break;
                }

                builder.AddLine(text, colour);
            }

            AddItemLabelTrainingInfo(builder, displayInfo);
        }

        private void AddItemLabelTrainingInfo(ItemLabelBuilder builder, ItemInfo displayInfo)
        {
            switch (displayInfo.ItemType)
            {
                case ItemType.Weapon:
                    if ((MouseItem.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return;

                    builder.StartSection();
                    builder.AddLine($"{displayInfo.ItemType} Level: " + (MouseItem.Level < Globals.WeaponExperienceList.Count ? MouseItem.Level.ToString() : "Max"), Color.White);

                    if (MouseItem.Level >= Globals.WeaponExperienceList.Count) return;

                    if ((MouseItem.Flags & UserItemFlags.Refinable) == UserItemFlags.Refinable)
                        builder.AddLine("Ready for Refine", Color.LightGreen);
                    else
                        builder.AddLine($"{displayInfo.ItemType} Training Points: {MouseItem.Experience / Globals.WeaponExperienceList[MouseItem.Level]:0.##%}", Color.White);
                    break;
                case ItemType.Necklace:
                case ItemType.Bracelet:
                case ItemType.Ring:
                    if ((MouseItem.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return;

                    builder.StartSection();
                    builder.AddLine($"{displayInfo.ItemType} Level: " + (MouseItem.Level < Globals.AccessoryExperienceList.Count ? MouseItem.Level.ToString() : "Max"), Color.White);

                    if (MouseItem.Level >= Globals.AccessoryExperienceList.Count) return;

                    if ((MouseItem.Flags & UserItemFlags.Refinable) == UserItemFlags.Refinable)
                        builder.AddLine("Ready for Refine", Color.LightGreen);
                    else
                        builder.AddLine($"{displayInfo.ItemType} Training Points: {MouseItem.Experience / Globals.AccessoryExperienceList[MouseItem.Level]:0.##%}", Color.White);
                    break;
            }
        }

        private void AddPotionItemInfo(ItemLabelBuilder builder)
        {
            Stats stats = new Stats();
            stats.Add(MouseItem.Info.Stats);

            foreach (KeyValuePair<Stat, int> pair in stats.Values)
            {
                string text = stats.GetDisplay(pair.Key);
                if (text == null) continue;

                Color colour = Color.White;
                switch (pair.Key)
                {
                    case Stat.Luck:
                    case Stat.DropRate:
                    case Stat.ExperienceRate:
                    case Stat.SkillRate:
                    case Stat.GoldRate:
                        colour = Color.Yellow;
                        break;
                    case Stat.DeathDrops:
                        colour = Color.OrangeRed;
                        break;
                }

                builder.AddLine(text, colour);
            }

            if (MouseItem.Info.Durability > 0)
                builder.AddLine($"Cooldown: {Functions.ToString(TimeSpan.FromMilliseconds(MouseItem.Info.Durability), true)}", Color.White);
        }

        private void AddItemLabelRequirements(ItemLabelBuilder builder, ItemInfo displayInfo)
        {
            bool started = false;

            if (displayInfo.RequiredGender != RequiredGender.None)
            {
                if (!started)
                {
                    builder.StartSection();
                    started = true;
                }

                Color colour = Color.White;
                switch (User.Gender)
                {
                    case MirGender.Male:
                        if (!displayInfo.RequiredGender.HasFlag(RequiredGender.Male))
                            colour = Color.Red;
                        break;
                    case MirGender.Female:
                        if (!displayInfo.RequiredGender.HasFlag(RequiredGender.Female))
                            colour = Color.Red;
                        break;
                }

                builder.AddLine($"Required Gender: {displayInfo.RequiredGender}", colour);
            }

            if (displayInfo.RequiredClass != RequiredClass.All)
            {
                if (!started)
                {
                    builder.StartSection();
                    started = true;
                }

                Color colour = Color.White;
                switch (User.Class)
                {
                    case MirClass.Warrior:
                        if (!displayInfo.RequiredClass.HasFlag(RequiredClass.Warrior))
                            colour = Color.Red;
                        break;
                    case MirClass.Wizard:
                        if (!displayInfo.RequiredClass.HasFlag(RequiredClass.Wizard))
                            colour = Color.Red;
                        break;
                    case MirClass.Taoist:
                        if (!displayInfo.RequiredClass.HasFlag(RequiredClass.Taoist))
                            colour = Color.Red;
                        break;
                    case MirClass.Assassin:
                        if (!displayInfo.RequiredClass.HasFlag(RequiredClass.Assassin))
                            colour = Color.Red;
                        break;
                }

                Type type = displayInfo.RequiredClass.GetType();
                MemberInfo[] infos = type.GetMember(displayInfo.RequiredClass.ToString());
                DescriptionAttribute description = infos[0].GetCustomAttribute<DescriptionAttribute>();

                builder.AddLine($"Required Class: {description?.Description ?? displayInfo.RequiredClass.ToString()}", colour);
            }

            if (displayInfo.RequiredAmount <= 0) return;

            if (!started)
                builder.StartSection();

            string text;
            Color requiredColour = displayInfo.Rarity == Rarity.Common ? Color.White : Color.FromArgb(0, 204, 0);
            switch (displayInfo.RequiredType)
            {
                case RequiredType.Level:
                    text = $"Required Level: {displayInfo.RequiredAmount}";
                    if (User.Level < displayInfo.RequiredAmount && User.Stats[Stat.Rebirth] == 0)
                        requiredColour = Color.Red;
                    break;
                case RequiredType.MaxLevel:
                    text = $"Max Level: {displayInfo.RequiredAmount}";
                    if (User.Level > displayInfo.RequiredAmount || User.Stats[Stat.Rebirth] > 0)
                        requiredColour = Color.Red;
                    break;
                case RequiredType.AC:
                    text = $"Required AC: {displayInfo.RequiredAmount}";
                    if (User.Stats[Stat.MaxAC] < displayInfo.RequiredAmount)
                        requiredColour = Color.Red;
                    break;
                case RequiredType.MR:
                    text = $"Required MR: {displayInfo.RequiredAmount}";
                    if (User.Stats[Stat.MaxMR] < displayInfo.RequiredAmount)
                        requiredColour = Color.Red;
                    break;
                case RequiredType.DC:
                    text = $"Required DC: {displayInfo.RequiredAmount}";
                    if (User.Stats[Stat.MaxDC] < displayInfo.RequiredAmount)
                        requiredColour = Color.Red;
                    break;
                case RequiredType.MC:
                    text = $"Required MC: {displayInfo.RequiredAmount}";
                    if (User.Stats[Stat.MaxMC] < displayInfo.RequiredAmount)
                        requiredColour = Color.Red;
                    break;
                case RequiredType.SC:
                    text = $"Required SC: {displayInfo.RequiredAmount}";
                    if (User.Stats[Stat.MaxSC] < displayInfo.RequiredAmount)
                        requiredColour = Color.Red;
                    break;
                case RequiredType.Health:
                    text = $"Required Health: {displayInfo.RequiredAmount}";
                    if (User.Stats[Stat.Health] < displayInfo.RequiredAmount)
                        requiredColour = Color.Red;
                    break;
                case RequiredType.Mana:
                    text = $"Required Mana: {displayInfo.RequiredAmount}";
                    if (User.Stats[Stat.Mana] < displayInfo.RequiredAmount)
                        requiredColour = Color.Red;
                    break;
                case RequiredType.CompanionLevel:
                    text = $"Companion Level: {displayInfo.RequiredAmount}";
                    if (Companion == null || Companion.Level < displayInfo.RequiredAmount)
                        requiredColour = Color.Red;
                    break;
                case RequiredType.MaxCompanionLevel:
                    text = $"Max Companion Level: {displayInfo.RequiredAmount}";
                    if (Companion == null || Companion.Level > displayInfo.RequiredAmount)
                        requiredColour = Color.Red;
                    break;
                case RequiredType.RebirthLevel:
                    text = $"Rebirth Level: {displayInfo.RequiredAmount}";
                    if (User.Stats[Stat.Rebirth] < displayInfo.RequiredAmount)
                        requiredColour = Color.Red;
                    break;
                case RequiredType.MaxRebirthLevel:
                    text = $"Rebirth Level: {displayInfo.RequiredAmount}";
                    if (User.Stats[Stat.Rebirth] > displayInfo.RequiredAmount)
                        requiredColour = Color.Red;
                    break;
                default:
                    text = "Unknown Type Required";
                    break;
            }

            builder.AddLine(text, requiredColour);
        }

        private void AddItemLabelTradeState(ItemLabelBuilder builder, ItemInfo displayInfo)
        {
            long sale = MouseItem.Price(Math.Max(1, MouseItem.Count));
            bool hasLines = false;

            if (sale > 0)
            {
                builder.StartSection();
                hasLines = true;
                builder.AddLine($"Sell Value: {sale}", Color.LightGoldenrodYellow);
            }

            if (MouseItem.Info.Durability > 0 && !MouseItem.Info.CanRepair && MouseItem.Info.StackSize == 1)
            {
                switch (MouseItem.Info.ItemType)
                {
                    case ItemType.Weapon:
                    case ItemType.Armour:
                    case ItemType.Helmet:
                    case ItemType.Necklace:
                    case ItemType.Bracelet:
                    case ItemType.Ring:
                    case ItemType.Shoes:
                    case ItemType.Shield:
                        if (!hasLines)
                        {
                            builder.StartSection();
                            hasLines = true;
                        }
                        builder.AddLine("Cannot be repaired.", Color.Yellow);
                        break;
                }
            }

            if (!MouseItem.Info.CanSell || (MouseItem.Flags & UserItemFlags.Worthless) == UserItemFlags.Worthless)
            {
                if (!hasLines)
                {
                    builder.StartSection();
                    hasLines = true;
                }
                builder.AddLine("Cannot be sold.", Color.Yellow);
            }

            if (!MouseItem.Info.CanStore)
            {
                if (!hasLines)
                {
                    builder.StartSection();
                    hasLines = true;
                }
                builder.AddLine("Cannot be stored.", Color.Yellow);
            }

            if (!MouseItem.Info.CanTrade || (MouseItem.Flags & UserItemFlags.Bound) == UserItemFlags.Bound)
            {
                if (!hasLines)
                {
                    builder.StartSection();
                    hasLines = true;
                }
                builder.AddLine("Cannot be traded.", Color.Yellow);
            }

            if (!MouseItem.Info.CanDrop)
            {
                if (!hasLines)
                {
                    builder.StartSection();
                    hasLines = true;
                }
                builder.AddLine("Cannot be dropped.", Color.Yellow);
            }

            if (!MouseItem.Info.CanDeathDrop || (MouseItem.Flags & UserItemFlags.Worthless) == UserItemFlags.Worthless || (MouseItem.Flags & UserItemFlags.Bound) == UserItemFlags.Bound)
            {
                if (!hasLines)
                {
                    builder.StartSection();
                    hasLines = true;
                }
                builder.AddLine("Cannot be dropped on death.", Color.Yellow);
            }

            if ((MouseItem.Flags & UserItemFlags.Bound) == UserItemFlags.Bound)
            {
                if (!hasLines)
                {
                    builder.StartSection();
                    hasLines = true;
                }
                builder.AddLine("Bound Item.", Color.Yellow);
            }

            if ((MouseItem.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable)
            {
                if (!hasLines)
                {
                    builder.StartSection();
                    hasLines = true;
                }

                if (MouseItem.Info.ItemType == ItemType.Book)
                    builder.AddLine("Does not contain Level 4 Pages.", Color.Red);
                else
                    builder.AddLine("Cannot be Refined or Upgraded.", Color.Yellow);
            }
            else if (MouseItem.Info.ItemType == ItemType.Book)
            {
                if (!hasLines)
                {
                    builder.StartSection();
                    hasLines = true;
                }
                builder.AddLine("Contains high level Pages.", Color.Green);
            }

            AddItemLabelDescription(builder, displayInfo, hasLines);
        }

        private void AddItemLabelDescription(ItemLabelBuilder builder, ItemInfo displayInfo, bool useCurrentSection = false)
        {
            if (string.IsNullOrEmpty(displayInfo.Description)) return;

            string description = displayInfo.Description
                .Replace("\\r\\n", "\r\n")
                .Replace("\\n", "\n")
                .Replace("\\r", "\r");

            if (!useCurrentSection)
                builder.StartSection();

            builder.AddLine(description, displayInfo.ItemEffect == ItemEffect.FootBallWhistle ? Color.Red : Color.Wheat);
        }

        private void AddSetItemInfo(ItemLabelBuilder builder, SetInfo set)
        {
            builder.AddLine("Item Set:", Color.LimeGreen);
            builder.AddLine($"    {set.SetName}", Color.LimeGreen);
            builder.AddLine("Parts:", Color.LimeGreen);

            bool hasFullSet = true;
            List<int> counted = new List<int>();
            Stats setBonus = new Stats();

            int level;
            MirClass userClass;
            ClientUserItem[] equipment;

            DXItemCell cell = MouseControl as DXItemCell;
            if (cell?.GridType == GridType.Inspect)
            {
                level = InspectBox.Level;
                userClass = InspectBox.Class;
                equipment = InspectBox.Equipment;
            }
            else
            {
                level = User.Level;
                userClass = User.Class;
                equipment = Equipment;
            }

            foreach (ItemInfo info in set.Items)
            {
                bool hasPart = false;
                for (int i = 0; i < equipment.Length; i++)
                {
                    if (counted.Contains(i)) continue;
                    if (equipment[i] == null) continue;
                    if (equipment[i].Info != info) continue;
                    if (equipment[i].CurrentDurability == 0 && equipment[i].Info.Durability > 0) continue;

                    counted.Add(i);
                    hasPart = true;
                    break;
                }

                if (!hasPart)
                    hasFullSet = false;

                builder.AddLine("    " + info.ItemName, hasPart ? Color.LimeGreen : Color.Gray);
            }

            builder.AddLine("Set Bonus:", Color.LimeGreen);

            foreach (SetInfoStat stat in set.SetStats)
            {
                if (level < stat.Level) continue;

                switch (userClass)
                {
                    case MirClass.Warrior:
                        if ((stat.Class & RequiredClass.Warrior) != RequiredClass.Warrior) continue;
                        break;
                    case MirClass.Wizard:
                        if ((stat.Class & RequiredClass.Wizard) != RequiredClass.Wizard) continue;
                        break;
                    case MirClass.Taoist:
                        if ((stat.Class & RequiredClass.Taoist) != RequiredClass.Taoist) continue;
                        break;
                    case MirClass.Assassin:
                        if ((stat.Class & RequiredClass.Assassin) != RequiredClass.Assassin) continue;
                        break;
                }

                setBonus[stat.Stat] += stat.Amount;
            }

            foreach (KeyValuePair<Stat, int> pair in setBonus.Values)
            {
                string text = setBonus.GetDisplay(pair.Key);
                if (text == null) continue;

                builder.AddLine("    " + text, hasFullSet ? Color.LimeGreen : Color.Gray);
            }
        }

        public void UseMagic(SpellKey key)
        {
            if (Game.Observer || User == null || User.Horse != HorseType.None || MagicBarBox == null) return;

            ClientUserMagic magic = null;

            foreach (KeyValuePair<MagicInfo, ClientUserMagic> pair in User.Magics)
            {
                switch (MagicBarBox.SpellSet)
                {
                    case 1:
                        if (pair.Value.Set1Key == key)
                            magic = pair.Value;
                        break;
                    case 2:
                        if (pair.Value.Set2Key == key)
                            magic = pair.Value;
                        break;
                    case 3:
                        if (pair.Value.Set3Key == key)
                            magic = pair.Value;
                        break;
                    case 4:
                        if (pair.Value.Set4Key == key)
                            magic = pair.Value;
                        break;
                }

                if (magic != null) break;
            }

            if (magic == null) return;

            if (magic.ItemRequired)
            {
                var magicItem = Equipment.FirstOrDefault(x => x != null && x.Info.ItemEffect == ItemEffect.MagicRing && x.Info.Shape == magic.Info.Index);

                if (magicItem == null) return;
            }
            else
            {
                if (User.Level < magic.Info.NeedLevel1) return;
            }

            switch (magic.Info.Magic)
            {
                case MagicType.Swordsmanship:
                case MagicType.SpiritSword:
                case MagicType.VineTreeDance:
                case MagicType.WillowDance:
                    return;
                case MagicType.Thrusting:
                    if (CEnvir.Now < ToggleTime) return;
                    ToggleTime = CEnvir.Now.AddSeconds(1);
                    CEnvir.Enqueue(new C.MagicToggle { Magic = magic.Info.Magic, CanUse = !User.CanThrusting });
                    return;
                case MagicType.HalfMoon:
                    if (CEnvir.Now < ToggleTime) return;
                    ToggleTime = CEnvir.Now.AddSeconds(1);
                    CEnvir.Enqueue(new C.MagicToggle { Magic = magic.Info.Magic, CanUse = !User.CanHalfMoon });
                    return;
                case MagicType.DestructiveSurge:
                    if (CEnvir.Now < ToggleTime) return;
                    ToggleTime = CEnvir.Now.AddSeconds(1);
                    CEnvir.Enqueue(new C.MagicToggle { Magic = magic.Info.Magic, CanUse = !User.CanDestructiveSurge });
                    return;
                case MagicType.FlamingSword:
                case MagicType.DragonRise:
                case MagicType.BladeStorm:
                case MagicType.DemonicRecovery:
                case MagicType.DefensiveBlow:
                case MagicType.OffensiveBlow:
                    if (CEnvir.Now < magic.NextCast || magic.Cost > User.CurrentMP) return;
                    magic.NextCast = CEnvir.Now.AddSeconds(0.5D); //Act as an anti spam
                    CEnvir.Enqueue(new C.MagicToggle { Magic = magic.Info.Magic });
                    return;
                case MagicType.FlameSplash:
                    if (CEnvir.Now < ToggleTime) return;
                    ToggleTime = CEnvir.Now.AddSeconds(1);
                    CEnvir.Enqueue(new C.MagicToggle { Magic = magic.Info.Magic, CanUse = !User.CanFlameSplash });
                    return;
                case MagicType.FullBloom:
                case MagicType.WhiteLotus:
                case MagicType.RedLotus:
                case MagicType.SweetBrier:
                    if (CEnvir.Now < ToggleTime || CEnvir.Now < magic.NextCast) return;

                    if (User.AttackMagic != magic.Info.Magic)
                    {
                        ReceiveChat(string.Format(CEnvir.Language.GameSceneSkillReady, magic.Info.Name), MessageType.Hint);
                        int attackDelay = Globals.AttackDelay - MapObject.User.Stats[Stat.AttackSpeed] * Globals.ASpeedRate;
                        attackDelay = Math.Max(800, attackDelay);

                        ToggleTime = CEnvir.Now + TimeSpan.FromMilliseconds(attackDelay + 200);

                        User.AttackMagic = magic.Info.Magic;
                    }
                    return;
                case MagicType.Karma:
                    if (CEnvir.Now < ToggleTime || CEnvir.Now < magic.NextCast || User.Buffs.All(x => x.Type != BuffType.Cloak)) return;

                    if (User.AttackMagic != magic.Info.Magic)
                    {
                        ReceiveChat(string.Format(CEnvir.Language.GameSceneSkillReady, magic.Info.Name), MessageType.Hint);
                        ToggleTime = CEnvir.Now + TimeSpan.FromMilliseconds(500);

                        User.AttackMagic = magic.Info.Magic;
                    }
                    return;
            }

            if (CEnvir.Now < User.NextMagicTime || User.Dead ||
                User.Buffs.Any(x => x.Type == BuffType.DragonRepulse) ||
                (User.Buffs.Any(x => x.Type == BuffType.ElementalHurricane) && magic.Info.Magic != MagicType.ElementalHurricane) ||
                (User.Poison & PoisonType.Paralysis) == PoisonType.Paralysis ||
                (User.Poison & PoisonType.Silenced) == PoisonType.Silenced) return;

            if (CEnvir.Now < magic.NextCast)
            {
                if (CEnvir.Now >= OutputTime)
                {
                    OutputTime = CEnvir.Now.AddSeconds(1);
                    ReceiveChat(string.Format(CEnvir.Language.GameSceneCastInCooldown, magic.Info.Name), MessageType.Hint);
                }
                return;
            }

            switch (magic.Info.Magic)
            {
                case MagicType.Cloak:
                    if (User.VisibleBuffs.ContainsKey(BuffType.Cloak)) break;
                    if (CEnvir.Now < User.CombatTime.AddSeconds(10))
                    {
                        if (CEnvir.Now >= OutputTime)
                        {
                            OutputTime = CEnvir.Now.AddSeconds(1);
                            ReceiveChat(string.Format(CEnvir.Language.GameSceneCastInCombat, magic.Info.Name), MessageType.Hint);
                        }
                        return;
                    }

                    if (User.Stats[Stat.Health] * magic.Cost / 1000 >= User.CurrentHP || User.CurrentHP < User.Stats[Stat.Health] / 10)
                    {
                        if (CEnvir.Now >= OutputTime)
                        {
                            OutputTime = CEnvir.Now.AddSeconds(1);
                            ReceiveChat(string.Format(CEnvir.Language.GameSceneCastNoEnoughHealth, magic.Info.Name), MessageType.Hint);
                        }
                        return;
                    }
                    break;
                case MagicType.DarkConversion:
                    if (User.VisibleBuffs.ContainsKey(BuffType.DarkConversion)) break;

                    if (magic.Cost > User.CurrentMP)
                    {
                        if (CEnvir.Now >= OutputTime)
                        {
                            OutputTime = CEnvir.Now.AddSeconds(1);
                            ReceiveChat(string.Format(CEnvir.Language.GameSceneCastNoEnoughMana, magic.Info.Name), MessageType.Hint);
                        }
                        return;
                    }
                    break;
                case MagicType.DragonRepulse:
                    if (User.Stats[Stat.Health] * magic.Cost / 1000 >= User.CurrentHP || User.CurrentHP < User.Stats[Stat.Health] / 10)
                    {
                        if (CEnvir.Now >= OutputTime)
                        {
                            OutputTime = CEnvir.Now.AddSeconds(1);
                            ReceiveChat(string.Format(CEnvir.Language.GameSceneCastNoEnoughHealth, magic.Info.Name), MessageType.Hint);
                        }
                        return;
                    }
                    if (User.Stats[Stat.Mana] * magic.Cost / 1000 >= User.CurrentMP || User.CurrentMP < User.Stats[Stat.Mana] / 10)
                    {
                        if (CEnvir.Now >= OutputTime)
                        {
                            OutputTime = CEnvir.Now.AddSeconds(1);
                            ReceiveChat(string.Format(CEnvir.Language.GameSceneCastNoEnoughMana, magic.Info.Name), MessageType.Hint);
                        }
                        return;
                    }
                    break;
                case MagicType.ElementalHurricane:
                    int cost = magic.Cost;
                    if (MapObject.User.VisibleBuffs.ContainsKey(BuffType.ElementalHurricane))
                        cost = 0;

                    if (cost > User.CurrentMP)
                    {
                        if (CEnvir.Now >= OutputTime)
                        {
                            OutputTime = CEnvir.Now.AddSeconds(1);
                            ReceiveChat($"Unable to cast {magic.Info.Name}, You do not have enough Mana.", MessageType.Hint);
                        }
                        return;
                    }
                    break;
                default:

                    if (magic.Cost > User.CurrentMP)
                    {
                        if (CEnvir.Now >= OutputTime)
                        {
                            OutputTime = CEnvir.Now.AddSeconds(1);
                            ReceiveChat(string.Format(CEnvir.Language.GameSceneCastNoEnoughMana, magic.Info.Name), MessageType.Hint);
                        }
                        return;
                    }
                    break;
            }
            MapObject target = null;
            MirDirection direction = MapControl.MouseDirection();

            switch (magic.Info.Magic)
            {
                case MagicType.ShoulderDash:
                    if (CEnvir.Now < User.ServerTime) return;
                    if ((User.Poison & PoisonType.WraithGrip) == PoisonType.WraithGrip) return;

                    User.ServerTime = CEnvir.Now.AddSeconds(5);
                    User.NextMagicTime = CEnvir.Now + Globals.MagicDelay;
                    CEnvir.Enqueue(new C.Magic { Direction = direction, Action = MirAction.Spell, Type = magic.Info.Magic });
                    return;

                case MagicType.DanceOfSwallow:
                    if (CEnvir.Now < User.ServerTime) return;
                    if (CanAttackTarget(MouseObject))
                        target = MouseObject;

                    if (target == null) return;

                    if (!Functions.InRange(target.CurrentLocation, User.CurrentLocation, Globals.MagicRange))
                    {
                        if (CEnvir.Now < OutputTime) return;
                        OutputTime = CEnvir.Now.AddSeconds(1);
                        ReceiveChat(string.Format(CEnvir.Language.GameSceneCastTooFar, magic.Info.Name), MessageType.Hint);
                        return;
                    }

                    User.ServerTime = CEnvir.Now.AddSeconds(5);
                    User.NextMagicTime = CEnvir.Now + Globals.MagicDelay;

                    MapObject.TargetObject = target;
                    MapObject.MagicObject = target;

                    CEnvir.Enqueue(new C.Magic { Action = MirAction.Spell, Type = magic.Info.Magic, Target = target.ObjectID });
                    return;

                case MagicType.FireBall:
                case MagicType.IceBolt:
                case MagicType.LightningBall:
                case MagicType.GustBlast:
                case MagicType.ElectricShock:
                case MagicType.AdamantineFireBall:
                case MagicType.FireBounce:
                case MagicType.ThunderBolt:
                case MagicType.ChainLightning:
                case MagicType.IceBlades:
                case MagicType.Cyclone:
                case MagicType.ExpelUndead:
                case MagicType.LightningStrike:
                case MagicType.IceRain:
                case MagicType.IceDragon:

                case MagicType.PoisonDust:
                case MagicType.ExplosiveTalisman:
                case MagicType.EvilSlayer:
                case MagicType.GreaterEvilSlayer:
                case MagicType.ImprovedExplosiveTalisman:
                case MagicType.Parasite:
                case MagicType.Neutralize:
                case MagicType.SearingLight:
                case MagicType.BindingTalisman:
                case MagicType.BrainStorm:

                case MagicType.Hemorrhage:
                case MagicType.FlamingDaggers:
                case MagicType.Shredding:
                    if (CanAttackTarget(MagicObject))
                        target = MagicObject;

                    if (CanAttackTarget(MouseObject))
                    {
                        target = MouseObject;

                        if (MouseObject.Race == ObjectType.Monster && ((MonsterObject)MouseObject).MonsterInfo.AI >= 0)
                            MapObject.MagicObject = target;
                        else
                            MapObject.MagicObject = null;
                    }
                    break;
                case MagicType.HundredFist:
                    if (CanAttackTarget(MagicObject))
                        target = MagicObject;
                    if (CanAttackTarget(MouseObject))
                    {
                        target = MouseObject;

                        if (MouseObject.Race == ObjectType.Monster && ((MonsterObject)MouseObject).MonsterInfo.AI >= 0)
                            MapObject.MagicObject = target;
                        else
                            MapObject.MagicObject = null;
                    }
                    if (target == null || !Functions.IsStraightEightDirection(User.CurrentLocation, target.CurrentLocation))
                        return;

                    break;

                case MagicType.WraithGrip:
                case MagicType.HellFire:
                case MagicType.Abyss:
                    if (CanAttackTarget(MouseObject))
                        target = MouseObject;
                    break;
                case MagicType.Interchange:
                case MagicType.Beckon:
                    if (CanAttackTarget(MouseObject))
                        target = MouseObject;
                    break;
                case MagicType.MagicCombustion:
                    if (!CanAttackTarget(MouseObject) || MouseObject.Race != ObjectType.Player) return;

                    target = MouseObject;
                    break;

                case MagicType.Heal:
                case MagicType.Purification:
                    target = MouseObject ?? User;
                    break;
                case MagicType.CelestialLight:
                    if (User.Buffs.All(x => x.Type == BuffType.CelestialLight)) return;
                    break;

                case MagicType.Resurrection:
                    if (MouseObject == null || !MouseObject.Dead || MouseObject.Race != ObjectType.Player) return;

                    target = MouseObject;
                    break;
                case MagicType.SoulResonance:
                    if (MouseObject == null || MouseObject.Dead || MouseObject.Race != ObjectType.Player || !IsAlly(MouseObject.ObjectID)) return;

                    target = MouseObject;
                    break;
                case MagicType.CursedDoll:
                    if (CanAttackTarget(MouseObject))
                        target = MouseObject;
                    break;
                case MagicType.CorpseExploder:
                case MagicType.SummonDead:
                    if (MouseObject != null && MouseObject.Dead && (MouseObject.Race == ObjectType.Player || MouseObject.Race == ObjectType.Monster))
                        target = MouseObject;
                    break;

                case MagicType.Spiritualism:
                    if (Equipment[(int)EquipmentSlot.Amulet] == null || Equipment[(int)EquipmentSlot.Amulet].Info.Shape != 0 || Equipment[(int)EquipmentSlot.Amulet].Count < 1) return;

                    direction = MirDirection.Down;
                    break;

                case MagicType.Rake:
                    if (!User.VisibleBuffs.ContainsKey(BuffType.Cloak)) return;
                    break;

                case MagicType.Chain:
                    if (CanAttackTarget(MouseObject) && MouseObject.Race == ObjectType.Monster)
                        target = MouseObject;
                    break;

                case MagicType.Defiance:
                case MagicType.Invincibility:
                    direction = MirDirection.Down;
                    break;
                case MagicType.Might:
                    direction = MirDirection.Down;
                    break;
                case MagicType.MassBeckon:
                    direction = MirDirection.Down;
                    break;
                case MagicType.ReflectDamage:
                    if (User.Buffs.Any(x => x.Type == BuffType.ReflectDamage)) return;
                    direction = MirDirection.Down;
                    break;
                case MagicType.Endurance:
                    direction = MirDirection.Down;
                    break;
                case MagicType.Renounce:
                    break;
                case MagicType.StrengthOfFaith:
                    break;
                case MagicType.MagicShield:
                    if (User.Buffs.Any(x => x.Type == BuffType.MagicShield || x.Type == BuffType.SuperiorMagicShield)) return;
                    break;
                case MagicType.SuperiorMagicShield:
                    if (User.Buffs.Any(x => x.Type == BuffType.SuperiorMagicShield)) return;
                    break;
                case MagicType.FrostBite:
                    if (User.Buffs.Any(x => x.Type == BuffType.FrostBite)) return;
                    break;
                case MagicType.JudgementOfHeaven:
                    break;

                case MagicType.SeismicSlam:
                case MagicType.CrushingWave:
                case MagicType.ElementalSwords:
                case MagicType.TaecheonSword:
                case MagicType.FireSword:

                case MagicType.Repulsion:
                case MagicType.ScortchedEarth:
                case MagicType.LightningBeam:
                case MagicType.Teleportation:
                case MagicType.FrozenEarth:
                case MagicType.BlowEarth:
                case MagicType.GreaterFrozenEarth:
                case MagicType.ThunderStrike:
                case MagicType.MirrorImage:
                case MagicType.ElementalHurricane:
                case MagicType.IceAura:
                case MagicType.IceBreaker:
                case MagicType.FrozenDragon:

                case MagicType.Invisibility:
                case MagicType.CombatKick:
                case MagicType.ThunderKick:
                case MagicType.Fetter:
                case MagicType.SummonSkeleton:
                case MagicType.SummonShinsu:
                case MagicType.SummonJinSkeleton:
                case MagicType.SummonDemonicCreature:
                case MagicType.DemonExplosion:
                case MagicType.DarkSoulPrison:
                case MagicType.HeavenlySky:
                case MagicType.PoisonCloud:

                case MagicType.PoisonousCloud:
                case MagicType.Cloak:
                case MagicType.SummonPuppet:
                case MagicType.TheNewBeginning:
                case MagicType.DarkConversion:
                case MagicType.DragonRepulse:
                case MagicType.FlashOfLight:
                case MagicType.Evasion:
                case MagicType.RagingWind:
                case MagicType.Concentration:
                case MagicType.Containment:
                case MagicType.FourWheels:
                case MagicType.CrescentMoon:
                    break;

                case MagicType.SwiftBlade:

                case MagicType.FireWall:
                case MagicType.FireStorm:
                case MagicType.LightningWave:
                case MagicType.IceStorm:
                case MagicType.DragonTornado:
                case MagicType.GeoManipulation:
                case MagicType.Transparency:
                case MagicType.MeteorShower:
                case MagicType.Tempest:
                case MagicType.Asteroid:
                case MagicType.Tornado:

                case MagicType.MagicResistance:
                case MagicType.Resilience:
                case MagicType.LifeSteal:
                case MagicType.MassInvisibility:
                case MagicType.TrapOctagon:
                case MagicType.ElementalSuperiority:
                case MagicType.BloodLust:
                case MagicType.MassHeal:

                case MagicType.BurningFire:
                    if (!Functions.InRange(MapControl.MapLocation, User.CurrentLocation, Globals.MagicRange))
                    {
                        if (CEnvir.Now < OutputTime) return;
                        OutputTime = CEnvir.Now.AddSeconds(1);
                        ReceiveChat(string.Format(CEnvir.Language.GameSceneCastTooFar, magic.Info.Name), MessageType.Hint);
                        return;
                    }
                    break;
                default:
                    return;
            }

            if (target != null && !Functions.InRange(target.CurrentLocation, User.CurrentLocation, Globals.MagicRange))
            {
                if (CEnvir.Now < OutputTime) return;
                OutputTime = CEnvir.Now.AddSeconds(1);
                ReceiveChat(string.Format(CEnvir.Language.GameSceneCastTooFar, magic.Info.Name), MessageType.Hint);
                return;
            }

            //Check Attack Range.

            if (target != null && target != User)
                direction = Functions.DirectionFromPoint(User.CurrentLocation, target.CurrentLocation);

            uint targetID = target?.ObjectID ?? 0;
            Point targetLocation;

            //Allows area casting instead of direct lock on (augmentations required)
            //targetID and separate maplocation passed through - allowing for target lock and area lookup to work
            switch (magic.Info.Magic)
            {
                case MagicType.Purification:
                case MagicType.EvilSlayer:
                case MagicType.GreaterEvilSlayer:
                case MagicType.ExplosiveTalisman:
                case MagicType.ImprovedExplosiveTalisman:
                case MagicType.PoisonDust:
                case MagicType.Neutralize:
                case MagicType.BindingTalisman:
                case MagicType.BrainStorm:
                    targetLocation = MapControl.MapLocation;
                    break;
                default:
                    targetLocation = target?.CurrentLocation ?? MapControl.MapLocation;
                    break;
            }

            //switch spell type.

            if (MouseObject != null && MouseObject.Race == ObjectType.Monster)
                FocusObject = (MonsterObject)MouseObject;

            User.MagicAction = new ObjectAction(MirAction.Spell, direction, MapObject.User.CurrentLocation, magic.Info.Magic, new List<uint> { targetID }, new List<Point> { targetLocation }, false, Element.None);
        }

        private bool CanAttackTarget(MapObject ob)
        {
            if (ob == null || ob.Dead || !ob.Visible) return false;

            switch (ob.Race)
            {
                case ObjectType.Player:
                    return true;
                case ObjectType.Monster:
                    MonsterObject mob = (MonsterObject)ob;

                    if (mob.MonsterInfo.AI < 0) return false;

                    return true;


                default:
                    return false;
            }
        }
        protected override void OnAfterDraw()
        {
            base.OnAfterDraw();

            int image = -1;
            Color color = Color.Empty;

            if (SelectedCell?.Item != null)
            {
                ItemInfo info = SelectedCell.Item.Info;

                if (info.ItemEffect == ItemEffect.ItemPart)
                    info = Globals.ItemInfoList.Binding.First(x => x.Index == SelectedCell.Item.AddedStats[Stat.ItemIndex]);

                image = info.Image;
                color = SelectedCell.Item.Colour;
            }
            else if (CurrencyPickedUp != null)
            {
                image = CEnvir.CurrencyImage(CurrencyPickedUp.Info.DropItem, CurrencyPickedUp.Amount);
            }

            MirLibrary library;

            if (image >= 0 && CEnvir.LibraryList.TryGetValue(LibraryFile.Inventory, out library))
            {
                Size imageSize = library.GetSize(image);
                Point p = new Point(CEnvir.MouseLocation.X - imageSize.Width / 2, CEnvir.MouseLocation.Y - imageSize.Height / 2);

                if (p.X + imageSize.Width >= Size.Width + Location.X)
                    p.X = Size.Width - imageSize.Width + Location.X;

                if (p.Y + imageSize.Height >= Size.Height + Location.Y)
                    p.Y = Size.Height - imageSize.Height + Location.Y;

                if (p.X < Location.X)
                    p.X = Location.X;

                if (p.Y <= Location.Y)
                    p.Y = Location.Y;


                library.Draw(image, p.X, p.Y, Color.White, false, 1f, ImageType.Image);

                if (color != Color.Empty)
                    library.Draw(image, p.X, p.Y, color, false, 1f, ImageType.Overlay);
            }

            if (ItemLabel != null && !ItemLabel.IsDisposed)
            {
                ItemLabel.Draw();
            }

            if (MagicLabel != null && !MagicLabel.IsDisposed)
            {
                MagicLabel.Draw();
            }

            if (FameLabel != null && !FameLabel.IsDisposed)
            {
                FameLabel.Draw();
            }

        }

        public void Displacement(MirDirection direction, Point location, bool clearQueue = false)
        {
            MapObject.User.ServerTime = DateTime.MinValue;
            MapObject.User.SetAction(new ObjectAction(MirAction.Standing, direction, location));
            MapObject.User.NextActionTime = CEnvir.Now.AddMilliseconds(300);

            if (clearQueue)
            {
                // Queue might contain actions at an old location (causing desync), so clear it out
                MapObject.User.ActionQueue.Clear();
            }
        }

        public void FillItems(List<ClientUserItem> items)
        {
            foreach (ClientUserItem item in items)
            {
                if (item.Slot >= Globals.EquipmentOffSet)
                {
                    CharacterBox.Grid[item.Slot - Globals.EquipmentOffSet].Item = item;
                    continue;
                }

                InventoryBox.Grid.Grid[item.Slot].Item = item;
            }
        }
        public void AddItems(List<ClientUserItem> items)
        {
            foreach (ClientUserItem item in items)
            {
                if (item.Info.ItemEffect == ItemEffect.Experience) continue;
                if ((item.Flags & UserItemFlags.QuestItem) == UserItemFlags.QuestItem) continue;

                var currency = User.GetCurrency(item.Info);
                if (currency != null)
                {
                    currency.Amount += item.Count;

                    GameScene.Game.CurrencyChanged();

                    if (currency.Info.Type == CurrencyType.Gold)
                        DXSoundManager.Play(SoundIndex.GoldGained);

                    continue;
                }

                bool handled = false;
                if (item.Info.StackSize > 1 && (item.Flags & UserItemFlags.Expirable) != UserItemFlags.Expirable)
                {
                    foreach (DXItemCell cell in InventoryBox.Grid.Grid)
                    {
                        if (cell.Item == null || cell.Item.Info != item.Info || cell.Item.Count >= cell.Item.Info.StackSize) continue;

                        if ((cell.Item.Flags & UserItemFlags.Expirable) == UserItemFlags.Expirable) continue;
                        if ((cell.Item.Flags & UserItemFlags.Bound) != (item.Flags & UserItemFlags.Bound)) continue;
                        if ((cell.Item.Flags & UserItemFlags.Worthless) != (item.Flags & UserItemFlags.Worthless)) continue;
                        if ((cell.Item.Flags & UserItemFlags.NonRefinable) != (item.Flags & UserItemFlags.NonRefinable)) continue;
                        if (!cell.Item.AddedStats.Compare(item.AddedStats)) continue;

                        if (cell.Item.Count + item.Count <= item.Info.StackSize)
                        {
                            cell.Item.Count += item.Count;
                            cell.RefreshItem();
                            handled = true;
                            break;
                        }

                        item.Count -= item.Info.StackSize - cell.Item.Count;
                        cell.Item.Count = item.Info.StackSize;
                        cell.RefreshItem();
                    }
                    if (handled) continue;
                }

                for (int i = 0; i < InventoryBox.Grid.Grid.Length; i++)
                {
                    if (InventoryBox.Grid.Grid[i].Item != null) continue;

                    InventoryBox.Grid.Grid[i].Item = item;
                    item.Slot = i;
                    break;
                }
            }
        }
        public void AddCompanionItems(List<ClientUserItem> items)
        {
            foreach (ClientUserItem item in items)
            {
                if (item.Info.ItemEffect == ItemEffect.Experience) continue;
                if ((item.Flags & UserItemFlags.QuestItem) == UserItemFlags.QuestItem) continue;

                var currency = User.GetCurrency(item.Info);
                if (currency != null)
                {
                    currency.Amount += item.Count;

                    GameScene.Game.CurrencyChanged();

                    if (currency.Info.Type == CurrencyType.Gold)
                        DXSoundManager.Play(SoundIndex.GoldGained);

                    continue;
                }

                bool handled = false;
                if (item.Info.StackSize > 1 && (item.Flags & UserItemFlags.Expirable) != UserItemFlags.Expirable)
                {
                    foreach (DXItemCell cell in CompanionBox.InventoryGrid.Grid)
                    {
                        if (cell.Item == null || cell.Item.Info != item.Info || cell.Item.Count >= cell.Item.Info.StackSize) continue;

                        if ((cell.Item.Flags & UserItemFlags.Expirable) == UserItemFlags.Expirable) continue;
                        if ((cell.Item.Flags & UserItemFlags.Bound) != (item.Flags & UserItemFlags.Bound)) continue;
                        if ((cell.Item.Flags & UserItemFlags.Worthless) != (item.Flags & UserItemFlags.Worthless)) continue;
                        if ((cell.Item.Flags & UserItemFlags.NonRefinable) != (item.Flags & UserItemFlags.NonRefinable)) continue;
                        if (!cell.Item.AddedStats.Compare(item.AddedStats)) continue;

                        if (cell.Item.Count + item.Count <= item.Info.StackSize)
                        {
                            cell.Item.Count += item.Count;
                            cell.RefreshItem();
                            handled = true;
                            break;
                        }

                        item.Count -= item.Info.StackSize - cell.Item.Count;
                        cell.Item.Count = item.Info.StackSize;
                        cell.RefreshItem();
                    }
                    if (handled) continue;
                }

                for (int i = 0; i < CompanionBox.InventoryGrid.Grid.Length; i++)
                {
                    if (CompanionBox.InventoryGrid.Grid[i].Item != null) continue;

                    CompanionBox.InventoryGrid.Grid[i].Item = item;
                    item.Slot = i;
                    break;
                }
            }
        }
        public bool CanUseItem(ClientUserItem item)
        {
            switch (User.Gender)
            {
                case MirGender.Male:
                    if (!item.Info.RequiredGender.HasFlag(RequiredGender.Male))
                        return false;
                    break;
                case MirGender.Female:
                    if (!item.Info.RequiredGender.HasFlag(RequiredGender.Female))
                        return false;
                    break;
            }

            switch (User.Class)
            {
                case MirClass.Warrior:
                    if (!item.Info.RequiredClass.HasFlag(RequiredClass.Warrior))
                        return false;
                    break;
                case MirClass.Wizard:
                    if (!item.Info.RequiredClass.HasFlag(RequiredClass.Wizard))
                        return false;
                    break;
                case MirClass.Taoist:
                    if (!item.Info.RequiredClass.HasFlag(RequiredClass.Taoist))
                        return false;
                    break;
                case MirClass.Assassin:
                    if (!item.Info.RequiredClass.HasFlag(RequiredClass.Assassin))
                        return false;
                    break;
            }
            switch (item.Info.RequiredType)
            {
                case RequiredType.Level:
                    if (User.Level < item.Info.RequiredAmount && User.Stats[Stat.Rebirth] == 0) return false;
                    break;
                case RequiredType.MaxLevel:
                    if (User.Level > item.Info.RequiredAmount || User.Stats[Stat.Rebirth] > 0) return false;
                    break;
                case RequiredType.AC:
                    if (User.Stats[Stat.MaxAC] < item.Info.RequiredAmount) return false;
                    break;
                case RequiredType.MR:
                    if (User.Stats[Stat.MaxMR] < item.Info.RequiredAmount) return false;
                    break;
                case RequiredType.DC:
                    if (User.Stats[Stat.MaxDC] < item.Info.RequiredAmount) return false;
                    break;
                case RequiredType.MC:
                    if (User.Stats[Stat.MaxMC] < item.Info.RequiredAmount) return false;
                    break;
                case RequiredType.SC:
                    if (User.Stats[Stat.MaxSC] < item.Info.RequiredAmount) return false;
                    break;
                case RequiredType.Health:
                    if (User.Stats[Stat.Health] < item.Info.RequiredAmount) return false;
                    break;
                case RequiredType.Mana:
                    if (User.Stats[Stat.Mana] < item.Info.RequiredAmount) return false;
                    break;
                case RequiredType.Accuracy:
                    if (User.Stats[Stat.Accuracy] < item.Info.RequiredAmount) return false;
                    break;
                case RequiredType.Agility:
                    if (User.Stats[Stat.Agility] < item.Info.RequiredAmount) return false;
                    break;
                case RequiredType.CompanionLevel:
                    if (Companion == null || Companion.Level < item.Info.RequiredAmount) return false;
                    break;
                case RequiredType.MaxCompanionLevel:
                    if (Companion == null || Companion.Level > item.Info.RequiredAmount) return false;
                    break;
                case RequiredType.RebirthLevel:
                    if (User.Stats[Stat.Rebirth] < item.Info.RequiredAmount) return false;
                    break;
                case RequiredType.MaxRebirthLevel:
                    if (User.Stats[Stat.Rebirth] > item.Info.RequiredAmount) return false;
                    break;
            }

            switch (item.Info.ItemType)
            {
                case ItemType.Book:
                    MagicInfo magic = Globals.MagicInfoList.Binding.FirstOrDefault(x => x.Index == item.Info.Shape);
                    if (magic == null) return false;
                    if (magic.School == MagicSchool.None) return false;
                    if (User.Magics.TryGetValue(magic, out ClientUserMagic value) && (value.Level < 3 || (item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable)) return false;
                    break;
                case ItemType.Consumable:
                    switch (item.Info.Shape)
                    {
                        case 1: //Item Buffs

                            ClientBuffInfo buff = User.Buffs.FirstOrDefault(x => x.Type == BuffType.ItemBuff && x.ItemIndex == item.Info.Index);

                            if (buff != null && buff.RemainingTime == TimeSpan.MaxValue) return false;
                            break;
                    }
                    break;
            }

            return true;
        }

        public bool CanWearItem(ClientUserItem item, EquipmentSlot slot)
        {
            if (!CanUseItem(item)) return false;

            switch (slot)
            {
                case EquipmentSlot.Weapon:
                case EquipmentSlot.Torch:
                case EquipmentSlot.Shield:
                    if (User.HandWeight - (Equipment[(int)slot]?.Info.Weight ?? 0) + item.Weight > User.Stats[Stat.HandWeight])
                    {
                        ReceiveChat(string.Format(CEnvir.Language.GameSceneHoldTooHeavy, item.Info.ItemName), MessageType.System);
                        return false;
                    }
                    break;
                case EquipmentSlot.Hook:
                case EquipmentSlot.Float:
                case EquipmentSlot.Bait:
                case EquipmentSlot.Finder:
                case EquipmentSlot.Reel:
                    if (Equipment[(int)EquipmentSlot.Weapon]?.Info.ItemEffect != ItemEffect.FishingRod)
                    {
                        ReceiveChat(string.Format(CEnvir.Language.GameSceneNeedFishingRod, item.Info.ItemName), MessageType.System);
                        return false;
                    }
                    break;
                default:
                    if (User.WearWeight - (Equipment[(int)slot]?.Info.Weight ?? 0) + item.Weight > User.Stats[Stat.WearWeight])
                    {
                        ReceiveChat(string.Format(CEnvir.Language.GameSceneWearTooHeavy, item.Info.ItemName), MessageType.System);
                        return false;
                    }
                    break;
            }

            return true;
        }

        public bool CanCompanionWearItem(ClientUserItem item, CompanionSlot slot)
        {
            if (Companion == null) return false;

            if (!CanCompanionUseItem(item.Info)) return false;

            return true;
        }
        public bool CanCompanionUseItem(ItemInfo info)
        {
            switch (info.RequiredType)
            {
                case RequiredType.CompanionLevel:
                    if (Companion == null || Companion.Level < info.RequiredAmount) return false;
                    break;
                case RequiredType.MaxCompanionLevel:
                    if (Companion == null || Companion.Level > info.RequiredAmount) return false;
                    break;
            }


            return true;
        }

        public void UserChanged()
        {
            LevelChanged();
            ClassChanged();
            StatsChanged();
            ExperienceChanged();
            HealthChanged();
            ManaChanged();
            FocusChanged();
            CurrencyChanged();
            SafeZoneChanged();
            AttackModeChanged();
            PetModeChanged();
            MagicBarBox.UpdateIcons();

            TradeBox.CloseButton.Enabled = !Observer;
            TradeBox.ConfirmButton.Visible = !Observer;

            NPCBox.CloseButton.Enabled = !Observer;
            NPCGoodsBox.CloseButton.Enabled = !Observer;
            NPCRefineBox.CloseButton.Enabled = !Observer;
            NPCRepairBox.CloseButton.Enabled = !Observer;
            NPCRefineRetrieveBox.CloseButton.Enabled = !Observer;
            NPCQuestBox.CloseButton.Enabled = !Observer;
        }
        public void LevelChanged()
        {
            if (User == null) return;

            //User.MaxExperience = User.Level < Globals.ExperienceList.Count ? Globals.ExperienceList[User.Level] : 0;
            MainPanel.LevelLabel.Text = User.Level.ToString();

            foreach (NPCGoodsCell cell in NPCGoodsBox.Cells)
                cell.UpdateColours();

            foreach (KeyValuePair<MagicInfo, MagicCell> pair in MagicBox.Magics)
                pair.Value.Refresh();

            CheckNewQuests();
        }
        public void ClassChanged()
        {
            if (User == null) return;

            MainPanel.ClassLabel.Text = User.Class.ToString();

            foreach (NPCGoodsCell cell in NPCGoodsBox.Cells)
                cell.UpdateColours();

            MainPanel.MCLabel.Visible = User.Class == MirClass.Wizard || User.Class == MirClass.Warrior;
            MainPanel.SCLabel.Visible = User.Class == MirClass.Taoist || User.Class == MirClass.Assassin;

            MagicBox?.CreateTabs();
        }
        public void StatsChanged()
        {
            if (User.Stats == null) return;

            User.Light = Math.Max(3, User.Stats[Stat.Light]);

            if (User.Stats[Stat.Light] == 0)
            {
                User.LightColour = Globals.PlayerLightColour;
            }
            else
            {
                User.LightColour = Globals.NoneColour;
            }

            MainPanel.ACLabel.Text = User.Stats.GetFormat(Stat.MaxAC);
            MainPanel.MACLabel.Text = User.Stats.GetFormat(Stat.MaxMR);

            MainPanel.DCLabel.Text = User.Stats.GetFormat(Stat.MaxDC);
            MainPanel.SCLabel.Text = User.Stats.GetFormat(Stat.MaxSC);
            MainPanel.MCLabel.Text = User.Stats.GetFormat(Stat.MaxMC);

            HealthChanged();
            ManaChanged();
            FocusChanged();

            foreach (NPCGoodsCell cell in NPCGoodsBox.Cells)
                cell.UpdateColours();

            CharacterBox.UpdateStats();

            FilterDropBox.UpdateDropFilters();
        }
        public void ExperienceChanged()
        {
            if (User == null) return;

            MainPanel.ExperienceBar.Hint = User.MaxExperience > 0 ? $"(Experience) {User.Experience / User.MaxExperience:#,##0.00%}" : "(Experience) Max";
        }
        public void HealthChanged()
        {
            if (User == null) return;

            MainPanel.HealthLabel.Text = $"{User.CurrentHP}/{User.Stats[Stat.Health]}";

        }
        public void ManaChanged()
        {
            if (User == null) return;

            MainPanel.ManaLabel.Text = $"{User.CurrentMP}/{User.Stats[Stat.Mana]}";
        }
        public void FocusChanged()
        {
            if (User == null) return;

            MainPanel.FocusLabel.Visible = User.Stats[Stat.Focus] > 0;
            MainPanel.FocusLabel.Text = $"{User.CurrentFP}/{User.Stats[Stat.Focus]}";
        }

        public void AttackModeChanged()
        {
            if (User == null) return;

            Type type = typeof(AttackMode);

            MemberInfo[] infos = type.GetMember(User.AttackMode.ToString());

            DescriptionAttribute description = infos[0].GetCustomAttribute<DescriptionAttribute>();

            MainPanel.AttackModeLabel.Text = description?.Description ?? User.AttackMode.ToString();
        }
        public void PetModeChanged()
        {
            if (User == null) return;

            Type type = typeof(PetMode);

            MemberInfo[] infos = type.GetMember(User.PetMode.ToString());

            DescriptionAttribute description = infos[0].GetCustomAttribute<DescriptionAttribute>();

            MainPanel.PetModeLabel.Text = description?.Description ?? User.PetMode.ToString();
        }
        public void CurrencyChanged()
        {
            if (User == null) return;

            InventoryBox.RefreshCurrency();

            MainPanel.FPLabel.Text = User.GetCurrency(CurrencyType.FP)?.Amount.ToString() ?? "0";
            MainPanel.CPLabel.Text = User.GetCurrency(CurrencyType.CP)?.Amount.ToString() ?? "0";

            GameStoreBox?.RefreshCurrency();
            NPCAdoptCompanionBox.RefreshUnlockButton();

            foreach (NPCGoodsCell cell in NPCGoodsBox.Cells)
            {
                cell.UpdateCosts();
                cell.UpdateColours();
            }

            //foreach (CurrencyCell cell in CurrencyBox.Cells)
            //{
            //    cell.UpdateAmount();
            //}
        }
        public void SafeZoneChanged()
        {

        }
        public void WeightChanged()
        {
            if (User == null) return;

            InventoryBox.WeightLabel.Text = $"{User.BagWeight} of {User.Stats[Stat.BagWeight]}";

            InventoryBox.WeightLabel.ForeColour = User.BagWeight > User.Stats[Stat.BagWeight] ? Color.Red : Color.White;

            CharacterBox.WearWeightLabel.Text = $"{User.WearWeight}/{User.Stats[Stat.WearWeight]}";
            CharacterBox.HandWeightLabel.Text = $"{User.HandWeight}/{User.Stats[Stat.HandWeight]}";

            CharacterBox.WearWeightLabel.ForeColour = User.WearWeight > User.Stats[Stat.WearWeight] ? Color.Red : Color.White;
            CharacterBox.HandWeightLabel.ForeColour = User.HandWeight > User.Stats[Stat.HandWeight] ? Color.Red : Color.White;
        }
        public void CompanionChanged()
        {
            NPCCompanionStorageBox.Refresh();

            CompanionBox.CompanionChanged();
        }
        public void MarriageChanged()
        {
            CharacterBox.MarriageIcon.Visible = !string.IsNullOrEmpty(Partner?.Name);
            CharacterBox.MarriageLabel.Visible = !string.IsNullOrEmpty(Partner?.Name);
            CharacterBox.MarriageLabel.Text = Partner?.Name;
        }

        public void ReceiveChat(string message, MessageType type, List<ClientUserItem> linkedItems = null)
        {
            if (Config.LogChat)
                CEnvir.ChatLog.Enqueue($"[{Time.Now:F}]: {message}");

            foreach (ChatTab tab in ChatTab.Tabs)
                tab.ReceiveChat(message, type, linkedItems);
        }
        public void ReceiveChat(MessageAction action, params object[] args)
        {
            foreach (ChatTab tab in ChatTab.Tabs)
                tab.ReceiveChat(action, args);
        }

        public bool CanAccept(QuestInfo quest)
        {
            if (quest?.StartNPC == null || quest.FinishNPC == null) return false;

            if (QuestLog.Any(x => x.Quest == quest)) return false;

            foreach (QuestRequirement requirement in quest.Requirements)
            {
                switch (requirement.Requirement)
                {
                    case QuestRequirementType.MinLevel:
                        if (User.Level < requirement.IntParameter1) return false;
                        break;
                    case QuestRequirementType.MaxLevel:
                        if (User.Level > requirement.IntParameter1) return false;
                        break;
                    case QuestRequirementType.NotAccepted:
                        if (QuestLog.Any(x => x.Quest == requirement.QuestParameter)) return false;

                        break;
                    case QuestRequirementType.HaveCompleted:
                        if (QuestLog.Any(x => x.Quest == requirement.QuestParameter && x.Completed)) break;

                        return false;
                    case QuestRequirementType.HaveNotCompleted:
                        if (QuestLog.Any(x => x.Quest == requirement.QuestParameter && x.Completed)) return false;

                        break;
                    case QuestRequirementType.Class:
                        switch (User.Class)
                        {
                            case MirClass.Warrior:
                                if ((requirement.Class & RequiredClass.Warrior) != RequiredClass.Warrior) return false;
                                break;
                            case MirClass.Wizard:
                                if ((requirement.Class & RequiredClass.Wizard) != RequiredClass.Wizard) return false;
                                break;
                            case MirClass.Taoist:
                                if ((requirement.Class & RequiredClass.Taoist) != RequiredClass.Taoist) return false;
                                break;
                            case MirClass.Assassin:
                                if ((requirement.Class & RequiredClass.Assassin) != RequiredClass.Assassin) return false;
                                break;
                        }
                        break;
                }

            }
            return true;
        }
        public void QuestChanged(ClientUserQuest quest)
        {
            CheckNewQuests();

            QuestBox.QuestChanged(quest);
        }
        public void CheckNewQuests()
        {
            QuestBox.PopulateQuests();

            QuestTrackerBox.PopulateQuests();

            NPCQuestListBox.UpdateQuestDisplay();

            UpdateQuestIcons();
        }
        public void CancelQuest(QuestInfo quest)
        {
            QuestBox.CancelQuest(quest);
        }

        public bool HasQuest(MonsterInfo info, MapInfo map)
        {
            foreach (QuestTaskMonsterDetails detail in info.QuestDetails)
            {
                if (detail.Map != null && detail.Map != map) continue;

                QuestInfo quest = QuestBox.CurrentTab.Quests.FirstOrDefault(x => x == detail.Task.Quest);

                if (quest == null) continue;

                ClientUserQuest userQuest = QuestLog.First(x => x.Quest == quest);

                if (userQuest.IsComplete) continue;

                ClientUserQuestTask UserTask = userQuest.Tasks.FirstOrDefault(x => x.Task == detail.Task);

                if (UserTask != null && UserTask.Completed) continue;

                return true;
            }

            return false;
        }

        public string GetQuestText(QuestInfo questInfo, ClientUserQuest userQuest, bool isLog)
        {
            string text;

            if (userQuest == null)
                text = questInfo.AcceptText; //Available
            else if (userQuest.Completed)
                text = questInfo.ArchiveText; //Completed
            else if (userQuest.IsComplete && !isLog)
                text = questInfo.CompletedText; //Completed
            else
                text = questInfo.ProgressText; //Current

            text = text.Replace("[PLAYERNAME]", User.Name, StringComparison.OrdinalIgnoreCase);
            text = text.Replace("[STARTNAME]", questInfo.StartNPC.NPCName, StringComparison.OrdinalIgnoreCase);
            text = text.Replace("[FINISHNAME]", questInfo.FinishNPC.NPCName, StringComparison.OrdinalIgnoreCase);

            return text;

        }

        public string GetTaskText(QuestInfo questInfo, ClientUserQuest userQuest)
        {
            StringBuilder builder = new StringBuilder();

            foreach (QuestTask task in questInfo.Tasks)
                builder.AppendLine(GetTaskText(task, userQuest));

            return builder.ToString(); //Available
        }
        public string GetTaskText(QuestTask task, ClientUserQuest userQuest)
        {
            StringBuilder builder = new StringBuilder();

            ClientUserQuestTask userTask = userQuest?.Tasks.FirstOrDefault(x => x.Task == task);

            switch (task.Task)
            {
                case QuestTaskType.KillMonster:
                    builder.AppendFormat("Kill {0} ", task.Amount);
                    break;
                case QuestTaskType.GainItem:
                    builder.AppendFormat("Collect {0} {1}", task.Amount, task.ItemParameter?.ItemName);
                    break;
                case QuestTaskType.Region:
                    builder.AppendFormat("Goto {0} in {1}", task.RegionParameter?.Description, task.RegionParameter?.Map.PlayerDescription);
                    break;
            }

            if (string.IsNullOrEmpty(task.MobDescription))
            {
                if (task.Task == QuestTaskType.GainItem && task.MonsterDetails.Count > 0)
                {
                    builder.Append(" from ");
                }

                bool needComma = false;
                for (int i = 0; i < task.MonsterDetails.Count; i++)
                {
                    QuestTaskMonsterDetails monster = task.MonsterDetails[i];
                    if (monster == null) continue;
                    if (i > 2)
                    {
                        builder.Append("...");
                        break;
                    }

                    if (needComma)
                        builder.Append(" or ");

                    needComma = true;

                    builder.Append(monster.Monster.MonsterName);

                    if (monster.Map != null)
                        builder.AppendFormat(" in {0}", monster.Map.PlayerDescription);
                }
            }
            else
            {
                if (task.Task == QuestTaskType.GainItem && task.MonsterDetails.Count > 0)
                {
                    builder.Append(" from ");
                }

                builder.Append(task.MobDescription);
            }

            if (userQuest != null)
            {
                if (userTask != null && userTask.Completed)
                    builder.Append(" (Completed)");
                else
                {
                    if (task.Task != QuestTaskType.Region)
                    {
                        builder.Append($" ({userTask?.Amount ?? 0}/{task.Amount})");
                    }
                }
            }

            return builder.ToString();
        }

        public void UpdateQuestIcons()
        {
            foreach (NPCInfo info in Globals.NPCInfoList.Binding)
                info.CurrentQuest = null;

            bool completed = false;

            foreach (QuestInfo quest in QuestBox.CurrentTab.Quests)
            {
                if (quest?.FinishNPC == null) continue;

                ClientUserQuest userQuest = QuestLog.First(x => x.Quest == quest);

                if (quest.FinishNPC.CurrentQuest != null) continue;

                var current = new CurrentQuest
                {
                    Type = quest.QuestType
                };

                if (userQuest.IsComplete)
                {
                    current.Icon = QuestIcon.Complete;
                    completed = true;
                }
                else
                {
                    current.Icon = QuestIcon.Incomplete;
                }

                quest.FinishNPC.CurrentQuest = current;
            }

            foreach (QuestInfo quest in QuestBox.AvailableTab.Quests)
            {
                if (quest?.StartNPC == null) continue;

                if (quest.StartNPC.CurrentQuest != null) continue;

                quest.StartNPC.CurrentQuest = new CurrentQuest
                {
                    Type = quest.QuestType,
                    Icon = QuestIcon.New
                };
            }

            UpdateQuestAlertIcons();
            MainPanel.CompletedQuestIcon.Visible = completed;

            foreach (NPCInfo info in Globals.NPCInfoList.Binding)
            {
                BigMapBox.Update(info);
                MiniMapBox.Update(info);
            }

            foreach (MapObject ob in MapControl.Objects)
                ob.UpdateQuests();

            foreach (ClientObjectData data in DataDictionary.Values)
            {
                BigMapBox.Update(data);
                MiniMapBox.Update(data);
            }

        }

        public void UpdateQuestAlertIcons()
        {
            QuestBox.UpdateAlertIcons();

            MainPanel.AvailableQuestIcon.Visible = QuestBox.AvailableTab.Quests.Count > 0 || HasUnclaimedMilestoneReward();
        }

        public bool HasUnclaimedMilestoneReward()
        {
            return User?.Milestones?.Any(x => x.IsComplete && !x.Claimed && x.Info?.Reward != null) == true;
        }

        public DXControl GetNPCControl(NPCInfo npc)
        {
            int icon = 0;
            Color colour = Color.White;
            string iconString = "";

            if (npc.CurrentQuest != null)
            {
                switch (npc.CurrentQuest.Type)
                {
                    case QuestType.General:
                        icon = 16;
                        colour = Color.Yellow;
                        break;
                    case QuestType.Daily:
                        icon = 76;
                        colour = Color.Blue;
                        break;
                    case QuestType.Weekly:
                        icon = 76;
                        colour = Color.Blue;
                        break;
                    case QuestType.Repeatable:
                        icon = 16;
                        colour = Color.Yellow;
                        break;
                    case QuestType.Story:
                        icon = 56;
                        colour = Color.Green;
                        break;
                    case QuestType.Account:
                        icon = 36;
                        colour = Color.MediumPurple;
                        break;
                }

                switch (npc.CurrentQuest.Icon)
                {
                    case QuestIcon.New:
                        icon += 0;
                        iconString = "!";
                        break;
                    case QuestIcon.Incomplete:
                        icon = 2;
                        colour = Color.White;
                        iconString = "?";
                        break;
                    case QuestIcon.Complete:
                        icon += 2;
                        iconString = "?";
                        break;
                }
            }

            if (!string.IsNullOrEmpty(iconString))
            {
                DXLabel label = new DXLabel
                {
                    Text = iconString,
                    ForeColour = colour,
                    Hint = npc.NPCName,
                    Tag = npc.CurrentQuest,
                    Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold)
                };

                return label;
            }
            else if (icon > 0)
            {
                DXImageControl image = new DXImageControl
                {
                    LibraryFile = LibraryFile.QuestIcon,
                    Index = icon,
                    ForeColour = colour,
                    Hint = npc.NPCName,
                    Tag = npc.CurrentQuest,
                };
                image.OpacityChanged += (o, e) => image.ImageOpacity = image.Opacity;

                return image;
            }
            else if (npc.MapIcon != MapIcon.None)
            {
                DXImageControl image = new DXImageControl
                {
                    LibraryFile = LibraryFile.MiniMapIcon,
                    Opacity = Opacity,
                    Hint = npc.NPCName,
                    ImageOpacity = Opacity,
                };
                image.OpacityChanged += (o, e) => image.ImageOpacity = image.Opacity;

                GameScene.Game.UpdateMapIcon(image, npc.MapIcon);

                return image;
            }

            return new DXMapInfoControl
            {
                Size = new Size(3, 3),
                DrawTexture = true,
                Hint = npc.NPCName,
                BackColour = Color.Yellow
            };
        }

        public void UpdateMapIcon(DXImageControl control, MapIcon icon)
        {
            switch (icon)
            {
                case MapIcon.Cave:
                    control.Index = 1;
                    control.ForeColour = Color.Red;
                    break;
                case MapIcon.Exit:
                    control.Index = 1;
                    control.ForeColour = Color.Green;
                    break;
                case MapIcon.Down:
                    control.Index = 1;
                    control.ForeColour = Color.MediumVioletRed;
                    break;
                case MapIcon.Up:
                    control.Index = 1;
                    control.ForeColour = Color.DeepSkyBlue;
                    break;
                case MapIcon.Province:
                    control.Index = 7;
                    break;
                case MapIcon.Building:
                    control.Index = 6;
                    break;
                default:
                    control.Index = (int)icon;
                    break;
            }
        }

        public bool IsAlly(uint objectID)
        {
            if (User.ObjectID == objectID) return true;

            if (Partner != null && Partner.ObjectID == objectID) return true;

            foreach (ClientPlayerInfo member in GroupBox.Members)
                if (member.ObjectID == objectID) return true;

            if (GuildBox.GuildInfo != null)
                foreach (ClientGuildMemberInfo member in GuildBox.GuildInfo.Members)
                    if (member.ObjectID == objectID) return true;

            return false;
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (Game == this) Game = null;

                _SelectedCell = null;

                _User = null;
                _MouseItem = null;
                _MouseMagic = null;
                _MouseFame = null;

                CurrencyPickedUp = null;

                MagicObject = null;
                MouseObject = null;
                TargetObject = null;
                FocusObject = null;

                if (ItemLabel != null)
                {
                    if (!ItemLabel.IsDisposed)
                        ItemLabel.Dispose();

                    ItemLabel = null;
                }

                if (MagicLabel != null)
                {
                    if (!MagicLabel.IsDisposed)
                        MagicLabel.Dispose();

                    MagicLabel = null;
                }

                if (FameLabel != null)
                {
                    if (!FameLabel.IsDisposed)
                        FameLabel.Dispose();

                    FameLabel = null;
                }

                if (MapControl != null)
                {
                    if (!MapControl.IsDisposed)
                        MapControl.Dispose();

                    MapControl = null;
                }

                if (MainPanel != null)
                {
                    if (!MainPanel.IsDisposed)
                        MainPanel.Dispose();

                    MainPanel = null;
                }

                if (MenuBox != null)
                {
                    if (!MenuBox.IsDisposed)
                        MenuBox.Dispose();

                    MenuBox = null;
                }

                if (ConfigBox != null)
                {
                    if (!ConfigBox.IsDisposed)
                        ConfigBox.Dispose();

                    ConfigBox = null;
                }

                if (InventoryBox != null)
                {
                    if (!InventoryBox.IsDisposed)
                        InventoryBox.Dispose();

                    InventoryBox = null;
                }

                if (CharacterBox != null)
                {
                    if (!CharacterBox.IsDisposed)
                        CharacterBox.Dispose();

                    CharacterBox = null;
                }

                if (ExitBox != null)
                {
                    if (!ExitBox.IsDisposed)
                        ExitBox.Dispose();

                    ExitBox = null;
                }

                if (HelpBox != null)
                {
                    if (!HelpBox.IsDisposed)
                        HelpBox.Dispose();

                    HelpBox = null;
                }

                if (ChatTextBox != null)
                {
                    if (!ChatTextBox.IsDisposed)
                        ChatTextBox.Dispose();

                    ChatTextBox = null;
                }

                if (BeltBox != null)
                {
                    if (!BeltBox.IsDisposed)
                        BeltBox.Dispose();

                    BeltBox = null;
                }

                if (ChatOptionsBox != null)
                {
                    if (!ChatOptionsBox.IsDisposed)
                        ChatOptionsBox.Dispose();

                    ChatOptionsBox = null;
                }

                if (NPCBox != null)
                {
                    if (!NPCBox.IsDisposed)
                        NPCBox.Dispose();

                    NPCBox = null;
                }

                if (NPCGoodsBox != null)
                {
                    if (!NPCGoodsBox.IsDisposed)
                        NPCGoodsBox.Dispose();

                    NPCGoodsBox = null;
                }

                if (NPCRefinementStoneBox != null)
                {
                    if (!NPCRefinementStoneBox.IsDisposed)
                        NPCRefinementStoneBox.Dispose();

                    NPCRefinementStoneBox = null;
                }

                if (NPCRepairBox != null)
                {
                    if (!NPCRepairBox.IsDisposed)
                        NPCRepairBox.Dispose();

                    NPCRepairBox = null;
                }

                if (NPCRefineBox != null)
                {
                    if (!NPCRefineBox.IsDisposed)
                        NPCRefineBox.Dispose();

                    NPCRefineBox = null;
                }

                if (NPCRefineRetrieveBox != null)
                {
                    if (!NPCRefineRetrieveBox.IsDisposed)
                        NPCRefineRetrieveBox.Dispose();

                    NPCRefineRetrieveBox = null;
                }

                if (NPCMasterRefineBox != null)
                {
                    if (!NPCMasterRefineBox.IsDisposed)
                        NPCMasterRefineBox.Dispose();

                    NPCMasterRefineBox = null;
                }

                if (NPCRollBox != null)
                {
                    if (!NPCRollBox.IsDisposed)
                        NPCRollBox.Dispose();

                    NPCRollBox = null;
                }

                if (NPCQuestListBox != null)
                {
                    if (!NPCQuestListBox.IsDisposed)
                        NPCQuestListBox.Dispose();

                    NPCQuestListBox = null;
                }

                if (NPCQuestBox != null)
                {
                    if (!NPCQuestBox.IsDisposed)
                        NPCQuestBox.Dispose();

                    NPCQuestBox = null;
                }

                if (NPCAdoptCompanionBox != null)
                {
                    if (!NPCAdoptCompanionBox.IsDisposed)
                        NPCAdoptCompanionBox.Dispose();

                    NPCAdoptCompanionBox = null;
                }

                if (NPCCompanionStorageBox != null)
                {
                    if (!NPCCompanionStorageBox.IsDisposed)
                        NPCCompanionStorageBox.Dispose();

                    NPCCompanionStorageBox = null;
                }

                if (NPCWeddingRingBox != null)
                {
                    if (!NPCWeddingRingBox.IsDisposed)
                        NPCWeddingRingBox.Dispose();

                    NPCWeddingRingBox = null;
                }

                if (NPCItemFragmentBox != null)
                {
                    if (!NPCItemFragmentBox.IsDisposed)
                        NPCItemFragmentBox.Dispose();

                    NPCItemFragmentBox = null;
                }

                if (NPCAccessoryUpgradeBox != null)
                {
                    if (!NPCAccessoryUpgradeBox.IsDisposed)
                        NPCAccessoryUpgradeBox.Dispose();

                    NPCAccessoryUpgradeBox = null;
                }

                if (NPCAccessoryLevelBox != null)
                {
                    if (!NPCAccessoryLevelBox.IsDisposed)
                        NPCAccessoryLevelBox.Dispose();

                    NPCAccessoryLevelBox = null;
                }

                if (NPCAccessoryResetBox != null)
                {
                    if (!NPCAccessoryResetBox.IsDisposed)
                        NPCAccessoryResetBox.Dispose();

                    NPCAccessoryResetBox = null;
                }

                if (MiniMapBox != null)
                {
                    if (!MiniMapBox.IsDisposed)
                        MiniMapBox.Dispose();

                    MiniMapBox = null;
                }

                if (BigMapBox != null)
                {
                    if (!BigMapBox.IsDisposed)
                        BigMapBox.Dispose();

                    BigMapBox = null;
                }

                if (MagicBox != null)
                {
                    if (!MagicBox.IsDisposed)
                        MagicBox.Dispose();

                    MagicBox = null;
                }

                if (GroupBox != null)
                {
                    if (!GroupBox.IsDisposed)
                        GroupBox.Dispose();

                    GroupBox = null;
                }

                if (GroupHealthBox != null)
                {
                    if (!GroupHealthBox.IsDisposed)
                        GroupHealthBox.Dispose();

                    GroupHealthBox = null;
                }

                if (BuffBox != null)
                {
                    if (!BuffBox.IsDisposed)
                        BuffBox.Dispose();

                    BuffBox = null;
                }

                if (StorageBox != null)
                {
                    if (!StorageBox.IsDisposed)
                        StorageBox.Dispose();

                    StorageBox = null;
                }

                if (AutoPotionBox != null)
                {
                    if (!AutoPotionBox.IsDisposed)
                        AutoPotionBox.Dispose();

                    AutoPotionBox = null;
                }

                if (InspectBox != null)
                {
                    if (!InspectBox.IsDisposed)
                        InspectBox.Dispose();

                    InspectBox = null;
                }

                if (RankingBox != null)
                {
                    if (!RankingBox.IsDisposed)
                        RankingBox.Dispose();

                    RankingBox = null;
                }

                if (GameStoreBox != null)
                {
                    if (!GameStoreBox.IsDisposed)
                        GameStoreBox.Dispose();

                    GameStoreBox = null;
                }

                if (ConsignmentBox != null)
                {
                    if (!ConsignmentBox.IsDisposed)
                        ConsignmentBox.Dispose();

                    ConsignmentBox = null;
                }

                if (CommunicationBox != null)
                {
                    if (!CommunicationBox.IsDisposed)
                        CommunicationBox.Dispose();

                    CommunicationBox = null;
                }

                if (TradeBox != null)
                {
                    if (!TradeBox.IsDisposed)
                        TradeBox.Dispose();

                    TradeBox = null;
                }

                if (GuildBox != null)
                {
                    if (!GuildBox.IsDisposed)
                        GuildBox.Dispose();

                    GuildBox = null;
                }

                if (GuildMemberBox != null)
                {
                    if (!GuildMemberBox.IsDisposed)
                        GuildMemberBox.Dispose();

                    GuildMemberBox = null;
                }

                if (QuestBox != null)
                {
                    if (!QuestBox.IsDisposed)
                        QuestBox.Dispose();

                    QuestBox = null;
                }

                if (QuestTrackerBox != null)
                {
                    if (!QuestTrackerBox.IsDisposed)
                        QuestTrackerBox.Dispose();

                    QuestTrackerBox = null;
                }

                if (CompanionBox != null)
                {
                    if (!CompanionBox.IsDisposed)
                        CompanionBox.Dispose();

                    CompanionBox = null;
                }

                if (MonsterBox != null)
                {
                    if (!MonsterBox.IsDisposed)
                        MonsterBox.Dispose();

                    MonsterBox = null;
                }

                if (MagicBarBox != null)
                {
                    if (!MagicBarBox.IsDisposed)
                        MagicBarBox.Dispose();

                    MagicBarBox = null;
                }

                if (NPCAccessoryRefineBox != null)
                {
                    if (!NPCAccessoryRefineBox.IsDisposed)
                        NPCAccessoryRefineBox.Dispose();

                    NPCAccessoryRefineBox = null;
                }

                if (NPCSocketBox != null)
                {
                    if (!NPCSocketBox.IsDisposed)
                        NPCSocketBox.Dispose();

                    NPCSocketBox = null;
                }

                if (NPCSocketCombineBox != null)
                {
                    if (!NPCSocketCombineBox.IsDisposed)
                        NPCSocketCombineBox.Dispose();

                    NPCSocketCombineBox = null;
                }

                if (FishingBox != null)
                {
                    if (!FishingBox.IsDisposed)
                        FishingBox.Dispose();

                    FishingBox = null;
                }

                if (FishingCatchBox != null)
                {
                    if (!FishingCatchBox.IsDisposed)
                        FishingCatchBox.Dispose();

                    FishingCatchBox = null;
                }

                Inventory = null;
                Equipment = null;
                QuestLog = null;

                DataDictionary.Clear();
                DataDictionary = null;

                MoveFrame = false;
                MoveTime = DateTime.MinValue;
                OutputTime = DateTime.MinValue;
                ItemRefreshTime = DateTime.MinValue;

                CanRun = false;
                AutoRun = false;
                _NPCID = 0;
                _Companion = null;
                _Partner = null;

                PickUpTime = DateTime.MinValue;
                UseItemTime = DateTime.MinValue;
                NPCTime = DateTime.MinValue;
                ToggleTime = DateTime.MinValue;
                InspectTime = DateTime.MinValue;
                ItemTime = DateTime.MinValue;
                ItemReviveTime = DateTime.MinValue;

                _DayTime = 0f;
            }
        }

        #endregion

    }
}
