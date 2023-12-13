using MirDB;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Library.SystemModels
{
    public sealed class InstanceInfo : DBObject
    {
        [IsIdentity]
        public string Name
        {
            get { return _Name; }
            set
            {
                if (_Name == value) return;

                var oldValue = _Name;
                _Name = value;

                OnChanged(oldValue, value, "Name");
            }
        }
        private string _Name;

        public InstanceType Type
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
        private InstanceType _Type;

        public byte MaxInstances
        {
            get { return _MaxInstances; }
            set
            {
                if (_MaxInstances == value) return;

                var oldValue = _MaxInstances;
                _MaxInstances = value;

                OnChanged(oldValue, value, "MaximumAllowed");
            }
        }
        private byte _MaxInstances;

        public bool ShowOnDungeonFinder
        {
            get { return _ShowOnDungeonFinder; }
            set
            {
                if (_ShowOnDungeonFinder == value) return;

                var oldValue = _ShowOnDungeonFinder;
                _ShowOnDungeonFinder = value;

                OnChanged(oldValue, value, "ShowOnDungeonFinder");
            }
        }

        private bool _ShowOnDungeonFinder;
        public bool SafeZoneOnly
        {
            get { return _SafeZoneOnly; }
            set
            {
                if (_SafeZoneOnly == value) return;

                var oldValue = _SafeZoneOnly;
                _SafeZoneOnly = value;

                OnChanged(oldValue, value, "SafeZoneOnly");
            }
        }
        private bool _SafeZoneOnly;

        public bool AllowRejoin
        {
            get { return _AllowRejoin; }
            set
            {
                if (_AllowRejoin == value) return;

                var oldValue = _AllowRejoin;
                _AllowRejoin = value;

                OnChanged(oldValue, value, "AllowRejoin");
            }
        }
        private bool _AllowRejoin;

        public bool SavePlace
        {
            get { return _SavePlace; }
            set
            {
                if (_SavePlace == value) return;

                var oldValue = _SavePlace;
                _SavePlace = value;

                OnChanged(oldValue, value, "SavePlace");
            }
        }
        private bool _SavePlace;

        public byte MinPlayerLevel
        {
            get { return _MinPlayerLevel; }
            set
            {
                if (_MinPlayerLevel == value) return;

                var oldValue = _MinPlayerLevel;
                _MinPlayerLevel = value;

                OnChanged(oldValue, value, "MinimumLevel");
            }
        }
        private byte _MinPlayerLevel;

        public byte MaxPlayerLevel
        {
            get { return _MaxPlayerLevel; }
            set
            {
                if (_MaxPlayerLevel == value) return;

                var oldValue = _MaxPlayerLevel;
                _MaxPlayerLevel = value;

                OnChanged(oldValue, value, "MaximumLevel");
            }
        }
        private byte _MaxPlayerLevel;

        public byte MinPlayerCount
        {
            get { return _MinPlayerCount; }
            set
            {
                if (_MinPlayerCount == value) return;

                var oldValue = _MinPlayerCount;
                _MinPlayerCount = value;

                OnChanged(oldValue, value, "MinimumCount");
            }
        }
        private byte _MinPlayerCount;

        public byte MaxPlayerCount
        {
            get { return _MaxPlayerCount; }
            set
            {
                if (_MaxPlayerCount == value) return;

                var oldValue = _MaxPlayerCount;
                _MaxPlayerCount = value;

                OnChanged(oldValue, value, "MaximumCount");
            }
        }
        private byte _MaxPlayerCount;

        public ItemInfo RequiredItem
        {
            get { return _RequiredItem; }
            set
            {
                if (_RequiredItem == value) return;

                var oldValue = _RequiredItem;
                _RequiredItem = value;

                OnChanged(oldValue, value, "RequiredItem");
            }
        }
        private ItemInfo _RequiredItem;

        public bool RequiredItemSingleUse
        {
            get { return _RequiredItemSingleUse; }
            set
            {
                if (_RequiredItemSingleUse == value) return;

                var oldValue = _RequiredItemSingleUse;
                _RequiredItemSingleUse = value;

                OnChanged(oldValue, value, "RequiredItemSingleUse");
            }
        }

        private bool _RequiredItemSingleUse;

        public MapRegion ConnectRegion
        {
            get { return _ConnectRegion; }
            set
            {
                if (_ConnectRegion == value) return;

                var oldValue = _ConnectRegion;
                _ConnectRegion = value;

                OnChanged(oldValue, value, "ConnectRegion");
            }
        }
        private MapRegion _ConnectRegion;

        public MapRegion ReconnectRegion
        {
            get { return _ReconnectRegion; }
            set
            {
                if (_ReconnectRegion == value) return;

                var oldValue = _ReconnectRegion;
                _ReconnectRegion = value;

                OnChanged(oldValue, value, "ReconnectRegion");
            }
        }
        private MapRegion _ReconnectRegion;

        public int CooldownTimeInMinutes
        {
            get { return _CooldownTimeInMinutes; }
            set
            {
                if (_CooldownTimeInMinutes == value) return;

                var oldValue = _CooldownTimeInMinutes;
                _CooldownTimeInMinutes = value;

                OnChanged(oldValue, value, "CooldownTimeInMinutes");
            }
        }
        private int _CooldownTimeInMinutes;

        public int TimeLimitInMinutes
        {
            get { return _TimeLimitInMinutes; }
            set
            {
                if (_TimeLimitInMinutes == value) return;

                var oldValue = _TimeLimitInMinutes;
                _TimeLimitInMinutes = value;

                OnChanged(oldValue, value, "TimeLimitInMinutes");
            }
        }
        private int _TimeLimitInMinutes;

        [Association("Map", true)]
        public DBBindingList<InstanceMapInfo> Maps { get; set; }

        [Association("InstanceInfoStats", true)]
        public DBBindingList<InstanceInfoStat> BuffStats { get; set; }

        [JsonIgnore]
        [IgnoreProperty]
        public Dictionary<string, byte> UserRecord { get; set; }

        [JsonIgnore]
        [IgnoreProperty]
        public Dictionary<string, DateTime> UserCooldown { get; set; }

        [JsonIgnore]
        [IgnoreProperty]
        public Dictionary<string, DateTime> GuildCooldown { get; set; }

        public Stats Stats = new();

        protected internal override void OnLoaded()
        {
            base.OnLoaded();

            UserRecord = new Dictionary<string, byte>();
            UserCooldown = new Dictionary<string, DateTime>();
            GuildCooldown = new Dictionary<string, DateTime>();

            StatsChanged();
        }

        public void StatsChanged()
        {
            Stats.Clear();
            foreach (InstanceInfoStat stat in BuffStats)
                Stats[stat.Stat] += stat.Amount;
        }
    }

    public class InstanceMapInfo : DBObject
    {
        [IsIdentity]
        [Association("Map")]
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

        public int RespawnIndex
        {
            get { return _RespawnIndex; }
            set
            {
                if (_RespawnIndex == value) return;

                var oldValue = _RespawnIndex;
                _RespawnIndex = value;

                OnChanged(oldValue, value, "RespawnIndex");
            }
        }
        private int _RespawnIndex;
    }

    public sealed class InstanceInfoStat : DBObject
    {
        [IsIdentity]
        [Association("InstanceInfoStats")]
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
}