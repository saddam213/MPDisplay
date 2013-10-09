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
    public class MyAnimePlugin : IPluginHelper
    {
        private GUIWindow _window;
        private SupportedPluginSettings _settings;

        public MyAnimePlugin(GUIWindow pluginWindow, SupportedPluginSettings settings)
        {
            _window = pluginWindow;
            _settings = settings;
        }

        public SupportedPluginSettings Settings
        {
            get { return _settings; }
        }

        public GUIWindow PluginWindow
        {
            get { return _window; }
        }

        public int WindowId
        {
            get { return IsEnabled ? _window.GetID : 0; }
        }

        public bool IsEnabled
        {
            get { return _window != null; }
        }

        public bool IsPlaying(string filename, APIPlaybackType playtype)
        {
            if (IsEnabled)
            {
                var currentMovie = ReflectionHelper.GetPropertyValue(_window, "curAnimeEpisode", string.Empty);
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

        public string GetListItemThumb(GUIListItem item, APIListLayout layout)
        {
            return string.Empty;
        }

        public APIPlaybackType PlayType
        {
            get { return APIPlaybackType.MyAnime; }
        }
    }
}