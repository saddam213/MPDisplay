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
    public class MvCentralPlugin : PluginHelper
    {
        public MvCentralPlugin(GUIWindow pluginindow, SupportedPluginSettings settings)
            : base(pluginindow, settings)
        {
        }

         public override bool IsPlaying(string filename, APIPlaybackType playtype)
        {
            if (IsEnabled)
            {
                var player = ReflectionHelper.GetFieldValue(PluginWindow, "Player");
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
            }
            return false;
        }



        public override APIPlaybackType PlayType
        {
            get { return APIPlaybackType.mvCentral; }
        }
    }
}