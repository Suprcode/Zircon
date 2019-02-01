using Library;
using MirDB;

namespace Server.DBModels
{
    [UserObject]
    public sealed class CharacterBeltLink : DBObject
    {
        [Association("BeltLinks")]
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
        
        public int Slot
        {
            get { return _Slot; }
            set
            {
                if (_Slot == value) return;

                var oldValue = _Slot;
                _Slot = value;

                OnChanged(oldValue, value, "Slot");
            }
        }
        private int _Slot;

        public int LinkInfoIndex
        {
            get { return _LinkInfoIndex; }
            set
            {
                if (_LinkInfoIndex == value) return;

                var oldValue = _LinkInfoIndex;
                _LinkInfoIndex = value;

                OnChanged(oldValue, value, "LinkInfoIndex");
            }
        }
        private int _LinkInfoIndex;
        

        public int LinkItemIndex
        {
            get { return _LinkItemIndex; }
            set
            {
                if (_LinkItemIndex == value) return;

                var oldValue = _LinkItemIndex;
                _LinkItemIndex = value;

                OnChanged(oldValue, value, "LinkItemIndex");
            }
        }
        private int _LinkItemIndex;

        protected override void OnDeleted()
        {
            Character = null;

            base.OnDeleted();
        }


        public ClientBeltLink ToClientInfo()
        {
            return new ClientBeltLink
            {
                Slot = Slot,
                LinkInfoIndex = LinkInfoIndex,
                LinkItemIndex = LinkItemIndex
            };
        }
    }
}
