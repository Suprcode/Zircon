using System;
using MirDB;

namespace Library.SystemModels
{
    public sealed class NPCInfo : DBObject
    {
        public MapRegion Region
        {
            get { return _Region; }
            set
            {
                if (_Region == value) return;

                var oldValue = _Region;
                _Region = value;

                OnChanged(oldValue, value, "Region");
            }
        }
        private MapRegion _Region;

        public string NPCName
        {
            get { return _NPCName; }
            set
            {
                if (_NPCName == value) return;

                var oldValue = _NPCName;
                _NPCName = value;

                OnChanged(oldValue, value, "NPCName");
            }
        }
        private string _NPCName;

        public int Image
        {
            get { return _Image; }
            set
            {
                if (_Image == value) return;

                var oldValue = _Image;
                _Image = value;

                OnChanged(oldValue, value, "Image");
            }
        }
        private int _Image;

        public int FaceImage
        {
            get { return _FaceImage; }
            set
            {
                if (_FaceImage == value) return;

                var oldValue = _FaceImage;
                _FaceImage = value;

                OnChanged(oldValue, value, "FaceImage");
            }
        }
        private int _FaceImage;

        public NPCPage EntryPage
        {
            get { return _EntryPage; }
            set
            {
                if (_EntryPage == value) return;

                var oldValue = _EntryPage;
                _EntryPage = value;

                OnChanged(oldValue, value, "EntryPage");
            }
        }
        private NPCPage _EntryPage;
      
        [IgnoreProperty]
        public string RegionName => Region?.ServerDescription ?? string.Empty;

        [Association("StartQuests")]
        public DBBindingList<QuestInfo> StartQuests { get; set; }

        [Association("FinishQuests")]
        public DBBindingList<QuestInfo> FinishQuests { get; set; }

        [Association("Requirements", true)]
        public DBBindingList<NPCRequirement> Requirements { get; set; }

        [IgnoreProperty]
        public CurrentQuest CurrentQuest { get; set; }
    }

    public sealed class CurrentQuest : IEquatable<CurrentQuest>
    {
        public QuestType Type { get; set; }
        public QuestIcon Icon { get; set; }

        public bool Equals(CurrentQuest other)
        {
            return other.Type == Type && other.Icon == Icon;
        }
    }

    public sealed class NPCPage : DBObject
    {
        public string Description
        {
            get { return _Description; }
            set
            {
                if (_Description == value) return;

                var oldValue = _Description;
                _Description = value;

                OnChanged(oldValue, value, "Description");
            }
        }
        private string _Description;

        public NPCDialogType DialogType
        {
            get { return _DialogType; }
            set
            {
                if (_DialogType == value) return;

                var oldValue = _DialogType;
                _DialogType = value;

                OnChanged(oldValue, value, "DialogType");
            }
        }
        private NPCDialogType _DialogType;

        public string Say
        {
            get { return _Say; }
            set
            {
                if (_Say == value) return;

                var oldValue = _Say;
                _Say = value;

                OnChanged(oldValue, value, "Say");
            }
        }
        private string _Say;

        public NPCPage SuccessPage
        {
            get { return _SuccessPage; }
            set
            {
                if (_SuccessPage == value) return;

                var oldValue = _SuccessPage;
                _SuccessPage = value;

                OnChanged(oldValue, value, "SuccessPage");
            }
        }
        private NPCPage _SuccessPage;

        public string Arguments
        {
            get { return _Arguments; }
            set
            {
                if (_Arguments == value) return;

                var oldValue = _Arguments;
                _Arguments = value;

                OnChanged(oldValue, value, "Arguments");
            }
        }
        private string _Arguments;

        public CurrencyInfo Currency
        {
            get { return _Currency; }
            set
            {
                if (_Currency == value) return;

                var oldValue = _Currency;
                _Currency = value;

                OnChanged(oldValue, value, "Currency");
            }
        }
        private CurrencyInfo _Currency;

        [Association("Checks", true)]
        public DBBindingList<NPCCheck> Checks { get; set; }

        [Association("Actions", true)]
        public DBBindingList<NPCAction> Actions { get; set; }

        [Association("Buttons", true)]
        public DBBindingList<NPCButton> Buttons { get; set; }

        [Association("Goods", true)]
        public DBBindingList<NPCGood> Goods { get; set; }

        [Association("Types", true)]
        public DBBindingList<NPCType> Types { get; set; }
    }

    public sealed class NPCGood : DBObject
    {
        [Association("Goods")]
        public NPCPage Page
        {
            get { return _Page; }
            set
            {
                if (_Page == value) return;

                var oldValue = _Page;
                _Page = value;

                OnChanged(oldValue, value, "Page");
            }
        }
        private NPCPage _Page;


        public ItemInfo Item
        {
            get { return _Item; }
            set
            {
                if (_Item == value) return;

                var oldValue = _Item;
                _Item = value;

                OnChanged(oldValue, value, "Item");
            }
        }
        private ItemInfo _Item;

        public decimal Rate
        {
            get { return _Rate; }
            set
            {
                if (_Rate == value) return;

                var oldValue = _Rate;
                _Rate = value;

                OnChanged(oldValue, value, "Rate");
            }
        }
        private decimal _Rate;


