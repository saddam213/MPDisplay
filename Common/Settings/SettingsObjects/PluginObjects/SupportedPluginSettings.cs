using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Common.Settings.SettingsObjects
{
    public class SupportedPluginSettings
    {
        private SupportedPlugin _pluginType = SupportedPlugin.None;
        private ObservableCollection<int> _windowIds = new ObservableCollection<int>();
        private string _verticalListItemThumbPath = "ThumbnailImage";
        private string _horizontalListItemThumbPath = "ThumbnailImage";
        private string _coverflowListItemThumbPath = "ThumbnailImage";
      


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



        public string VerticalListItemThumbPath
        {
            get { return _verticalListItemThumbPath; }
            set { _verticalListItemThumbPath = value; }
        }

        public string HorizontalListItemThumbPath
        {
            get { return _horizontalListItemThumbPath; }
            set { _horizontalListItemThumbPath = value; }
        }

        public string CoverflowListItemThumbPath
        {
            get { return _coverflowListItemThumbPath; }
            set { _coverflowListItemThumbPath = value; }
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
