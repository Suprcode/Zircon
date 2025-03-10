using MirDB;

namespace Library.SystemModels
{
    public class BundleInfo : DBObject
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

        public BundleType Type
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
        private BundleType _Type;

        public bool AutoOpen
        {
            get { return _AutoOpen; }
            set
            {
                if (_AutoOpen == value) return;

                var oldValue = _AutoOpen;
                _AutoOpen = value;

                OnChanged(oldValue, value, "AutoOpen");
            }
        }
        private bool _AutoOpen;

        [Association("Contents", true)]
        public DBBindingList<BundleItemInfo> Contents { get; set; }
    }

    public class BundleItemInfo : DBObject
    {
        [Association("Contents")]
        public BundleInfo Bundle
        {
            get { return _Bundle; }
            set
            {
                if (_Bundle == value) return;

                var oldValue = _Bundle;
                _Bundle = value;

                OnChanged(oldValue, value, "Bundle");
            }
        }
        private BundleInfo _Bundle;

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
