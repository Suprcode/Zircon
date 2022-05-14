using MirDB;
using System;
using System.Collections.Generic;

namespace Library.SystemModels
{
    //TODO - 
    //Add instance dialog button to rejoin instance after death (same button, join an existing instance??)
    //Add conquest on instances
    //Add teleporting(player, npc) on instances(figure how to teleport off instances but also through the same instances)

    public sealed class InstanceInfo : DBObject
    {
        [Association("Map", true)]
        public DBBindingList<InstanceMapInfo> Maps { get; set; }

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

        [IgnoreProperty]
        public Dictionary<string, byte> UserRecord { get; set; }

        [IgnoreProperty]
        public Dictionary<string, DateTime> UserCooldown { get; set; }

        [IgnoreProperty]
        public Dictionary<string, DateTime> GuildCooldown { get; set; }

        protected internal override void OnLoaded()
        {
            base.OnLoaded();

            UserRecord = new Dictionary<string, byte>();
            UserCooldown = new Dictionary<string, DateTime>();
            GuildCooldown = new Dictionary<string, DateTime>();
        }
    }

    public class InstanceMapInfo : DBObject
    {
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
    }
}