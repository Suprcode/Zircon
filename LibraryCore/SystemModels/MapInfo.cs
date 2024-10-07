using MirDB;
using System;
using System.Text.Json.Serialization;

namespace Library.SystemModels
{
    public sealed class MapInfo : DBObject
    {
        [IsIdentity]
        public string FileName
        {
            get { return _FileName; }
            set
            {
                if (_FileName == value) return;

                var oldValue = _FileName;
                _FileName = value;

                OnChanged(oldValue, value, "FileName");
            }
        }
        private string _FileName;

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

        [JsonIgnore]
        [IgnoreProperty]
        public string ServerDescription => $"{FileName} - {Description}";

        public int MiniMap
        {
            get { return _MiniMap; }
            set
            {
                if (_MiniMap == value) return;

                var oldValue = _MiniMap;
                _MiniMap = value;

                OnChanged(oldValue, value, "MiniMap");
            }
        }
        private int _MiniMap;

        public LightSetting Light
        {
            get { return _Light; }
            set
            {
                if (_Light == value) return;

                var oldValue = _Light;
                _Light = value;

                OnChanged(oldValue, value, "Light");
            }
        }
        private LightSetting _Light;

        public Weather Weather
        {
            get { return _Weather; }
            set
            {
                if (_Weather == value) return;

                var oldValue = _Weather;
                _Weather = value;

                OnChanged(oldValue, value, "Weather");
            }
        }
        private Weather _Weather;

        public FightSetting Fight
        {
            get { return _Fight; }
            set
            {
                if (_Fight == value) return;

                var oldValue = _Fight;
                _Fight = value;

                OnChanged(oldValue, value, "Fight");
            }
        }
        private FightSetting _Fight;

        public bool AllowRT
        {
            get { return _AllowRT; }
            set
            {
                if (_AllowRT == value) return;

                var oldValue = _AllowRT;
                _AllowRT = value;

                OnChanged(oldValue, value, "AllowRT");
            }
        }
        private bool _AllowRT;

        public int SkillDelay
        {
            get { return _SkillDelay; }
            set
            {
                if (_SkillDelay == value) return;

                var oldValue = _SkillDelay;
                _SkillDelay = value;

                OnChanged(oldValue, value, "SkillDelay");
            }
        }
        private int _SkillDelay;

        public bool CanHorse
        {
            get { return _CanHorse; }
            set
            {
                if (_CanHorse == value) return;

                var oldValue = _CanHorse;
                _CanHorse = value;

                OnChanged(oldValue, value, "CanHorse");
            }
        }
        private bool _CanHorse;

        public bool AllowTT
        {
            get { return _AllowTT; }
            set
            {
                if (_AllowTT == value) return;

                var oldValue = _AllowTT;
                _AllowTT = value;

                OnChanged(oldValue, value, "AllowTT");
            }
        }
        private bool _AllowTT;

        public bool CanMine
        {
            get { return _CanMine; }
            set
            {
                if (_CanMine == value) return;

                var oldValue = _CanMine;
                _CanMine = value;

                OnChanged(oldValue, value, "CanMine");
            }
        }
        private bool _CanMine;

        public bool CanMarriageRecall
        {
            get { return _CanMarriageRecall; }
            set
            {
                if (_CanMarriageRecall == value) return;

                var oldValue = _CanMarriageRecall;
                _CanMarriageRecall = value;

                OnChanged(oldValue, value, "CanMarriageRecall");
            }
        }
        private bool _CanMarriageRecall;

        public bool AllowRecall
        {
            get => _AllowRecall;
            set
            {
                if (_AllowRecall == value) return;

                bool oldValue = _AllowRecall;
                _AllowRecall = value;

                OnChanged(oldValue, value, "AllowRecall");
            }
        }
        private bool _AllowRecall;

        public int MinimumLevel
        {
            get { return _MinimumLevel; }
            set
            {
                if (_MinimumLevel == value) return;

                var oldValue = _MinimumLevel;
                _MinimumLevel = value;

                OnChanged(oldValue, value, "MinimumLevel");
            }
        }
        private int _MinimumLevel;

        public int MaximumLevel
        {
            get { return _MaximumLevel; }
            set
            {
                if (_MaximumLevel == value) return;

                var oldValue = _MaximumLevel;
                _MaximumLevel = value;

                OnChanged(oldValue, value, "MaximumLevel");
            }
        }
        private int _MaximumLevel;

        public MapInfo ReconnectMap
        {
            get { return _ReconnectMap; }
            set
            {
                if (_ReconnectMap == value) return;

                var oldValue = _ReconnectMap;
                _ReconnectMap = value;

                OnChanged(oldValue, value, "ReconnectMap");
            }
        }
        private MapInfo _ReconnectMap;

        public SoundIndex Music
        {
            get { return _Music; }
            set
            {
                if (_Music == value) return;

                var oldValue = _Music;
                _Music = value;

                OnChanged(oldValue, value, "Music");
            }
        }
        private SoundIndex _Music;

        public int Background
        {
            get { return _Background; }
            set
            {
                if (_Background == value) return;

                var oldValue = _Background;
                _Background = value;

                OnChanged(oldValue, value, "Background");
            }
        }
        private int _Background;

        //DO NOT USE

        public int MonsterHealth
        {
            get { return _MonsterHealth; }
            set
            {
                if (_MonsterHealth == value) return;

                var oldValue = _MonsterHealth;
                _MonsterHealth = value;

                OnChanged(oldValue, value, "MonsterHealth");
            }
        }
        private int _MonsterHealth;

