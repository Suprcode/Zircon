using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MirDB;

namespace Server.DBModels
{
    [UserObject]
    public sealed class AuctionHistoryInfo : DBObject
    {
        public int Info
        {
            get { return _Info; }
            set
            {
                if (_Info == value) return;

                var oldValue = _Info;
                _Info = value;

                OnChanged(oldValue, value, "Info");
            }
        }
        private int _Info;
        
        public long SaleCount
        {
            get { return _SaleCount; }
            set
            {
                if (_SaleCount == value) return;

                var oldValue = _SaleCount;
                _SaleCount = value;

                OnChanged(oldValue, value, "SaleCount");
            }
        }
        private long _SaleCount;

        public int LastPrice
        {
            get { return _LastPrice; }
            set
            {
                if (_LastPrice == value) return;

                var oldValue = _LastPrice;
                _LastPrice = value;

                OnChanged(oldValue, value, "LastPrice");
            }
        }
        private int _LastPrice;
        
        public int[] Average
        {
            get { return _Average; }
            set
            {
                if (_Average == value) return;

                var oldValue = _Average;
                _Average = value;

                OnChanged(oldValue, value, "Average");
            }
        }
        private int[] _Average;

        public int PartIndex
        {
            get { return _PartIndex; }
            set
            {
                if (_PartIndex == value) return;

                var oldValue = _PartIndex;
                _PartIndex = value;

                OnChanged(oldValue, value, "PartIndex");
            }
        }
        private int _PartIndex;
        

        protected override void OnCreated()
        {
            base.OnCreated();

            Average = new int[20];
        }
    }
}
