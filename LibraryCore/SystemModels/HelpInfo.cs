using MirDB;

namespace Library.SystemModels
{
    public sealed class HelpInfo : DBObject
    {
        [IsIdentity]
        public string Title
        {
            get { return _Title; }
            set
            {
                if (_Title == value) return;

                var oldValue = _Title;
                _Title = value;

                OnChanged(oldValue, value, "Title");
            }
        }
        private string _Title;

        public int Order
        {
            get { return _Order; }
            set
            {
                if (_Order == value) return;

                var oldValue = _Order;
                _Order = value;

                OnChanged(oldValue, value, "Order");
            }
        }
        private int _Order;

        public string Description
        {
            get { return _Description; }
            set
            {
                if (_Description == value) return;

                var oldValue = _Description;
                _Description = value;

                OnChanged(oldValue, value, "Description");
            }
        }
        private string _Description;

        [Association("Pages", true)]
        public DBBindingList<HelpPageInfo> Pages { get; set; }
    }

    public sealed class HelpPageInfo : DBObject
    {
        [Association("Pages")]
        public HelpInfo Help
        {
            get { return _Help; }
            set
            {
                if (_Help == value) return;

                var oldValue = _Help;
                _Help = value;

                OnChanged(oldValue, value, "Help");
            }
        }
        private HelpInfo _Help;

        [IsIdentity]
        public string Title
        {
            get { return _Title; }
            set
            {
                if (_Title == value) return;

                var oldValue = _Title;
                _Title = value;

                OnChanged(oldValue, value, "Title");
            }
        }
        private string _Title;

        public int Order
        {
            get { return _Order; }
            set
            {
                if (_Order == value) return;

                var oldValue = _Order;
                _Order = value;

                OnChanged(oldValue, value, "Order");
            }
        }
        private int _Order;

        [Association("Sections", true)]
        public DBBindingList<HelpSectionInfo> Sections { get; set; }
    }

    public sealed class HelpSectionInfo : DBObject
    {
        [Association("Sections")]
        public HelpPageInfo Page
        {
            get { return _Page; }
            set
            {
                if (_Page == value) return;

                var oldValue = _Page;
                _Page = value;

                OnChanged(oldValue, value, "Page");
            }
        }
        private HelpPageInfo _Page;

        [IsIdentity]
        public string Title
        {
            get { return _Title; }
            set
            {
                if (_Title == value) return;

                var oldValue = _Title;
                _Title = value;

                OnChanged(oldValue, value, "Title");
            }
        }
        private string _Title;

        public int Order
        {
            get { return _Order; }
            set
            {
                if (_Order == value) return;

                var oldValue = _Order;
                _Order = value;

                OnChanged(oldValue, value, "Order");
            }
        }
        private int _Order;

        [IsIdentity]
        public string Content
        {
            get { return _Content; }
            set
            {
                if (_Content == value) return;

                var oldValue = _Content;
                _Content = value;

                OnChanged(oldValue, value, "Content");
            }
        }
        private string _Content;
    }
}
