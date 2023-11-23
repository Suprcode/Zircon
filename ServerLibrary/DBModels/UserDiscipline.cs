using Library;
using Library.Network.ClientPackets;
using Library.SystemModels;
using MirDB;
using System.Collections.Generic;
using System.Linq;

namespace Server.DBModels
{
    [UserObject]
    public class UserDiscipline : DBObject
    {
        public DisciplineInfo Info
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
        private DisciplineInfo _Info;

        [Association("Discipline")]
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

        public long Experience
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
        private long _Experience;

        [Association("DisciplineMagics", true)]
        public DBBindingList<UserMagic> Magics { get; set; }

        protected override void OnDeleted()
        {
            for (int i = Magics.Count - 1; i >= 0; i--)
            {
                Magics[i].Delete();
            }

            Info = null;
            Character = null;

            base.OnDeleted();
        }

        public ClientUserDiscipline ToClientInfo()
        {
            return new ClientUserDiscipline
            {
                InfoIndex = Info.Index,
                Level = Level,
                Experience = Experience,
                Magics = Magics.Select(x => x.ToClientInfo()).ToList()
            };
        }
    }
}
