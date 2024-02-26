using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MirDB;

namespace Client.UserModels
{
    [UserObject]
    public sealed class KeyBindInfo : DBObject
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
        
        public KeyBindAction Action
        {
            get { return _Action; }
            set
            {
                if (_Action == value) return;

                var oldValue = _Action;
                _Action = value;

                OnChanged(oldValue, value, "Action");
            }
        }
        private KeyBindAction _Action;

        public bool Control1
        {
            get { return _Control1; }
            set
            {
                if (_Control1 == value) return;

                var oldValue = _Control1;
                _Control1 = value;

                OnChanged(oldValue, value, "Control1");
            }
        }
        private bool _Control1;

        public bool Alt1
        {
            get { return _Alt1; }
            set
            {
                if (_Alt1 == value) return;

                var oldValue = _Alt1;
                _Alt1 = value;

                OnChanged(oldValue, value, "Alt1");
            }
        }
        private bool _Alt1;

        public bool Shift1
        {
            get { return _Shift1; }
            set
            {
                if (_Shift1 == value) return;

                var oldValue = _Shift1;
                _Shift1 = value;

                OnChanged(oldValue, value, "Shift1");
            }
        }
        private bool _Shift1;

        public Keys Key1
        {
            get { return _Key1; }
            set
            {
                if (_Key1 == value) return;

                var oldValue = _Key1;
                _Key1 = value;

                OnChanged(oldValue, value, "Key1");
            }
        }
        private Keys _Key1;
        

        public bool Control2
        {
            get { return _Control2; }
            set
            {
                if (_Control2 == value) return;

                var oldValue = _Control2;
                _Control2 = value;

                OnChanged(oldValue, value, "Control2");
            }
        }
        private bool _Control2;

        public bool Shift2
        {
            get { return _Shift2; }
            set
            {
                if (_Shift2 == value) return;

                var oldValue = _Shift2;
                _Shift2 = value;

                OnChanged(oldValue, value, "Shift2");
            }
        }
        private bool _Shift2;

        public bool Alt2
        {
            get { return _Alt2; }
            set
            {
                if (_Alt2 == value) return;

                var oldValue = _Alt2;
                _Alt2 = value;

                OnChanged(oldValue, value, "Alt2");
            }
        }
        private bool _Alt2;

        public Keys Key2
        {
            get { return _Key2; }
            set
            {
                if (_Key2 == value) return;

                var oldValue = _Key2;
                _Key2 = value;

                OnChanged(oldValue, value, "Key2");
            }
        }
        private Keys _Key2;
    }

    public enum KeyBindAction
    {
        None,

        [Description("Config Window")]
        ConfigWindow,
        [Description("Character Window")]
        CharacterWindow,
        [Description("Inventory Window")]
        InventoryWindow,
        [Description("Magic List Window")]
        MagicWindow,
        [Description("Magic Bar Window")]
        MagicBarWindow,
        [Description("Ranking Window")]
        RankingWindow,
        [Description("Game Store Window")]
        GameStoreWindow,
        [Description("Dungeon Finder Window")]
        DungeonFinderWindow,
        [Description("Companion Window")]
        CompanionWindow,
        [Description("Group Window")]
        GroupWindow,
        [Description("Auto Potion Window")]
        AutoPotionWindow,
        [Description("Storage Window")]
        StorageWindow,
        [Description("Block List Window")]
        BlockListWindow,
        [Description("Guild Window")]
        GuildWindow,
        [Description("Quest Log Window")]
        QuestLogWindow,
        [Description("Quest Tracker Window")]
        QuestTrackerWindow,
        [Description("Belt Window")]
        BeltWindow,
        [Description("Market Place Window")]
        MarketPlaceWindow,
        [Description("Mini Map Window")]
        MapMiniWindow,
        [Description("Big Map Window")]
        MapBigWindow,
        [Description("Mail Box Window")]
        MailBoxWindow,
        [Description("Mail Send Window")]
        MailSendWindow,
        [Description("Chat Options Window")]
        ChatOptionsWindow,
        [Description("Exit Game Window")]
        ExitGameWindow,


        [Description("Change Attack Mode")]
        ChangeAttackMode,
        [Description("Change Pet Mode")]
        ChangePetMode,

        [Description("Toggle Allow Group")]
        GroupAllowSwitch,
        [Description("Add Target To Group")]
        GroupTarget,

        [Description("Request Trade")]
        TradeRequest,
        [Description("Toggle Allow Trade")]
        TradeAllowSwitch,

        [Description("Pick Up Item")]
        ItemPickUp,

        [Description("Wedding Teleport")]
        PartnerTeleport,

        [Description("Toggle Mount")]
        MountToggle,
        [Description("Toggle Auto Run")]
        AutoRunToggle,
        [Description("Change Chat Mode")]
        ChangeChatMode,


        [Description("Use Belt Item 1")]
        UseBelt01,
        [Description("Use Belt Item 2")]
        UseBelt02,
        [Description("Use Belt Item 3")]
        UseBelt03,
        [Description("Use Belt Item 4")]
        UseBelt04,
        [Description("Use Belt Item 5")]
        UseBelt05,
        [Description("Use Belt Item 6")]
        UseBelt06,
        [Description("Use Belt Item 7")]
        UseBelt07,
        [Description("Use Belt Item 8")]
        UseBelt08,
        [Description("Use Belt Item 9")]
        UseBelt09,
        [Description("Use Belt Item 10")]
        UseBelt10,

        [Description("Spell Set 1")]
        SpellSet01,
        [Description("Spell Set 2")]
        SpellSet02,
        [Description("Spell Set 3")]
        SpellSet03,
        [Description("Spell Set 4")]
        SpellSet04,

        [Description("Use Spell 1")]
        SpellUse01,
        [Description("Use Spell 2")]
        SpellUse02,
        [Description("Use Spell 3")]
        SpellUse03,
        [Description("Use Spell 4")]
        SpellUse04,
        [Description("Use Spell 5")]
        SpellUse05,
        [Description("Use Spell 6")]
        SpellUse06,
        [Description("Use Spell 7")]
        SpellUse07,
        [Description("Use Spell 8")]
        SpellUse08,
        [Description("Use Spell 9")]
        SpellUse09,
        [Description("Use Spell 10")]
        SpellUse10,
        [Description("Use Spell 11")]
        SpellUse11,
        [Description("Use Spell 12")]
        SpellUse12,
        [Description("Use Spell 13")]
        SpellUse13,
        [Description("Use Spell 14")]
        SpellUse14,
        [Description("Use Spell 15")]
        SpellUse15,
        [Description("Use Spell 16")]
        SpellUse16,
        [Description("Use Spell 17")]
        SpellUse17,
        [Description("Use Spell 18")]
        SpellUse18,
        [Description("Use Spell 19")]
        SpellUse19,
        [Description("Use Spell 20")]
        SpellUse20,
        [Description("Use Spell 21")]
        SpellUse21,
        [Description("Use Spell 22")]
        SpellUse22,
        [Description("Use Spell 23")]
        SpellUse23,
        [Description("Use Spell 24")]
        SpellUse24,
        [Description("Toggle Item Lock")]
        ToggleItemLock,

        [Description("Fortune Window")]
        FortuneWindow,

        [Description("Currency Window")]
        CurrencyWindow,

        [Description("Filter Drop Window")]
        FilterDropWindow,

        [Description("Menu Window")]
        MenuWindow,
    }
}
