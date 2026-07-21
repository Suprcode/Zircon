using MirDB;

namespace Library.SystemModels
{
    public sealed class DungeonInfo : DBObject
    {
        [IsIdentity]
        public string Name
        {
            get => _Name;
            set
            {
                if (_Name == value) return;

                string oldValue = _Name;
                _Name = value;

                OnChanged(oldValue, value, nameof(Name));
            }
        }
        private string _Name;

        public string Description
        {
            get => _Description;
            set
            {
                if (_Description == value) return;

                string oldValue = _Description;
                _Description = value;

                OnChanged(oldValue, value, nameof(Description));
            }
        }
        private string _Description;

        public int SpawnMultiplier
        {
            get => _SpawnMultiplier;
            set
            {
                value = System.Math.Max(1, value);

                if (_SpawnMultiplier == value) return;

                int oldValue = _SpawnMultiplier;
                _SpawnMultiplier = value;

                OnChanged(oldValue, value, nameof(SpawnMultiplier));
            }
        }
        private int _SpawnMultiplier;

        [Association("DungeonMaps", true)]
        public DBBindingList<DungeonMapInfo> Maps { get; set; }

        protected internal override void OnCreated()
        {
            base.OnCreated();

            SpawnMultiplier = 1;
        }

        protected internal override void OnLoaded()
        {
            base.OnLoaded();

            if (SpawnMultiplier < 1)
                SpawnMultiplier = 1;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public sealed class DungeonMapInfo : DBObject
    {
        [IsIdentity]
        [Association("DungeonMaps")]
        public DungeonInfo Dungeon
        {
            get => _Dungeon;
            set
            {
                if (_Dungeon == value) return;

                DungeonInfo oldValue = _Dungeon;
                _Dungeon = value;

                OnChanged(oldValue, value, nameof(Dungeon));
            }
        }
        private DungeonInfo _Dungeon;

        [IsIdentity]
        [Association("DungeonMap")]
        public MapInfo Map
        {
            get => _Map;
            set
            {
                if (_Map == value) return;

                MapInfo oldValue = _Map;
                _Map = value;

                OnChanged(oldValue, value, nameof(Map));
            }
        }
        private MapInfo _Map;

        public int Floor
        {
            get => _Floor;
            set
            {
                if (_Floor == value) return;

                int oldValue = _Floor;
                _Floor = value;

                OnChanged(oldValue, value, nameof(Floor));
            }
        }
        private int _Floor;

        public DungeonMapRole Role
        {
            get => _Role;
            set
            {
                if (_Role == value) return;

                DungeonMapRole oldValue = _Role;
                _Role = value;

                OnChanged(oldValue, value, nameof(Role));
            }
        }
        private DungeonMapRole _Role;

        protected internal override void OnCreated()
        {
            base.OnCreated();

            Role = DungeonMapRole.Floor;
        }
    }
}
