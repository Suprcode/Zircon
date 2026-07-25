using Library.SystemModels;
using MirDB;

namespace Server.DBModels
{
    [UserObject]
    public sealed class GameStoreFavourite : DBObject
    {
        [Association("StoreFavourites")]
        public AccountInfo Account
        {
            get => _Account;
            set
            {
                if (_Account == value) return;
                AccountInfo oldValue = _Account;
                _Account = value;
                OnChanged(oldValue, value, "Account");
            }
        }
        private AccountInfo _Account;

        public StoreInfo StoreInfo
        {
            get => _StoreInfo;
            set
            {
                if (_StoreInfo == value) return;
                StoreInfo oldValue = _StoreInfo;
                _StoreInfo = value;
                OnChanged(oldValue, value, "StoreInfo");
            }
        }
        private StoreInfo _StoreInfo;

        protected override void OnDeleted()
        {
            Account = null;
            StoreInfo = null;
            base.OnDeleted();
        }
    }
}
