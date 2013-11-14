using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Common.Settings.SettingsObjects
{
    [XmlType("TvSeriesPluginSettings")]
    public  class TvSeriesPluginSettings : SupportedPluginSettings
    {
        private string _seasonViewVerticalListItemThumbPath;
        private string _seasonViewHorizontalListItemThumbPath;
        private string _seasonViewCoverflowListItemThumbPath;
        private string _seriesViewVerticalListItemThumbPath;
        private string _seriesViewHorizontalListItemThumbPath;
        private string _seriesViewCoverflowListItemThumbPath;

        public string SeasonViewVerticalListItemThumbPath
        {
            get { return _seasonViewVerticalListItemThumbPath; }
            set { _seasonViewVerticalListItemThumbPath = value; }
        }

        public string SeasonViewHorizontalListItemThumbPath
        {
            get { return _seasonViewHorizontalListItemThumbPath; }
            set { _seasonViewHorizontalListItemThumbPath = value; }
        }

        public string SeasonViewCoverflowListItemThumbPath
        {
            get { return _seasonViewCoverflowListItemThumbPath; }
            set { _seasonViewCoverflowListItemThumbPath = value; }
        }



        public string SeriesViewVerticalListItemThumbPath
        {
            get { return _seriesViewVerticalListItemThumbPath; }
            set { _seriesViewVerticalListItemThumbPath = value; }
        }

        public string SeriesViewHorizontalListItemThumbPath
        {
            get { return _seriesViewHorizontalListItemThumbPath; }
            set { _seriesViewHorizontalListItemThumbPath = value; }
        }

        public string SeriesViewCoverflowListItemThumbPath
        {
            get { return _seriesViewCoverflowListItemThumbPath; }
            set { _seriesViewCoverflowListItemThumbPath = value; }
        }
    }
}
