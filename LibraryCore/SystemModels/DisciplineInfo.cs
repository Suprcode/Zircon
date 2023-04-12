using MirDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.SystemModels
{
    public class DisciplineInfo : DBObject
    {
        [IsIdentity]
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

        public long RequiredExperience
        {
            get { return _RequiredExperience; }
            set
            {
                if (_RequiredExperience == value) return;

                var oldValue = _RequiredExperience;
                _RequiredExperience = value;

                OnChanged(oldValue, value, "RequiredExperience");
            }
        }
        private long _RequiredExperience;

        public int RequiredGold
        {
            get { return _RequiredGold; }
            set
            {
                if (_RequiredGold == value) return;

                var oldValue = _RequiredGold;
                _RequiredGold = value;

                OnChanged(oldValue, value, "RequiredGold");
            }
        }
        private int _RequiredGold;

        public int FocusPoints
        {
            get { return _FocusPoints; }
            set
            {
                if (_FocusPoints == value) return;

                var oldValue = _FocusPoints;
                _FocusPoints = value;

                OnChanged(oldValue, value, "FocusPoints");
            }
        }
        private int _FocusPoints;
    }
}
