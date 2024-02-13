using Library;
using Library.SystemModels;
using MirDB;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.DBModels
{
    [UserObject]
    public sealed class AccountInfo : DBObject
    {
        public string EMailAddress
        {
            get { return _EMailAddress; }
            set
            {
                if (_EMailAddress == value) return;

                var oldValue = _EMailAddress;
                _EMailAddress = value;

                OnChanged(oldValue, value, "EMailAddress");
            }
        }
        private string _EMailAddress;
        
        public byte[] Password
        {
            get { return _Password; }
            set
            {
                if (_Password == value) return;

                var oldValue = _Password;
                _Password = value;

                OnChanged(oldValue, value, "Password");
            }
        }
        private byte[] _Password;

        public string RealName
        {
            get { return _RealName; }
            set
            {
                if (_RealName == value) return;

                var oldValue = _RealName;
                _RealName = value;

                OnChanged(oldValue, value, "RealName");
            }
        }
        private string _RealName;

        public DateTime BirthDate
        {
            get { return _BirthDate; }
            set
            {
                if (_BirthDate == value) return;

                var oldValue = _BirthDate;
                _BirthDate = value;

                OnChanged(oldValue, value, "BirthDate");
            }
        }
        private DateTime _BirthDate;

        public string Question
        {
            get { return _Question; }
            set
            {
                if (_Question == value) return;

                var oldValue = _Question;
                _Question = value;

                OnChanged(oldValue, value, "Question");
            }
        }
        private string _Question;

        public string Answer
        {
            get { return _Answer; }
            set
            {
                if (_Answer == value) return;

                var oldValue = _Answer;
                _Answer = value;

                OnChanged(oldValue, value, "Answer");
            }
        }
        private string _Answer;

        [Association("Referrals")]
        public AccountInfo Referral
        {
            get { return _Referral; }
            set
            {
                if (_Referral == value) return;

                var oldValue = _Referral;
                _Referral = value;

                OnChanged(oldValue, value, "Referral");
            }
        }
        private AccountInfo _Referral;
        
        public string CreationIP
        {
            get { return _CreationIP; }
            set
            {
                if (_CreationIP == value) return;

                var oldValue = _CreationIP;
                _CreationIP = value;

                OnChanged(oldValue, value, "CreationIP");
            }
        }
        private string _CreationIP;
        
        public DateTime CreationDate
        {
            get { return _CreationDate; }
            set
            {
                if (_CreationDate == value) return;

                var oldValue = _CreationDate;
                _CreationDate = value;

                OnChanged(oldValue, value, "CreationDate");
            }
        }
        private DateTime _CreationDate;

        public string LastIP
        {
            get { return _LastIP; }
            set
            {
                if (_LastIP == value) return;

                var oldValue = _LastIP;
                _LastIP = value;

                OnChanged(oldValue, value, "LastIP");
            }
        }
        private string _LastIP;

        public DateTime LastLogin
        {
            get { return _LastLogin; }
            set
            {
                if (_LastLogin == value) return;

                var oldValue = _LastLogin;
                _LastLogin = value;

                OnChanged(oldValue, value, "LastLogin");
            }
        }
        private DateTime _LastLogin;

        public string ActivationKey
        {
            get { return _ActivationKey; }
            set
            {
                if (_ActivationKey == value) return;

                var oldValue = _ActivationKey;
                _ActivationKey = value;

                OnChanged(oldValue, value, "ActivationKey");
            }
        }
        private string _ActivationKey;

        public DateTime ActivationTime
        {
            get { return _ActivationTime; }
            set
            {
                if (_ActivationTime == value) return;

                var oldValue = _ActivationTime;
                _ActivationTime = value;

                OnChanged(oldValue, value, "ActivationTime");
            }
        }
        private DateTime _ActivationTime;
        
        public bool Activated
        {
            get { return _Activated; }
            set
            {
                if (_Activated == value) return;

                var oldValue = _Activated;
                _Activated = value;

                OnChanged(oldValue, value, "Activated");
            }
        }
        private bool _Activated;

        public string ResetKey
        {
            get { return _ResetKey; }
            set
            {
                if (_ResetKey == value) return;

                var oldValue = _ResetKey;
                _ResetKey = value;

                OnChanged(oldValue, value, "ResetKey");
            }
        }
        private string _ResetKey;

        public DateTime ResetTime
        {
            get { return _ResetTime; }
            set
            {
                if (_ResetTime == value) return;

                var oldValue = _ResetTime;
                _ResetTime = value;

                OnChanged(oldValue, value, "ResetTime");
            }
        }
        private DateTime _ResetTime;

        public DateTime PasswordTime
        {
            get { return _PasswordTime; }
            set
            {
                if (_PasswordTime == value) return;

                var oldValue = _PasswordTime;
                _PasswordTime = value;

                OnChanged(oldValue, value, "PasswordTime");
            }
        }
        private DateTime _PasswordTime;

        public DateTime ChatBanExpiry
        {
            get { return _ChatBanExpiry; }
            set
            {
                if (_ChatBanExpiry == value) return;

                var oldValue = _ChatBanExpiry;
                _ChatBanExpiry = value;

                OnChanged(oldValue, value, "ChatBanExpiry");
            }
        }
        private DateTime _ChatBanExpiry;

        public bool Banned
        {
            get { return _Banned; }
            set
            {
                if (_Banned == value) return;

                var oldValue = _Banned;
                _Banned = value;

                OnChanged(oldValue, value, "Banned");
            }
        }
        private bool _Banned;

        public DateTime ExpiryDate
        {
            get { return _ExpiryDate; }
            set
            {
                if (_ExpiryDate == value) return;

                var oldValue = _ExpiryDate;
                _ExpiryDate = value;

                OnChanged(oldValue, value, "ExpiryDate");
            }
        }
        private DateTime _ExpiryDate;

        public string BanReason
        {
            get { return _BanReason; }
            set
            {
                if (_BanReason == value) return;

                var oldValue = _BanReason;
                _BanReason = value;

                OnChanged(oldValue, value, "BanReason");
            }
        }
        private string _BanReason;

        [IgnoreProperty]
        public UserCurrency Gold => Currencies.First(x => x.Info.Type == CurrencyType.Gold);

        [IgnoreProperty]
        public UserCurrency GameGold => Currencies.First(x => x.Info.Type == CurrencyType.GameGold);

        [IgnoreProperty]
        public UserCurrency HuntGold => Currencies.First(x => x.Info.Type == CurrencyType.HuntGold);

        public bool AllowGroup
        {
            get { return _AllowGroup; }
            set
            {
                if (_AllowGroup == value) return;

                var oldValue = _AllowGroup;
                _AllowGroup = value;

                OnChanged(oldValue, value, "AllowGroup");
            }
        }
        private bool _AllowGroup;

        public bool AllowTrade
        {
            get { return _AllowTrade; }
            set
            {
                if (_AllowTrade == value) return;

                var oldValue = _AllowTrade;
                _AllowTrade = value;

                OnChanged(oldValue, value, "AllowTrade");
            }
        }
        private bool _AllowTrade;

        public bool AllowGuild
        {
            get { return _AllowGuild; }
            set
            {
                if (_AllowGuild == value) return;

                var oldValue = _AllowGuild;
                _AllowGuild = value;

                OnChanged(oldValue, value, "AllowGuild");
            }
        }
        private bool _AllowGuild;

        public bool AllowGroupRecall
        {
            get { return _AllowGroupRecall; }
            set
            {
                if (_AllowGroupRecall == value) return;

                var oldValue = _AllowGroupRecall;
                _AllowGroupRecall = value;

                OnChanged(oldValue, value, "AllowGroupRecall");
            }
        }
        private bool _AllowGroupRecall;
        


        [Association("Member")]
        public GuildMemberInfo GuildMember
        {
            get { return _GuildMember; }
            set
            {
                if (_GuildMember == value) return;

                var oldValue = _GuildMember;
                _GuildMember = value;

                OnChanged(oldValue, value, "GuildMember");
            }
        }
        private GuildMemberInfo _GuildMember;
        
        public DateTime GlobalTime
        {
            get { return _GlobalTime; }
            set
            {
                if (_GlobalTime == value) return;

                var oldValue = _GlobalTime;
                _GlobalTime = value;

                OnChanged(oldValue, value, "GlobalTime");
            }
        }
        private DateTime _GlobalTime;

        public HorseType Horse
        {
            get { return _Horse; }
            set
            {
                if (_Horse == value) return;

                var oldValue = _Horse;
                _Horse = value;

                OnChanged(oldValue, value, "Horse");
            }
        }
        private HorseType _Horse;

        public bool Admin
        {
            get { return _Admin; }
            set
            {
                if (_Admin == value) return;

                var oldValue = _Admin;
                _Admin = value;

                OnChanged(oldValue, value, "Admin");
            }
        }
        private bool _Admin;

        public int StorageSize
        {
            get { return _StorageSize; }
            set
            {
                if (_StorageSize == value) return;

                var oldValue = _StorageSize;
                _StorageSize = value;

                OnChanged(oldValue, value, "StorageSize");
            }
        }
        private int _StorageSize;

        public string LastSum
        {
            get { return _LastSum; }
            set
            {
                if (_LastSum == value) return;

                var oldValue = _LastSum;
                _LastSum = value;

                OnChanged(oldValue, value, "LastSum");
            }
        }
        private string _LastSum;

        public bool GoldBot
        {
            get { return _GoldBot; }
            set
            {
                if (_GoldBot == value) return;

                var oldValue = _GoldBot;
                _GoldBot = value;

                OnChanged(oldValue, value, "GoldBot");
            }
        }
        private bool _GoldBot;

        public bool ItemBot
        {
            get { return _ItemBot; }
            set
            {
                if (_ItemBot == value) return;

                var oldValue = _ItemBot;
                _ItemBot = value;

                OnChanged(oldValue, value, "ItemBot");
            }
        }
        private bool _ItemBot;
        

        public DateTime GuildTime
        {
            get { return _GuildTime; }
            set
            {
                if (_GuildTime == value) return;

                var oldValue = _GuildTime;
                _GuildTime = value;

                OnChanged(oldValue, value, "GuildTime");
            }
        }
        private DateTime _GuildTime;

        public bool Observer
        {
            get { return _Observer; }
            set
            {
                if (_Observer == value) return;

                var oldValue = _Observer;
                _Observer = value;

                OnChanged(oldValue, value, "Observer");
            }
        }
        private bool _Observer;
        
        public bool TempAdmin;

        [Association("Currencies")]
        public DBBindingList<UserCurrency> Currencies { get; set; }

        [Association("Items")]
        public DBBindingList<UserItem> Items { get; set; }

        [Association("Referrals")]
        public DBBindingList<AccountInfo> Referrals { get; set; }

        [Association("Characters")]
        public DBBindingList<CharacterInfo> Characters { get; set; }

        [Association("Buffs")]
        public DBBindingList<BuffInfo> Buffs { get; set; }

        [Association("Auctions")]
        public DBBindingList<AuctionInfo> Auctions { get; set; }
        
        [Association("Mail")]
        public DBBindingList<MailInfo> Mail { get; set; }
        
        [Association("UserDrops")]
        public DBBindingList<UserDrop> UserDrops { get; set; }

        [Association("Companions")]
        public DBBindingList<UserCompanion> Companions { get; set; }

        [Association("CompanionUnlocks")]
        public DBBindingList<UserCompanionUnlock> CompanionUnlocks { get; set; }

        [Association("BlockingList")]
        public DBBindingList<BlockInfo> BlockingList { get; set; } //Who this account is Blocking.

        [Association("BlockedByList")]
        public DBBindingList<BlockInfo> BlockedByList { get; set; } //Who's blocked this account.

        [Association("Payments")]
        public DBBindingList<GameGoldPayment> Payments { get; set; }

        [Association("StoreSales")]
        public DBBindingList<GameStoreSale> StoreSales { get; set; }

        [Association("Fortunes")]
        public DBBindingList<UserFortuneInfo> Fortunes { get; set; }

        [Association("Quests")]
        public DBBindingList<UserQuest> Quests { get; set; }

        public CharacterInfo LastCharacter
        {
            get { return _LastCharacter; }
            set
            {
                if (_LastCharacter == value) return;

                var oldValue = _LastCharacter;
                _LastCharacter = value;

                OnChanged(oldValue, value, "LastCharacter");
            }
        }
        private CharacterInfo _LastCharacter;
        

        public int WrongPasswordCount;
        public SConnection Connection;
        public string Key;

        protected override void OnCreated()
        {
            base.OnCreated();

            StorageSize = Globals.StorageSize;

            BuffInfo buff = SEnvir.BuffInfoList.CreateNewObject();

            buff.Account = this;
            buff.Type = BuffType.HuntGold;
            buff.TickFrequency = TimeSpan.FromMinutes(1);
            buff.Stats = new Stats { [Stat.AvailableHuntGoldCap] = 15 };
            buff.RemainingTime = TimeSpan.MaxValue;

            AddDefaultCurrencies();
        }
        protected override void OnLoaded()
        {
            base.OnLoaded();

            if (StorageSize == 0)
                StorageSize = Globals.StorageSize;

            BuffInfo buff = Buffs.FirstOrDefault(x => x.Type == BuffType.HuntGold);
            if (buff != null)
                buff.Stats = new Stats { [Stat.AvailableHuntGoldCap] = 15 };

            var removeList = new List<UserItem>();

            foreach (var item in Items)
            {
                if (item.Info == null)
                    removeList.Add(item);
            }

            foreach (var item in removeList)
                Items.Remove(item);

            AddDefaultCurrencies();
            RemoveDeletedCurrencies();
        }

        private void AddDefaultCurrencies()
        {
            foreach (var currency in Session.GetCollection<CurrencyInfo>().Binding)
            {
                var userCurrency = Currencies.FirstOrDefault(x => x.Info == currency);

                if (userCurrency == null)
                {
                    userCurrency = Session.GetCollection<UserCurrency>().CreateNewObject();
                    userCurrency.Account = this;
                    userCurrency.Info = currency;
                }
            }
        }

        private void RemoveDeletedCurrencies()
        {
            for (int i = Currencies.Count - 1; i >= 0; i--)
            {
                var currency = Currencies[i];

                if (currency.Info == null)
                {
                    Currencies.RemoveAt(i);
                }
            }
        }

        public bool IsAdmin(bool includeTemp = false)
        {
            return Admin || (includeTemp && TempAdmin);
        }

        public int HighestLevel()
        {
            int count = 0;

            foreach (CharacterInfo character in Characters)
                if (character.Level > count)
                    count = character.Level;

            return count;
        }

        public List<SelectInfo> GetSelectInfo()
        {
            List<SelectInfo> characters = new List<SelectInfo>();

            foreach (CharacterInfo character in Characters)
            {
                if (character.Deleted) continue;

                characters.Add(character.ToSelectInfo());
            }

            return characters;
        }

        public override string ToString()
        {
            return EMailAddress;
        }
    }
}
