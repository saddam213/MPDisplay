namespace MPDisplay.Common.Controls
{
    public class AutoCompleteEntry
    {
        private string[] _keywordStrings;

        public string[] KeywordStrings => _keywordStrings ?? (_keywordStrings = new[] {DisplayName});

        public string DisplayName { get; set; }

        public AutoCompleteEntry(string name, params string[] keywords)
        {
            DisplayName = name;
            _keywordStrings = keywords;
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}
