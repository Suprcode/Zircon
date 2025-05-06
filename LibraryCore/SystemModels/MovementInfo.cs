using MirDB;

namespace Library.SystemModels
{
    public sealed class MovementInfo : DBObject
    {
        [IsIdentity]
        public MapRegion SourceRegion
        {
            get { return _SourceRegion; }
            set
            {
                if (_SourceRegion == value) return;

                var oldValue = _SourceRegion;
                _SourceRegion = value;

                OnChanged(oldValue, value, "SourceRegion");
            }
        }
        private MapRegion _SourceRegion;

        [IsIdentity]
        public MapRegion DestinationRegion
        {
            get { return _DestinationRegion; }
            set
            {
                if (_DestinationRegion == value) return;

                var oldValue = _DestinationRegion;
                _DestinationRegion = value;

                OnChanged(oldValue, value, "DestinationRegion");
            }
        }
        private MapRegion _DestinationRegion;

        public MapIcon Icon
        {
            get { return _Icon; }
            set
            {
                if (_Icon == value) return;

                var oldValue = _Icon;
                _Icon = value;

                OnChanged(oldValue, value, "Icon");
            }
        }
        private MapIcon _Icon;

        public ItemInfo NeedItem
        {
            get { return _NeedItem; }
            set
            {
                if (_NeedItem == value) return;

                var oldValue = _NeedItem;
                _NeedItem = value;

                OnChanged(oldValue, value, "NeedItem");
            }
        }
        private ItemInfo _NeedItem;

        public RespawnInfo NeedSpawn
        {
            get { return _NeedSpawn; }
            set
            {
                if (_NeedSpawn == value) return;

                var oldValue = _NeedSpawn;
                _NeedSpawn = value;

                OnChanged(oldValue, value, "NeedSpawn");
            }
        }
        private RespawnInfo _NeedSpawn;

        public bool NeedHole
        {
            get { return _NeedHole; }
            set
            {
                if (_NeedHole == value) return;

                var oldValue = _NeedHole;
                _NeedHole = value;

                OnChanged(oldValue, value, "NeedHole");
            }
        }
        private bool _NeedHole;

        public InstanceInfo NeedInstance
        {
            get { return _NeedInstance; }
            set
            {
                if (_NeedInstance == value) return;

                var oldValue = _NeedInstance;
                _NeedInstance = value;

                OnChanged(oldValue, value, "NeedInstance");
            }
        }
        private InstanceInfo _NeedInstance;

        public MovementEffect Effect
        {
            get { return _Effect; }
            set
            {
                if (_Effect == value) return;

                var oldValue = _Effect;
                _Effect = value;

                OnChanged(oldValue, value, "Effect");
            }
        }
        private MovementEffect _Effect;

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


        protected internal override void OnCreated()
        {
            base.OnCreated();

            RequiredClass = RequiredClass.All;
        }
    }
}
