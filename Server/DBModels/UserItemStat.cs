using Library;
using MirDB;

namespace Server.DBModels
{
    [UserObject]
    public sealed class UserItemStat : DBObject
    {
        [Association("AddedStats")]
        public UserItem Item
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
        private UserItem _Item;

        public Stat Stat
        {
            get { return _Stat; }
            set
            {
                if (_Stat == value) return;

                var oldValue = _Stat;
                _Stat = value;

                OnChanged(oldValue, value, "Stat");
            }
        }
        private Stat _Stat;

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

        public StatSource StatSource
        {
            get { return _StatSource; }
            set
            {
                if (_StatSource == value) return;

                var oldValue = _StatSource;
                _StatSource = value;

                OnChanged(oldValue, value, "StatSource");
            }
        }
        private StatSource _StatSource;

        protected override void OnDeleted()
        {
            Item = null;

            base.OnDeleted();
        }

    }
}
