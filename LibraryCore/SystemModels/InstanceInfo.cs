using MirDB;
using System.Collections.Generic;

namespace Library.SystemModels
{
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

        public byte RecommendedPlayerCount
        {
            get { return _RecommendedPlayerCount; }
            set
            {
                if (_RecommendedPlayerCount == value) return;

                var oldValue = _RecommendedPlayerCount;
                _RecommendedPlayerCount = value;

                OnChanged(oldValue, value, "RecommendedPlayerCount");
            }
        }
        private byte _RecommendedPlayerCount;

        //TODO - Rewards
        //TODO - Entry point
        //TODO - Save Points on death??
        //TODO - Reconnect map??
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