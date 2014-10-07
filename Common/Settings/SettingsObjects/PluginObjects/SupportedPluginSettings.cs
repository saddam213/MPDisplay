using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Common.Settings.SettingsObjects
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
    }

    public enum SupportedPlugin
    {
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
        RadioTime = 25650,
    }
}
