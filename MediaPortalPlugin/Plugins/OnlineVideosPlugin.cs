using Common.Settings.SettingsObjects;
using MediaPortal.GUI.Library;
using MessageFramework.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MediaPortalPlugin.PluginHelpers
{
    public class OnlineVideosPlugin : PluginHelper
    {
        public OnlineVideosPlugin(GUIWindow pluginindow, SupportedPluginSettings settings)
            : base(pluginindow, settings)
        {
        }

        public override bool IsPlaying(string filename, APIPlaybackType playtype)
        {
            if (IsEnabled)
            {
                return filename.Contains("OnlineVideo.mp4");
            }
            return false;
        }

        public override APIPlaybackType PlayType
        {
            get { return APIPlaybackType.OnlineVideos; }
        }
    }
}