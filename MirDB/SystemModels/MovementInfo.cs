using Library;
using MirDB;

namespace Server.DBModels
{
    public sealed class MovementInfo : DBObject
    {
        [Association("Movements")]
        public MapInfo SourceMap
        {
            get { return _SourceMap; }
            set
            {
                if (_SourceMap == value) return;

                var oldValue = _SourceMap;
                _SourceMap = value;

                OnChanged(oldValue, value, "SourceMap");
            }
        }
        private MapInfo _SourceMap;


        public int SourceX
        {
            get { return _SourceX; }
            set
            {
                if (_SourceX == value) return;

                var oldValue = _SourceX;
                _SourceX = value;

                OnChanged(oldValue, value, "SourceX");
            }
        }
        private int _SourceX;

        public int SourceY
        {
            get { return _SourceY; }
            set
            {
                if (_SourceY == value) return;

                var oldValue = _SourceY;
                _SourceY = value;

                OnChanged(oldValue, value, "SourceY");
            }
        }
        private int _SourceY;

        public MapInfo DestinationMap
        {
            get { return _DestinationMap; }
            set
            {
                if (_DestinationMap == value) return;

                var oldValue = _DestinationMap;
                _DestinationMap = value;

                OnChanged(oldValue, value, "DestinationMap");
            }
        }
        private MapInfo _DestinationMap;

        public int DestinationX
        {
            get { return _DestinationX; }
            set
            {
                if (_DestinationX == value) return;

                var oldValue = _DestinationX;
                _DestinationX = value;

                OnChanged(oldValue, value, "DestinationX");
            }
        }
        private int _DestinationX;

        public int DestinationY
        {
            get { return _DestinationY; }
            set
            {
                if (_DestinationY == value) return;

                var oldValue = _DestinationY;
                _DestinationY = value;

                OnChanged(oldValue, value, "DestinationY");
            }
        }
        private int _DestinationY;

        public MapIcon MapIcon
        {
            get { return _MapIcon; }
            set
            {
                if (_MapIcon == value) return;

                var oldValue = _MapIcon;
                _MapIcon = value;

                OnChanged(oldValue, value, "MapIcon");
            }
        }
        private MapIcon _MapIcon;
    }
}
