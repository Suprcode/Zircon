using MirDB;

namespace Library.SystemModels
{
    public sealed class CurrencyInfo : DBObject
    {
        [IsIdentity]
        public string Name
        {
            get { return _Name; }
            set
            {
                if (_Name == value) return;

                var oldValue = _Name;
                _Name = value;

                OnChanged(oldValue, value, "Name");
            }
        }
        private string _Name;

        public string Abbreviation
        {
            get { return _Abbreviation; }
            set
            {
                if (_Abbreviation == value) return;

                var oldValue = _Abbreviation;
                _Abbreviation = value;

                OnChanged(oldValue, value, "Abbreviation");
            }
        }
        private string _Abbreviation;

        public CurrencyType Type
        {
            get { return _Type; }
            set
            {
                if (_Type == value) return;

                var oldValue = _Type;
                _Type = value;

                OnChanged(oldValue, value, "Type");
            }
        }
        private CurrencyType _Type;

        public ItemInfo DropItem
        {
            get { return _DropItem; }
            set
            {
                if (_DropItem == value) return;

                var oldValue = _DropItem;
                _DropItem = value;

                OnChanged(oldValue, value, "DropItem");
            }
        }
        private ItemInfo _DropItem;

        public decimal ExchangeRate
        {
            get { return _ExchangeRate; }
            set
            {
                if (_ExchangeRate == value) return;

                var oldValue = _ExchangeRate;
                _ExchangeRate = value;

                OnChanged(oldValue, value, "ExchangeRate");
            }
        }
        private decimal _ExchangeRate;

        [Association("Images", true)]
        public DBBindingList<CurrencyInfoImage> Images { get; set; }

        protected internal override void OnCreated()
        {
            base.OnCreated();

            ExchangeRate = 1M;
        }
    }

    public sealed class CurrencyInfoImage : DBObject
    {
        [Association("Images")]
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

        public int Image
        {
            get { return _Image; }
            set
            {
                if (_Image == value) return;

                var oldValue = _Image;
                _Image = value;

                OnChanged(oldValue, value, "Image");
            }
        }
        private int _Image;

        public long Amount
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
        private long _Amount;
    }
}
