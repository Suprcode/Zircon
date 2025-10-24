using MirDB;

namespace Library.SystemModels
{
    public sealed class CompanionSpeech : DBObject
    {
        [Association("CompanionSpeeches")]
        public CompanionInfo Companion
        {
            get { return _Companion; }
            set
            {
                if (_Companion == value) return;

                var oldValue = _Companion;
                _Companion = value;

                OnChanged(oldValue, value, nameof(Companion));
            }
        }
        private CompanionInfo _Companion;

        public string Speech
        {
            get { return _Speech; }
            set
            {
                if (_Speech == value) return;

                var oldValue = _Speech;
                _Speech = value;

                OnChanged(oldValue, value, nameof(Speech));
            }
        }
        private string _Speech;
    }
}
