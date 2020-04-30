using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MirDB;

namespace Library.SystemModels
{
   public sealed   class BaseStat : DBObject
    {
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

        public int BagWeight
        {
            get { return _BagWeight; }
            set
            {
                if (_BagWeight == value) return;

                var oldValue = _BagWeight;
                _BagWeight = value;

                OnChanged(oldValue, value, "BagWeight");
            }
        }
        private int _BagWeight;

        public int WearWeight
        {
            get { return _WearWeight; }
            set
            {
                if (_WearWeight == value) return;

                var oldValue = _WearWeight;
                _WearWeight = value;

                OnChanged(oldValue, value, "WearWeight");
            }
        }
        private int _WearWeight;

        public int HandWeight
        {
            get { return _HandWeight; }
            set
            {
                if (_HandWeight == value) return;

                var oldValue = _HandWeight;
                _HandWeight = value;

                OnChanged(oldValue, value, "HandWeight");
            }
        }
        private int _HandWeight;

        public int Accuracy
        {
            get { return _Accuracy; }
            set
            {
                if (_Accuracy == value) return;

                var oldValue = _Accuracy;
                _Accuracy = value;

                OnChanged(oldValue, value, "Accuracy");
            }
        }
        private int _Accuracy;

        public int Agility
        {
            get { return _Agility; }
            set
            {
                if (_Agility == value) return;

                var oldValue = _Agility;
                _Agility = value;

                OnChanged(oldValue, value, "Agility");
            }
        }
        private int _Agility;



        public int MinAC
        {
            get { return _MinAC; }
            set
            {
                if (_MinAC == value) return;

                var oldValue = _MinAC;
                _MinAC = value;

                OnChanged(oldValue, value, "MinAC");
            }
        }
        private int _MinAC;

        public int MaxAC
        {
            get { return _MaxAC; }
            set
            {
                if (_MaxAC == value) return;

                var oldValue = _MaxAC;
                _MaxAC = value;

                OnChanged(oldValue, value, "MaxAC");
            }
        }
        private int _MaxAC;

        public int MinMR
        {
            get { return _MinMR; }
            set
            {
                if (_MinMR == value) return;

                var oldValue = _MinMR;
                _MinMR = value;

                OnChanged(oldValue, value, "MinMR");
            }
        }
        private int _MinMR;

        public int MaxMR
        {
            get { return _MaxMR; }
            set
            {
                if (_MaxMR == value) return;

                var oldValue = _MaxMR;
                _MaxMR = value;

                OnChanged(oldValue, value, "MaxMR");
            }
        }
        private int _MaxMR;
        

        public int MinDC
        {
            get { return _MinDC; }
            set
            {
                if (_MinDC == value) return;

                var oldValue = _MinDC;
                _MinDC = value;

                OnChanged(oldValue, value, "MinDC");
            }
        }
        private int _MinDC;

        public int MaxDC
        {
            get { return _MaxDC; }
            set
            {
                if (_MaxDC == value) return;

                var oldValue = _MaxDC;
                _MaxDC = value;

                OnChanged(oldValue, value, "MaxDC");
            }
        }
        private int _MaxDC;
        

        public int MinMC
        {
            get { return _MinMC; }
            set
            {
                if (_MinMC == value) return;

                var oldValue = _MinMC;
                _MinMC = value;

                OnChanged(oldValue, value, "MinMC");
            }
        }
        private int _MinMC;

        public int MaxMC
        {
            get { return _MaxMC; }
            set
            {
                if (_MaxMC == value) return;

                var oldValue = _MaxMC;
                _MaxMC = value;

                OnChanged(oldValue, value, "MaxMC");
            }
        }
        private int _MaxMC;

        public int MinSC
        {
            get { return _MinSC; }
            set
            {
                if (_MinSC == value) return;

                var oldValue = _MinSC;
                _MinSC = value;

                OnChanged(oldValue, value, "MinSC");
            }
        }
        private int _MinSC;

        public int MaxSC
        {
            get { return _MaxSC; }
            set
            {
                if (_MaxSC == value) return;

                var oldValue = _MaxSC;
                _MaxSC = value;

                OnChanged(oldValue, value, "MaxSC");
            }
        }
        private int _MaxSC;
        
    }
}
