using MirDB;

namespace Library.SystemModels
{
    public class SetInfo : DBObject
    {
        public string SetName
        {
            get { return _SetName; }
            set
            {
                if (_SetName == value) return;

                var oldValue = _SetName;
                _SetName = value;

                OnChanged(oldValue, value, "SetName");
            }
        }
        private string _SetName;

        [Association("Set")]
        public DBBindingList<ItemInfo> Items { get; set; }

        [Association("SetStats")]
        public DBBindingList<SetInfoStat> SetStats { get; set; }
    }
}
