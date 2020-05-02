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
    public sealed class UserCompanionUnlock : DBObject
    {
        [Association("CompanionUnlocks")]
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

        public CompanionInfo CompanionInfo
        {
            get { return _CompanionInfo; }
            set
            {
                if (_CompanionInfo == value) return;

                var oldValue = _CompanionInfo;
                _CompanionInfo = value;

                OnChanged(oldValue, value, "CompanionInfo");
            }
        }
        private CompanionInfo _CompanionInfo;

        protected override void OnDeleted()
        {
            Account = null;
            CompanionInfo = null;

            base.OnDeleted();
        }

    }
}
