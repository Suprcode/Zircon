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
        public DXControl ItemLabel, MagicLabel;
        
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

        public MapControl MapControl;
        public MainPanel MainPanel;

        public MenuDialog MenuBox;
        public DXConfigWindow ConfigBox;
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
        public MarketPlaceDialog MarketPlaceBox;
        public DungeonFinderDialog DungeonFinderBox;
        public CommunicationDialog CommunicationBox;
        public TradeDialog TradeBox;
        public GuildDialog GuildBox;
        public GuildMemberDialog GuildMemberBox;
        public QuestDialog QuestBox;
        public QuestTrackerDialog QuestTrackerBox;
        public CompanionDialog CompanionBox;
        public MonsterDialog MonsterBox;
        public MagicBarDialog MagicBarBox;
        public EditCharacterDialog EditCharacterBox;
        public FortuneCheckerDialog FortuneCheckerBox;
        public NPCWeaponCraftWindow NPCWeaponCraftBox;
        public NPCAccessoryRefineDialog NPCAccessoryRefineBox;
        public CurrencyDialog CurrencyBox;
        public TimerDialog TimerBox;

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
            MenuBox?.LoadSettings();

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
                ColourTab = { TabButton = { Visible = true } },
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
            MarketPlaceBox = new MarketPlaceDialog
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
        }

        #region Methods
        private void SetDefaultLocations()
        {
            if (ConfigBox == null) return;

            MenuBox.Location = new Point(Size.Width - MenuBox.Size.Width, Size.Height - MenuBox.Size.Height - MainPanel.Size.Height);

            ConfigBox.Location = new Point((Size.Width - ConfigBox.Size.Width)/2, (Size.Height - ConfigBox.Size.Height)/2);

            CaptionBox.Location = Point.Empty;

            ChatOptionsBox.Location = new Point((Size.Width - ChatOptionsBox.Size.Width)/2, (Size.Height - ChatOptionsBox.Size.Height)/2);
            
            ExitBox.Location = new Point((Size.Width - ExitBox.Size.Width) / 2, (Size.Height - ExitBox.Size.Height) / 2);

            TradeBox.Location = new Point((Size.Width - TradeBox.Size.Width) / 2, (Size.Height - TradeBox.Size.Height) / 2);

            GuildBox.Location = new Point((Size.Width - GuildBox.Size.Width) / 2, (Size.Height - GuildBox.Size.Height) / 2);

            GuildMemberBox.Location = new Point((Size.Width - GuildMemberBox.Size.Width) / 2, (Size.Height - GuildMemberBox.Size.Height) / 2);

            InventoryBox.Location = new Point(Size.Width - InventoryBox.Size.Width, MiniMapBox.Size.Height);
            
            CharacterBox.Location = Point.Empty;

            MapControl.Size = Size;

            MainPanel.Location = new Point((Size.Width - MainPanel.Size.Width)/2, Size.Height - MainPanel.Size.Height);

            ChatTextBox.Location = new Point((Size.Width - ChatTextBox.Size.Width) / 2, (Size.Height - ChatTextBox.Size.Height) / 2);

            BeltBox.Location = new Point(MainPanel.Location.X + MainPanel.Size.Width - BeltBox.Size.Width, MainPanel.Location.Y - BeltBox.Size.Height);
            
            NPCBox.Location = Point.Empty;

            NPCGoodsBox.Location = new Point(0, NPCBox.Size.Height);

            NPCRollBox.Location = new Point((Size.Width - NPCRollBox.Size.Width) / 2, (Size.Height - NPCRollBox.Size.Height) / 2);

            NPCRepairBox.Location = new Point(0, NPCBox.Size.Height);

            MiniMapBox.Location = new Point(Size.Width - MiniMapBox.Size.Width, 0);

            QuestTrackerBox.Location = new Point(Size.Width - QuestTrackerBox.Size.Width, MiniMapBox.Size.Height + 5);

            BuffBox.Location = new Point(Size.Width - MiniMapBox.Size.Width - BuffBox.Size.Width - 5, 0);

            MagicBox.Location = new Point(Size.Width - MagicBox.Size.Width, 0);

            GroupBox.Location = new Point((Size.Width - GroupBox.Size.Width)/2, (Size.Height - GroupBox.Size.Height)/2);

            StorageBox.Location = new Point(Size.Width - StorageBox.Size.Width - InventoryBox.Size.Width, 0);

            AutoPotionBox.Location = new Point((Size.Width - AutoPotionBox.Size.Width)/2, (Size.Height - AutoPotionBox.Size.Height)/2);

            InspectBox.Location = new Point(CharacterBox.Size.Width, 0);

            RankingBox.Location = new Point((Size.Width - RankingBox.Size.Width) / 2, (Size.Height - RankingBox.Size.Height) / 2);

            MarketPlaceBox.Location = new Point((Size.Width - MarketPlaceBox.Size.Width) / 2, (Size.Height - MarketPlaceBox.Size.Height) / 2);

            CommunicationBox.Location = new Point((Size.Width - CommunicationBox.Size.Width) / 2, (Size.Height - CommunicationBox.Size.Height) / 2);

            CompanionBox.Location = new Point((Size.Width - CompanionBox.Size.Width) / 2, (Size.Height - CompanionBox.Size.Height) / 2);

            MonsterBox.Location = new Point((Size.Width - MonsterBox.Size.Width) / 2, 50);

            EditCharacterBox.Location = new Point((Size.Width - EditCharacterBox.Size.Width) / 2, (Size.Height - EditCharacterBox.Size.Height) / 2);

            FortuneCheckerBox.Location = new Point((Size.Width - FortuneCheckerBox.Size.Width) / 2, (Size.Height - FortuneCheckerBox.Size.Height) / 2);

            NPCWeaponCraftBox.Location = new Point((Size.Width - NPCWeaponCraftBox.Size.Width) / 2, (Size.Height - NPCWeaponCraftBox.Size.Height) / 2);

            CurrencyBox.Location = new Point((Size.Width - CurrencyBox.Size.Width) / 2, (Size.Height - CurrencyBox.Size.Height) / 2);

            FishingBox.Location = new Point(CharacterBox.Location.X + CharacterBox.Size.Width, CharacterBox.Location.Y);

            FishingCatchBox.Location = new Point(((Size.Width - FishingCatchBox.Size.Width) / 2), ((Size.Height - FishingCatchBox.Size.Height) / 2) + 200);

            TimerBox.Location = new Point(Size.Width - 120, Size.Height - 180);
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
                    MouseItem = ((ItemObject) MapObject.MouseObject).Item;
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
                        if (MarketPlaceBox.StoreTab.IsVisible)
                            MarketPlaceBox.Visible = false;
                        else
                        {
                            MarketPlaceBox.Visible = true;
                            MarketPlaceBox.StoreTab.TabButton.InvokeMouseClick();
                        }
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
                    case KeyBindAction.MarketPlaceWindow:
                        if (MarketPlaceBox.ConsignTab.IsVisible || MarketPlaceBox.SearchTab.IsVisible)
                            MarketPlaceBox.Visible = false;
                        else
                        {
                            MarketPlaceBox.Visible = true;
                            MarketPlaceBox.SearchTab.TabButton.InvokeMouseClick();
                        }
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
                        if (!BigMapBox.Visible)
                        {
                            BigMapBox.Opacity = 1F;
                            BigMapBox.Visible = true;
                            return;
                        }

                        if (BigMapBox.Opacity == 1F)
                        {
                            BigMapBox.Opacity = 0.5F;
                            return;
                        }

                        BigMapBox.Visible = false;
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
                        User.AttackMode = (AttackMode) (((int) User.AttackMode + 1) % 5);
                        CEnvir.Enqueue(new C.ChangeAttackMode { Mode = User.AttackMode });
                        break;
                    case KeyBindAction.ChangePetMode:
                        if (Observer) continue;

                        User.PetMode = (PetMode) (((int) User.PetMode + 1) % 5);
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

            Stats stats = new Stats();
            stats.Add(MouseItem.Info.Stats);
            stats.Add(MouseItem.AddedStats);

            ItemLabel = new DXControl
            {
                BackColour = Color.FromArgb(200, 0, 24, 48),
                Border = true,
                BorderColour = Color.Yellow, // Color.FromArgb(144, 148, 48),
                DrawTexture = true,
                IsControl = false,
                IsVisible = true,
            };

            ItemInfo displayInfo = MouseItem.Info;

            if (MouseItem.Info.ItemEffect == ItemEffect.ItemPart)
                displayInfo = Globals.ItemInfoList.Binding.First(x => x.Index == MouseItem.AddedStats[Stat.ItemIndex]);
            

            DXLabel label = new DXLabel
            {
                ForeColour = Color.Yellow,
                Location = new Point(4, 4),
                Parent = ItemLabel,
                Text = displayInfo.ItemName 
            };

            if (MouseItem.Info.ItemEffect == ItemEffect.ItemPart)
                label.Text += " - [Part]";
            ItemLabel.Size = new Size(label.DisplayArea.Right + 4, label.DisplayArea.Bottom);




            bool needSpacer = false;
            if (displayInfo.ItemType != ItemType.Nothing)
            {
                label = new DXLabel
                {
                    ForeColour = Color.Yellow,
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = $"{displayInfo.ItemType}",
                };


                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
                needSpacer = true;

            }

            if (MouseItem.Info.Weight > 0)
            {
                label = new DXLabel
                {
                    ForeColour = Color.White,
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = $"Weight: {MouseItem.Info.Weight}",
                };

                switch (MouseItem.Info.ItemType)
                {
                    case ItemType.Weapon:
                    case ItemType.Shield:
                    case ItemType.Torch:
                        if (User.HandWeight - (Equipment[(int) EquipmentSlot.Weapon]?.Info.Weight ?? 0) + MouseItem.Info.Weight > User.Stats[Stat.HandWeight])
                            label.ForeColour = Color.Red;
                        break;
                    case ItemType.Armour:
                    case ItemType.Helmet:
                    case ItemType.Necklace:
                    case ItemType.Bracelet:
                    case ItemType.Ring:
                    case ItemType.Shoes:
                    case ItemType.Poison:
                    case ItemType.Amulet:
                        if (User.WearWeight - (Equipment[(int)EquipmentSlot.Armour]?.Info.Weight ?? 0) + MouseItem.Info.Weight > User.Stats[Stat.WearWeight])
                            label.ForeColour = Color.Red;
                        break;
                }
                
                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
                needSpacer = true;
            }

            if (needSpacer)
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);

            if (CEnvir.IsCurrencyItem(MouseItem.Info) || MouseItem.Info.ItemEffect == ItemEffect.Experience)
            {
                label = new DXLabel
                {
                    ForeColour = Color.Yellow,
                    Location = new Point(ItemLabel.DisplayArea.Right, 4),
                    Parent = ItemLabel,
                    Text = $"Amount: {MouseItem.Count:#,##0}"
                };
                ItemLabel.Size = new Size(label.DisplayArea.Right + 4, ItemLabel.Size.Height + 4);

                if (!string.IsNullOrEmpty(displayInfo.Description))
                {
                    label = new DXLabel
                    {
                        ForeColour = Color.Wheat,
                        Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                        Parent = ItemLabel,
                        Text = displayInfo.Description,
                    };

                    ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                        label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
                    ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);
                }

                return;
            }


            if (MouseItem.Info.ItemEffect == ItemEffect.ItemPart)
            {
                label = new DXLabel
                {
                    ForeColour = Color.LightSeaGreen,
                    Location = new Point(ItemLabel.DisplayArea.Right, 4),
                    Parent = ItemLabel,
                    Text = $"Parts: {MouseItem.Count}/{displayInfo.PartCount}.",
                };

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4, ItemLabel.Size.Height);
            }
            else if (MouseItem.Info.StackSize > 1)
            {
                label = new DXLabel
                {
                    ForeColour = Color.Yellow,
                    Location = new Point(ItemLabel.DisplayArea.Right, 4),
                    Parent = ItemLabel,
                    Text = $"Count: {MouseItem.Count}/{MouseItem.Info.StackSize}"
                };
                ItemLabel.Size = new Size(label.DisplayArea.Right + 4, ItemLabel.Size.Height);
            }

            switch (displayInfo.ItemType)
            {
                case ItemType.Consumable:
                case ItemType.Scroll:
                    if (MouseItem.Info.ItemEffect == ItemEffect.StatExtractor || MouseItem.Info.ItemEffect == ItemEffect.RefineExtractor)
                        EquipmentItemInfo();
                    else
                        CreatePotionLabel();
                    break;
                case ItemType.Book:
                    if (MouseItem.Info.Durability > 0)
                    {
                        label = new DXLabel
                        {
                            ForeColour = Color.White,
                            Location = new Point(ItemLabel.DisplayArea.Right, 4),
                            Parent = ItemLabel,
                            Text = $"Pages: {MouseItem.CurrentDurability}/{MouseItem.MaxDurability}",
                        };

                        ItemLabel.Size = new Size(label.DisplayArea.Right + 4, ItemLabel.Size.Height);
                    }
                    break;
                case ItemType.Meat:
                    if (MouseItem.Info.Durability > 0)
                    {
                        label = new DXLabel
                        {
                            ForeColour = MouseItem.CurrentDurability == 0 ? Color.Red : Color.White,
                            Location = new Point(ItemLabel.DisplayArea.Right, 4),
                            Parent = ItemLabel,
                            Text = $"Quality: {Math.Round(MouseItem.CurrentDurability/1000M)}/{Math.Round(MouseItem.MaxDurability/1000M)}",
                        };

                        ItemLabel.Size = new Size(label.DisplayArea.Right + 4, ItemLabel.Size.Height);
                    }
                    break;
                case ItemType.Ore:
                    if (MouseItem.Info.Durability > 0)
                    {
                        label = new DXLabel
                        {
                            ForeColour = MouseItem.CurrentDurability == 0 ? Color.Red : Color.White,
                            Location = new Point(ItemLabel.DisplayArea.Right, 4),
                            Parent = ItemLabel,
                            Text = $"Purity: {Math.Round(MouseItem.CurrentDurability/1000M)}",
                        };

                        ItemLabel.Size = new Size(label.DisplayArea.Right + 4, ItemLabel.Size.Height);
                    }
                    break;
                default:
                    EquipmentItemInfo();
                    break;
            }


            if (displayInfo.RequiredGender != RequiredGender.None)
            {
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

                label = new DXLabel
                {
                    ForeColour = colour,
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = $"Required Gender: {MouseItem.Info.RequiredGender}",
                };

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
            }


            if (displayInfo.RequiredClass != RequiredClass.All)
            {
                Color colour = Color.White;
                switch (User.Class)
                {
                    case MirClass.Warrior:
                        if (!MouseItem.Info.RequiredClass.HasFlag(RequiredClass.Warrior))
                            colour = Color.Red;
                        break;
                    case MirClass.Wizard:
                        if (!MouseItem.Info.RequiredClass.HasFlag(RequiredClass.Wizard))
                            colour = Color.Red;
                        break;
                    case MirClass.Taoist:
                        if (!MouseItem.Info.RequiredClass.HasFlag(RequiredClass.Taoist))
                            colour = Color.Red;
                        break;
                    case MirClass.Assassin:
                        if (!MouseItem.Info.RequiredClass.HasFlag(RequiredClass.Assassin))
                            colour = Color.Red;
                        break;
                }

                Type type = displayInfo.RequiredClass.GetType();

                MemberInfo[] infos = type.GetMember(displayInfo.RequiredClass.ToString());

                DescriptionAttribute description = infos[0].GetCustomAttribute<DescriptionAttribute>();
                
                label = new DXLabel
                {
                    ForeColour = colour,
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = $"Required Class: {description?.Description ?? displayInfo.RequiredClass.ToString()}",
                };

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
            }



            if (displayInfo.RequiredAmount > 0)
            {
                string text;
                Color colour = displayInfo.Rarity == Rarity.Common ? Color.White : Color.FromArgb(0, 204, 0);
                switch (displayInfo.RequiredType)
                {
                    case RequiredType.Level:
                        text = $"Required Level: {MouseItem.Info.RequiredAmount}";
                        if (User.Level < MouseItem.Info.RequiredAmount && User.Stats[Stat.Rebirth] == 0)
                            colour = Color.Red;
                        break;
                    case RequiredType.MaxLevel:
                        text = $"Max Level: {MouseItem.Info.RequiredAmount}";
                        if (User.Level > MouseItem.Info.RequiredAmount || User.Stats[Stat.Rebirth] > 0)
                            colour = Color.Red;
                        break;
                    case RequiredType.AC:
                        text = $"Required AC: {MouseItem.Info.RequiredAmount}";
                        if (User.Stats[Stat.MaxAC] < MouseItem.Info.RequiredAmount)
                            colour = Color.Red;
                        break;
                    case RequiredType.MR:
                        text = $"Required MR: {MouseItem.Info.RequiredAmount}";
                        if (User.Stats[Stat.MaxMR] < MouseItem.Info.RequiredAmount)
                            colour = Color.Red;
                        break;
                    case RequiredType.DC:
                        text = $"Required DC: {MouseItem.Info.RequiredAmount}";
                        if (User.Stats[Stat.MaxDC] < MouseItem.Info.RequiredAmount)
                            colour = Color.Red;
                        break;
                    case RequiredType.MC:
                        text = $"Required MC: {MouseItem.Info.RequiredAmount}";
                        if (User.Stats[Stat.MaxMC] < MouseItem.Info.RequiredAmount)
                            colour = Color.Red;
                        break;
                    case RequiredType.SC:
                        text = $"Required SC: {MouseItem.Info.RequiredAmount}";
                        if (User.Stats[Stat.MaxSC] < MouseItem.Info.RequiredAmount)
                            colour = Color.Red;
                        break;
                    case RequiredType.Health:
                        text = $"Required Health: {MouseItem.Info.RequiredAmount}";
                        if (User.Stats[Stat.Health] < MouseItem.Info.RequiredAmount)
                            colour = Color.Red;
                        break;
                    case RequiredType.Mana:
                        text = $"Required Mana: {MouseItem.Info.RequiredAmount}";
                        if (User.Stats[Stat.Mana] < MouseItem.Info.RequiredAmount)
                            colour = Color.Red;
                        break;
                    case RequiredType.CompanionLevel:
                        text = $"Companion Level: {MouseItem.Info.RequiredAmount}";
                        if (Companion == null || Companion.Level < MouseItem.Info.RequiredAmount)
                            colour = Color.Red;
                        break;
                    case RequiredType.MaxCompanionLevel:
                        text = $"Max Companion Level: {MouseItem.Info.RequiredAmount}";
                        if (Companion == null || Companion.Level > MouseItem.Info.RequiredAmount)
                            colour = Color.Red;
                        break;
                    case RequiredType.RebirthLevel:
                        text = $"Rebirth Level: {MouseItem.Info.RequiredAmount}";
                        if (User.Stats[Stat.Rebirth] < MouseItem.Info.RequiredAmount)
                            colour = Color.Red;
                        break;
                    case RequiredType.MaxRebirthLevel:
                        text = $"Rebirth Level: {MouseItem.Info.RequiredAmount}";
                        if (User.Stats[Stat.Rebirth] > MouseItem.Info.RequiredAmount)
                            colour = Color.Red;
                        break;
                    default:
                        text = "Unknown Type Required";
                        break;
                }

                if (displayInfo.Rarity > Rarity.Common)
                    text += $" ({displayInfo.Rarity})";


                label = new DXLabel
                {
                    ForeColour = colour,
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = text,
                };

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
            }
            else if (displayInfo.Rarity > Rarity.Common)
            {

                label = new DXLabel
                {
                    ForeColour = Color.FromArgb(0, 204, 0),
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = displayInfo.Rarity.ToString(),
                };

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
            }


            bool spacer = false;
            long sale = MouseItem.Price(Math.Max(1, MouseItem.Count));
            if (sale > 0)
            {
                label = new DXLabel
                {
                    ForeColour = Color.LightGoldenrodYellow,
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = $"Sell Value: {sale}",
                };

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
            }
            ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);


            if (MouseItem.Info.Durability > 0 && !MouseItem.Info.CanRepair && MouseItem.Info.StackSize == 1)
            {
                label = new DXLabel
                {
                    ForeColour = Color.Yellow,
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = "Cannot be repaired.",
                };

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height);
                spacer = true;
            }

            if (!MouseItem.Info.CanSell || (MouseItem.Flags & UserItemFlags.Worthless) == UserItemFlags.Worthless)
            {
                label = new DXLabel
                {
                    ForeColour = Color.Yellow,
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = "Cannot be sold.",
                };

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height);
                spacer = true;
            }

            if (!MouseItem.Info.CanStore)
            {
                label = new DXLabel
                {
                    ForeColour = Color.Yellow,
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = "Cannot be stored.",
                };

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height);
                spacer = true;
            }
            
            if (!MouseItem.Info.CanTrade || (MouseItem.Flags & UserItemFlags.Bound) == UserItemFlags.Bound)
            {
                label = new DXLabel
                {
                    ForeColour = Color.Yellow,
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = "Cannot be traded.",
                };

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height);
                spacer = true;
            }

            if (!MouseItem.Info.CanDrop)
            {
                label = new DXLabel
                {
                    ForeColour = Color.Yellow,
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = "Cannot be dropped.",
                };

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height);
                spacer = true;
            }

            if (!MouseItem.Info.CanDeathDrop || (MouseItem.Flags & UserItemFlags.Worthless) == UserItemFlags.Worthless || (MouseItem.Flags & UserItemFlags.Bound) == UserItemFlags.Bound)
            {
                label = new DXLabel
                {
                    ForeColour = Color.Yellow,
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = "Cannot be dropped on death.",
                };

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height);
                spacer = true;
            }

            if ((MouseItem.Flags & UserItemFlags.Bound) == UserItemFlags.Bound)
            {
                label = new DXLabel
                {
                    ForeColour = Color.Yellow,
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = "Bound Item.",
                };

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height);
                spacer = true;
            }

            if ((MouseItem.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable)
            {
                label = new DXLabel
                {
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                };

                switch (MouseItem.Info.ItemType)
                {
                    case ItemType.Book:
                        label.ForeColour = Color.Red;
                        label.Text = "Does not contain Level 4 Pages.";
                        break;
                    default:
                        label.ForeColour = Color.Yellow;
                        label.Text = "Cannot be Refined or Upgraded.";
                        break;
                }

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height);
                spacer = true;
            }
            else if (MouseItem.Info.ItemType == ItemType.Book)
            {
                label = new DXLabel
                {
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    ForeColour = Color.Green,
                    Text = "Contains high level Pages.",
                };

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height);
                spacer = true;
            }

            if (!string.IsNullOrEmpty(displayInfo.Description))
            {
                label = new DXLabel
                {
                    ForeColour = Color.Wheat,
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = displayInfo.Description,
                };

                if (displayInfo.ItemEffect == ItemEffect.FootBallWhistle)
                    label.ForeColour = Color.Red;

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height);
                spacer = true;
            }

            if (spacer)
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);


            if (MouseItem.Info.Durability > 0 && MouseItem.Info.CanRepair && MouseItem.Info.StackSize == 1 && MouseItem.Info.ItemType != ItemType.Book)
            {
                label = new DXLabel
                {
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                };

                if (CEnvir.Now >= MouseItem.NextSpecialRepair)
                {
                    label.Text = "Can Special Repair";
                    label.ForeColour = Color.LimeGreen;
                }
                else
                {
                    label.Text = $"Special Repair in {Functions.ToString(MouseItem.NextSpecialRepair - CEnvir.Now, true)}";
                    label.ForeColour = Color.Red;
                }


                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);

                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);
            }

            if ((MouseItem.Flags & UserItemFlags.Expirable) == UserItemFlags.Expirable)
            {
                label = new DXLabel
                {
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = $"Expires in {Functions.ToString(MouseItem.ExpireTime, true)}",
                    ForeColour = Color.Chocolate,
                };
                

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);

                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);
            }

            if (stats[Stat.ItemReviveTime] > 0)
            {
                label = new DXLabel
                {
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                };

                DateTime value = MouseItem.Info.ItemEffect == ItemEffect.PillOfReincarnation ? ReincarnationPillTime : ItemReviveTime;

                if (CEnvir.Now >= value)
                {
                    label.Text = "Revival ready";
                    label.ForeColour = Color.LimeGreen;
                }
                else
                {
                    label.Text = $"Revival ready in {Functions.ToString(value - CEnvir.Now, true)}";
                    label.ForeColour = Color.Red;
                }


                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);

                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);
            }


            if (MouseItem.Info.Set != null)
                SetItemInfo(MouseItem.Info.Set);

            if ((MouseItem.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage)
            {
                label = new DXLabel
                {
                    ForeColour = Color.MediumOrchid,
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = "Wedding Ring.",
                };

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height);

                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);
            }

            if ((MouseItem.Flags & UserItemFlags.GameMaster) == UserItemFlags.GameMaster)
            {
                label = new DXLabel
                {
                    ForeColour = Color.LightSeaGreen,
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = "Created by a Game Master.",
                };

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height);

                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);
            }

            if (NPCItemFragmentBox.IsVisible && MouseItem.CanFragment())
            {
                label = new DXLabel
                {
                    ForeColour = Color.MediumAquamarine,
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = $"Fragment Cost: {MouseItem.FragmentCost():#,##0}",
                };

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);

                label = new DXLabel
                {
                    ForeColour = Color.MediumAquamarine,
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = $"Fragments: {(MouseItem.Info.Rarity == Rarity.Common ? "Fragment" : "Framgent (II)")} x{MouseItem.FragmentCount():#,##0}",
                };

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);

                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);
            }

            if (CEnvir.Now < MouseItem.NextReset)
            {
                label = new DXLabel
                {
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = $"Reset Available in {Functions.ToString(MouseItem.NextReset - CEnvir.Now, true)}",
                    ForeColour = Color.Red,
                };


                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);

                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);
            }

            if ((MouseItem.Flags & UserItemFlags.Locked) == UserItemFlags.Locked)
            {
                label = new DXLabel
                {
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = $"Locked: Prevents accidentally selling or throwing away\n" +
                           $"[Middle Mouse Button] or [Scroll Lock] to Unlock.",
                };

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height);

                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);
            }


        }
        private void EquipmentItemInfo()
        {
            Stats stats = new Stats();

            ItemInfo displayInfo = MouseItem.Info;

            if (MouseItem.Info.ItemEffect == ItemEffect.ItemPart)
                displayInfo = Globals.ItemInfoList.Binding.First(x => x.Index == MouseItem.AddedStats[Stat.ItemIndex]);

            stats.Add(displayInfo.Stats, displayInfo.ItemType != ItemType.Weapon);
            stats.Add(MouseItem.AddedStats, MouseItem.Info.ItemType != ItemType.Weapon);
            
            if (displayInfo.ItemType == ItemType.Weapon)
            {
                Stat ele = MouseItem.AddedStats.GetWeaponElement();

                if (ele == Stat.None)
                    ele = displayInfo.Stats.GetWeaponElement();

                if (ele != Stat.None)
                    stats[ele] += MouseItem.AddedStats.GetWeaponElementValue() + displayInfo.Stats.GetWeaponElementValue();
            }

            DXLabel label;
            if (MouseItem.Info.Durability > 0)
            {
                label = new DXLabel
                {
                    ForeColour = MouseItem.CurrentDurability == 0 ? Color.Red : Color.FromArgb(132, 255, 255),
                    Location = new Point(ItemLabel.DisplayArea.Right, 4),
                    Parent = ItemLabel,
                    Text = $"Durability: {Math.Round(MouseItem.CurrentDurability/1000M)}/{Math.Round(MouseItem.MaxDurability/1000M)}",
                };

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4, ItemLabel.Size.Height);
            }

            ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 5);

            bool firstele = stats.HasElementalWeakness();
            foreach (KeyValuePair<Stat, int> pair in stats.Values)
            {
                string text = stats.GetDisplay(pair.Key);

                if (text == null) continue;

                string added = MouseItem.AddedStats.GetFormat(pair.Key);

                label = new DXLabel
                {
                    ForeColour = Color.White,
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = text
                };

                switch (pair.Key)
                {
                    case Stat.Luck:
                        label.ForeColour = Color.Yellow;
                        break;
                    case Stat.Strength:
                        label.ForeColour = Color.FromArgb(148, 255, 206);
                        break;
                    case Stat.DropRate:
                    case Stat.ExperienceRate:
                    case Stat.SkillRate:
                    case Stat.GoldRate:
                        label.ForeColour = Color.Yellow;

                        if (added == null) break;
                        label.Text += $" ({added})";
                        break;
                    case Stat.FireAttack:
                    case Stat.IceAttack:
                    case Stat.LightningAttack:
                    case Stat.WindAttack:
                    case Stat.HolyAttack:
                    case Stat.DarkAttack:
                    case Stat.PhantomAttack:
                        label.ForeColour = Color.DeepSkyBlue;
                        break;
                    case Stat.FireResistance:
                    case Stat.IceResistance:
                    case Stat.LightningResistance:
                    case Stat.WindResistance:
                    case Stat.HolyResistance:
                    case Stat.DarkResistance:
                    case Stat.PhantomResistance:
                    case Stat.PhysicalResistance:
                        label.ForeColour = !firstele ? Color.Lime : Color.IndianRed;
                        firstele = true;
                        break;
                    default:
                        if (MouseItem.AddedStats[pair.Key] == 0) break;
                        label.Text += $"   ({added})";
                        label.ForeColour = Color.FromArgb(148, 255, 206);
                        break;
                }

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
            }
            ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 5);

            switch (displayInfo.ItemType)
            {
                case ItemType.Weapon:
                    if ((MouseItem.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) break;

                    label = new DXLabel
                    {
                        ForeColour = Color.White,
                        Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                        Parent = ItemLabel,
                        Text = $"{displayInfo.ItemType} Level: " + (MouseItem.Level < Globals.WeaponExperienceList.Count ? MouseItem.Level.ToString() : "Max")
                    };

                    ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                        label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);

                    if (MouseItem.Level < Globals.WeaponExperienceList.Count)
                    {
                        label = new DXLabel
                        {
                            Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                            Parent = ItemLabel,
                        };

                        if ((MouseItem.Flags & UserItemFlags.Refinable) == UserItemFlags.Refinable)
                        {
                            label.Text = "Ready for Refine";
                            label.ForeColour = Color.LightGreen;
                        }
                        else
                        {
                            label.Text = $"{displayInfo.ItemType} Training Points: {MouseItem.Experience / Globals.WeaponExperienceList[MouseItem.Level]:0.##%}";
                            label.ForeColour = Color.White;
                        }



                        ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                            label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
                    }
                    ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 5);
                    break;
                case ItemType.Necklace:
                case ItemType.Bracelet:
                case ItemType.Ring:

                    if ((MouseItem.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) break;

                    label = new DXLabel
                    {
                        ForeColour = Color.White,
                        Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                        Parent = ItemLabel,
                        Text = $"{displayInfo.ItemType} Level: " + (MouseItem.Level < Globals.AccessoryExperienceList.Count ? MouseItem.Level.ToString() : "Max")
                    };

                    ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                        label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);

                    if (MouseItem.Level < Globals.AccessoryExperienceList.Count)
                    {
                        label = new DXLabel
                        {
                            Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                            Parent = ItemLabel,
                        };

                        if ((MouseItem.Flags & UserItemFlags.Refinable) == UserItemFlags.Refinable)
                        {
                            label.Text = "Ready for Refine";
                            label.ForeColour = Color.LightGreen;
                        }
                        else
                        {
                            label.Text = $"{displayInfo.ItemType} Training Points: {MouseItem.Experience / Globals.AccessoryExperienceList[MouseItem.Level]:0.##%}";
                            label.ForeColour = Color.White;
                        }



                        ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                            label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
                    }
                    ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 5);
                    break;

            }
        }
        private void CreatePotionLabel()
        {
            if (MouseItem == null) return;

            Stats stats = new Stats();
            
            stats.Add(MouseItem.Info.Stats);
            

            DXLabel label;
            foreach (KeyValuePair<Stat, int> pair in stats.Values)
            {
                string text = stats.GetDisplay(pair.Key);

                if (text == null) continue;

                label = new DXLabel
                {
                    ForeColour = Color.White,
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = text
                };

                switch (pair.Key)
                {
                    case Stat.Luck:
                    case Stat.DropRate:
                    case Stat.ExperienceRate:
                    case Stat.SkillRate:
                    case Stat.GoldRate:
                        label.ForeColour = Color.Yellow;
                        break;
                    case Stat.DeathDrops:
                        label.ForeColour = Color.OrangeRed;
                        break;
                }

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
            }
            ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 5);

            if (MouseItem.Info.Durability > 0)
            {
                label = new DXLabel
                {
                    ForeColour = Color.White,
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = $"Cooldown: {MouseItem.Info.Durability/1000M:#,##0.#} Seconds"
                };

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
            }
        }
        private void CreateMagicLabel()
        {
            if (MouseMagic == null) return;

            MagicLabel = new DXControl
            {
                BackColour = Color.FromArgb(200, 0, 24, 48),
                Border = true,
                BorderColour = Color.Yellow, // Color.FromArgb(144, 148, 48),
                DrawTexture = true,
                IsControl = false,
                IsVisible = true,
            };

            DXLabel label = new DXLabel
            {
                ForeColour = Color.Yellow,
                Location = new Point(4, 4),
                Parent = MagicLabel,
                Text = MouseMagic.Name
            };
            MagicLabel.Size = new Size(label.DisplayArea.Right + 4, label.DisplayArea.Bottom + 4);

            label = new DXLabel
            {
                ForeColour = Color.Yellow,
                Location = new Point(4, MagicLabel.DisplayArea.Bottom),
                Parent = MagicLabel,
                Text = $"<{MouseMagic.Property}>"
            };
            MagicLabel.Size = new Size(label.DisplayArea.Right + 4, label.DisplayArea.Bottom + 4);

            ClientUserMagic magic;

            int width;
            bool disciplineSkill = false;

            if (User.Magics.TryGetValue(MouseMagic, out magic))
            {
                int level = magic.Level;
                disciplineSkill = magic.Info.School == MagicSchool.Discipline;

                if (disciplineSkill)
                {
                    MagicLabel.BorderColour = Color.LimeGreen;
                }

                label = new DXLabel
                {
                    ForeColour = Color.LimeGreen,
                    Location = new Point(4, MagicLabel.DisplayArea.Bottom),
                    Parent = MagicLabel,
                    Text = $"Current Level: {level}",
                };

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

                width = label.DisplayArea.Right;
                label = new DXLabel
                {
                    ForeColour = Color.LimeGreen,
                    Location = new Point(width + 4, MagicLabel.DisplayArea.Bottom),
                    Parent = MagicLabel,
                    Text = $"Experience: {text}",
                };
            }
            else
            {
                label = new DXLabel
                {
                    ForeColour = Color.Red,
                    Location = new Point(4, MagicLabel.DisplayArea.Bottom),
                    Parent = MagicLabel,
                    Text = $"Not learned",
                };
            }
            MagicLabel.Size = new Size(label.DisplayArea.Right + 4 > MagicLabel.Size.Width ? label.DisplayArea.Right + 4 : MagicLabel.Size.Width, label.DisplayArea.Bottom);

            label = new DXLabel
            {
                ForeColour = User.Level < MouseMagic.NeedLevel1 ? Color.Red : Color.White,
                Location = new Point(4, MagicLabel.DisplayArea.Bottom),
                Parent = MagicLabel,
                Text = $"Rank 1 Requirement: Level {MouseMagic.NeedLevel1}",
            };
            width = label.DisplayArea.Right + 10;
            label = new DXLabel
            {
                ForeColour = Color.White,
                Location = new Point(width, MagicLabel.DisplayArea.Bottom),
                Parent = MagicLabel,
                Text = $"Experience: {MouseMagic.Experience1:#,##0}",
            };

            MagicLabel.Size = new Size(label.DisplayArea.Right + 4 > MagicLabel.Size.Width ? label.DisplayArea.Right + 4 : MagicLabel.Size.Width, label.DisplayArea.Bottom);

            new DXLabel
            {
                ForeColour = User.Level < MouseMagic.NeedLevel2 ? Color.Red : Color.White,
                Location = new Point(4, MagicLabel.DisplayArea.Bottom),
                Parent = MagicLabel,
                Text = $"Rank 2 Requirement: Level {MouseMagic.NeedLevel2}",
            };

            label = new DXLabel
            {
                ForeColour = Color.White,
                Location = new Point(width , MagicLabel.DisplayArea.Bottom),
                Parent = MagicLabel,
                Text = $"Experience: {MouseMagic.Experience2:#,##0}",
            };

            MagicLabel.Size = new Size(label.DisplayArea.Right + 4 > MagicLabel.Size.Width ? label.DisplayArea.Right + 4 : MagicLabel.Size.Width, label.DisplayArea.Bottom);

            new DXLabel
            {
                ForeColour = User.Level < MouseMagic.NeedLevel3 ? Color.Red : Color.White,
                Location = new Point(4, MagicLabel.DisplayArea.Bottom),
                Parent = MagicLabel,
                Text = $"Rank 3 Requirement: Level {MouseMagic.NeedLevel3}",
            };

            label = new DXLabel
            {
                ForeColour = Color.White,
                Location = new Point(width, MagicLabel.DisplayArea.Bottom),
                Parent = MagicLabel,
                Text = $"Experience: {MouseMagic.Experience3:#,##0}",
            };
            MagicLabel.Size = new Size(label.DisplayArea.Right + 4 > MagicLabel.Size.Width ? label.DisplayArea.Right + 4 : MagicLabel.Size.Width, label.DisplayArea.Bottom);

            if (!disciplineSkill)
            {
                label = new DXLabel
                {
                    ForeColour = magic?.Level < 3 ? Color.Red : Color.White,
                    Location = new Point(4, MagicLabel.DisplayArea.Bottom),
                    Parent = MagicLabel,
                    Text = $"Rank 4+ Requirement: Books",
                };
                MagicLabel.Size = new Size(label.DisplayArea.Right + 4 > MagicLabel.Size.Width ? label.DisplayArea.Right + 4 : MagicLabel.Size.Width, label.DisplayArea.Bottom);
            }

            label = new DXLabel
            {
                AutoSize = false,
                ForeColour = Color.Wheat,
                Location = new Point(4, MagicLabel.DisplayArea.Bottom),
                Parent = MagicLabel,
                Text = MouseMagic.Description,
            };
            label.Size = DXLabel.GetHeight(label, MagicLabel.Size.Width);

            MagicLabel.Size = new Size(label.DisplayArea.Right + 4 > MagicLabel.Size.Width ? label.DisplayArea.Right + 4 : MagicLabel.Size.Width, label.DisplayArea.Bottom + 4);
        }

        private void SetItemInfo(SetInfo set)
        {

            DXLabel label = new DXLabel
            {
                ForeColour = Color.LimeGreen,
                Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                Parent = ItemLabel,
                Text = $"Item Set:"
            };

            ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);

            label = new DXLabel
            {
                ForeColour = Color.LimeGreen,
                Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                Parent = ItemLabel,
                Text = $"    {set.SetName}"
            };

            ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
            
            label = new DXLabel
            {
                ForeColour = Color.LimeGreen,
                Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                Parent = ItemLabel,
                Text = "Parts:"
            };

            ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);

            bool hasFullSet = true;
            List<int> counted = new List<int>();

            Stats setBonus = new Stats();

            int l;
            MirClass c;
            ClientUserItem[] equip;

            DXItemCell cell = MouseControl as DXItemCell;
            if (cell?.GridType == GridType.Inspect)
            {
                l = InspectBox.Level;
                c = InspectBox.Class;
                equip = InspectBox.Equipment;
            }
            else
            {
                l = User.Level;
                c = User.Class;
                equip = Equipment;
            }

            foreach (ItemInfo info in set.Items)
            {
                bool hasPart = false;
                for (int j = 0; j < equip.Length; j++)
                {
                    if (counted.Contains(j)) continue;
                    if (equip[j] == null) continue;
                    if (equip[j].Info != info) continue;
                    if (equip[j].CurrentDurability == 0 && equip[j].Info.Durability > 0) continue;
                    
                    counted.Add(j);

                    hasPart = true;
                    break;
                }

                if (!hasPart)
                    hasFullSet = false;

                label = new DXLabel
                {
                    ForeColour = hasPart ? Color.LimeGreen : Color.Gray,
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = "    " + info.ItemName
                };

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                    label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);
            }
            label = new DXLabel
            {
                ForeColour = Color.LimeGreen,
                Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                Parent = ItemLabel,
                Text = $"Set Bonus:"
            };

            ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);


            foreach (SetInfoStat stat in set.SetStats)
            {
                if (l < stat.Level) continue;

                switch (c)
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

                label = new DXLabel
                {
                    ForeColour = hasFullSet ? Color.LimeGreen : Color.Gray,
                    Location = new Point(4, ItemLabel.DisplayArea.Bottom),
                    Parent = ItemLabel,
                    Text = "    " + text
                };

                ItemLabel.Size = new Size(label.DisplayArea.Right + 4 > ItemLabel.Size.Width ? label.DisplayArea.Right + 4 : ItemLabel.Size.Width,
                                          label.DisplayArea.Bottom > ItemLabel.Size.Height ? label.DisplayArea.Bottom : ItemLabel.Size.Height);

            }


            ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);
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

            if (magic == null || User.Level < magic.Info.NeedLevel1) return;

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
                    if (User.VisibleBuffs.Contains(BuffType.Cloak)) break;
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
                    if (User.VisibleBuffs.Contains(BuffType.DarkConversion)) break;

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
                    if (MapObject.User.VisibleBuffs.Contains(BuffType.ElementalHurricane))
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

                case MagicType.ElementalSwords:

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

                case MagicType.PoisonDust:
                case MagicType.ExplosiveTalisman:
                case MagicType.EvilSlayer:
                case MagicType.GreaterEvilSlayer:
                case MagicType.ImprovedExplosiveTalisman:
                case MagicType.Parasite:
                case MagicType.Neutralize:
                case MagicType.SearingLight:

                case MagicType.Hemorrhage:
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
                    if (!User.VisibleBuffs.Contains(BuffType.Cloak)) return;
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
                    targetLocation = MapControl.MapLocation;
                    break;
                default:
                    targetLocation = target?.CurrentLocation ?? MapControl.MapLocation;
                    break;
            }

            //switch spell type.

            if (MouseObject != null && MouseObject.Race == ObjectType.Monster)
                FocusObject = (MonsterObject) MouseObject;

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
                    MonsterObject mob = (MonsterObject) ob;

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
                Point p = new Point(CEnvir.MouseLocation.X - imageSize.Width/2, CEnvir.MouseLocation.Y - imageSize.Height/2);

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
                ItemLabel.Draw();

            if (MagicLabel != null && !MagicLabel.IsDisposed)
                MagicLabel.Draw();
        }

        public void Displacement(MirDirection direction, Point location)
        {
            //if (MapObject.User.Direction == direction && MapObject.User.CurrentLocation == location) return;

            MapObject.User.ServerTime = DateTime.MinValue;
            MapObject.User.SetAction(new ObjectAction(MirAction.Standing, direction, location));
            MapObject.User.NextActionTime = CEnvir.Now.AddMilliseconds(300);
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
                    if (User.HandWeight - (Equipment[(int) slot]?.Info.Weight ?? 0) + item.Weight > User.Stats[Stat.HandWeight])
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
                    if (User.WearWeight - (Equipment[(int) slot]?.Info.Weight ?? 0) + item.Weight > User.Stats[Stat.WearWeight])
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
            MarketPlaceBox.ConsignTab.TabButton.Visible = !Observer;
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

            MarketPlaceBox.GameGoldBox.Value = User.GameGold.Amount;
            MarketPlaceBox.HuntGoldBox.Value = User.HuntGold.Amount;
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
            NPCCompanionStorageBox.UpdateScrollBar();

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


            text = text.Replace("[PLAYERNAME]", User.Name);
            text = text.Replace("[STARTNAME]", questInfo.StartNPC.NPCName);
            text = text.Replace("[FINISHNAME]", questInfo.FinishNPC.NPCName);

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
                    builder.AppendFormat("Collect {0} {1} from ", task.Amount, task.ItemParameter?.ItemName);  
                    break;
                case QuestTaskType.Region:
                    builder.AppendFormat("Goto {0} in {1}", task.RegionParameter?.Description, task.RegionParameter?.Map.Description);
                    break;
            }

            if (string.IsNullOrEmpty(task.MobDescription))
            {
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
                        builder.AppendFormat(" in {0}", monster.Map.Description);
                }
            }
            else
                builder.Append(task.MobDescription);

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
                if (quest.StartNPC.CurrentQuest != null) continue;

                quest.StartNPC.CurrentQuest = new CurrentQuest
                {
                    Type = quest.QuestType,
                    Icon = QuestIcon.New
                };
            }

            MainPanel.AvailableQuestIcon.Visible = QuestBox.AvailableTab.Quests.Count > 0;
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

            return new DXControl
            {
                Size = new Size(3, 3),
                DrawTexture = true,
                Hint = npc.NPCName,
                BackColour = Color.Lime
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

                if (MarketPlaceBox != null)
                {
                    if (!MarketPlaceBox.IsDisposed)
                        MarketPlaceBox.Dispose();

                    MarketPlaceBox = null;
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
