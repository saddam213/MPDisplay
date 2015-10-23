using Common.Helpers;
using Common.Settings;
using MediaPortal.GUI.Library;
using MessageFramework.DataObjects;

namespace MediaPortalPlugin.Plugins
{
    public class MyAnimePlugin : PluginHelper
    {
        public MyAnimePlugin(GUIWindow pluginindow, SupportedPluginSettings settings)
            : base(pluginindow, settings)
        {
        }

        public override bool IsPlaying(string filename, APIPlaybackType playtype)
        {
            if (!IsEnabled) return false;
            var currentMovie = ReflectionHelper.GetPropertyValue(PluginWindow, "curAnimeEpisode", string.Empty);
            if (currentMovie == null) return false;
            return ReflectionHelper.GetPropertyValue(currentMovie, "FullPath", string.Empty) == filename;
        }

        public override APIPlaybackType PlayType
        {
            get { return APIPlaybackType.MyAnime; }
        }
    }
}