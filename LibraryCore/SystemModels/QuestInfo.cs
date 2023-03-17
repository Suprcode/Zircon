using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MirDB;

namespace Library.SystemModels
{
    public sealed class QuestInfo : DBObject
    {
        [IsIdentity]
        public string QuestName
        {
            get { return _QuestName; }
            set
            {
                if (_QuestName == value) return;

                var oldValue = _QuestName;
                _QuestName = value;

                OnChanged(oldValue, value, "QuestName");
            }
        }
        private string _QuestName;

        public QuestType QuestType
        {
            get { return _QuestType; }
            set
            {
                if (_QuestType == value) return;

                var oldValue = _QuestType;
                _QuestType = value;

                OnChanged(oldValue, value, "QuestType");
            }
        }
        private QuestType _QuestType;

        public string AcceptText
        {
            get { return _AcceptText; }
            set
            {
                if (_AcceptText == value) return;

                var oldValue = _AcceptText;
                _AcceptText = value;

                OnChanged(oldValue, value, "AcceptText");
            }
        }
        private string _AcceptText;
        
        public string ProgressText
        {
            get { return _ProgressText; }
            set
            {
                if (_ProgressText == value) return;

                var oldValue = _ProgressText;
                _ProgressText = value;

                OnChanged(oldValue, value, "ProgressText");
            }
        }
        private string _ProgressText;

        public string CompletedText
        {
            get { return _CompletedText; }
            set
            {
                if (_CompletedText == value) return;

                var oldValue = _CompletedText;
                _CompletedText = value;

                OnChanged(oldValue, value, "CompletedText");
            }
        }
        private string _CompletedText;

        public string ArchiveText
        {
            get { return _ArchiveText; }
            set
            {
                if (_ArchiveText == value) return;

                var oldValue = _ArchiveText;
                _ArchiveText = value;

                OnChanged(oldValue, value, "ArchiveText");
            }
        }
        private string _ArchiveText;
              
        [Association("Requirements", true)]
        public DBBindingList<QuestRequirement> Requirements { get; set; }

        [Association("StartQuests")]
        public NPCInfo StartNPC
        {
            get { return _StartNPC; }
            set
            {
                if (_StartNPC == value) return;

                var oldValue = _StartNPC;
                _StartNPC = value;

                OnChanged(oldValue, value, "StartNPC");
            }
        }
        private NPCInfo _StartNPC;

        [Association("FinishQuests")]
        public NPCInfo FinishNPC
        {
            get { return _FinishNPC; }
            set
            {
                if (_FinishNPC == value) return;

                var oldValue = _FinishNPC;
                _FinishNPC = value;

                OnChanged(oldValue, value, "FinishNPC");
            }
        }
        private NPCInfo _FinishNPC;


        [Association("Rewards", true)]
        public DBBindingList<QuestReward> Rewards { get; set; }

        [Association("Tasks", true)]
        public DBBindingList<QuestTask> Tasks { get; set; }

        protected internal override void OnCreated()
        {
            base.OnCreated();

            QuestRequirement requirement = Requirements.AddNew();

            requirement.Requirement = QuestRequirementType.HaveNotCompleted;
            requirement.Quest = this;
            requirement.QuestParameter = this;
        }
    }

    public sealed class QuestReward : DBObject
    {
        [Association("Rewards")]
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

        public bool Choice
        {
            get { return _Choice; }
            set
            {
                if (_Choice == value) return;

                var oldValue = _Choice;
                _Choice = value;

                OnChanged(oldValue, value, "Choice");
            }
        }
        private bool _Choice;

        public bool Bound
        {
            get { return _Bound; }
            set
            {
                if (_Bound == value) return;

                var oldValue = _Bound;
                _Bound = value;

                OnChanged(oldValue, value, "Bound");
            }
        }
        private bool _Bound;

        public int Duration
        {
            get { return _Duration; }
            set
            {
                if (_Duration == value) return;

                var oldValue = _Duration;
                _Duration = value;

                OnChanged(oldValue, value, "Duration");
            }
        }
        private int _Duration;

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

        protected internal override void OnCreated()
        {
            base.OnCreated();

            Amount = 1;

            Class = RequiredClass.All;
        }
    }



    public sealed class QuestRequirement : DBObject
    {
        [Association("Requirements")]
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

        public QuestRequirementType Requirement
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
        private QuestRequirementType _Requirement;
        
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
        
    }

    public sealed class QuestTask : DBObject
    {
        [Association("Tasks")]
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

        public QuestTaskType Task
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
        private QuestTaskType _Task;
        
        public ItemInfo ItemParameter
        {
            get { return _ItemParameter; }
            set
            {
                if (_ItemParameter == value) return;

                var oldValue = _ItemParameter;
                _ItemParameter = value;

                OnChanged(oldValue, value, "ItemParameter");
            }
        }
        private ItemInfo _ItemParameter;

        public MapRegion RegionParameter
        {
            get { return _RegionParameter; }
            set
            {
                if (_RegionParameter == value) return;

                var oldValue = _RegionParameter;
                _RegionParameter = value;

                OnChanged(oldValue, value, "RegionParameter");
            }
        }
        private MapRegion _RegionParameter;

        public string MobDescription
        {
            get { return _MobDescription; }
            set
            {
                if (_MobDescription == value) return;

                var oldValue = _MobDescription;
                _MobDescription = value;

                OnChanged(oldValue, value, "MobDescription");
            }
        }
        private string _MobDescription;
        
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

        [Association("MonsterDetails", true)]
        public DBBindingList<QuestTaskMonsterDetails> MonsterDetails { get; set; }
    }

    public sealed class QuestTaskMonsterDetails : DBObject
    {
        [Association("MonsterDetails")]
        public QuestTask Task
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
        private QuestTask _Task;

        [Association("QuestDetails")]
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

        //Can Be null
        public MapInfo Map
        {
            get { return _Map; }
            set
            {
                if (_Map == value) return;

                var oldValue = _Map;
                _Map = value;

                OnChanged(oldValue, value, "Map");
            }
        }
        private MapInfo _Map;
        
        public int Chance
        {
            get { return _Chance; }
            set
            {
                if (_Chance == value) return;

                var oldValue = _Chance;
                _Chance = value;

                OnChanged(oldValue, value, "Chance");
            }
        }
        private int _Chance;

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


        //Can be 0
        public int DropSet
        {
            get { return _DropSet; }
            set
            {
                if (_DropSet == value) return;

                var oldValue = _DropSet;
                _DropSet = value;

                OnChanged(oldValue, value, "DropSet");
            }
        }
        private int _DropSet;

        protected internal override void OnCreated()
        {
            base.OnCreated();

            Chance = 1;
            Amount = 1;
        }
    }
}
