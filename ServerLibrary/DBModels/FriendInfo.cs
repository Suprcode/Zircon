using Library;
using MirDB;

namespace Server.DBModels
{
    [UserObject]
    public class FriendInfo : DBObject
    {
        [Association("Friends")]
        public CharacterInfo Character
        {
            get { return _Character; }
            set
            {
                if (_Character == value) return;

                var oldValue = _Character;
                _Character = value;

                OnChanged(oldValue, value, "Character");
            }
        }
        private CharacterInfo _Character;

        [Association("FriendedBy")]
        public CharacterInfo FriendedCharacter
        {
            get { return _FriendedCharacter; }
            set
            {
                if (_FriendedCharacter == value) return;

                var oldValue = _FriendedCharacter;
                _FriendedCharacter = value;

                OnChanged(oldValue, value, "FriendedCharacter");
            }
        }
        private CharacterInfo _FriendedCharacter;

        public string FriendName
        {
            get { return _FriendName; }
            set
            {
                if (_FriendName == value) return;

                var oldValue = _FriendName;
                _FriendName = value;

                OnChanged(oldValue, value, "FriendName");
            }
        }
        private string _FriendName;

        protected override void OnDeleted()
        {
            Character = null;
            FriendedCharacter = null;

            base.OnDeleted();
        }

        public ClientFriendInfo ToClientInfo()
        {
            return new ClientFriendInfo
            {
                Index = Index,
                Name = FriendName,
                State = FriendedCharacter.Player == null ? OnlineState.Offline : FriendedCharacter.OnlineState
            };
        }
    }
}
