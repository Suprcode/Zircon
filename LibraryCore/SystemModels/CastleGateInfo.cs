using Library.SystemModels;
using MirDB;

namespace Library.SystemModels
{
    public sealed class CastleGateInfo : DBObject
    {
        [IsIdentity]
        [Association("Gates")]
        public CastleInfo Castle
        {
            get { return _Castle; }
            set
            {
                if (_Castle == value) return;

                var oldValue = _Castle;
                _Castle = value;

                OnChanged(oldValue, value, "Castle");
            }
        }
        private CastleInfo _Castle;

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

        public int RepairCost
        {
            get { return _RepairCost; }
            set
            {
                if (_RepairCost == value) return;

                var oldValue = _RepairCost;
                _RepairCost = value;

                OnChanged(oldValue, value, "RepairCost");
            }
        }
        private int _RepairCost;
    }
}
