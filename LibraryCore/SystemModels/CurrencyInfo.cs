using System;
using Library;
using MirDB;

namespace Library.SystemModels
{
    public sealed class CurrencyInfo : DBObject
    {
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

        protected internal override void OnCreated()
        {
            base.OnCreated();

            ExchangeRate = 1M;
        }
    }
}
