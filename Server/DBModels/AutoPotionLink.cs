using Library;
using MirDB;

namespace Server.DBModels
{
    [UserObject]
    public class AutoPotionLink : DBObject
    {
        [Association("AutoPotionLinks")]
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

        public int Health
        {
            get { return _Health; }
            set
            {
                if (_Health == value) return;

                var oldValue = _Health;
                _Health = value;

                OnChanged(oldValue, value, "Health");
            }
        }
        private int _Health;

        public int Mana
        {
            get { return _Mana; }
            set
            {
                if (_Mana == value) return;

                var oldValue = _Mana;
                _Mana = value;

                OnChanged(oldValue, value, "Mana");
            }
        }
        private int _Mana;

        public bool Enabled
        {
            get { return _Enabled; }
            set
            {
                if (_Enabled == value) return;

                var oldValue = _Enabled;
                _Enabled = value;

                OnChanged(oldValue, value, "Enabled");
            }
        }
        private bool _Enabled;


        protected override void OnDeleted()
        {
            Character = null;

            base.OnDeleted();
        }


        public ClientAutoPotionLink ToClientInfo()
        {
            return new ClientAutoPotionLink
            {
                Slot = Slot,
                LinkInfoIndex = LinkInfoIndex,
                Health = Health,
                Mana = Mana,
                Enabled = Enabled,
            };
        }
    }
}
