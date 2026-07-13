using MirDB;

namespace Server.DBModels
{
    [UserObject]
    public sealed class UserItemSocket : DBObject
    {
        [Association("Sockets")]
        public UserItem Item
        {
            get => _Item;
            set
            {
                if (_Item == value) return;

                UserItem oldValue = _Item;
                _Item = value;

                OnChanged(oldValue, value, "Item");
            }
        }
        private UserItem _Item;

        [Association("SocketGem")]
        public UserItem Gem
        {
            get => _Gem;
            set
            {
                if (_Gem == value) return;

                UserItem oldValue = _Gem;
                _Gem = value;

                OnChanged(oldValue, value, "Gem");
            }
        }
        private UserItem _Gem;

        public int Slot
        {
            get => _Slot;
            set
            {
                if (_Slot == value) return;

                int oldValue = _Slot;
                _Slot = value;

                OnChanged(oldValue, value, "Slot");
            }
        }
        private int _Slot;

        protected override void OnDeleted()
        {
            UserItem gem = Gem;

            Item = null;
            Gem = null;

            if (gem != null)
                gem.Delete();

            base.OnDeleted();
        }
    }
}
