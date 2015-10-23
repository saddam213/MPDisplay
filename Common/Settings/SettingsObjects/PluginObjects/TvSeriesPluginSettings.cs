using System.Xml.Serialization;

namespace Common.Settings
{
    [XmlType("TvSeriesPluginSettings")]
    public  class TvSeriesPluginSettings : SupportedPluginSettings
    {
        public string SeasonViewVerticalListItemThumbPath { get; set; }

        public string SeasonViewVerticalIconListItemThumbPath { get; set; }

        public string SeasonViewHorizontalListItemThumbPath { get; set; }

        public string SeasonViewCoverflowListItemThumbPath { get; set; }

        public string SeriesViewVerticalListItemThumbPath { get; set; }

        public string SeriesViewVerticalIconListItemThumbPath { get; set; }

        public string SeriesViewHorizontalListItemThumbPath { get; set; }

        public string SeriesViewCoverflowListItemThumbPath { get; set; }
    }
}
