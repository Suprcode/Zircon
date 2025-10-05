using MirDB;

namespace Library.SystemModels
{
    public class LootBoxInfo : DBObject
    {
        [IsIdentity]
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

        [Association("Contents", true)]
        public DBBindingList<LootBoxItemInfo> Contents { get; set; }

        protected internal override void OnCreated()
        {
            base.OnCreated();
        }

        protected internal override void OnLoaded()
        {
            base.OnLoaded();
        }

        // Warning: do not make any larger as the size works off 16bit functionality
        public const int SlotSize = 15;
    }

    public class LootBoxItemInfo : DBObject
    {
        [Association("Contents")]
        public LootBoxInfo LootBox
        {
            get { return _LootBox; }
            set
            {
                if (_LootBox == value) return;

                var oldValue = _LootBox;
                _LootBox = value;

                OnChanged(oldValue, value, "LootBox");
            }
        }
        private LootBoxInfo _LootBox;

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
        private int _Amount = 1;
    }
}
