using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.SystemModels;
using MirDB;

namespace Server.DBModels
{
    [UserObject]
    public sealed class UserConquest : DBObject
    {
        [Association("Conquest")]
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

        public CastleInfo Castle
        {
            get { return _Castle; }
            set
            {
                if (_Castle == value) return;

                var oldValue = _Castle;
                _Castle = value;

                OnChanged(oldValue, value, "Castle");
            }
        }
        private CastleInfo _Castle;
        
        public DateTime WarDate
        {
            get { return _WarDate; }
            set
            {
                if (_WarDate == value) return;

                var oldValue = _WarDate;
                _WarDate = value;

                OnChanged(oldValue, value, "WarDate");
            }
        }
        private DateTime _WarDate;

        protected override void OnLoaded()
        {
            base.OnLoaded();
        }


        protected override void OnDeleted()
        {
            Guild = null;
            Castle = null;

            base.OnDeleted();
        }

    }
}
