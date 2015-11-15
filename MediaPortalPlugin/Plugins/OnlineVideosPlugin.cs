using Common.Settings;
using MediaPortal.GUI.Library;
using MessageFramework.DataObjects;

namespace MediaPortalPlugin.Plugins
{
    public class OnlineVideosPlugin : PluginHelper
    {
        public OnlineVideosPlugin(GUIWindow pluginindow, SupportedPluginSettings settings)
            : base(pluginindow, settings)
        {
        }

        public override bool IsPlaying(string filename, APIPlaybackType playtype)
        {
            return IsEnabled && filename.Contains("OnlineVideo.mp4");
        }

        public override APIPlaybackType PlayType => APIPlaybackType.OnlineVideos;
    }
}