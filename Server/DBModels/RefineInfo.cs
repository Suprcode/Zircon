using System;
using Library;
using MirDB;
using Server.Envir;

namespace Server.DBModels
{
    [UserObject]
    public class RefineInfo : DBObject
    {
        [Association("Refines")]
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

        [Association("Refine")]
        public UserItem Weapon
        {
            get { return _Weapon; }
            set
            {
                if (_Weapon == value) return;

                var oldValue = _Weapon;
                _Weapon = value;

                OnChanged(oldValue, value, "Weapon");
            }
        }
        private UserItem _Weapon;

        public RefineQuality Quality
        {
            get { return _Quality; }
            set
            {
                if (_Quality == value) return;

                var oldValue = _Quality;
                _Quality = value;

                OnChanged(oldValue, value, "Quality");
            }
        }
        private RefineQuality _Quality;

        public RefineType Type
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
        private RefineType _Type;

        public DateTime RetrieveTime
        {
            get { return _RetrieveTime; }
            set
            {
                if (_RetrieveTime == value) return;

                var oldValue = _RetrieveTime;
                _RetrieveTime = value;

                OnChanged(oldValue, value, "RetrieveTime");
            }
        }
        private DateTime _RetrieveTime;

        public int Chance
        {
            get { return _Chance; }
            set
            {
                if (_Chance == value) return;

                var oldValue = _Chance;
                _Chance = value;

                OnChanged(oldValue, value, "Chance");
            }
        }
        private int _Chance;

        public int MaxChance
        {
            get { return _MaxChance; }
            set
            {
                if (_MaxChance == value) return;

                var oldValue = _MaxChance;
                _MaxChance = value;

                OnChanged(oldValue, value, "MaxChance");
            }
        }
        private int _MaxChance;


        protected override void OnDeleted()
        {
            Character = null;
            Weapon = null;

            base.OnDeleted();
        }


        public ClientRefineInfo ToClientInfo()
        {
            return new ClientRefineInfo
            {
                Index = Index,
                Type = Type,
                Quality = Quality,
                Weapon = Weapon.ToClientInfo(),
                Chance = Chance,
                MaxChance = MaxChance,
                ReadyDuration = RetrieveTime > SEnvir.Now ? RetrieveTime - SEnvir.Now : TimeSpan.Zero,
            };

        }
    }
}
