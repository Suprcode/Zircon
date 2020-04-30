using MirDB;

namespace Library.SystemModels
{
    public sealed class SetInfoStat : DBObject
    {
        [Association("SetStats")]
        public SetInfo Set
        {
            get { return _Set; }
            set
            {
                if (_Set == value) return;

                var oldValue = _Set;
                _Set = value;

                OnChanged(oldValue, value, "Set");
            }
        }
        private SetInfo _Set;

        public Stat Stat
        {
            get { return _Stat; }
            set
            {
                if (_Stat == value) return;

                var oldValue = _Stat;
                _Stat = value;

                OnChanged(oldValue, value, "Stat");
            }
        }
        private Stat _Stat;

        public int Amount
        {
            get { return _Amount; }
            set
            {
                if (_Amount == value) return;

                var oldValue = _Amount;
                _Amount = value;

                OnChanged(oldValue, value, "Amount");
            }
        }
        private int _Amount;

        public RequiredClass Class
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
        private RequiredClass _Class;

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


        protected internal override void OnCreated()
        {
            base.OnCreated();

            Class = RequiredClass.All;
        }
    }
}
