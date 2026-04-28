using Library;
using Library.SystemModels;
using MirDB;
using System;

namespace Server.DBModels
{
    [UserObject]
    public class UserMilestoneLog : DBObject
    {
        [Association("MilestoneLogs")]
        public CharacterInfo Character
        {
            get { return _Character; }
            set
            {
                if (_Character == value) return;

                var oldValue = _Character;
                _Character = value;

                OnChanged(oldValue, value, "Character");
            }
        }
        private CharacterInfo _Character;

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

        public int Count
        {
            get { return _Count; }
            set
            {
                if (_Count == value) return;

                var oldValue = _Count;
                _Count = value;

                OnChanged(oldValue, value, "Count");
            }
        }
        private int _Count;

        public CharacterInfo Player
        {
            get { return _Player; }
            set
            {
                if (_Player == value) return;

                var oldValue = _Player;
                _Player = value;

                OnChanged(oldValue, value, "Player");
            }
        }
        private CharacterInfo _Player;

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

        protected override void OnDeleted()
        {
            Character = null;
            Player = null;
            Item = null;
            Monster = null;
            Currency = null;
            Region = null;
            Instance = null;

            base.OnDeleted();
        }
    }

    [UserObject]
    public class UserMilestone : DBObject
    {
        public MilestoneInfo Info
        {
            get { return _Info; }
            set
            {
                if (_Info == value) return;

                var oldValue = _Info;
                _Info = value;

                OnChanged(oldValue, value, "Info");
            }
        }
        private MilestoneInfo _Info;

        [Association("Milestones")]
        public CharacterInfo Character
        {
            get { return _Character; }
            set
            {
                if (_Character == value) return;

                var oldValue = _Character;
                _Character = value;

                OnChanged(oldValue, value, "Character");
            }
        }
        private CharacterInfo _Character;

        public DateTime DateEarned
        {
            get { return _DateEarned; }
            set
            {
                if (_DateEarned == value) return;

                var oldValue = _DateEarned;
                _DateEarned = value;

                OnChanged(oldValue, value, "DateEarned");
            }
        }
        private DateTime _DateEarned;

        public bool Active
        {
            get { return _Active; }
            set
            {
                if (_Active == value) return;

                var oldValue = _Active;
                _Active = value;

                OnChanged(oldValue, value, "Active");
            }
        }
        private bool _Active;

        protected override void OnDeleted()
        {
            Info = null;
            Character = null;

            base.OnDeleted();
        }

        public ClientUserMilestone ToClientInfo()
        {
            return new ClientUserMilestone
            {
                Index = Index,
                InfoIndex = Info.Index,

                DateEarned = DateEarned,
                Active = Active
            };
        }
    }
}
