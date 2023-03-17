using MirDB;
using System.Text.Json.Serialization;

namespace Library.SystemModels
{
    public sealed class GuardInfo : DBObject
    {
        [IsIdentity]
        [Association("Guards")]
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

        [IsIdentity]
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

        [IsIdentity]
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

        public MirDirection Direction
        {
            get { return _Direction; }
            set
            {
                if (_Direction == value) return;

                var oldValue = _Direction;
                _Direction = value;

                OnChanged(oldValue, value, "Direction");
            }
        }
        private MirDirection _Direction;
    }
}
