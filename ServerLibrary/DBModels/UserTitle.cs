using Library;
using Library.SystemModels;
using MirDB;
using System;

namespace Server.DBModels
{
    [UserObject]
    public class UserTitle : DBObject
    {
        public TitleInfo Info
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
        private TitleInfo _Info;

        [Association("Titles")]
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

        public DateTime DateEarned
        {
            get { return _DateEarned; }
            set
            {
                if (_DateEarned == value) return;

                var oldValue = _DateEarned;
                _DateEarned = value;

                OnChanged(oldValue, value, "DateEarned");
            }
        }
        private DateTime _DateEarned;

        public bool Active
        {
            get { return _Active; }
            set
            {
                if (_Active == value) return;

                var oldValue = _Active;
                _Active = value;

                OnChanged(oldValue, value, "Active");
            }
        }
        private bool _Active;

        protected override void OnDeleted()
        {
            Info = null;
            Character = null;

            base.OnDeleted();
        }

        public ClientUserTitle ToClientInfo()
        {
            return new ClientUserTitle
            {
                Index = Index,
                InfoIndex = Info.Index,

                DateEarned = DateEarned,
                Active = Active
            };
        }
    }
}
