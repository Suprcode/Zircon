using System.Collections.Generic;
using System.Drawing;
using MirDB;

namespace Library.SystemModels
{
    public sealed class SafeZoneInfo : DBObject
    {
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
        
        public MapRegion BindRegion
        {
            get { return _BindRegion; }
            set
            {
                if (_BindRegion == value) return;

                var oldValue = _BindRegion;
                _BindRegion = value;

                OnChanged(oldValue, value, "BindRegion");
            }
        }
        private MapRegion _BindRegion;
        
        public RequiredClass StartClass
        {
            get { return _StartClass; }
            set
            {
                if (_StartClass == value) return;

                var oldValue = _StartClass;
                _StartClass = value;

                OnChanged(oldValue, value, "StartClass");
            }
        }
        private RequiredClass _StartClass;

        public bool RedZone
        {
            get { return _RedZone; }
            set
            {
                if (_RedZone == value) return;

                var oldValue = _RedZone;
                _RedZone = value;

                OnChanged(oldValue, value, "RedZone");
            }
        }
        private bool _RedZone;

        public List<Point> ValidBindPoints = new List<Point>();
    }
}
