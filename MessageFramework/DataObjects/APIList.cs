using System.Collections.Generic;

namespace MessageFramework.DataObjects
{
    public class APIList
    {
        public int BatchId { get; set; }
        public int BatchNumber { get; set; }
        public int BatchCount { get; set; }
        public APIListType ListType { get; set; }
        public List<APIListItem> ListItems { get; set; }
        public APIListLayout ListLayout { get; set; }
    }

    public class APIListItem
    {
        public int Index { get; set; }
        public string Label { get; set; }
        public string Label2 { get; set; }
        public string Label3 { get; set; }
        public APIImage Image { get; set; }
        public APIImage Image2 { get; set; }
        public APIImage Image3 { get; set; }
    }

    public enum APIListType
    {
        None,
        List,
        Menu,
        GroupMenu,
        DialogList
    }

    public enum APIListLayout
    {
        Vertical,
        VerticalIcon,
        Horizontal,
        CoverFlow
    }
}
