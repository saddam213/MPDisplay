using Common.Helpers;
using Common.Settings;
using MediaPortal.GUI.Library;
using MessageFramework.DataObjects;

namespace MediaPortalPlugin.Plugins
{
    public class MovingPicturesPlugin : PluginHelper
    {
        public MovingPicturesPlugin(GUIWindow pluginindow, SupportedPluginSettings settings)
            : base(pluginindow, settings) 
        {
        }
     

        public override bool IsPlaying(string filename, APIPlaybackType playtype)
        {
            if (!IsEnabled) return false;
            var moviePlayer = ReflectionHelper.GetFieldValue(PluginWindow, "moviePlayer");
            if (moviePlayer == null) return false;
            var currentMovie = ReflectionHelper.GetPropertyValue<object>(moviePlayer, "CurrentMedia",null);
            if (currentMovie == null) return false;
            return ReflectionHelper.GetPropertyValue(currentMovie, "FullPath", string.Empty) == filename;
        }

        public override APIPlaybackType PlayType => APIPlaybackType.MovingPictures;
    }
}