        public int MonsterDamage
        {
            get { return _MonsterDamage; }
            set
            {
                if (_MonsterDamage == value) return;

                var oldValue = _MonsterDamage;
                _MonsterDamage = value;

                OnChanged(oldValue, value, "MonsterDamage");
            }
        }
        private int _MonsterDamage;

        public int DropRate
        {
            get { return _DropRate; }
            set
            {
                if (_DropRate == value) return;

                var oldValue = _DropRate;
                _DropRate = value;

                OnChanged(oldValue, value, "DropRate");
            }
        }
        private int _DropRate;

        public int ExperienceRate
        {
            get { return _ExperienceRate; }
            set
            {
                if (_ExperienceRate == value) return;

                var oldValue = _ExperienceRate;
                _ExperienceRate = value;

                OnChanged(oldValue, value, "ExperienceRate");
            }
        }
        private int _ExperienceRate;

        public int GoldRate
        {
            get { return _GoldRate; }
            set
            {
                if (_GoldRate == value) return;

                var oldValue = _GoldRate;
                _GoldRate = value;

                OnChanged(oldValue, value, "GoldRate");
            }
        }
        private int _GoldRate;

        public int MaxMonsterHealth
        {
            get { return _MaxMonsterHealth; }
            set
            {
                if (_MaxMonsterHealth == value) return;

                var oldValue = _MaxMonsterHealth;
                _MaxMonsterHealth = value;

                OnChanged(oldValue, value, "MaxMonsterHealth");
            }
        }
        private int _MaxMonsterHealth;

        public int MaxMonsterDamage
        {
            get { return _MaxMonsterDamage; }
            set
            {
                if (_MaxMonsterDamage == value) return;

                var oldValue = _MaxMonsterDamage;
                _MaxMonsterDamage = value;

                OnChanged(oldValue, value, "MaxMonsterDamage");
            }
        }
        private int _MaxMonsterDamage;

        public int MaxDropRate
        {
            get { return _MaxDropRate; }
            set
            {
                if (_MaxDropRate == value) return;

                var oldValue = _MaxDropRate;
                _MaxDropRate = value;

                OnChanged(oldValue, value, "MaxDropRate");
            }
        }
        private int _MaxDropRate;

        public int MaxExperienceRate
        {
            get { return _MaxExperienceRate; }
            set
            {
                if (_MaxExperienceRate == value) return;

                var oldValue = _MaxExperienceRate;
                _MaxExperienceRate = value;

                OnChanged(oldValue, value, "MaxExperienceRate");
            }
        }
        private int _MaxExperienceRate;

        public int MaxGoldRate
        {
            get { return _MaxGoldRate; }
            set
            {
                if (_MaxGoldRate == value) return;

                var oldValue = _MaxGoldRate;
                _MaxGoldRate = value;

                OnChanged(oldValue, value, "MaxGoldRate");
            }
        }
        private int _MaxGoldRate;
        //DO NOT USE

        [JsonIgnore]
        [Association("Maps")]
        public InstanceInfo Instance
        {
            get { return _Instance; }
            set
            {
                if (_Instance == value) return;

                var oldValue = _Instance;
                _Instance = value;

                OnChanged(oldValue, value, "Instance");
            }
        }
        private InstanceInfo _Instance;

        public RequiredClass RequiredClass
        {
            get => _requiredClass;
            set
            {
                if (_requiredClass == value) return;
                var oldValue = _requiredClass;
                _requiredClass = value;
                OnChanged(oldValue, value, nameof(RequiredClass));
            }
        }
        private RequiredClass _requiredClass;

        [Association("Guards", true)]
        public DBBindingList<GuardInfo> Guards { get; set; }

        [JsonIgnore]
        [Association("Regions", true)]
        public DBBindingList<MapRegion> Regions { get; set; }

        [Association("Mining", true)]
        public DBBindingList<MineInfo> Mining { get; set; }

        [JsonIgnore]
        [Association("Castles", true)]
        public DBBindingList<CastleInfo> Castles { get; set; }

        [Association("MapInfoStats", true)]
        public DBBindingList<MapInfoStat> BuffStats { get; set; }

        public Stats Stats = new();

        protected internal override void OnCreated()
        {
            base.OnCreated();

            AllowRT = true;
            AllowTT = true;
            CanMarriageRecall = true;
            AllowRecall = true;
        }

        protected internal override void OnLoaded()
        {
            base.OnLoaded();

            StatsChanged();
        }

        public void StatsChanged()
        {
            Stats.Clear();
            foreach (MapInfoStat stat in BuffStats)
                Stats[stat.Stat] += stat.Amount;
        }

        //Client Variables

        public bool Expanded = true;
    }


    public sealed class MapInfoStat : DBObject
    {
        [IsIdentity]
        [Association("MapInfoStats")]
        public MapInfo Map
        {
            get { return _Map; }
            set
            {
                if (_Map == value) return;

                var oldValue = _Map;
                _Map = value;

                OnChanged(oldValue, value, "Map");
            }
        }
        private MapInfo _Map;

        [IsIdentity]
        public Stat Stat
        {
            get { return _Stat; }
            set
            {
                if (_Stat == value) return;

                var oldValue = _Stat;
                _Stat = value;

                OnChanged(oldValue, value, "Stat");
            }
        }
        private Stat _Stat;

        public int Amount
        {
            get { return _Amount; }
            set
            {
                if (_Amount == value) return;

                var oldValue = _Amount;
                _Amount = value;

                OnChanged(oldValue, value, "Amount");
            }
        }
        private int _Amount;
    }
}
