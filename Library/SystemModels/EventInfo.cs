using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MirDB;

namespace Library.SystemModels
{
    public sealed class EventInfo : DBObject
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

        public int MaxValue
        {
            get { return _MaxValue; }
            set
            {
                if (_MaxValue == value) return;

                var oldValue = _MaxValue;
                _MaxValue = value;

                OnChanged(oldValue, value, "MaxValue");
            }
        }
        private int _MaxValue;



        [Association("Targets", true)]
        public DBBindingList<EventTarget> Targets { get; set; }

        [Association("Actions", true)]
        public DBBindingList<EventAction> Actions { get; set; }


        public int CurrentValue; //Server Variable.

    }

    public sealed class EventTarget : DBObject
    {
        [Association("Targets")]
        public EventInfo Event
        {
            get { return _Event; }
            set
            {
                if (_Event == value) return;

                var oldValue = _Event;
                _Event = value;

                OnChanged(oldValue, value, "Event");
            }
        }
        private EventInfo _Event;

        [Association("Events")]
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

        public int Value
        {
            get { return _Value; }
            set
            {
                if (_Value == value) return;

                var oldValue = _Value;
                _Value = value;

                OnChanged(oldValue, value, "Value");
            }
        }
        private int _Value;
    }

    public sealed class EventAction : DBObject
    {
        [Association("Actions")]
        public EventInfo Event
        {
            get { return _Event; }
            set
            {
                if (_Event == value) return;

                var oldValue = _Event;
                _Event = value;

                OnChanged(oldValue, value, "Event");
            }
        }
        private EventInfo _Event;
        

        public int TriggerValue
        {
            get { return _TriggerValue; }
            set
            {
                if (_TriggerValue == value) return;

                var oldValue = _TriggerValue;
                _TriggerValue = value;

                OnChanged(oldValue, value, "TriggerValue");
            }
        }
        private int _TriggerValue;

        public EventActionType Type
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
        private EventActionType _Type;

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

        public MonsterInfo MonsterParameter1
        {
            get { return _MonsterParameter1; }
            set
            {
                if (_MonsterParameter1 == value) return;

                var oldValue = _MonsterParameter1;
                _MonsterParameter1 = value;

                OnChanged(oldValue, value, "MonsterParameter1");
            }
        }
        private MonsterInfo _MonsterParameter1;

        public RespawnInfo RespawnParameter1
        {
            get { return _RespawnParameter1; }
            set
            {
                if (_RespawnParameter1 == value) return;

                var oldValue = _RespawnParameter1;
                _RespawnParameter1 = value;

                OnChanged(oldValue, value, "RespawnParameter1");
            }
        }
        private RespawnInfo _RespawnParameter1;
        
        
        public MapRegion RegionParameter1
        {
            get { return _RegionParameter1; }
            set
            {
                if (_RegionParameter1 == value) return;

                var oldValue = _RegionParameter1;
                _RegionParameter1 = value;

                OnChanged(oldValue, value, "RegionParameter1");
            }
        }
        private MapRegion _RegionParameter1;

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
        
        
        
    }


    public enum EventActionType
    {
        None,
        GlobalMessage,
        MapMessage,
        PlayerMessage,
        MonsterSpawn,
        MonsterPlayerSpawn,
        MovementSettings,
        PlayerRecall,
        PlayerEscape,
    }
}
