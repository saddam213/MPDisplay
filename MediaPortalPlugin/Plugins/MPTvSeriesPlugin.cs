using Common.Helpers;
using Common.Settings.SettingsObjects;
using MediaPortal.GUI.Library;
using MediaPortalPlugin.InfoManagers;
using MessageFramework.DataObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MediaPortalPlugin.PluginHelpers
{
    public class MPTvSeriesPlugin : PluginHelper
    {
        public MPTvSeriesPlugin(GUIWindow pluginindow, SupportedPluginSettings settings)
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
                        filename = isSeason ? ReflectionHelper.GetPropertyPath<string>(item, CustomSettings.SeasonViewVerticalListItemThumbPath, string.Empty)
                                            : ReflectionHelper.GetPropertyPath<string>(item, CustomSettings.SeriesViewVerticalListItemThumbPath, string.Empty);
                        break;
                    case APIListLayout.VerticalIcon:
                        filename = isSeason ? ReflectionHelper.GetPropertyPath<string>(item, CustomSettings.SeasonViewVerticalIconListItemThumbPath, string.Empty)
                                            : ReflectionHelper.GetPropertyPath<string>(item, CustomSettings.SeriesViewVerticalIconListItemThumbPath, string.Empty);
                        break;
                    case APIListLayout.Horizontal:
                        filename = isSeason ? ReflectionHelper.GetPropertyPath<string>(item, CustomSettings.SeasonViewHorizontalListItemThumbPath, string.Empty)
                                            : ReflectionHelper.GetPropertyPath<string>(item, CustomSettings.SeriesViewHorizontalListItemThumbPath, string.Empty);
                        break;
                    case APIListLayout.CoverFlow:
                        filename = isSeason ? ReflectionHelper.GetPropertyPath<string>(item, CustomSettings.SeasonViewCoverflowListItemThumbPath, string.Empty)
                                            : ReflectionHelper.GetPropertyPath<string>(item, CustomSettings.SeriesViewCoverflowListItemThumbPath, string.Empty);
                        break;
                    default:
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