using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MirDB;

namespace Library.SystemModels
{
    public sealed class WeaponCraftStatInfo : DBObject
    {
        [IsIdentity]
        public RequiredClass RequiredClass
        {
            get { return _RequiredClass; }
            set
            {
                if (_RequiredClass == value) return;

                var oldValue = _RequiredClass;
                _RequiredClass = value;

                OnChanged(oldValue, value, "RequiredClass");
            }
        }
        private RequiredClass _RequiredClass;

        [IsIdentity]
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

        public int MinValue
        {
            get { return _MinValue; }
            set
            {
                if (_MinValue == value) return;

                var oldValue = _MinValue;
                _MinValue = value;

                OnChanged(oldValue, value, "MinValue");
            }
        }
        private int _MinValue;

        public int MaxValue
        {
            get { return _MaxValue; }
            set
            {
                if (_MaxValue == value) return;

                var oldValue = _MaxValue;
                _MaxValue = value;

                OnChanged(oldValue, value, "MaxValue");
            }
        }
        private int _MaxValue;

        [IsIdentity]
        public int Weight
        {
            get { return _Weight; }
            set
            {
                if (_Weight == value) return;

                var oldValue = _Weight;
                _Weight = value;

                OnChanged(oldValue, value, "Weight");
            }
        }
        private int _Weight;
        
    }
}
