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
using System.Net;

namespace MediaPortalPlugin.PluginHelpers
{
    public class RadioTimePlugin : IPluginHelper
    {
        private GUIWindow _window;
        private SupportedPluginSettings _settings;

        public RadioTimePlugin(GUIWindow pluginWindow, SupportedPluginSettings settings)
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

        public bool IsPlaying(string filename, APIPlaybackType playtype)
        {
            if (IsEnabled)
            {
                if (playtype == APIPlaybackType.IsRadio && filename.StartsWith("http"))
                {
                    return true;
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
            get { return APIPlaybackType.RadioTime; }
        }
    
    }
}