        protected internal override void OnCreated()
        {
            base.OnCreated();

            Rate = 1M;
        }


        [IgnoreProperty]
        public int Cost => (int)Math.Round(Item.Price * Rate);
    }

    public sealed class NPCType : DBObject
    {
        [Association("Types")]
        public NPCPage Page
        {
            get { return _Page; }
            set
            {
                if (_Page == value) return;

                var oldValue = _Page;
                _Page = value;

                OnChanged(oldValue, value, "Page");
            }
        }
        private NPCPage _Page;

        public ItemType ItemType
        {
            get { return _ItemType; }
            set
            {
                if (_ItemType == value) return;

                var oldValue = _ItemType;
                _ItemType = value;

                OnChanged(oldValue, value, "ItemType");
            }
        }
        private ItemType _ItemType;
    }

    public sealed class NPCCheck : DBObject
    {
        [Association("Checks")]
        public NPCPage Page
        {
            get { return _Page; }
            set
            {
                if (_Page == value) return;

                var oldValue = _Page;
                _Page = value;

                OnChanged(oldValue, value, "Page");
            }
        }
        private NPCPage _Page;

        public NPCCheckType CheckType
        {
            get { return _CheckType; }
            set
            {
                if (_CheckType == value) return;

                var oldValue = _CheckType;
                _CheckType = value;

                OnChanged(oldValue, value, "CheckType");
            }
        }
        private NPCCheckType _CheckType;

        public Operator Operator
        {
            get { return _Operator; }
            set
            {
                if (_Operator == value) return;

                var oldValue = _Operator;
                _Operator = value;

                OnChanged(oldValue, value, "Operator");
            }
        }
        private Operator _Operator;

        public string StringParameter1
        {
            get { return _StringParameter1; }
            set
            {
                if (_StringParameter1 == value) return;

                var oldValue = _StringParameter1;
                _StringParameter1 = value;

                OnChanged(oldValue, value, "StringParameter1");
            }
        }
        private string _StringParameter1;

        public int IntParameter1
        {
            get { return _IntParameter1; }
            set
            {
                if (_IntParameter1 == value) return;

                var oldValue = _IntParameter1;
                _IntParameter1 = value;

                OnChanged(oldValue, value, "IntParameter1");
            }
        }
        private int _IntParameter1;

        public int IntParameter2
        {
            get { return _IntParameter2; }
            set
            {
                if (_IntParameter2 == value) return;

                var oldValue = _IntParameter2;
                _IntParameter2 = value;

                OnChanged(oldValue, value, "IntParameter2");
            }
        }
        private int _IntParameter2;

        public ItemInfo ItemParameter1
        {
            get { return _ItemParameter1; }
            set
            {
                if (_ItemParameter1 == value) return;

                var oldValue = _ItemParameter1;
                _ItemParameter1 = value;

                OnChanged(oldValue, value, "ItemParameter1");
            }
        }
        private ItemInfo _ItemParameter1;


        public Stat StatParameter1
        {
            get { return _StatParameter1; }
            set
            {
                if (_StatParameter1 == value) return;

                var oldValue = _StatParameter1;
                _StatParameter1 = value;

                OnChanged(oldValue, value, "StatParameter1");
            }
        }
        private Stat _StatParameter1;

        public NPCPage FailPage
        {
            get { return _FailPage; }
            set
            {
                if (_FailPage == value) return;

                var oldValue = _FailPage;
                _FailPage = value;

                OnChanged(oldValue, value, "FailPage");
            }
        }
        private NPCPage _FailPage;
    }

    public sealed class NPCAction : DBObject
    {
        [Association("Actions")]
        public NPCPage Page
        {
            get { return _Page; }
            set
            {
                if (_Page == value) return;

                var oldValue = _Page;
                _Page = value;

                OnChanged(oldValue, value, "Page");
            }
        }
        private NPCPage _Page;

        public NPCActionType ActionType
        {
            get { return _ActionType; }
            set
            {
                if (_ActionType == value) return;

                var oldValue = _ActionType;
                _ActionType = value;

                OnChanged(oldValue, value, "ActionType");
            }
        }
        private NPCActionType _ActionType;

        public string StringParameter1
        {
            get { return _StringParameter1; }
            set
            {
                if (_StringParameter1 == value) return;

                var oldValue = _StringParameter1;
                _StringParameter1 = value;

                OnChanged(oldValue, value, "StringParameter1");
            }
        }
        private string _StringParameter1;

        public int IntParameter1
        {
            get { return _IntParameter1; }
            set
            {
                if (_IntParameter1 == value) return;

                var oldValue = _IntParameter1;
                _IntParameter1 = value;

                OnChanged(oldValue, value, "IntParameter1");
            }
        }
        private int _IntParameter1;

        public int IntParameter2
        {
            get { return _IntParameter2; }
            set
            {
                if (_IntParameter2 == value) return;

                var oldValue = _IntParameter2;
                _IntParameter2 = value;

                OnChanged(oldValue, value, "IntParameter2");
            }
        }
        private int _IntParameter2;

