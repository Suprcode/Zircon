using Library;
using MirDB;

namespace Server.DBModels
{
    [UserObject]
    public sealed class BlockInfo : DBObject
    {
        [Association("BlockingList")]
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

        [Association("BlockedByList")]
        public AccountInfo BlockedAccount
        {
            get { return _BlockedAccount; }
            set
            {
                if (_BlockedAccount == value) return;

                var oldValue = _BlockedAccount;
                _BlockedAccount = value;

                OnChanged(oldValue, value, "BlockedAccount");
            }
        }
        private AccountInfo _BlockedAccount;

        public string BlockedName
        {
            get { return _BlockedName; }
            set
            {
                if (_BlockedName == value) return;

                var oldValue = _BlockedName;
                _BlockedName = value;

                OnChanged(oldValue, value, "BlockedName");
            }
        }
        private string _BlockedName;

        protected override void OnDeleted()
        {
            Account = null;
            BlockedAccount = null;

            base.OnDeleted();
        }

        public ClientBlockInfo ToClientInfo()
        {
            return new ClientBlockInfo
            {
                Index = Index,
                Name = BlockedName
            };
        }
    }
}
