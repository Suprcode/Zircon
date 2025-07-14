using MirDB;
using System;

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

        public int SlotSize
        {
            get { return _SlotSize; }
            set
            {
                if (_SlotSize == value) return;

                var oldValue = _SlotSize;
                _SlotSize = value;

                OnChanged(oldValue, value, "SlotSize");
            }
        }
        private int _SlotSize;

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

        public bool LootBox
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
        private bool _LootBox;

        [Association("Contents", true)]
        public DBBindingList<BundleItemInfo> Contents { get; set; }

        protected internal override void OnCreated()
        {
            base.OnCreated();

            SlotSize = 16;
        }

        protected internal override void OnLoaded()
        {
            base.OnLoaded();

            SlotSize = Math.Clamp(SlotSize, 1, 16);
        }
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
