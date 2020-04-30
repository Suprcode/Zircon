using MirDB;

namespace Library.SystemModels
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
        

        public int NeedLevel1
        {
            get { return _NeedLevel1; }
            set
            {
                if (_NeedLevel1 == value) return;

                var oldValue = _NeedLevel1;
                _NeedLevel1 = value;

                OnChanged(oldValue, value, "NeedLevel1");
            }
        }
        private int _NeedLevel1;

        public int NeedLevel2
        {
            get { return _NeedLevel2; }
            set
            {
                if (_NeedLevel2 == value) return;

                var oldValue = _NeedLevel2;
                _NeedLevel2 = value;

                OnChanged(oldValue, value, "NeedLevel2");
            }
        }
        private int _NeedLevel2;

        public int NeedLevel3
        {
            get { return _NeedLevel3; }
            set
            {
                if (_NeedLevel3 == value) return;

                var oldValue = _NeedLevel3;
                _NeedLevel3 = value;

                OnChanged(oldValue, value, "NeedLevel3");
            }
        }
        private int _NeedLevel3;

        public int Experience1
        {
            get { return _Experience1; }
            set
            {
                if (_Experience1 == value) return;

                var oldValue = _Experience1;
                _Experience1 = value;

                OnChanged(oldValue, value, "Experience1");
            }
        }
        private int _Experience1;

        public int Experience2
        {
            get { return _Experience2; }
            set
            {
                if (_Experience2 == value) return;

                var oldValue = _Experience2;
                _Experience2 = value;

                OnChanged(oldValue, value, "Experience2");
            }
        }
        private int _Experience2;

        public int Experience3
        {
            get { return _experience3; }
            set
            {
                if (_experience3 == value) return;

                var oldValue = _experience3;
                _experience3 = value;

                OnChanged(oldValue, value, "Experience3");
            }
        }
        private int _experience3;


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

        public string Description
        {
            get { return _Description; }
            set
            {
                if (_Description == value) return;

                var oldValue = _Description;
                _Description = value;

                OnChanged(oldValue, value, "Description");
            }
        }
        private string _Description;
        
    }
}
