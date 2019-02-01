using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;
using MirDB;

namespace Server.DBModels
{
    [UserObject]
    public class UserConquestStats : DBObject
    {
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

        public DateTime WarStartDate
        {
            get { return _WarStartDate; }
            set
            {
                if (_WarStartDate == value) return;

                var oldValue = _WarStartDate;
                _WarStartDate = value;

                OnChanged(oldValue, value, "WarStartDate");
            }
        }
        private DateTime _WarStartDate;

        public string CastleName
        {
            get { return _CastleName; }
            set
            {
                if (_CastleName == value) return;

                var oldValue = _CastleName;
                _CastleName = value;

                OnChanged(oldValue, value, "CastleName");
            }
        }
        private string _CastleName;
        

        public string CharacterName
        {
            get { return _CharacterName; }
            set
            {
                if (_CharacterName == value) return;

                var oldValue = _CharacterName;
                _CharacterName = value;

                OnChanged(oldValue, value, "CharacterName");
            }
        }
        private string _CharacterName;

        public string GuildName
        {
            get { return _GuildName; }
            set
            {
                if (_GuildName == value) return;

                var oldValue = _GuildName;
                _GuildName = value;

                OnChanged(oldValue, value, "GuildName");
            }
        }
        private string _GuildName;

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

        public MirClass Class
        {
            get { return _Class; }
            set
            {
                if (_Class == value) return;

                var oldValue = _Class;
                _Class = value;

                OnChanged(oldValue, value, "Class");
            }
        }
        private MirClass _Class;
        


        public int BossDamageTaken
        {
            get { return _BossDamageTaken; }
            set
            {
                if (_BossDamageTaken == value) return;

                var oldValue = _BossDamageTaken;
                _BossDamageTaken = value;

                OnChanged(oldValue, value, "BossDamageTaken");
            }
        }
        private int _BossDamageTaken;

        public int BossDamageDealt
        {
            get { return _BossDamageDealt; }
            set
            {
                if (_BossDamageDealt == value) return;

                var oldValue = _BossDamageDealt;
                _BossDamageDealt = value;

                OnChanged(oldValue, value, "BossDamageDealt");
            }
        }
        private int _BossDamageDealt;

        public int BossDeathCount
        {
            get { return _BossDeathCount; }
            set
            {
                if (_BossDeathCount == value) return;

                var oldValue = _BossDeathCount;
                _BossDeathCount = value;

                OnChanged(oldValue, value, "BossDeathCount");
            }
        }
        private int _BossDeathCount;

        public int BossKillCount
        {
            get { return _BossKillCount; }
            set
            {
                if (_BossKillCount == value) return;

                var oldValue = _BossKillCount;
                _BossKillCount = value;

                OnChanged(oldValue, value, "BossKillCount");
            }
        }
        private int _BossKillCount;
        
        public int PvPDamageTaken
        {
            get { return _PvPDamageTaken; }
            set
            {
                if (_PvPDamageTaken == value) return;

                var oldValue = _PvPDamageTaken;
                _PvPDamageTaken = value;

                OnChanged(oldValue, value, "PvPDamageTaken");
            }
        }
        private int _PvPDamageTaken;

        public int PvPDamageDealt
        {
            get { return _PvPDamageDealt; }
            set
            {
                if (_PvPDamageDealt == value) return;

                var oldValue = _PvPDamageDealt;
                _PvPDamageDealt = value;

                OnChanged(oldValue, value, "PvPDamageDealt");
            }
        }
        private int _PvPDamageDealt;

        public int PvPKillCount
        {
            get { return _PvPKillCount; }
            set
            {
                if (_PvPKillCount == value) return;

                var oldValue = _PvPKillCount;
                _PvPKillCount = value;

                OnChanged(oldValue, value, "PvPKillCount");
            }
        }
        private int _PvPKillCount;

        public int PvPDeathCount
        {
            get { return _PvPDeathCount; }
            set
            {
                if (_PvPDeathCount == value) return;

                var oldValue = _PvPDeathCount;
                _PvPDeathCount = value;

                OnChanged(oldValue, value, "PvPDeathCount");
            }
        }
        private int _PvPDeathCount;
    }
}
