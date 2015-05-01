using System.Collections;
using System.Linq;
using System.Reflection;
using Common.Helpers;
using Common.Settings;
using MediaPortal.GUI.Library;
using MessageFramework.DataObjects;

namespace MediaPortalPlugin.Plugins
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
                    var playlistPlayer = ReflectionHelper.GetFieldValue(player, "playlistPlayer", null, BindingFlags.Instance | BindingFlags.Public);
                    if (playlistPlayer != null)
                    {
                        var playlist = ReflectionHelper.GetFieldValue<IEnumerable>(playlistPlayer, "_mvCentralPlayList", null);
                        if (playlist != null)
                        {
                            return playlist.Cast<object>().Any(item => ReflectionHelper.GetPropertyValue(item, "FileName", string.Empty) == filename);
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