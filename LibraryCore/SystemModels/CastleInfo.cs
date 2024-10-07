using MirDB;
using System;

namespace Library.SystemModels
{
    public sealed class CastleInfo : DBObject
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

        [Association("Castles", true)]
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

        public TimeSpan StartTime
        {
            get { return _StartTime; }
            set
            {
                if (_StartTime == value) return;

                var oldValue = _StartTime;
                _StartTime = value;

                OnChanged(oldValue, value, "StartTime");
            }
        }
        private TimeSpan _StartTime;

        public TimeSpan Duration
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
        private TimeSpan _Duration;

        public MapRegion CastleRegion
        {
            get { return _CastleRegion; }
            set
            {
                if (_CastleRegion == value) return;

                var oldValue = _CastleRegion;
                _CastleRegion = value;

                OnChanged(oldValue, value, "CastleRegion");
            }
        }
        private MapRegion _CastleRegion;

        public MapRegion AttackSpawnRegion
        {
            get { return _AttackSpawnRegion; }
            set
            {
                if (_AttackSpawnRegion == value) return;

                var oldValue = _AttackSpawnRegion;
                _AttackSpawnRegion = value;

                OnChanged(oldValue, value, "AttackSpawnRegion");
            }
        }
        private MapRegion _AttackSpawnRegion;

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
            get => _Monster;
            set
            {
                if (_Monster == value) return;

                var oldValue = _Monster;
                _Monster = value;

                OnChanged(oldValue, value, "Monster");
            }
        }
        private MonsterInfo _Monster;

        public decimal Discount
        {
            get { return _Discount; }
            set
            {
                if (_Discount == value) return;

                var oldValue = _Discount;
                _Discount = value;

                OnChanged(oldValue, value, "Discount");
            }
        }
        private decimal _Discount;

        [Association("Flags", true)]
        public DBBindingList<CastleFlagInfo> Flags { get; set; }

        [Association("Gates", true)]
        public DBBindingList<CastleGateInfo> Gates { get; set; }

        [Association("Guards", true)]
        public DBBindingList<CastleGuardInfo> Guards { get; set; }

        public DateTime WarDate;
    }
}



