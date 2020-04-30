using System;
using System.Drawing;
using Library;
using Library.SystemModels;
using MirDB;
using Server.Envir;

namespace Server.DBModels
{
    [UserObject]
    public sealed class UserItem : DBObject
    {
        public ItemInfo Info
        {
            get { return _Info; }
            set
            {
                if (_Info == value) return;

                var oldValue = _Info;
                _Info = value;

                OnChanged(oldValue, value, "Info");
            }
        }
        private ItemInfo _Info;
        
        public int CurrentDurability
        {
            get { return _CurrentDurability; }
            set
            {
                if (_CurrentDurability == value) return;

                var oldValue = _CurrentDurability;
                _CurrentDurability = value;

                OnChanged(oldValue, value, "CurrentDurability");
            }
        }
        private int _CurrentDurability;

        public int MaxDurability
        {
            get { return _MaxDurability; }
            set
            {
                if (_MaxDurability == value) return;

                var oldValue = _MaxDurability;
                _MaxDurability = value;

                OnChanged(oldValue, value, "MaxDurability");
            }
        }
        private int _MaxDurability;

        public long Count
        {
            get { return _Count; }
            set
            {
                if (_Count == value) return;

                var oldValue = _Count;
                _Count = value;

                OnChanged(oldValue, value, "Count");
            }
        }
        private long _Count;

        public int Slot
        {
            get { return _Slot; }
            set
            {
                if (_Slot == value) return;

                var oldValue = _Slot;
                _Slot = value;

                OnChanged(oldValue, value, "Slot");
            }
        }
        private int _Slot;

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

        public decimal Experience
        {
            get { return _Experience; }
            set
            {
                if (_Experience == value) return;

                var oldValue = _Experience;
                _Experience = value;

                OnChanged(oldValue, value, "Experience");
            }
        }
        private decimal _Experience;

        public Color Colour
        {
            get { return _Colour; }
            set
            {
                if (_Colour == value) return;

                var oldValue = _Colour;
                _Colour = value;

                OnChanged(oldValue, value, "Colour");
            }
        }
        private Color _Colour;

        public DateTime SpecialRepairCoolDown
        {
            get { return _specialRepairCoolDown; }
            set
            {
                if (_specialRepairCoolDown == value) return;

                var oldValue = _specialRepairCoolDown;
                _specialRepairCoolDown = value;

                OnChanged(oldValue, value, "SpecialRepairCoolDown");
            }
        }
        private DateTime _specialRepairCoolDown;

        public DateTime ResetCoolDown
        {
            get { return _ResetCoolDown; }
            set
            {
                if (_ResetCoolDown == value) return;

                var oldValue = _ResetCoolDown;
                _ResetCoolDown = value;

                OnChanged(oldValue, value, "ResetCoolDown");
            }
        }
        private DateTime _ResetCoolDown;
        


        public UserQuestTask UserTask;


        [Association("Items")]
        public CharacterInfo Character
        {
            get { return _Character; }
            set
            {
                if (_Character == value) return;

                var oldValue = _Character;
                _Character = value;

                OnChanged(oldValue, value, "Character");
            }
        }
        private CharacterInfo _Character;

        [Association("Items")]
        public AccountInfo Account
        {
            get { return _Account; }
            set
            {
                if (_Account == value) return;

                var oldValue = _Account;
                _Account = value;

                OnChanged(oldValue, value, "Account");
            }
        }
        private AccountInfo _Account;

        [Association("Items")]
        public GuildInfo Guild
        {
            get { return _Guild; }
            set
            {
                if (_Guild == value) return;

                var oldValue = _Guild;
                _Guild = value;

                OnChanged(oldValue, value, "Guild");
            }
        }
        private GuildInfo _Guild;

        [Association("Items")]
        public UserCompanion Companion
        {
            get { return _Companion; }
            set
            {
                if (_Companion == value) return;

                var oldValue = _Companion;
                _Companion = value;

                OnChanged(oldValue, value, "Companion");
            }
        }
        private UserCompanion _Companion;


        [Association("Refine")]
        public RefineInfo Refine
        {
            get { return _Refine; }
            set
            {
                if (_Refine == value) return;

                var oldValue = _Refine;
                _Refine = value;

                OnChanged(oldValue, value, "Refine");
            }
        }
        private RefineInfo _Refine;
        
