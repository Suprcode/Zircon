using Library;
using Library.SystemModels;
using MirDB;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.DBModels
{
    [UserObject]
    public sealed class UserQuest : DBObject
    {
        public QuestInfo QuestInfo
        {
            get { return _QuestInfo; }
            set
            {
                if (_QuestInfo == value) return;

                var oldValue = _QuestInfo;
                _QuestInfo = value;

                OnChanged(oldValue, value, "QuestInfo");
            }
        }
        private QuestInfo _QuestInfo;

        [Association("Quests")]
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

        [Association("Quests")]
        public AccountInfo Account
        {
            get { return _Account; }
            set
            {
                if (_Account == value) return;

                var oldValue = _Account;
                _Account = value;

                OnChanged(oldValue, value, "Account");
            }
        }
        private AccountInfo _Account;

        public bool Completed
        {
            get { return _Completed; }
            set
            {
                if (_Completed == value) return;

                var oldValue = _Completed;
                _Completed = value;

                OnChanged(oldValue, value, "Completed");
            }
        }
        private bool _Completed;

        public int SelectedReward
        {
            get { return _SelectedReward; }
            set
            {
                if (_SelectedReward == value) return;

                var oldValue = _SelectedReward;
                _SelectedReward = value;

                OnChanged(oldValue, value, "SelectedReward");
            }
        }
        private int _SelectedReward;

        public bool Track
        {
            get { return _Track; }
            set
            {
                if (_Track == value) return;

                var oldValue = _Track;
                _Track = value;

                OnChanged(oldValue, value, "Track");
            }
        }
        private bool _Track;

        public DateTime DateTaken
        {
            get { return _DateTaken; }
            set
            {
                if (_DateTaken == value) return;

                var oldValue = _DateTaken;
                _DateTaken = value;

                OnChanged(oldValue, value, "DateTaken");
            }
        }
        private DateTime _DateTaken;

        public DateTime DateCompleted
        {
            get { return _DateCompleted; }
            set
            {
                if (_DateCompleted == value) return;

                var oldValue = _DateCompleted;
                _DateCompleted = value;

                OnChanged(oldValue, value, "DateCompleted");
            }
        }
        private DateTime _DateCompleted;


        [IgnoreProperty]
        public bool IsComplete => Tasks.Count == QuestInfo.Tasks.Count && Tasks.All(x => x.Completed);


        [Association("Tasks", true)]
        public DBBindingList<UserQuestTask> Tasks { get; set; }

        protected override void OnDeleted()
        {
            QuestInfo = null;
            Character = null;
            Account = null;

            for (int i = Tasks.Count - 1; i >= 0; i--)
                Tasks[i].Delete();

            base.OnDeleted();
        }

        public ClientUserQuest ToClientInfo()
        {
            return new ClientUserQuest
            {
                Index = Index,
                QuestIndex =  QuestInfo.Index,
                Completed = Completed,
                SelectedReward = SelectedReward,
                Track = Track,
                DateTaken = DateTaken,
                DateCompleted = DateCompleted,

                Tasks = Tasks.Select(x=> x.ToClientInfo()).ToList(),
            };
        }

        protected override void OnCreated()
        {
            base.OnCreated();

            Track = true;
        }
    }


    [UserObject]
    public sealed class UserQuestTask : DBObject
    {
        [Association("Tasks")]
        public UserQuest Quest
        {
            get { return _Quest; }
            set
            {
                if (_Quest == value) return;

                var oldValue = _Quest;
                _Quest = value;

                OnChanged(oldValue, value, "Quest");
            }
        }
        private UserQuest _Quest;

        public QuestTask Task
        {
            get { return _Task; }
            set
            {
                if (_Task == value) return;

                var oldValue = _Task;
                _Task = value;

                OnChanged(oldValue, value, "Task");
            }
        }
        private QuestTask _Task;
        
        public long Amount
        {
            get { return _Amount; }
            set
            {
                if (_Amount == value) return;

                var oldValue = _Amount;
                _Amount = value;

                OnChanged(oldValue, value, "Amount");
            }
        }
        private long _Amount;

        [IgnoreProperty]
        public bool Completed => Amount >= Task.Amount;

        protected override void OnDeleted()
        {
            Quest = null;
            Task = null;

            base.OnDeleted();
        }

        public ClientUserQuestTask ToClientInfo()
        {
            return new ClientUserQuestTask
            {
                Index = Index,

                TaskIndex = Task.Index,

                Amount = Amount
            };
        }

        public List<ItemObject> Objects = new List<ItemObject>();
    }
}
