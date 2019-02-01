using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MirDB;

namespace Library.SystemModels
{
    public sealed class StoreInfo : DBObject
    {
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

        public int HuntGoldPrice
        {
            get => _HuntGoldPrice;
            set
            {
                if (_HuntGoldPrice == value) return;

                int oldValue = _HuntGoldPrice;
                _HuntGoldPrice = value;

                OnChanged(oldValue, value, "HuntGoldPrice");
            }
        }
        private int _HuntGoldPrice;
        
        public string Filter
        {
            get { return _Filter; }
            set
            {
                if (_Filter == value) return;

                var oldValue = _Filter;
                _Filter = value;

                OnChanged(oldValue, value, "Filter");
            }
        }
        private string _Filter;

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

        public int Duration
        {
            get { return _Duration; }
            set
            {
                if (_Duration == value) return;

                var oldValue = _Duration;
                _Duration = value;

                OnChanged(oldValue, value, "Duration");
            }
        }
        private int _Duration;
    }

}
