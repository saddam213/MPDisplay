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
             if (!IsEnabled) return false;
             var player = ReflectionHelper.GetFieldValue(PluginWindow, "Player");
             if (player == null) return false;
             var playlistPlayer = ReflectionHelper.GetFieldValue(player, "playlistPlayer", null, BindingFlags.Instance | BindingFlags.Public);
             if (playlistPlayer == null) return false;
             var playlist = ReflectionHelper.GetFieldValue<IEnumerable>(playlistPlayer, "_mvCentralPlayList", null);
             return playlist != null && playlist.Cast<object>().Any(item => ReflectionHelper.GetPropertyValue(item, "FileName", string.Empty) == filename);
        }

        public override APIPlaybackType PlayType => APIPlaybackType.mvCentral;
    }
}