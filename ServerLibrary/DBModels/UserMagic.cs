using System;
using Library;
using Library.SystemModels;
using MirDB;
using Server.Envir;

namespace Server.DBModels
{
    [UserObject]
    public sealed class UserMagic : DBObject
    {
        public MagicInfo Info
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
        private MagicInfo _Info;

        [Association("Magics")]
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

        public SpellKey Set1Key
        {
            get { return _Set1Key; }
            set
            {
                if (_Set1Key == value) return;

                var oldValue = _Set1Key;
                _Set1Key = value;

                OnChanged(oldValue, value, "Set1Key");
            }
        }
        private SpellKey _Set1Key;

        public SpellKey Set2Key
        {
            get { return _Set2Key; }
            set
            {
                if (_Set2Key == value) return;

                var oldValue = _Set2Key;
                _Set2Key = value;

                OnChanged(oldValue, value, "Set2Key");
            }
        }
        private SpellKey _Set2Key;

        public SpellKey Set3Key
        {
            get { return _Set3Key; }
            set
            {
                if (_Set3Key == value) return;

                var oldValue = _Set3Key;
                _Set3Key = value;

                OnChanged(oldValue, value, "Set3Key");
            }
        }
        private SpellKey _Set3Key;

        public SpellKey Set4Key
        {
            get { return _Set4Key; }
            set
            {
                if (_Set4Key == value) return;

                var oldValue = _Set4Key;
                _Set4Key = value;

                OnChanged(oldValue, value, "Set4Key");
            }
        }
        private SpellKey _Set4Key;

        public int Level
        {
            get { return _Level; }
            set
            {
                if (_Level == value) return;

                var oldValue = _Level;
                _Level = value;

                OnChanged(oldValue, value, "Level");
            }
        }
        private int _Level;

        public long Experience
        {
            get { return _Experience; }
            set
            {
                if (_Experience == value) return;

                var oldValue = _Experience;
                _Experience = value;

                OnChanged(oldValue, value, "Experience");
            }
        }
        private long _Experience;

        public bool ItemRequired
        {
            get { return _ItemRequired; }
            set
            {
                if (_ItemRequired == value)
                {
                    return;
                }
                var oldValue = _ItemRequired;
                _ItemRequired = value;
                OnChanged(oldValue, value, "ItemRequired");
            }
        }
        private bool _ItemRequired;

        [Association("DisciplineMagics")]
        public UserDiscipline Discipline
        {
            get { return _Discipline; }
            set
            {
                if (_Discipline == value) return;

                var oldValue = _Discipline;
                _Discipline = value;

                OnChanged(oldValue, value, "Discipline");
            }
        }
        private UserDiscipline _Discipline;

        public DateTime Cooldown;
        
        [IgnoreProperty]
        public int Cost => Info.BaseCost + Level * Info.LevelCost / 3;

        protected override void OnDeleted()
        {
            Info = null;
            Character = null;
            Discipline = null;

            base.OnDeleted();
        }

        public int GetPower()
        {
            int min = Info.MinBasePower + Level * Info.MinLevelPower / 3;
            int max = Info.MaxBasePower + Level * Info.MaxLevelPower / 3;

            if (min < 0) min = 0;
            if (min >= max) return min;

            return SEnvir.Random.Next(min, max + 1);
        }

        public ClientUserMagic ToClientInfo()
        {
            return new ClientUserMagic
            {
                Index = Index,
                InfoIndex = Info.Index,
              
                Set1Key = Set1Key,
                Set2Key = Set2Key,
                Set3Key = Set3Key,
                Set4Key = Set4Key,

                Level = Level,
                Experience = Experience,
                ItemRequired = ItemRequired,

                Cooldown = Cooldown > SEnvir.Now ? Cooldown - SEnvir.Now : TimeSpan.Zero,
            };
        }
    }
}
