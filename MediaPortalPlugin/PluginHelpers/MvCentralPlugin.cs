using Common.Settings.SettingsObjects;
using MediaPortal.GUI.Library;
using MessageFramework.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Helpers;
using System.Collections;

namespace MediaPortalPlugin.PluginHelpers
{
    public class MvCentralPlugin : IPluginHelper
    {
        private GUIWindow _window;
        private SupportedPluginSettings _settings;

        public MvCentralPlugin(GUIWindow pluginWindow, SupportedPluginSettings settings)
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
            var player = ReflectionHelper.GetFieldValue(_window, "Player");
            if (player != null)
            {
                var playlistPlayer = ReflectionHelper.GetFieldValue(player, "playlistPlayer", null, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                if (playlistPlayer != null)
                {
                    var playlist = ReflectionHelper.GetFieldValue<IEnumerable>(playlistPlayer, "_mvCentralPlayList", null);
                    if (playlist != null)
                    {
                        foreach (var item in playlist)
                        {
                            if (ReflectionHelper.GetPropertyValue<string>(item, "FileName", string.Empty) == filename)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public string GetListItemThumb(GUIListItem item, APIListLayout layout)
        {
            return SupportedPluginManager.GetListItemImage(_settings, item, layout);
        }

        public APIPlaybackType PlayType
        {
            get { return APIPlaybackType.mvCentral; }
        }
    }
}