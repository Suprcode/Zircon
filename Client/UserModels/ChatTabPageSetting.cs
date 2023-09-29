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
    public sealed class ChatTabPageSetting : DBObject
    {
        [Association("Controls")]
        public ChatTabControlSetting Parent
        {
            get { return _Parent; }
            set
            {
                if (_Parent == value) return;

                var oldValue = _Parent;
                _Parent = value;

                OnChanged(oldValue, value, "Parent");
            }
        }
        private ChatTabControlSetting _Parent;
        
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

        public bool Transparent
        {
            get { return _Transparent; }
            set
            {
                if (_Transparent == value) return;

                var oldValue = _Transparent;
                _Transparent = value;

                OnChanged(oldValue, value, "Transparent");
            }
        }
        private bool _Transparent;

        public bool Alert
        {
            get { return _Alert; }
            set
            {
                if (_Alert == value) return;

                var oldValue = _Alert;
                _Alert = value;

                OnChanged(oldValue, value, "Alert");
            }
        }
        private bool _Alert;


        public bool HideTab
        {
            get { return _HideTab; }
            set
            {
                if (_HideTab == value) return;

                var oldValue = _HideTab;
                _HideTab = value;

                OnChanged(oldValue, value, "HideTab");
            }
        }
        private bool _HideTab;

        public bool ReverseList
        {
            get { return _ReverseList; }
            set
            {
                if (_ReverseList == value) return;

                var oldValue = _ReverseList;
                _ReverseList = value;

                OnChanged(oldValue, value, "ReverseList");
            }
        }
        private bool _ReverseList;

        public bool CleanUp
        {
            get { return _CleanUp; }
            set
            {
                if (_CleanUp == value) return;

                var oldValue = _CleanUp;
                _CleanUp = value;

                OnChanged(oldValue, value, "CleanUp");
            }
        }
        private bool _CleanUp;

        public bool FadeOut
        {
            get { return _FadeOut; }
            set
            {
                if (_FadeOut == value) return;

                var oldValue = _FadeOut;
                _FadeOut = value;

                OnChanged(oldValue, value, "FadeOut");
            }
        }
        private bool _FadeOut;

        public bool LocalChat
        {
            get { return _LocalChat; }
            set
            {
                if (_LocalChat == value) return;

                var oldValue = _LocalChat;
                _LocalChat = value;

                OnChanged(oldValue, value, "LocalChat");
            }
        }
        private bool _LocalChat;

        public bool WhisperChat
        {
            get { return _WhisperChat; }
            set
            {
                if (_WhisperChat == value) return;

                var oldValue = _WhisperChat;
                _WhisperChat = value;

                OnChanged(oldValue, value, "WhisperChat");
            }
        }
        private bool _WhisperChat;

        public bool GroupChat
        {
            get { return _GroupChat; }
            set
            {
                if (_GroupChat == value) return;

                var oldValue = _GroupChat;
                _GroupChat = value;

                OnChanged(oldValue, value, "GroupChat");
            }
        }
        private bool _GroupChat;

        public bool GuildChat
        {
            get { return _GuildChat; }
            set
            {
                if (_GuildChat == value) return;

                var oldValue = _GuildChat;
                _GuildChat = value;

                OnChanged(oldValue, value, "GuildChat");
            }
        }
        private bool _GuildChat;

        public bool ShoutChat
        {
            get { return _ShoutChat; }
            set
            {
                if (_ShoutChat == value) return;

                var oldValue = _ShoutChat;
                _ShoutChat = value;

                OnChanged(oldValue, value, "ShoutChat");
            }
        }
        private bool _ShoutChat;

        public bool GlobalChat
        {
            get { return _GlobalChat; }
            set
            {
                if (_GlobalChat == value) return;

                var oldValue = _GlobalChat;
                _GlobalChat = value;

                OnChanged(oldValue, value, "GlobalChat");
            }
        }
        private bool _GlobalChat;
        
        public bool ObserverChat
        {
            get { return _ObserverChat; }
            set
            {
                if (_ObserverChat == value) return;

                var oldValue = _ObserverChat;
                _ObserverChat = value;

                OnChanged(oldValue, value, "ObserverChat");
            }
        }
        private bool _ObserverChat;

        public bool HintChat
        {
            get { return _HintChat; }
            set
            {
                if (_HintChat == value) return;

                var oldValue = _HintChat;
                _HintChat = value;

                OnChanged(oldValue, value, "HintChat");
            }
        }
        private bool _HintChat;

        public bool SystemChat
        {
            get { return _SystemChat; }
            set
            {
                if (_SystemChat == value) return;

                var oldValue = _SystemChat;
                _SystemChat = value;

                OnChanged(oldValue, value, "SystemChat");
            }
        }
        private bool _SystemChat;

        public bool GainsChat
        {
            get { return _GainsChat; }
            set
            {
                if (_GainsChat == value) return;

                var oldValue = _GainsChat;
                _GainsChat = value;

                OnChanged(oldValue, value, "GainsChat");
            }
        }
        private bool _GainsChat;
    }
}
