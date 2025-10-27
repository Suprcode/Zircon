using MirDB;

namespace Library.SystemModels
{
    public class CompanionInfo : DBObject
    {
        [IsIdentity]
        public MonsterInfo MonsterInfo
        {
            get { return _MonsterInfo; }
            set
            {
                if (_MonsterInfo == value) return;

                var oldValue = _MonsterInfo;
                _MonsterInfo = value;

                OnChanged(oldValue, value, "MonsterInfo");
            }
        }
        private MonsterInfo _MonsterInfo;

        public string Description
        {
            get { return _Description; }
            set
            {
                if (_Description == value) return;

                var oldValue = _Description;
                _Description = value;

                OnChanged(oldValue, value, "Description");
            }
        }
        private string _Description;

        public int Price
        {
            get { return _Price; }
            set
            {
                if (_Price == value) return;

                var oldValue = _Price;
                _Price = value;

                OnChanged(oldValue, value, "Price");
            }
        }
        private int _Price;

        public CurrencyInfo Currency
        {
            get { return _Currency; }
            set
            {
                if (_Currency == value) return;

                var oldValue = _Currency;
                _Currency = value;

                OnChanged(oldValue, value, "Currency");
            }
        }
        private CurrencyInfo _Currency;

        public bool Available
        {
            get { return _Available; }
            set
            {
                if (_Available == value) return;

                var oldValue = _Available;
                _Available = value;

                OnChanged(oldValue, value, "Available");
            }
        }
        private bool _Available;

        public ItemInfo UnlockItem
        {
            get { return _UnlockItem; }
            set
            {
                if (_UnlockItem == value) return;

                var oldValue = _UnlockItem;
                _UnlockItem = value;

                OnChanged(oldValue, value, "UnlockItem");
            }
        }
        private ItemInfo _UnlockItem;

        [Association("CompanionSpeeches", true)]
        public DBBindingList<CompanionSpeech> CompanionSpeeches { get; set; }
    }
}
