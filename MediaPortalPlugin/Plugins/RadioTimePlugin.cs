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
    public class RadioTimePlugin : PluginHelper
    {
        public RadioTimePlugin(GUIWindow pluginindow, SupportedPluginSettings settings)
            : base(pluginindow, settings)
        {
        }

        public override APIPlaybackType PlayType
        {
            get { return APIPlaybackType.RadioTime; }
        }

        public override bool IsPlaying(string filename, APIPlaybackType playtype)
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
    }
}
