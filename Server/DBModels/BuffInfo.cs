using System;
using Library;
using MirDB;

namespace Server.DBModels
{
    [UserObject]
    public sealed class BuffInfo : DBObject
    {
        [Association("Buffs")]
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

        [Association("Buffs")]
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
        
        public BuffType Type
        {
            get { return _Type; }
            set
            {
                if (_Type == value) return;

                var oldValue = _Type;
                _Type = value;

                OnChanged(oldValue, value, "Type");
            }
        }
        private BuffType _Type;
        
        public Stats Stats
        {
            get { return _Stats; }
            set
            {
                if (_Stats == value) return;

                var oldValue = _Stats;
                _Stats = value;

                OnChanged(oldValue, value, "Stats");
            }
        }
        private Stats _Stats;

        public TimeSpan RemainingTime
        {
            get { return _RemainingTime; }
            set
            {
                if (_RemainingTime == value) return;

                var oldValue = _RemainingTime;
                _RemainingTime = value;

                OnChanged(oldValue, value, "RemainingTime");
            }
        }
        private TimeSpan _RemainingTime;

        public TimeSpan TickFrequency
        {
            get { return _TickFrequency; }
            set
            {
                if (_TickFrequency == value) return;

                var oldValue = _TickFrequency;
                _TickFrequency = value;

                OnChanged(oldValue, value, "TickFrequency");
            }
        }
        private TimeSpan _TickFrequency;

        public TimeSpan TickTime
        {
            get { return _TickTime; }
            set
            {
                if (_TickTime == value) return;

                var oldValue = _TickTime;
                _TickTime = value;

                OnChanged(oldValue, value, "TickTime");
            }
        }
        private TimeSpan _TickTime;

        public int ItemIndex
        {
            get { return _ItemIndex; }
            set
            {
                if (_ItemIndex == value) return;

                var oldValue = _ItemIndex;
                _ItemIndex = value;

                OnChanged(oldValue, value, "ItemIndex");
            }
        }
        private int _ItemIndex;
        

        public bool Visible
        {
            get { return _Visible; }
            set
            {
                if (_Visible == value) return;

                var oldValue = _Visible;
                _Visible = value;

                OnChanged(oldValue, value, "Visible");
            }
        }
        private bool _Visible;

        public bool Pause
        {
            get { return _Pause; }
            set
            {
                if (_Pause == value) return;

                var oldValue = _Pause;
                _Pause = value;

                OnChanged(oldValue, value, "Pause");
            }
        }
        private bool _Pause;

        protected override void OnDeleted()
        {
            Account = null;
            Character = null;

            base.OnDeleted();
        }


        protected override void OnChanged(object oldValue, object newValue, string propertyName)
        {
            base.OnChanged(oldValue, newValue, propertyName);

            switch (propertyName)
            {
                case "Character":
                    if (newValue == null) break;

                    Account = null;
                    break;
                case "Account":
                    if (newValue == null) break;

                    Character = null;
                    break;
            }
        }


        public ClientBuffInfo ToClientInfo()
        {
            return new ClientBuffInfo
            {
                Index = Index,
                RemainingTime = RemainingTime,
                TickFrequency = TickFrequency,
                Type = Type,
                Pause = Pause,
                Stats = Stats,
                ItemIndex = ItemIndex,
            };
        }
    }
}
