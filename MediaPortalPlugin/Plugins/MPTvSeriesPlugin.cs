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

        public TvSeriesPluginSettings CustomSettings => (Settings as TvSeriesPluginSettings);

        public override bool IsPlaying(string filename, APIPlaybackType playtype)
        {
            if (!IsEnabled) return false;
            var selectedSeries = ReflectionHelper.GetStaticField(PluginWindow, "m_SelectedEpisode", null);
            if (selectedSeries == null) return false;
            var episodeFilename = ReflectionHelper.GetPropertyValue<object>(selectedSeries, "Item", null, new object[] { "EpisodeFilename" });
            return episodeFilename != null && filename.Equals(episodeFilename.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        public override bool MustResendListOnLayoutChange()
        {
            return true;
        }

        public override APIPlaybackType PlayType => APIPlaybackType.MPTVSeries;

        public override APIImage GetListItemImage1(GUIListItem item, APIListLayout layout)
        {
            var filename = string.Empty;
            if (Settings != null && item != null)
            {
                var view = ReflectionHelper.GetStaticField<object>(PluginWindow, "CurrentViewLevel", null);
                var isSeason = view != null && view.ToString() == "Season";

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

            return !image.IsEmpty ? image : base.GetListItemImage1(item, layout);
        }
    }
}