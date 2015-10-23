using Common.Helpers;
using Common.Settings;
using MediaPortal.GUI.Library;
using MessageFramework.DataObjects;

namespace MediaPortalPlugin.Plugins
{
    public class MyFilmsPlugin : PluginHelper
    {
        public MyFilmsPlugin(GUIWindow pluginindow, SupportedPluginSettings settings)
            : base(pluginindow, settings)
        {
        }

        public override bool IsPlaying(string filename, APIPlaybackType playtype)
        {
            if (!IsEnabled) return false;
            var currentMovie = ReflectionHelper.GetStaticField(PluginWindow, "currentMovie", null);
            if (currentMovie == null) return false;
            return ReflectionHelper.GetPropertyValue(currentMovie, "File", string.Empty) == filename;
        }

        public override APIPlaybackType PlayType
        {
            get { return APIPlaybackType.MyFilms; }
        }
    }
}
