using System.Collections;
using System.Linq;
using Common.Helpers;
using Common.Settings;
using MediaPortal.GUI.Library;
using MessageFramework.DataObjects;

namespace MediaPortalPlugin.Plugins
{
    public class RockStarPlugin : PluginHelper
    {
        public RockStarPlugin(GUIWindow pluginindow, SupportedPluginSettings settings)
            : base(pluginindow, settings)
        {
        }
      
        public override bool IsPlaying(string filename, APIPlaybackType playtype)
        {
            if (!IsEnabled) return false;
            var playerManager = ReflectionHelper.GetFieldValue(PluginWindow, "playerManager");
            if (playerManager == null) return false;
            var players = ReflectionHelper.GetFieldValue<IDictionary>(playerManager, "players", null);
            return players != null && players.Values.Cast<object>().Any(item => ReflectionHelper.GetPropertyValue(item, "CurrentFile", string.Empty) == filename);
        }

        public override APIImage GetListItemImage1(GUIListItem item, APIListLayout layout)
        {
            if (item == null) return base.GetListItemImage1(null, layout);
            var image = ImageHelper.CreateImage(item.IconImage);
            return !image.IsEmpty ? image : base.GetListItemImage1(item, layout);
        }

        public override APIPlaybackType PlayType
        {
            get { return APIPlaybackType.Rockstar; }
        }
    }
}
