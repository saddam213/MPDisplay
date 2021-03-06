﻿using Common.Settings;
using MediaPortal.GUI.Library;
using MessageFramework.DataObjects;

namespace MediaPortalPlugin.Plugins
{
    public class TuneInPlugin : PluginHelper
    {
        public TuneInPlugin(GUIWindow pluginindow, SupportedPluginSettings settings)
            : base(pluginindow, settings)
        {
        }

        public override APIPlaybackType PlayType => APIPlaybackType.TuneIn;

        public override bool IsPlaying(string filename, APIPlaybackType playtype)
        {
            if (!IsEnabled) return false;
            return playtype == APIPlaybackType.IsRadio && filename.StartsWith("http");
        }
    }
}
