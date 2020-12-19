using System;
using System.Collections.Generic;
using Library;
using MirDB;

namespace Server.DBModels
{
    [UserObject]
    public sealed class CompanionFilters : DBObject
    {
        public string FilterClass
        {
            get { return _FilterClass; }
            set
            {
                if (_FilterClass == value) return;

                var oldValue = _FilterClass;
                _FilterClass = value;

                OnChanged(oldValue, value, "FilterClass");
            }
        }
        private string _FilterClass = String.Empty;

        public string FilterRarity
        {
            get { return _FilterRarity; }
            set
            {
                if (_FilterRarity == value) return;

                var oldValue = _FilterRarity;
                _FilterRarity = value;

                OnChanged(oldValue, value, "FilterRarity");
            }
        }
        private string _FilterRarity = String.Empty;


        public string FilterItemType
        {
            get { return _FilterItemType; }
            set
            {
                if (_FilterItemType == value) return;

                var oldValue = _FilterItemType;
                _FilterItemType = value;

                OnChanged(oldValue, value, "FilterItemType");
            }
        }
        private string _FilterItemType = String.Empty;

        public CompanionFiltersInfo ToClientInfo()
        {
            return new CompanionFiltersInfo
            {
                FilterClass = FilterClass,
                FilterRarity = FilterRarity,
                FilterItemType = FilterItemType,
            };
        }
    }
}
