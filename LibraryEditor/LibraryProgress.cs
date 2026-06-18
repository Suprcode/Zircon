namespace LibraryEditor
{
    public sealed class LibraryProgress
    {
        public string Message { get; set; }
        public int Value { get; set; }
        public int Maximum { get; set; }
        public bool IsMarquee { get; set; }
        public string CountText { get; set; }
        public int OverallValue { get; set; }
        public int OverallMaximum { get; set; }
        public string OverallText { get; set; }
        public int GroupValue { get; set; }
        public int GroupMaximum { get; set; }
        public string GroupText { get; set; }

        public LibraryProgress(string message, int value = 0, int maximum = 0, bool isMarquee = false)
        {
            Message = message;
            Value = value;
            Maximum = maximum;
            IsMarquee = isMarquee;
        }
    }
}
