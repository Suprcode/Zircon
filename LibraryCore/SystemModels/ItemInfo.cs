using MirDB;
using System;
using System.Text.Json.Serialization;

namespace Library.SystemModels
{
    public class ItemInfo : DBObject
    {
        [IsIdentity]
        public string ItemName
        {
            get { return _ItemName; }
            set
            {
                if (_ItemName == value) return;

                var oldValue = _ItemName;
                _ItemName = value;

                OnChanged(oldValue, value, "ItemName");
            }
        }
        private string _ItemName;

        public ItemType ItemType
        {
            get { return _ItemType; }
            set
            {
                if (_ItemType == value) return;

                var oldValue = _ItemType;
                _ItemType = value;

                OnChanged(oldValue, value, "ItemType");
            }
        }
        private ItemType _ItemType;

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

        public RequiredGender RequiredGender
        {
            get { return _RequiredGender; }
            set
            {
                if (_RequiredGender == value) return;

                var oldValue = _RequiredGender;
                _RequiredGender = value;

                OnChanged(oldValue, value, "RequiredGender");
            }
        }
        private RequiredGender _RequiredGender;

        public RequiredType RequiredType
        {
            get { return _RequiredType; }
            set
            {
                if (_RequiredType == value) return;

                var oldValue = _RequiredType;
                _RequiredType = value;

                OnChanged(oldValue, value, "RequiredType");
            }
        }
        private RequiredType _RequiredType;

        public int RequiredAmount
        {
            get { return _RequiredAmount; }
            set
            {
                if (_RequiredAmount == value) return;

                var oldValue = _RequiredAmount;
                _RequiredAmount = value;

                OnChanged(oldValue, value, "RequiredAmount");
            }
        }
        private int _RequiredAmount;

        public int Shape
        {
            get { return _Shape; }
            set
            {
                if (_Shape == value) return;

                var oldValue = _Shape;
                _Shape = value;

                OnChanged(oldValue, value, "Shape");
            }
        }
        private int _Shape;

        [JsonIgnore]
        [Obsolete("Use ItemEffect instead")]
        public ItemEffect Effect
        {
            get { return _Effect; }
            set
            {
                if (_Effect == value) return;

                var oldValue = _Effect;
                _Effect = value;

                OnChanged(oldValue, value, "Effect");
                ItemEffect = value;
            }
        }
        private ItemEffect _Effect;

        public ItemEffect ItemEffect
        {
            get { return _ItemEffect; }
            set
            {
                if (_ItemEffect == value) return;

                var oldValue = _ItemEffect;
                _ItemEffect = value;

                OnChanged(oldValue, value, "ItemEffect");
            }
        }
        private ItemEffect _ItemEffect;

        public ExteriorEffect ExteriorEffect
        {
            get { return _ExteriorEffect; }
            set
            {
                if (_ExteriorEffect == value) return;

                var oldValue = _ExteriorEffect;
                _ExteriorEffect = value;

                OnChanged(oldValue, value, "ExteriorEffect");
            }
        }
        private ExteriorEffect _ExteriorEffect;

        public int Image
        {
            get { return _Image; }
            set
            {
                if (_Image == value) return;

                var oldValue = _Image;
                _Image = value;

                OnChanged(oldValue, value, "Image");
            }
        }
        private int _Image;

        public int Durability
        {
            get { return _Durability; }
            set
            {
                if (_Durability == value) return;

                var oldValue = _Durability;
                _Durability = value;

                OnChanged(oldValue, value, "Durability");
            }
        }
        private int _Durability;

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

        public int Weight
        {
            get { return _Weight; }
            set
            {
                if (_Weight == value) return;

                var oldValue = _Weight;
                _Weight = value;

                OnChanged(oldValue, value, "Weight");
            }
        }
        private int _Weight;

        public int StackSize
        {
            get { return _StackSize; }
            set
            {
                if (_StackSize == value) return;

                var oldValue = _StackSize;
                _StackSize = value;

                OnChanged(oldValue, value, "StackSize");
            }
        }
        private int _StackSize;

        public bool StartItem
        {
            get { return _StartItem; }
            set
            {
                if (_StartItem == value) return;

                var oldValue = _StartItem;
                _StartItem = value;

                OnChanged(oldValue, value, "StartItem");
            }
        }
        private bool _StartItem;

        public decimal SellRate
        {
            get { return _SellRate; }
            set
            {
                if (_SellRate == value) return;

                var oldValue = _SellRate;
                _SellRate = value;

                OnChanged(oldValue, value, "SellRate");
            }
        }
        private decimal _SellRate;

