using Common.Helpers;
using Common.Settings.SettingsObjects;
using MediaPortal.GUI.Library;
using MessageFramework.DataObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MediaPortalPlugin.PluginHelpers
{
    public class RockStarPlugin : IPluginHelper
    {
        private GUIWindow _window;
        private SupportedPluginSettings _settings;

        public RockStarPlugin(GUIWindow pluginWindow, SupportedPluginSettings settings)
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

        public bool IsEnabled
        {
            get { return _window != null; }
        }

        public int WindowId
        {
            get { return IsEnabled ? _window.GetID : -1; }
        }

        public bool IsPlaying(string filename)
        {
            if (IsEnabled)
            {
                var playerManager = ReflectionHelper.GetFieldValue(_window, "playerManager");
                if (playerManager != null)
                {
                    var players = ReflectionHelper.GetFieldValue<IDictionary>(playerManager, "players", null);
                    if (players != null)
                    {
                        foreach (var item in players.Values)
                        {
                            if (ReflectionHelper.GetPropertyValue<string>(item, "CurrentFile", string.Empty) == filename)
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
            if (item != null)
            {
                string imagePath = string.Format("{0}\\Media\\{1}", GUIGraphicsContext.Skin, item.IconImage);
                if (File.Exists(imagePath))
                {
                    return imagePath;
                }
            }
            return SupportedPluginManager.GetListItemImage(_settings, item, layout);
        }

        public APIPlaybackType PlayType
        {
            get { return APIPlaybackType.Rockstar; }
        }
    
    }
}
