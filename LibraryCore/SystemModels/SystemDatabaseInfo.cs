using MirDB;

namespace Library.SystemModels
{
    public sealed class SystemDatabaseInfo : DBObject
    {
        [IsIdentity]
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

        public string Version
        {
            get { return _Version; }
            set
            {
                if (_Version == value) return;

                var oldValue = _Version;
                _Version = value;

                OnChanged(oldValue, value, "Version");
            }
        }
        private string _Version;
    }
}
