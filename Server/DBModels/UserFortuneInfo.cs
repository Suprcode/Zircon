using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;
using Library.SystemModels;
using MirDB;
using Server.Envir;

namespace Server.DBModels
{
    [UserObject]
    public class UserFortuneInfo : DBObject
    {
        [Association("Fortunes")]
        public AccountInfo Account
        {
            get { return _Account; }
            set
            {
                if (_Account == value) return;

                var oldValue = _Account;
                _Account = value;

                OnChanged(oldValue, value, "Account");
            }
        }
        private AccountInfo _Account;

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
        

        public long DropCount
        {
            get { return _DropCount; }
            set
            {
                if (_DropCount == value) return;

                var oldValue = _DropCount;
                _DropCount = value;

                OnChanged(oldValue, value, "DropCount");
            }
        }
        private long _DropCount;

        public decimal DropProgress
        {
            get { return _DropProgress; }
            set
            {
                if (_DropProgress == value) return;

                var oldValue = _DropProgress;
                _DropProgress = value;

                OnChanged(oldValue, value, "DropProgress");
            }
        }
        private decimal _DropProgress;
        
        public DateTime CheckTime
        {
            get { return _CheckTime; }
            set
            {
                if (_CheckTime == value) return;

                var oldValue = _CheckTime;
                _CheckTime = value;

                OnChanged(oldValue, value, "CheckTime");
            }
        }
        private DateTime _CheckTime;


        public ClientFortuneInfo ToClientInfo()
        {
            return new ClientFortuneInfo
            {
                ItemIndex = Item.Index,
                CheckTime = SEnvir.Now - CheckTime,
                DropCount = DropCount,
                Progress = DropProgress,
            };
        }
    }
}
