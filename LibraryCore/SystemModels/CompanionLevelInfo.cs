using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MirDB;

namespace Library.SystemModels
{
    public sealed class CompanionLevelInfo : DBObject
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

        public int MaxExperience
        {
            get { return _MaxExperience; }
            set
            {
                if (_MaxExperience == value) return;

                var oldValue = _MaxExperience;
                _MaxExperience = value;

                OnChanged(oldValue, value, "MaxExperience");
            }
        }
        private int _MaxExperience;

        public int InventorySpace
        {
            get { return _InventorySpace; }
            set
            {
                if (_InventorySpace == value) return;

                var oldValue = _InventorySpace;
                _InventorySpace = value;

                OnChanged(oldValue, value, "InventorySpace");
            }
        }
        private int _InventorySpace;

        public int InventoryWeight
        {
            get { return _InventoryWeight; }
            set
            {
                if (_InventoryWeight == value) return;

                var oldValue = _InventoryWeight;
                _InventoryWeight = value;

                OnChanged(oldValue, value, "InventoryWeight");
            }
        }
        private int _InventoryWeight;

        public int MaxHunger
        {
            get { return _MaxHunger; }
            set
            {
                if (_MaxHunger == value) return;

                var oldValue = _MaxHunger;
                _MaxHunger = value;

                OnChanged(oldValue, value, "MaxHunger");
            }
        }
        private int _MaxHunger;
        
    }
}
