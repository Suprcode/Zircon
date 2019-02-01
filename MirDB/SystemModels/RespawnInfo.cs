using MirDB;

namespace Server.DBModels
{
    public sealed class RespawnInfo : DBObject
    {
        [Association("Respawns")]
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


        [Association("Respawns")]
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


        public int X
        {
            get { return _X; }
            set
            {
                if (_X == value) return;

                var oldValue = _X;
                _X = value;

                OnChanged(oldValue, value, "X");
            }
        }
        private int _X;

        public int Y
        {
            get { return _Y; }
            set
            {
                if (_Y == value) return;

                var oldValue = _Y;
                _Y = value;

                OnChanged(oldValue, value, "Y");
            }
        }
        private int _Y;

        public int Delay
        {
            get { return _Delay; }
            set
            {
                if (_Delay == value) return;

                var oldValue = _Delay;
                _Delay = value;

                OnChanged(oldValue, value, "Delay");
            }
        }
        private int _Delay;

        public int Spread
        {
            get { return _Spread; }
            set
            {
                if (_Spread == value) return;

                var oldValue = _Spread;
                _Spread = value;

                OnChanged(oldValue, value, "Spread");
            }
        }
        private int _Spread;

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
    }
}