        public bool CanRepair
        {
            get { return _CanRepair; }
            set
            {
                if (_CanRepair == value) return;

                var oldValue = _CanRepair;
                _CanRepair = value;

                OnChanged(oldValue, value, "CanRepair");
            }
        }
        private bool _CanRepair;

        public bool CanSell
        {
            get { return _CanSell; }
            set
            {
                if (_CanSell == value) return;

                var oldValue = _CanSell;
                _CanSell = value;

                OnChanged(oldValue, value, "CanSell");
            }
        }
        private bool _CanSell;

        public bool CanStore
        {
            get { return _CanStore; }
            set
            {
                if (_CanStore == value) return;

                var oldValue = _CanStore;
                _CanStore = value;

                OnChanged(oldValue, value, "CanStore");
            }
        }
        private bool _CanStore;

        public bool CanTrade
        {
            get { return _CanTrade; }
            set
            {
                if (_CanTrade == value) return;

                var oldValue = _CanTrade;
                _CanTrade = value;

                OnChanged(oldValue, value, "CanTrade");
            }
        }
        private bool _CanTrade;

        public bool CanDrop
        {
            get { return _CanDrop; }
            set
            {
                if (_CanDrop == value) return;

                var oldValue = _CanDrop;
                _CanDrop = value;

                OnChanged(oldValue, value, "CanDrop");
            }
        }
        private bool _CanDrop;

        public bool CanDeathDrop
        {
            get { return _CanDeathDrop; }
            set
            {
                if (_CanDeathDrop == value) return;

                var oldValue = _CanDeathDrop;
                _CanDeathDrop = value;

                OnChanged(oldValue, value, "CanDeathDrop");
            }
        }
        private bool _CanDeathDrop;
        
        public string Description
        {
            get { return _Description; }
            set
            {
                if (_Description == value) return;

                var oldValue = _Description;
                _Description = value;

                OnChanged(oldValue, value, "Description");
            }
        }
        private string _Description;

        public Rarity Rarity
        {
            get { return _Rarity; }
            set
            {
                if (_Rarity == value) return;

                var oldValue = _Rarity;
                _Rarity = value;

                OnChanged(oldValue, value, "Rarity");
            }
        }
        private Rarity _Rarity;

        public bool CanAutoPot
        {
            get { return _CanAutoPot; }
            set
            {
                if (_CanAutoPot == value) return;

                var oldValue = _CanAutoPot;
                _CanAutoPot = value;

                OnChanged(oldValue, value, "CanAutoPot");
            }
        }
        private bool _CanAutoPot;

        public int BuffIcon
        {
            get { return _BuffIcon; }
            set
            {
                if (_BuffIcon == value) return;

                var oldValue = _BuffIcon;
                _BuffIcon = value;

                OnChanged(oldValue, value, "BuffIcon");
            }
        }
        private int _BuffIcon;

        public int PartCount
        {
            get { return _PartCount; }
            set
            {
                if (_PartCount == value) return;

                var oldValue = _PartCount;
                _PartCount = value;

                OnChanged(oldValue, value, "PartCount");
            }
        }
        private int _PartCount;

        [Association("Set")]
        public SetInfo Set
        {
            get { return _Set; }
            set
            {
                if (_Set == value) return;

                var oldValue = _Set;
                _Set = value;

                OnChanged(oldValue, value, "Set");
            }
        }
        private SetInfo _Set;

        [Association("ItemStats")]
        public DBBindingList<ItemInfoStat> ItemStats { get; set; }

        [JsonIgnore]
        [Association("Drops", true)]
        public DBBindingList<DropInfo> Drops { get; set; }

        [JsonIgnore]
        [IgnoreProperty]
        public bool ShouldLinkInfo => StackSize > 1 || ItemType == ItemType.Consumable || ItemType == ItemType.Scroll;

        public Stats Stats = new();

        protected internal override void OnCreated()
        {
            base.OnCreated();

            StackSize = 1;

            RequiredGender = RequiredGender.None;
            RequiredClass = RequiredClass.All;

            SellRate = 0.5M;

            CanRepair = true;
            CanSell = true;
            CanStore = true;
            CanTrade = true;
            CanDrop = true;
        }
        protected internal override void OnLoaded()
        {
            base.OnLoaded();
            StatsChanged();
        }

        public void StatsChanged()
        {
            Stats.Clear();
            foreach (ItemInfoStat stat in ItemStats)
                Stats[stat.Stat] += stat.Amount;
        }

        public override string ToString()
        {
            return ItemName;
        }
    }
}
