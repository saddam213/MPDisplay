using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace Common.Settings
{
    [XmlType("GenericPluginSettings")]
    public class SupportedPluginSettings
    {
        private SupportedPlugin _pluginType = SupportedPlugin.None;
        private ObservableCollection<int> _windowIds = new ObservableCollection<int>();

        private string _verticalListItemThumb1Path = "ThumbnailImage";
        private string _verticalListItemThumb2Path = "";
        private string _verticalListItemThumb3Path = "";
        private string _verticalListItemLabel1Path = "Label";
        private string _verticalListItemLabel2Path = "Label2";
        private string _verticalListItemLabel3Path = "Label3";

        private string _verticalIconListItemThumb1Path = "ThumbnailImage";
        private string _verticalIconListItemThumb2Path = "";
        private string _verticalIconListItemThumb3Path = "";
        private string _verticalIconListItemLabel1Path = "Label";
        private string _verticalIconListItemLabel2Path = "Label2";
        private string _verticalIconListItemLabel3Path = "Label3";

        private string _horizontalListItemThumb1Path = "ThumbnailImage";
        private string _horizontalListItemThumb2Path = "";
        private string _horizontalListItemThumb3Path = "";
        private string _horizontalListItemLabel1Path = "Label";
        private string _horizontalListItemLabel2Path = "Label2";
        private string _horizontalListItemLabel3Path = "Label3";

        private string _coverflowListItemThumb1Path = "ThumbnailImage";
        private string _coverflowListItemThumb2Path = "";
        private string _coverflowListItemThumb3Path = "";
        private string _coverflowListItemLabel1Path = "Label";
        private string _coverflowListItemLabel2Path = "Label2";
        private string _coverflowListItemLabel3Path = "Label3";

        private string _listLayoutAlbumview = "Coverflow";
        private string _listLayoutCoverflow = "Coverflow";
        private string _listLayoutFilmstrip = "Horizontal";
        private string _listLayoutSmallIcons = "Horizontal";
        private string _listLayoutList = "Vertical";
        private string _listLayoutPlaylist = "Vertical";
        private string _listLayoutLargeIcons = "Horizontal";

        [XmlAttribute("Plugin")]
        public SupportedPlugin PluginType
        {
            get { return _pluginType; }
            set { _pluginType = value; }
        }
     
        [XmlArrayItem("WindowId")]
        public ObservableCollection<int> WindowIds
        {
            get { return _windowIds; }
            set { _windowIds = value; }
        }

        public string VerticalListItemThumb1Path
        {
            get { return _verticalListItemThumb1Path; }
            set { _verticalListItemThumb1Path = value; }
        }

        public string VerticalListItemThumb2Path
        {
            get { return _verticalListItemThumb2Path; }
            set { _verticalListItemThumb2Path = value; }
        }

        public string VerticalListItemThumb3Path
        {
            get { return _verticalListItemThumb3Path; }
            set { _verticalListItemThumb3Path = value; }
        }

        public string VerticalListItemLabel1Path
        {
            get { return _verticalListItemLabel1Path; }
            set { _verticalListItemLabel1Path = value; }
        }

        public string VerticalListItemLabel2Path
        {
            get { return _verticalListItemLabel2Path; }
            set { _verticalListItemLabel2Path = value; }
        }

        public string VerticalListItemLabel3Path
        {
            get { return _verticalListItemLabel3Path; }
            set { _verticalListItemLabel3Path = value; }
        }

        public string VerticalIconListItemThumb1Path
        {
            get { return _verticalIconListItemThumb1Path; }
            set { _verticalIconListItemThumb1Path = value; }
        }

        public string VerticalIconListItemThumb2Path
        {
            get { return _verticalIconListItemThumb2Path; }
            set { _verticalIconListItemThumb2Path = value; }
        }

        public string VerticalIconListItemThumb3Path
        {
            get { return _verticalIconListItemThumb3Path; }
            set { _verticalIconListItemThumb3Path = value; }
        }

        public string VerticalIconListItemLabel1Path
        {
            get { return _verticalIconListItemLabel1Path; }
            set { _verticalIconListItemLabel1Path = value; }
        }

        public string VerticalIconListItemLabel2Path
        {
            get { return _verticalIconListItemLabel2Path; }
            set { _verticalIconListItemLabel2Path = value; }
        }

        public string VerticalIconListItemLabel3Path
        {
            get { return _verticalIconListItemLabel3Path; }
            set { _verticalIconListItemLabel3Path = value; }
        }

        public string HorizontalListItemThumb1Path
        {
            get { return _horizontalListItemThumb1Path; }
            set { _horizontalListItemThumb1Path = value; }
        }

        public string HorizontalListItemThumb2Path
        {
            get { return _horizontalListItemThumb2Path; }
            set { _horizontalListItemThumb2Path = value; }
        }

        public string HorizontalListItemThumb3Path
        {
            get { return _horizontalListItemThumb3Path; }
            set { _horizontalListItemThumb3Path = value; }
        }

        public string HorizontalListItemLabel1Path
        {
            get { return _horizontalListItemLabel1Path; }
            set { _horizontalListItemLabel1Path = value; }
        }

        public string HorizontalListItemLabel2Path
        {
            get { return _horizontalListItemLabel2Path; }
            set { _horizontalListItemLabel2Path = value; }
        }

        public string HorizontalListItemLabel3Path
        {
            get { return _horizontalListItemLabel3Path; }
            set { _horizontalListItemLabel3Path = value; }
        }

        public string CoverflowListItemThumb1Path
        {
            get { return _coverflowListItemThumb1Path; }
            set { _coverflowListItemThumb1Path = value; }
        }

        public string CoverflowListItemThumb2Path
        {
            get { return _coverflowListItemThumb2Path; }
            set { _coverflowListItemThumb2Path = value; }
        }

        public string CoverflowListItemThumb3Path
        {
            get { return _coverflowListItemThumb3Path; }
            set { _coverflowListItemThumb3Path = value; }
        }

        public string CoverflowListItemLabel1Path
        {
            get { return _coverflowListItemLabel1Path; }
            set { _coverflowListItemLabel1Path = value; }
        }

        public string CoverflowListItemLabel2Path
        {
            get { return _coverflowListItemLabel2Path; }
            set { _coverflowListItemLabel2Path = value; }
        }

        public string CoverflowListItemLabel3Path
        {
            get { return _coverflowListItemLabel3Path; }
            set { _coverflowListItemLabel3Path = value; }
        }

        public string ListLayoutAlbumview
        {
            get { return _listLayoutAlbumview; }
            set { _listLayoutAlbumview = value; }
        }

        public string ListLayoutCoverflow
        {
            get { return _listLayoutCoverflow; }
            set { _listLayoutCoverflow = value; }
        }

        public string ListLayoutFilmstrip
        {
            get { return _listLayoutFilmstrip; }
            set { _listLayoutFilmstrip = value; }
        }

        public string ListLayoutSmallIcons
        {
            get { return _listLayoutSmallIcons; }
            set { _listLayoutSmallIcons = value; }
        }

        public string ListLayoutList
        {
            get { return _listLayoutList; }
            set { _listLayoutList = value; }
        }

        public string ListLayoutPlaylist
        {
            get { return _listLayoutPlaylist; }
            set { _listLayoutPlaylist = value; }
        }

        public string ListLayoutLargeIcons
        {
            get { return _listLayoutLargeIcons; }
            set { _listLayoutLargeIcons = value; }
        }
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
