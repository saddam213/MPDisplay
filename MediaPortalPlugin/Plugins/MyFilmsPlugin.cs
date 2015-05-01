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
            if (IsEnabled)
            {
                var currentMovie = ReflectionHelper.GetStaticField(PluginWindow, "currentMovie", null);
                if (currentMovie != null)
                {
                    if (ReflectionHelper.GetPropertyValue(currentMovie, "File", string.Empty) == filename)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override APIPlaybackType PlayType
        {
            get { return APIPlaybackType.MyFilms; }
        }
    }
}
