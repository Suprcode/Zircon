using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MirDB;

namespace Server.DBModels
{

    [UserObject]
    public sealed class GuildWarInfo : DBObject
    {
        public GuildInfo Guild1
        {
            get { return _Guild1; }
            set
            {
                if (_Guild1 == value) return;

                var oldValue = _Guild1;
                _Guild1 = value;

                OnChanged(oldValue, value, "Guild1");
            }
        }
        private GuildInfo _Guild1;

        public GuildInfo Guild2
        {
            get { return _Guild2; }
            set
            {
                if (_Guild2 == value) return;

                var oldValue = _Guild2;
                _Guild2 = value;

                OnChanged(oldValue, value, "Guild2");
            }
        }
        private GuildInfo _Guild2;
        
        public TimeSpan Duration
        {
            get { return _Duration; }
            set
            {
                if (_Duration == value) return;

                var oldValue = _Duration;
                _Duration = value;

                OnChanged(oldValue, value, "Duration");
            }
        }
        private TimeSpan _Duration;
    }
}
