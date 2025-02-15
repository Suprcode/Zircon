using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MirDB;

namespace Library.SystemModels
{
    public sealed class MineInfo : DBObject
    {
        [IsIdentity]
        [Association("Mining")]
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

        public int Quantity
        {
            get { return _Quantity; }
            set
            {
                if (_Quantity == value) return;

                var oldValue = _Quantity;
                _Quantity = value;

                OnChanged(oldValue, value, "Quantity");
            }
        }
        private int _Quantity = -1;

        public int RestockTimeInMinutes
        {
            get { return _RestockTimeInMinutes; }
            set
            {
                if (_RestockTimeInMinutes == value) return;

                var oldValue = _RestockTimeInMinutes;
                _RestockTimeInMinutes = value;

                OnChanged(oldValue, value, "RestockTimeInMinutes");
            }
        }
        private int _RestockTimeInMinutes = -1;

        public int RemainingQuantity;
        public DateTime NextRestock = DateTime.MaxValue;

        protected internal override void OnLoaded()
        {
            base.OnLoaded();

            RemainingQuantity = Quantity;
        }
    }
}
