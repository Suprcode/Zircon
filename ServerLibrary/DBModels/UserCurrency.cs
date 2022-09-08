using Library;
using Library.SystemModels;
using MirDB;

namespace Server.DBModels
{
    [UserObject]
    public sealed class UserCurrency : DBObject
    {
        public CurrencyInfo Info
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
        private CurrencyInfo _Info;

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

        [Association("Currencies")]
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
        protected override void OnDeleted()
        {
            Info = null;

            Account = null;

            base.OnDeleted();
        }

        public ClientUserCurrency ToClientInfo(bool hideAmount = false)
        {
            return new ClientUserCurrency
            {
                CurrencyIndex = Info.Index,
                Amount = hideAmount ? 0 : Amount,
            };
        }
    }
}
