using MirDB;

namespace Library.SystemModels
{
    public sealed class TitleInfo : DBObject
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

        public string Category
        {
            get { return _Category; }
            set
            {
                if (_Category == value) return;

                var oldValue = _Category;
                _Category = value;

                OnChanged(oldValue, value, "Category");
            }
        }
        private string _Category;

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

        public ItemInfo Item
        {
            get { return _Item; }
            set
            {
                if (_Item == value) return;

                var oldValue = _Item;
                _Item = value;

                OnChanged(oldValue, value, "Item");
            }
        }
        private ItemInfo _Item;
    }
}
