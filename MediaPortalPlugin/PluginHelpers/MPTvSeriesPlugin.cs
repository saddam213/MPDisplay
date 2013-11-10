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

        public override APIPlaybackType PlayType
        {
            get { return APIPlaybackType.MPTVSeries; }
        }
    }
}