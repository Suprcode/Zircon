using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MirDB;

namespace Client.UserModels
{
    [UserObject]
    public class WindowSetting : DBObject
    {
        public WindowType Window
        {
            get { return _Window; }
            set
            {
                if (_Window == value) return;

                var oldValue = _Window;
                _Window = value;

                OnChanged(oldValue, value, "Window");
            }
        }
        private WindowType _Window;

        public Size Resolution
        {
            get { return _Resolution; }
            set
            {
                if (_Resolution == value) return;

                var oldValue = _Resolution;
                _Resolution = value;

                OnChanged(oldValue, value, "Resolution");
            }
        }
        private Size _Resolution;

        public bool Visible
        {
            get { return _Visible; }
            set
            {
                if (_Visible == value) return;

                var oldValue = _Visible;
                _Visible = value;

                OnChanged(oldValue, value, "Visible");
            }
        }
        private bool _Visible;

        public Point Location
        {
            get { return _Location; }
            set
            {
                if (_Location == value) return;

                var oldValue = _Location;
                _Location = value;

                OnChanged(oldValue, value, "Location");
            }
        }
        private Point _Location;

        public Size Size
        {
            get { return _Size; }
            set
            {
                if (_Size == value) return;

                var oldValue = _Size;
                _Size = value;

                OnChanged(oldValue, value, "Size");
            }
        }
        private Size _Size;
        
        public int Extra
        {
            get { return _Extra; }
            set
            {
                if (_Extra == value) return;

                var oldValue = _Extra;
                _Extra = value;

                OnChanged(oldValue, value, "Extra");
            }
        }
        private int _Extra;

        public int Extra2
        {
            get { return _Extra2; }
            set
            {
                if (_Extra2 == value) return;

                var oldValue = _Extra2;
                _Extra2 = value;

                OnChanged(oldValue, value, "Extra2");
            }
        }
        private int _Extra2;
    }


    public enum WindowType
    {
        None,
        ConfigBox,
        CaptionBox,
        InventoryBox,
        CharacterBox,
        ExitBox,
        ChatTextBox,
        BeltBox,
        ChatOptionsBox,
        MiniMapBox,
        GroupBox,
        BuffBox,
        StorageBox,
        AutoPotionBox,
        InspectBox,
        RankingBox,
        MarketPlaceBox,
        DungeonFinderBox,
        MailBox,
        ReadMailBox,
        SendMailBox,
        TradeBox,
        GuildBox,
        GuildMemberBox,
        QuestBox,
        QuestTrackerBox,
        CompanionBox,
        CompanionFilterBox,
        BlockBox,
        MonsterBox,
        MagicBox,
        MagicBarBox,
        MessageBox,
        ItemAmountBox,
        InputWindow,
        FilterDropBox,
        CommunicationBox,
        FishingBox,
        MenuBox
    }
}
