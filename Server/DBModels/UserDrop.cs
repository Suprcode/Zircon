using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.SystemModels;
using MirDB;

namespace Server.DBModels
{
    [UserObject]
    public sealed class UserDrop : DBObject
    {
        [Association("UserDrops")]
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

        public decimal Progress
        {
            get { return _Progress; }
            set
            {
                if (_Progress == value) return;

                var oldValue = _Progress;
                _Progress = value;

                OnChanged(oldValue, value, "Progress");
            }
        }
        private decimal _Progress;

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


        protected override void OnDeleted()
        {
            Account = null;
            Item = null;

            base.OnDeleted();
        }

    }
}