        [Association("Auction")]
        public AuctionInfo Auction
        {
            get { return _Auction; }
            set
            {
                if (_Auction == value) return;

                var oldValue = _Auction;
                _Auction = value;

                OnChanged(oldValue, value, "Auction");
            }
        }
        private AuctionInfo _Auction;

        [Association("Mail")]
        public MailInfo Mail
        {
            get { return _Mail; }
            set
            {
                if (_Mail == value) return;

                var oldValue = _Mail;
                _Mail = value;

                OnChanged(oldValue, value, "Mail");
            }
        }
        private MailInfo _Mail;

        public UserItemFlags Flags
        {
            get { return _Flags; }
            set
            {
                if (_Flags == value) return;

                var oldValue = _Flags;
                _Flags = value;

                OnChanged(oldValue, value, "Flags");
            }
        }
        private UserItemFlags _Flags;
        
        public TimeSpan ExpireTime
        {
            get { return _ExpireTime; }
            set
            {
                if (_ExpireTime == value) return;

                var oldValue = _ExpireTime;
                _ExpireTime = value;

                OnChanged(oldValue, value, "ExpireTime");
            }
        }
        private TimeSpan _ExpireTime;

        
        
        [Association("AddedStats", true)]
        public DBBindingList<UserItemStat> AddedStats { get; set; }

        [IgnoreProperty]
        public int Weight
        {
            get
            {
                switch (Info.ItemType)
                {
                    case ItemType.Poison:
                    case ItemType.Amulet:
                        return Info.Weight;
                    default:
                        return (int)Math.Min(int.MaxValue, Info.Weight * Count);
                }
            }
        }

        public Stats Stats = new Stats();

        protected override void OnChanged(object oldValue, object newValue, string propertyName)
        {
            base.OnChanged(oldValue, newValue, propertyName);

            switch (propertyName)
            {
                case "Account":
                    if (Account != null)
                    {
                        Character = null;
                        Refine = null;
                        Auction = null;
                        Mail = null;
                        Guild = null;
                        Companion = null;
                    }
                    break;
                case "Character":
                    if (Character != null)
                    {
                        Account = null;
                        Refine = null;
                        Auction = null;
                        Mail = null;
                        Guild = null;
                        Companion = null;
                    }
                    break;
                case "Refine":
                    if (Refine != null)
                    {
                        Account = null;
                        Character = null;
                        Auction = null;
                        Mail = null;
                        Guild = null;
                        Companion = null;
                    }
                    break;
                case "Auction":
                    if (Auction != null)
                    {
                        Account = null;
                        Character = null;
                        Refine = null;
                        Mail = null;
                        Guild = null;
                        Companion = null;
                    }
                    break;
                case "Mail":
                    if (Mail != null)
                    {
                        Account = null;
                        Character = null;
                        Refine = null;
                        Auction = null;
                        Guild = null;
                        Companion = null;
                    }
                    break;
                case "Guild":
                    if (Guild != null)
                    {
                        Character = null;
                        Account = null;
                        Refine = null;
                        Auction = null;
                        Mail = null;
                        Companion = null;
                    }
                    break;
                case "Companion":
                    if (Companion != null)
                    {
                        Character = null;
                        Account = null;
                        Refine = null;
                        Auction = null;
                        Mail = null;
                        Guild = null;
                    }
                    break;

            }
        }

        protected override void OnDeleted()
        {
            Info = null;

            Character = null;
            Account = null;
            Guild = null;
            Companion = null;
            Refine = null;
            Auction = null;
            Mail = null;
            UserTask = null;

            for (int i = AddedStats.Count - 1; i >= 0; i--)
                AddedStats[i].Delete();

            UserTask = null;

            base.OnDeleted();
        }


        protected override void OnCreated()
        {
            base.OnCreated();

            Count = 1;
            Slot = -1;
            Level = 1;
        }
        protected override void OnLoaded()
        {
            base.OnLoaded();

            StatsChanged();
        }

        public void StatsChanged()
        {
            Stats.Clear();

            foreach (UserItemStat stat in AddedStats)
                Stats[stat.Stat] += stat.Amount;
        }
        public void AddStat(Stat stat, int amount, StatSource source)
        {
            foreach (UserItemStat addedStat in AddedStats)
            {
                if (addedStat.Stat != stat || addedStat.StatSource != source) continue;


                addedStat.Amount += amount;

                return;
            }

            if (amount == 0) return;

            UserItemStat newStat = SEnvir.UserItemStatsList.CreateNewObject();

            newStat.StatSource = source;
            newStat.Stat = stat;
            newStat.Amount = amount;
            newStat.Item = this;
        }

