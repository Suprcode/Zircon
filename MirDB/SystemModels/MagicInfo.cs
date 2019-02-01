using Library;
using MirDB;

namespace Server.DBModels
{
    public sealed class MagicInfo : DBObject
    {
        public string Name
        {
            get { return _Name; }
            set
            {
                if (_Name == value) return;

                var oldValue = _Name;
                _Name = value;

                OnChanged(oldValue, value, "Name");
            }
        }
        private string _Name;

        public MagicType Magic
        {
            get { return _Magic; }
            set
            {
                if (_Magic == value) return;

                var oldValue = _Magic;
                _Magic = value;

                OnChanged(oldValue, value, "Magic");
            }
        }
        private MagicType _Magic;

        public MirClass Class
        {
            get { return _Class; }
            set
            {
                if (_Class == value) return;

                var oldValue = _Class;
                _Class = value;

                OnChanged(oldValue, value, "Class");
            }
        }
        private MirClass _Class;

        public MagicSchool School
        {
            get { return _School; }
            set
            {
                if (_School == value) return;

                var oldValue = _School;
                _School = value;

                OnChanged(oldValue, value, "School");
            }
        }
        private MagicSchool _School;

        public int Icon
        {
            get { return _Icon; }
            set
            {
                if (_Icon == value) return;

                var oldValue = _Icon;
                _Icon = value;

                OnChanged(oldValue, value, "Icon");
            }
        }
        private int _Icon;

        public int RequiredLevel
        {
            get { return _RequiredLevel; }
            set
            {
                if (_RequiredLevel == value) return;

                var oldValue = _RequiredLevel;
                _RequiredLevel = value;

                OnChanged(oldValue, value, "RequiredLevel");
            }
        }
        private int _RequiredLevel;

        public int MinBasePower
        {
            get { return _MinBasePower; }
            set
            {
                if (_MinBasePower == value) return;

                var oldValue = _MinBasePower;
                _MinBasePower = value;

                OnChanged(oldValue, value, "MinBasePower");
            }
        }
        private int _MinBasePower;

        public int MaxBasePower
        {
            get { return _MaxBasePower; }
            set
            {
                if (_MaxBasePower == value) return;

                var oldValue = _MaxBasePower;
                _MaxBasePower = value;

                OnChanged(oldValue, value, "MaxBasePower");
            }
        }
        private int _MaxBasePower;

        public int MinLevelPower
        {
            get { return _MinLevelPower; }
            set
            {
                if (_MinLevelPower == value) return;

                var oldValue = _MinLevelPower;
                _MinLevelPower = value;

                OnChanged(oldValue, value, "MinLevelPower");
            }
        }
        private int _MinLevelPower;

        public int MaxLevelPower
        {
            get { return _MaxLevelPower; }
            set
            {
                if (_MaxLevelPower == value) return;

                var oldValue = _MaxLevelPower;
                _MaxLevelPower = value;

                OnChanged(oldValue, value, "MaxLevelPower");
            }
        }
        private int _MaxLevelPower;

        public int BaseCost
        {
            get { return _BaseCost; }
            set
            {
                if (_BaseCost == value) return;

                var oldValue = _BaseCost;
                _BaseCost = value;

                OnChanged(oldValue, value, "BaseCost");
            }
        }
        private int _BaseCost;

        public int LevelCost
        {
            get { return _LevelCost; }
            set
            {
                if (_LevelCost == value) return;

                var oldValue = _LevelCost;
                _LevelCost = value;

                OnChanged(oldValue, value, "LevelCost");
            }
        }
        private int _LevelCost;

        public int BaseExperience
        {
            get { return _BaseExperience; }
            set
            {
                if (_BaseExperience == value) return;

                var oldValue = _BaseExperience;
                _BaseExperience = value;

                OnChanged(oldValue, value, "BaseExperience");
            }
        }
        private int _BaseExperience;

        public int ExperienceRate
        {
            get { return _ExperienceRate; }
            set
            {
                if (_ExperienceRate == value) return;

                var oldValue = _ExperienceRate;
                _ExperienceRate = value;

                OnChanged(oldValue, value, "ExperienceRate");
            }
        }
        private int _ExperienceRate;

        public int MaxLevel
        {
            get { return _MaxLevel; }
            set
            {
                if (_MaxLevel == value) return;

                var oldValue = _MaxLevel;
                _MaxLevel = value;

                OnChanged(oldValue, value, "MaxLevel");
            }
        }
        private int _MaxLevel;

        public int Delay
        {
            get { return _Delay; }
            set
            {
                if (_Delay == value) return;

                var oldValue = _Delay;
                _Delay = value;

                OnChanged(oldValue, value, "Delay");
            }
        }
        private int _Delay;



        protected internal override void OnCreated()
        {
            base.OnCreated();

            ExperienceRate = 5;
        }

        public ClientMagicInfo ToClientInfo()
        {
            ClientMagicInfo info = new ClientMagicInfo
            {
                Index = Index,
                Name = Name,

                Magic = Magic,
                Class = Class,
                School = School,
                Icon = Icon,
                RequiredLevel = RequiredLevel,

                MinBasePower = MinBasePower,
                MaxBasePower = MaxBasePower,
                MinLevelPower = MinLevelPower,
                MaxLevelPower = MaxLevelPower,
                BaseCost = BaseCost,
                LevelCost = LevelCost,
                BaseExperience = BaseExperience,
                ExperienceRate = ExperienceRate,
                Delay = Delay,
            };

            return info;
        }
    }
}