        public ItemInfo ItemParameter1
        {
            get { return _ItemParameter1; }
            set
            {
                if (_ItemParameter1 == value) return;

                var oldValue = _ItemParameter1;
                _ItemParameter1 = value;

                OnChanged(oldValue, value, "ItemParameter1");
            }
        }
        private ItemInfo _ItemParameter1;
        
        public MapInfo MapParameter1
        {
            get { return _MapParameter1; }
            set
            {
                if (_MapParameter1 == value) return;

                var oldValue = _MapParameter1;
                _MapParameter1 = value;

                OnChanged(oldValue, value, "MapParameter1");
            }
        }
        private MapInfo _MapParameter1;


        public InstanceInfo InstanceParameter1
        {
            get { return _InstanceParameter1; }
            set
            {
                if (_InstanceParameter1 == value) return;

                var oldValue = _InstanceParameter1;
                _InstanceParameter1 = value;

                OnChanged(oldValue, value, "InstanceParameter1");
            }
        }
        private InstanceInfo _InstanceParameter1;

        public Stat StatParameter1
        {
            get { return _StatParameter1; }
            set
            {
                if (_StatParameter1 == value) return;

                var oldValue = _StatParameter1;
                _StatParameter1 = value;

                OnChanged(oldValue, value, "StatParameter1");
            }
        }
        private Stat _StatParameter1;
    }

    public sealed class NPCButton : DBObject
    {
        [Association("Buttons")]
        public NPCPage Page
        {
            get { return _Page; }
            set
            {
                if (_Page == value) return;

                var oldValue = _Page;
                _Page = value;

                OnChanged(oldValue, value, "Page");
            }
        }
        private NPCPage _Page;

        public int ButtonID
        {
            get { return _ButtonID; }
            set
            {
                if (_ButtonID == value) return;

                var oldValue = _ButtonID;
                _ButtonID = value;

                OnChanged(oldValue, value, "ButtonID");
            }
        }
        private int _ButtonID;

        public NPCPage DestinationPage
        {
            get { return _DestinationPage; }
            set
            {
                if (_DestinationPage == value) return;

                var oldValue = _DestinationPage;
                _DestinationPage = value;

                OnChanged(oldValue, value, "DestinationPage");
            }
        }
        private NPCPage _DestinationPage;



    }

    public sealed class NPCRequirement : DBObject
    {
        [Association("Requirements")]
        public NPCInfo NPC
        {
            get { return _NPC; }
            set
            {
                if (_NPC == value) return;

                var oldValue = _NPC;
                _NPC = value;

                OnChanged(oldValue, value, "NPC");
            }
        }
        private NPCInfo _NPC;

        public NPCRequirementType Requirement
        {
            get { return _Requirement; }
            set
            {
                if (_Requirement == value) return;

                var oldValue = _Requirement;
                _Requirement = value;

                OnChanged(oldValue, value, "Requirement");
            }
        }
        private NPCRequirementType _Requirement;

        public int IntParameter1
        {
            get { return _IntParameter1; }
            set
            {
                if (_IntParameter1 == value) return;

                var oldValue = _IntParameter1;
                _IntParameter1 = value;

                OnChanged(oldValue, value, "IntParameter1");
            }
        }
        private int _IntParameter1;

        public QuestInfo QuestParameter
        {
            get { return _QuestParameter; }
            set
            {
                if (_QuestParameter == value) return;

                var oldValue = _QuestParameter;
                _QuestParameter = value;

                OnChanged(oldValue, value, "QuestParameter");
            }
        }
        private QuestInfo _QuestParameter;

        public RequiredClass Class
        {
            get { return _Class; }
            set
            {
                if (_Class == value) return;

                var oldValue = _Class;
                _Class = value;

                OnChanged(oldValue, value, "Class");
            }
        }
        private RequiredClass _Class;

        public DaysOfWeek DaysOfWeek
        {
            get { return _DayOfWeek; }
            set
            {
                if (_DayOfWeek == value) return;

                var oldValue = _DayOfWeek;
                _DayOfWeek = value;

                OnChanged(oldValue, value, "DaysOfWeek");
            }
        }
        private DaysOfWeek _DayOfWeek;
    }

    public enum NPCCheckType
    {
        Level,
        Class,
        Gender,
        Gold,
        HasItem,
        PKPoints,

        HasWeapon,
        WeaponLevel,
        WeaponElement,
        WeaponCanRefine,

        Horse,

        Marriage,
        WeddingRing,

        CanGainItem,
        CanResetWeapon,

        Random,

        WeaponAddedStats,

        Currency
    }
    public enum Operator
    {
        Equal,
        NotEqual,
        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual,
    }
    public enum NPCActionType
    {
        Teleport,
        GiveGold,
        TakeGold,
        GiveItem,
        TakeItem,
        ChangeElement,
        ChangeHorse,
        Message,

        Marriage,
        Divorce,
        RemoveWeddingRing,

        ResetWeapon,
        GiveItemExperience,

        SpecialRefine,
        Rebirth,

        GiveCurrency,
        TakeCurrency
    }
}
