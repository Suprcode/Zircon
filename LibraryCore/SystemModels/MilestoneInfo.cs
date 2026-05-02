using MirDB;
using System.Drawing;

namespace Library.SystemModels
{
    public sealed class MilestoneInfo : DBObject
    {
        [IsIdentity]
        public string Title
        {
            get { return _Title; }
            set
            {
                if (_Title == value) return;

                var oldValue = _Title;
                _Title = value;

                OnChanged(oldValue, value, "Title");
            }
        }
        private string _Title;

        public string Category
        {
            get { return _Category; }
            set
            {
                if (_Category == value) return;

                var oldValue = _Category;
                _Category = value;

                OnChanged(oldValue, value, "Category");
            }
        }
        private string _Category;

        public MilestoneGrade Grade
        {
            get { return _Grade; }
            set
            {
                if (_Grade == value) return;

                var oldValue = _Grade;
                _Grade = value;

                OnChanged(oldValue, value, "Grade");
            }
        }
        private MilestoneGrade _Grade;

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

        public string Task
        {
            get { return _Task; }
            set
            {
                if (_Task == value) return;

                var oldValue = _Task;
                _Task = value;

                OnChanged(oldValue, value, "Task");
            }
        }
        private string _Task;

        public bool ShowCount
        {
            get { return _ShowCount; }
            set
            {
                if (_ShowCount == value) return;

                var oldValue = _ShowCount;
                _ShowCount = value;

                OnChanged(oldValue, value, "ShowCount");
            }
        }
        private bool _ShowCount;

        public RequiredClass RequiredClass
        {
            get { return _RequiredClass; }
            set
            {
                if (_RequiredClass == value) return;

                var oldValue = _RequiredClass;
                _RequiredClass = value;

                OnChanged(oldValue, value, "RequiredClass");
            }
        }
        private RequiredClass _RequiredClass;

        public ItemInfo Reward
        {
            get { return _Reward; }
            set
            {
                if (_Reward == value) return;

                var oldValue = _Reward;
                _Reward = value;

                OnChanged(oldValue, value, "Reward");
            }
        }
        private ItemInfo _Reward;

        public int RewardAmount
        {
            get { return _RewardAmount; }
            set
            {
                if (_RewardAmount == value) return;

                var oldValue = _RewardAmount;
                _RewardAmount = value;

                OnChanged(oldValue, value, "RewardAmount");
            }
        }
        private int _RewardAmount;

        [Association("MilestoneInfoTasks", true)]
        public DBBindingList<MilestoneInfoTask> Tasks { get; set; }

        protected internal override void OnCreated()
        {
            base.OnCreated();

            RequiredClass = RequiredClass.All;
            RewardAmount = 1;
            ShowCount = true;
        }

        [IgnoreProperty]
        public Color OutlineColour
        {
            get
            {
                return Grade switch
                {
                    MilestoneGrade.Low => Color.Black,
                    MilestoneGrade.Medium => Color.Blue,
                    MilestoneGrade.High => Color.Red,
                    _ => Color.Black,
                };
            }
        }

        public override string ToString()
        {
            return Title;
        }
    }

    public sealed class MilestoneInfoTask : DBObject
    {
        [IsIdentity]
        [Association("MilestoneInfoTasks")]
        public MilestoneInfo Milestone
        {
            get { return _Milestone; }
            set
            {
                if (_Milestone == value) return;

                var oldValue = _Milestone;
                _Milestone = value;

                OnChanged(oldValue, value, "Milestone");
            }
        }
        private MilestoneInfo _Milestone;

        [IsIdentity]
        public MilestoneType Type
        {
            get { return _Type; }
            set
            {
                if (_Type == value) return;

                var oldValue = _Type;
                _Type = value;

                OnChanged(oldValue, value, "Type");
            }
        }
        private MilestoneType _Type;

        [IsIdentity]
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

        [IsIdentity]
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

        [IsIdentity]
        public MonsterInfo Monster
        {
            get { return _Monster; }
            set
            {
                if (_Monster == value) return;

                var oldValue = _Monster;
                _Monster = value;

                OnChanged(oldValue, value, "Monster");
            }
        }
        private MonsterInfo _Monster;

        [IsIdentity]
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

        [IsIdentity]
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

        [IsIdentity]
        public InstanceInfo Instance
        {
            get { return _Instance; }
            set
            {
                if (_Instance == value) return;

                var oldValue = _Instance;
                _Instance = value;

                OnChanged(oldValue, value, "Instance");
            }
        }
        private InstanceInfo _Instance;

        [IsIdentity]
        public QuestInfo Quest
        {
            get { return _Quest; }
            set
            {
                if (_Quest == value) return;

                var oldValue = _Quest;
                _Quest = value;

                OnChanged(oldValue, value, "Quest");
            }
        }
        private QuestInfo _Quest;

        [IsIdentity]
        public MagicInfo Magic
        {
            get { return _Magic; }
            set
            {
                if (_Magic == value) return;

                var oldValue = _Magic;
                _Magic = value;

                OnChanged(oldValue, value, "Magic");
            }
        }
        private MagicInfo _Magic;

        [IsIdentity]
        public int Amount
        {
            get { return _Amount; }
            set
            {
                if (_Amount == value) return;

                var oldValue = _Amount;
                _Amount = value;

                OnChanged(oldValue, value, "Amount");
            }
        }
        private int _Amount;

        protected internal override void OnCreated()
        {
            base.OnCreated();

            Class = RequiredClass.None;
            Amount = 1;
        }
    }
}
