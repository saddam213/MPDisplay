using System;
using Common.Helpers;
using Common.Settings;
using MediaPortal.GUI.Library;
using MessageFramework.DataObjects;

namespace MediaPortalPlugin.Plugins
{
    public class MpTvSeriesPlugin : PluginHelper
    {
        public MpTvSeriesPlugin(GUIWindow pluginindow, SupportedPluginSettings settings)
            : base(pluginindow, settings)
        {
        }

        public TvSeriesPluginSettings CustomSettings
        {
            get { return (Settings as TvSeriesPluginSettings); }
        }

        public override bool IsPlaying(string filename, APIPlaybackType playtype)
        {
            if (IsEnabled)
            {
                var selectedSeries = ReflectionHelper.GetStaticField(PluginWindow, "m_SelectedEpisode", null);
                if (selectedSeries != null)
                {
                    var episodeFilename = ReflectionHelper.GetPropertyValue<object>(selectedSeries, "Item", null, new object[] { "EpisodeFilename" });
                    if (episodeFilename != null)
                    {
                        return filename.Equals(episodeFilename.ToString(), StringComparison.OrdinalIgnoreCase);
                    }
                }
            }
            return false;
        }

        public override bool MustResendListOnLayoutChange()
        {
            return true;
        }

        public override APIPlaybackType PlayType
        {
            get { return APIPlaybackType.MPTVSeries; }
        }

        public override APIImage GetListItemImage1(GUIListItem item, APIListLayout layout)
        {
            string filename = string.Empty;
            if (Settings != null && item != null)
            {
                var view = ReflectionHelper.GetStaticField<object>(PluginWindow, "CurrentViewLevel", null);
                bool isSeason = view != null && view.ToString() == "Season";

                switch (layout)
                {
                    case APIListLayout.Vertical:
                        filename = isSeason ? ReflectionHelper.GetPropertyPath(item, CustomSettings.SeasonViewVerticalListItemThumbPath, string.Empty)
                                            : ReflectionHelper.GetPropertyPath(item, CustomSettings.SeriesViewVerticalListItemThumbPath, string.Empty);
                        break;
                    case APIListLayout.VerticalIcon:
                        filename = isSeason ? ReflectionHelper.GetPropertyPath(item, CustomSettings.SeasonViewVerticalIconListItemThumbPath, string.Empty)
                                            : ReflectionHelper.GetPropertyPath(item, CustomSettings.SeriesViewVerticalIconListItemThumbPath, string.Empty);
                        break;
                    case APIListLayout.Horizontal:
                        filename = isSeason ? ReflectionHelper.GetPropertyPath(item, CustomSettings.SeasonViewHorizontalListItemThumbPath, string.Empty)
                                            : ReflectionHelper.GetPropertyPath(item, CustomSettings.SeriesViewHorizontalListItemThumbPath, string.Empty);
                        break;
                    case APIListLayout.CoverFlow:
                        filename = isSeason ? ReflectionHelper.GetPropertyPath(item, CustomSettings.SeasonViewCoverflowListItemThumbPath, string.Empty)
                                            : ReflectionHelper.GetPropertyPath(item, CustomSettings.SeriesViewCoverflowListItemThumbPath, string.Empty);
                        break;
                }
            }

            var image = ImageHelper.CreateImage(filename);
            if (!image.IsEmpty)
            {
                return image;
            }
            return base.GetListItemImage1(item, layout);
        }
    }
}