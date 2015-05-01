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
            if (IsEnabled)
            {
                var moviePlayer = ReflectionHelper.GetFieldValue(PluginWindow, "moviePlayer");
                if (moviePlayer != null)
                {
                    var currentMovie = ReflectionHelper.GetPropertyValue<object>(moviePlayer, "CurrentMedia",null);
                    if (currentMovie != null)
                    {
                        if (ReflectionHelper.GetPropertyValue(currentMovie, "FullPath", string.Empty) == filename)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public override APIPlaybackType PlayType
        {
            get { return APIPlaybackType.MovingPictures; }
        }
    }
}
