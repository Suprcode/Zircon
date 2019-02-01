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
    public sealed class ChatTabControlSetting : DBObject
    {
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

        public ChatTabPageSetting SelectedPage
        {
            get { return _SelectedPage; }
            set
            {
                if (_SelectedPage == value) return;

                var oldValue = _SelectedPage;
                _SelectedPage = value;

                OnChanged(oldValue, value, "SelectedPage");
            }
        }
        private ChatTabPageSetting _SelectedPage;
        
        
        [Association("Controls", true)]
        public DBBindingList<ChatTabPageSetting> Controls { get; set; }
    }


}
