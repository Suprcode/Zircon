using Library.SystemModels;
using MirDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DBModels
{
    [UserObject]
    public class GameNPCList : DBObject
    {
        public string Category
        {
            get { return _Category; }
            set
            {
                if (_Category == value) return;

                var oldValue = _Category;
                _Category = value;

                OnChanged(oldValue, value, "Category");
            }
        }
        private string _Category;

        public string TypeValue
        {
            get { return _TypeValue; }
            set
            {
                if (_TypeValue == value) return;

                var oldValue = _TypeValue;
                _TypeValue = value;

                OnChanged(oldValue, value, "TypeValue");
            }
        }
        private string _TypeValue;

        public int IntValue1
        {
            get { return _IntValue1; }
            set
            {
                if (_IntValue1 == value) return;

                var oldValue = _IntValue1;
                _IntValue1 = value;

                OnChanged(oldValue, value, "IntValue1");
            }
        }
        private int _IntValue1;
    }
}
