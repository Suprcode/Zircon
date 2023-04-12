using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MirDB;

namespace Library.SystemModels
{
    public class CompanionInfo : DBObject
    {
        [IsIdentity]
        public MonsterInfo MonsterInfo
        {
            get { return _MonsterInfo; }
            set
            {
                if (_MonsterInfo == value) return;

                var oldValue = _MonsterInfo;
                _MonsterInfo = value;

                OnChanged(oldValue, value, "MonsterInfo");
            }
        }
        private MonsterInfo _MonsterInfo;

        public int Price
        {
            get { return _Price; }
            set
            {
                if (_Price == value) return;

                var oldValue = _Price;
                _Price = value;

                OnChanged(oldValue, value, "Price");
            }
        }
        private int _Price;

        public bool Available
        {
            get { return _Available; }
            set
            {
                if (_Available == value) return;

                var oldValue = _Available;
                _Available = value;

                OnChanged(oldValue, value, "Available");
            }
        }
        private bool _Available;
    }
}
