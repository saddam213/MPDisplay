using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace Common.Settings
{
    [XmlType("GenericPluginSettings")]
    public class SupportedPluginSettings
    {
        [XmlAttribute("Plugin")]
        public SupportedPlugin PluginType { get; set; } = SupportedPlugin.None;

        [XmlArrayItem("WindowId")]
        public ObservableCollection<int> WindowIds { get; set; } = new ObservableCollection<int>();

        public string VerticalListItemThumb1Path { get; set; } = "ThumbnailImage";

        public string VerticalListItemThumb2Path { get; set; } = "";

        public string VerticalListItemThumb3Path { get; set; } = "";

        public string VerticalListItemLabel1Path { get; set; } = "Label";

        public string VerticalListItemLabel2Path { get; set; } = "Label2";

        public string VerticalListItemLabel3Path { get; set; } = "Label3";

        public string VerticalIconListItemThumb1Path { get; set; } = "ThumbnailImage";

        public string VerticalIconListItemThumb2Path { get; set; } = "";

        public string VerticalIconListItemThumb3Path { get; set; } = "";

        public string VerticalIconListItemLabel1Path { get; set; } = "Label";

        public string VerticalIconListItemLabel2Path { get; set; } = "Label2";

        public string VerticalIconListItemLabel3Path { get; set; } = "Label3";

        public string HorizontalListItemThumb1Path { get; set; } = "ThumbnailImage";

        public string HorizontalListItemThumb2Path { get; set; } = "";

        public string HorizontalListItemThumb3Path { get; set; } = "";

        public string HorizontalListItemLabel1Path { get; set; } = "Label";

        public string HorizontalListItemLabel2Path { get; set; } = "Label2";

        public string HorizontalListItemLabel3Path { get; set; } = "Label3";

        public string CoverflowListItemThumb1Path { get; set; } = "ThumbnailImage";

        public string CoverflowListItemThumb2Path { get; set; } = "";

        public string CoverflowListItemThumb3Path { get; set; } = "";

        public string CoverflowListItemLabel1Path { get; set; } = "Label";

        public string CoverflowListItemLabel2Path { get; set; } = "Label2";

        public string CoverflowListItemLabel3Path { get; set; } = "Label3";

        public string ListLayoutAlbumview { get; set; } = "Coverflow";

        public string ListLayoutCoverflow { get; set; } = "Coverflow";

        public string ListLayoutFilmstrip { get; set; } = "Horizontal";

        public string ListLayoutSmallIcons { get; set; } = "Horizontal";

        public string ListLayoutList { get; set; } = "Vertical";

        public string ListLayoutPlaylist { get; set; } = "Vertical";

        public string ListLayoutLargeIcons { get; set; } = "Horizontal";
    }

    public enum SupportedPlugin
    {
        // ReSharper disable InconsistentNaming
        None = -1,
        MyFilms = 7986,
        MovingPictures = 96742,
        MPTVSeries = 9811,
        mvCentral = 112011,
        OnlineVideos = 4755,
        MyAnime = 6101,
        Rockstar = 47286,
        //YoutubeFm,
        //PandoraMusicBox,
        //RadioTime,
        //Streamradio
        TuneIn = 25650
    }
}
