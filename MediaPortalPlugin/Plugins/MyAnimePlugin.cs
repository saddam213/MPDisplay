using Common.Helpers;
using Common.Settings.SettingsObjects;
using MediaPortal.GUI.Library;
using MessageFramework.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MediaPortalPlugin.PluginHelpers
{
    public class MyAnimePlugin : PluginHelper
    {
        public MyAnimePlugin(GUIWindow pluginindow, SupportedPluginSettings settings)
            : base(pluginindow, settings)
        {
        }

        public override bool IsPlaying(string filename, APIPlaybackType playtype)
        {
            if (IsEnabled)
            {
                var currentMovie = ReflectionHelper.GetPropertyValue(PluginWindow, "curAnimeEpisode", string.Empty);
                if (currentMovie != null)
                {
                    if (ReflectionHelper.GetPropertyValue<string>(currentMovie, "FullPath", string.Empty) == filename)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override APIPlaybackType PlayType
        {
            get { return APIPlaybackType.MyAnime; }
        }
    }
}