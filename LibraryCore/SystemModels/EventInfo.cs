using MirDB;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Library.SystemModels
{
    #region World Event

    public sealed class WorldEventInfo : DBObject
    {
        [IsIdentity]
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

        public bool ResetWhenMax
        {
            get { return _ResetWhenMax; }
            set
            {
                if (_ResetWhenMax == value) return;

                var oldValue = _ResetWhenMax;
                _ResetWhenMax = value;

                OnChanged(oldValue, value, "ResetWhenMax");
            }
        }
        private bool _ResetWhenMax;

        [Association("Triggers", true)]
        public DBBindingList<WorldEventTrigger> Triggers { get; set; }

        [Association("Actions", true)]
        public DBBindingList<WorldEventAction> Actions { get; set; }
    }

    public sealed class WorldEventTrigger : DBObject
    {
        [Association("Triggers")]
        public WorldEventInfo Event
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
        private WorldEventInfo _Event;

        public WorldEventTriggerType Type
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
        private WorldEventTriggerType _Type;

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

        public int MaxTriggers
        {
            get { return _MaxTriggers; }
            set
            {
                if (_MaxTriggers == value) return;

                var oldValue = _MaxTriggers;
                _MaxTriggers = value;

                OnChanged(oldValue, value, "MaxTriggers");
            }
        }
        private int _MaxTriggers;
    }

    public sealed class WorldEventAction : BaseEventAction
    {
        [Association("Actions")]
        public WorldEventInfo Event
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
        private WorldEventInfo _Event;

        [Association("TriggerStats", true)]
        public DBBindingList<WorldEventInfoTriggerStat> Stats { get; set; }

        protected internal override void OnLoaded()
        {
            base.OnLoaded();

            StatsChanged();
        }

        public void StatsChanged()
        {
            CalculatedStats.Clear();

            foreach (var stat in Stats)
                CalculatedStats[stat.Stat] += stat.Amount;
        }
    }

    public class WorldEventInfoTriggerStat : DBObject
    {
        [IsIdentity]
        [Association("TriggerStats")]
        public WorldEventAction Action
        {
            get { return _Action; }
            set
            {
                if (_Action == value) return;

                var oldValue = _Action;
                _Action = value;

                OnChanged(oldValue, value, "Action");
            }
        }
        private WorldEventAction _Action;

        [IsIdentity]
        public Stat Stat
        {
            get { return _Stat; }
            set
            {
                if (_Stat == value) return;

                var oldValue = _Stat;
                _Stat = value;

                OnChanged(oldValue, value, "Stat");
            }
        }
        private Stat _Stat;

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
    }

    #endregion

    #region Player Event

    public sealed class PlayerEventInfo : DBObject
    {
        [IsIdentity]
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

        public EventTrackingType TrackingType
        {
            get { return _TrackingType; }
            set
            {
                if (_TrackingType == value) return;

                var oldValue = _TrackingType;
                _TrackingType = value;

                OnChanged(oldValue, value, "TrackingType");
            }
        }
        private EventTrackingType _TrackingType;

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

        public bool ResetWhenMax
        {
            get { return _ResetWhenMax; }
            set
            {
                if (_ResetWhenMax == value) return;

                var oldValue = _ResetWhenMax;
                _ResetWhenMax = value;

                OnChanged(oldValue, value, "ResetWhenMax");
            }
        }
        private bool _ResetWhenMax;

        [Association("Triggers", true)]
        public DBBindingList<PlayerEventTrigger> Triggers { get; set; }

        [Association("Actions", true)]
        public DBBindingList<PlayerEventAction> Actions { get; set; }
    }

    public sealed class PlayerEventTrigger : DBObject
    {
        [Association("Triggers")]
        public PlayerEventInfo Event
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
        private PlayerEventInfo _Event;

        public PlayerEventTriggerType Type
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
        private PlayerEventTriggerType _Type; 
        
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

        public int MaxTriggers
        {
            get { return _MaxTriggers; }
            set
            {
                if (_MaxTriggers == value) return;

                var oldValue = _MaxTriggers;
                _MaxTriggers = value;

                OnChanged(oldValue, value, "MaxTriggers");
            }
        }
        private int _MaxTriggers;
    }

    public sealed class PlayerEventAction : BaseEventAction
    {
        [Association("Actions")]
        public PlayerEventInfo Event
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
        private PlayerEventInfo _Event;

        [Association("TriggerStats", true)]
        public DBBindingList<PlayerEventInfoTriggerStat> Stats { get; set; }

        protected internal override void OnLoaded()
        {
            base.OnLoaded();

            StatsChanged();
        }

        public void StatsChanged()
        {
            CalculatedStats.Clear();

            foreach (var stat in Stats)
                CalculatedStats[stat.Stat] += stat.Amount;
        }
    }

    public class PlayerEventInfoTriggerStat : DBObject
    {
        [IsIdentity]
        [Association("TriggerStats")]
        public PlayerEventAction Action
        {
            get { return _Action; }
            set
            {
                if (_Action == value) return;

                var oldValue = _Action;
                _Action = value;

                OnChanged(oldValue, value, "Action");
            }
        }
        private PlayerEventAction _Action;

        [IsIdentity]
        public Stat Stat
        {
            get { return _Stat; }
            set
            {
                if (_Stat == value) return;

                var oldValue = _Stat;
                _Stat = value;

                OnChanged(oldValue, value, "Stat");
            }
        }
        private Stat _Stat;

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
    }

    #endregion

    #region Monster Event

    public sealed class MonsterEventInfo : DBObject
    {
        [IsIdentity]
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

        public EventTrackingType TrackingType
        {
            get { return _TrackingType; }
            set
            {
                if (_TrackingType == value) return;

                var oldValue = _TrackingType;
                _TrackingType = value;

                OnChanged(oldValue, value, "TrackingType");
            }
        }
        private EventTrackingType _TrackingType;

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

        public bool ResetWhenMax
        {
            get { return _ResetWhenMax; }
            set
            {
                if (_ResetWhenMax == value) return;

                var oldValue = _ResetWhenMax;
                _ResetWhenMax = value;

                OnChanged(oldValue, value, "ResetWhenMax");
            }
        }
        private bool _ResetWhenMax;

        [Association("Triggers", true)]
        public DBBindingList<MonsterEventTrigger> Triggers { get; set; }

        [Association("Actions", true)]
        public DBBindingList<MonsterEventAction> Actions { get; set; }
    }

    public sealed class MonsterEventTrigger : DBObject
    {
        [Association("Triggers")]
        public MonsterEventInfo Event
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
        private MonsterEventInfo _Event;

        public MonsterEventTriggerType Type
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
        private MonsterEventTriggerType _Type;

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

        public int MaxTriggers
        {
            get { return _MaxTriggers; }
            set
            {
                if (_MaxTriggers == value) return;

                var oldValue = _MaxTriggers;
                _MaxTriggers = value;

                OnChanged(oldValue, value, "MaxTriggers");
            }
        }
        private int _MaxTriggers;
    }

    public sealed class MonsterEventAction : BaseEventAction
    {
        [Association("Actions")]
        public MonsterEventInfo Event
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
        private MonsterEventInfo _Event;

        [Association("TriggerStats", true)]
        public DBBindingList<MonsterEventInfoTriggerStat> Stats { get; set; }

        protected internal override void OnLoaded()
        {
            base.OnLoaded();

            StatsChanged();
        }

        public void StatsChanged()
        {
            CalculatedStats.Clear();

            foreach (var stat in Stats)
                CalculatedStats[stat.Stat] += stat.Amount;
        }
    }

    public class MonsterEventInfoTriggerStat : DBObject
    {
        [IsIdentity]
        [Association("TriggerStats")]
        public MonsterEventAction Action
        {
            get { return _Action; }
            set
            {
                if (_Action == value) return;

                var oldValue = _Action;
                _Action = value;

                OnChanged(oldValue, value, "Action");
            }
        }
        private MonsterEventAction _Action;

        [IsIdentity]
        public Stat Stat
        {
            get { return _Stat; }
            set
            {
                if (_Stat == value) return;

                var oldValue = _Stat;
                _Stat = value;

                OnChanged(oldValue, value, "Stat");
            }
        }
        private Stat _Stat;

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
    }

    #endregion

    #region Base Event

    public class BaseEventAction : DBObject
    {
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

        public bool Restrict
        {
            get { return _Restrict; }
            set
            {
                if (_Restrict == value) return;

                var oldValue = _Restrict;
                _Restrict = value;

                OnChanged(oldValue, value, "Restrict");
            }
        }
        private bool _Restrict;

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

        public Stats CalculatedStats = new();
    }

    #endregion

    #region Enums

    public enum EventTrackingType
    {
        Global = 0,
        Player = 1,
        Group = 2,
        Guild = 3,

        Instance = 10
    }

    public enum WorldEventTriggerType
    {
        Dawn = 0,
        Day = 1,
        Dusk = 2,
        Night = 3,
    }

    public enum PlayerEventTriggerType
    {
        PlayerEnter = 0,
        PlayerLeave = 1,
        PlayerDie = 2,
        PlayerCommand = 3,

        TimerMinute = 10,
    }

    public enum MonsterEventTriggerType
    {
        MonsterDie = 0,
        MonsterClear = 1
    }

    public enum EventActionType
    {
        MonsterSpawn = 0,
        MonsterPlayerSpawn = 1,
        MonsterBuffAdd = 2,
        MonsterBuffRemove = 3,

        PlayerMessage = 10,
        PlayerTeleport = 11,
        PlayerEscape = 12,
        PlayerBuffAdd = 13,
        PlayerBuffRemove = 14,

        TimerStart = 20,
        TimerStop = 21,
        TimerReset = 22,

        ItemDrop = 30,
        ItemGive = 31
    }

    #endregion

    #region Event Log

    public class EventLog
    {
        public string Key { get; set; }

        public WorldEventInfo WorldEvent { get; set; }
        public PlayerEventInfo PlayerEvent { get; set; }
        public MonsterEventInfo MonsterEvent { get; set; }

        public int PlayerIndex { get; set; }

        public InstanceInfo InstanceInfo { get; set; }
        public byte InstanceSequence { get; set; }

        public int CurrentValue;

        public Dictionary<WorldEventTrigger, int> WorldTriggerCount { get; set; } = [];
        public Dictionary<PlayerEventTrigger, int> PlayerTriggerCount { get; set; } = [];
        public Dictionary<MonsterEventTrigger, int> MonsterTriggerCount { get; set; } = [];

        public void Reset()
        {
            CurrentValue = 0;

            WorldTriggerCount.Clear();
            PlayerTriggerCount.Clear();
            MonsterTriggerCount.Clear();
        }
    }

    #endregion
}