        public ClientUserItem ToClientInfo()
        {
            return new ClientUserItem
            {
                Index =  Index,

                InfoIndex = Info.Index,

                CurrentDurability = CurrentDurability,
                MaxDurability = MaxDurability,

                Count = Count,
                
                Slot = Slot,

                Level = Level,
                Experience = Experience,

                Colour = Colour,

                SpecialRepairCoolDown = SpecialRepairCoolDown > SEnvir.Now ? SpecialRepairCoolDown - SEnvir.Now : TimeSpan.Zero,
                ResetCoolDown = ResetCoolDown > SEnvir.Now ? ResetCoolDown - SEnvir.Now : TimeSpan.Zero,

                AddedStats = new Stats(Stats),

                Flags = Flags,

                ExpireTime =  ExpireTime,
            };
        }

        public long Price(long count)
        {
            if (Info == null) return 0;
            if ((Flags & UserItemFlags.Worthless) == UserItemFlags.Worthless) return 0;

            decimal p = Info.Price;

            if (Info.Durability > 0)
            {
                decimal r = Info.Price / 2M / Info.Durability;

                p = MaxDurability * r;

                r = MaxDurability > 0 ? CurrentDurability / (decimal)MaxDurability : 0;

                p = Math.Floor(p / 2M + p / 2M * r + Info.Price / 2M);
            }

            p = p * (Stats.Count * 0.1M + 1M);

            if (Info.Stats[Stat.SaleBonus20] > 0 && Info.Stats[Stat.SaleBonus20] <= count)
                p *= 1.2M;
            else if (Info.Stats[Stat.SaleBonus15] > 0 && Info.Stats[Stat.SaleBonus15] <= count)
                p *= 1.15M;
            else if (Info.Stats[Stat.SaleBonus10] > 0 && Info.Stats[Stat.SaleBonus10] <= count)
                p *= 1.1M;
            else if (Info.Stats[Stat.SaleBonus5] > 0 && Info.Stats[Stat.SaleBonus5] <= count)
                p *= 1.05M;

            return (long)(p * count * Info.SellRate);
        }
        public long RepairCost(bool special)
        {
            if (Info.Durability == 0 || CurrentDurability >= MaxDurability) return 0;

            int rate = special ? 2 : 1;

            decimal p = Math.Floor(MaxDurability * (Info.Price / 2M / Info.Durability) + Info.Price / 2M);
            p = p * (Stats.Count * 0.1M + 1M);

            return (long)(p * Count - Price(Count)) * rate;
        }
        public bool CanFragment()
        {
            if ((Flags & UserItemFlags.Worthless) == UserItemFlags.Worthless || (Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return false;

            switch (Info.Rarity)
            {
                case Rarity.Common:
                    if (Info.RequiredAmount <= 15) return false;
                    break;
                case Rarity.Superior:
                    break;
                case Rarity.Elite:
                    break;
            }

            switch (Info.ItemType)
            {
                case ItemType.Weapon:
                case ItemType.Armour:
                case ItemType.Helmet:
                case ItemType.Necklace:
                case ItemType.Bracelet:
                case ItemType.Ring:
                case ItemType.Shoes:
                    break;
                default:
                    return false;
            }

            return true;
        }
        public int FragmentCost()
        {
            switch (Info.Rarity)
            {
                case Rarity.Common:
                    switch (Info.ItemType)
                    {
                        case ItemType.Armour:
                        case ItemType.Weapon:
                        case ItemType.Helmet:
                        case ItemType.Necklace:
                        case ItemType.Bracelet:
                        case ItemType.Ring:
                        case ItemType.Shoes:
                            return Info.RequiredAmount * 10000 / 9;
                      /*  case ItemType.Helmet:
                        case ItemType.Necklace:
                        case ItemType.Bracelet:
                        case ItemType.Ring:
                        case ItemType.Shoes:
                            return Info.RequiredAmount * 7000 / 9;*/
                        default:
                            return 0;
                    }
                case Rarity.Superior:
                    switch (Info.ItemType)
                    {
                        case ItemType.Armour:
                        case ItemType.Weapon:
                        case ItemType.Helmet:
                        case ItemType.Necklace:
                        case ItemType.Bracelet:
                        case ItemType.Ring:
                        case ItemType.Shoes:
                            return Info.RequiredAmount * 10000 / 2;
                      /*  case ItemType.Helmet:
                        case ItemType.Necklace:
                        case ItemType.Bracelet:
                        case ItemType.Ring:
                        case ItemType.Shoes:
                            return Info.RequiredAmount * 10000 / 10;*/
                        default:
                            return 0;
                    }
                case Rarity.Elite:
                    switch (Info.ItemType)
                    {
                        case ItemType.Weapon:
                        case ItemType.Armour:
                            return 250000;
                        case ItemType.Helmet:
                            return 50000;
                        case ItemType.Necklace:
                        case ItemType.Bracelet:
                        case ItemType.Ring:
                            return 150000;
                        case ItemType.Shoes:
                            return 30000;
                        default:
                            return 0;
                    }
                default:
                    return 0;
            }
        }
        public int FragmentCount()
        {
            switch (Info.Rarity)
            {
                case Rarity.Common:
                    switch (Info.ItemType)
                    {
                        case ItemType.Armour:
                        case ItemType.Weapon:
                        case ItemType.Helmet:
                        case ItemType.Necklace:
                        case ItemType.Bracelet:
                        case ItemType.Ring:
                        case ItemType.Shoes:
                            return Math.Max(1, Info.RequiredAmount / 2 + 5);
                      /*  case ItemType.Helmet:
                            return Math.Max(1, (Info.RequiredAmount - 30) / 6);
                        case ItemType.Necklace:
                            return Math.Max(1, Info.RequiredAmount / 8);
                        case ItemType.Bracelet:
                            return Math.Max(1, Info.RequiredAmount / 15);
                        case ItemType.Ring:
                            return Math.Max(1, Info.RequiredAmount / 9);
                        case ItemType.Shoes:
                            return Math.Max(1, (Info.RequiredAmount - 35) / 6);*/
                        default:
                            return 0;
                    }
                case Rarity.Superior:
                    switch (Info.ItemType)
                    {
                        case ItemType.Armour:
                        case ItemType.Weapon:
                        case ItemType.Helmet:
                        case ItemType.Necklace:
                        case ItemType.Bracelet:
                        case ItemType.Ring:
                        case ItemType.Shoes:
                            return Math.Max(1, Info.RequiredAmount / 2 + 5);
                     /*   case ItemType.Helmet:
                            return Math.Max(1, (Info.RequiredAmount - 30) / 6);
                        case ItemType.Necklace:
                            return Math.Max(1, Info.RequiredAmount / 10);
                        case ItemType.Bracelet:
                            return Math.Max(1, Info.RequiredAmount / 15);
                        case ItemType.Ring:
                            return Math.Max(1, Info.RequiredAmount / 10);
                        case ItemType.Shoes:
                            return Math.Max(1, (Info.RequiredAmount - 35) / 6);*/
                        default:
                            return 0;
                    }
                case Rarity.Elite:
                    switch (Info.ItemType)
                    {
                        case ItemType.Armour:
                        case ItemType.Weapon:
                            return 50;
                        case ItemType.Helmet:
                            return 5;
                        case ItemType.Necklace:
                        case ItemType.Bracelet:
                        case ItemType.Ring:
                            return 10;
                        case ItemType.Shoes:
                            return 3;
                        default:
                            return 0;
                    }
                default:
                    return 0;
            }
        }

        public int MergeRefineElements(out Stat element)
        {
            int value = 0;
            element = Stats.GetWeaponElement();

            for (int i = AddedStats.Count - 1; i >= 0; i--)
            {
                UserItemStat stat = AddedStats[i];
                if (stat.StatSource != StatSource.Refine) continue;

                switch (stat.Stat)
                {
                    case Stat.FireAttack:
                    case Stat.IceAttack:
                    case Stat.LightningAttack:
                    case Stat.WindAttack:
                    case Stat.HolyAttack:
                    case Stat.DarkAttack:
                    case Stat.PhantomAttack:
                        value += stat.Amount;
                        stat.Delete();
                        break;
                }

            }

            if (value > 0 && element != Stat.None)
                AddStat(element, value, StatSource.Refine);

            return value;
        }


        public override string ToString()
        {
            return Info.ToString();
        }
    }
}
