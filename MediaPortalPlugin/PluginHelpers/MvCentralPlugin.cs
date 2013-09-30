using Common.Settings.SettingsObjects;
using MediaPortal.GUI.Library;
using MessageFramework.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public bool IsPlaying(string filename)
        {
            return false;
        }

        public string GetListItemThumb(GUIListItem item, APIListLayout layout)
        {
            return string.Empty;
        }

        public APIPlaybackType PlayType
        {
            get { return APIPlaybackType.mvCentral; }
        }
    }
}