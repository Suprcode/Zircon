using MirDB;
using System.Text.Json.Serialization;

namespace Library.SystemModels
{
    public sealed class DropInfo : DBObject
    {
        [IsIdentity]
        [Association("Drops")]
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
        [Association("Drops")]
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

        public bool PartOnly
        {
            get { return _PartOnly; }
            set
            {
                if (_PartOnly == value) return;

                var oldValue = _PartOnly;
                _PartOnly = value;

                OnChanged(oldValue, value, "PartOnly");
            }
        }
        private bool _PartOnly;

        public bool EasterEvent
        {
            get { return _EasterEvent; }
            set
            {
                if (_EasterEvent == value) return;

                var oldValue = _EasterEvent;
                _EasterEvent = value;

                OnChanged(oldValue, value, "EasterEvent");
            }
        }
        private bool _EasterEvent;

        protected internal override void OnCreated()
        {
            base.OnCreated();

            Amount = 1;
        }
    }
}
