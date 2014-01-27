using Common.Helpers;
using Common.Settings.SettingsObjects;
using MediaPortal.GUI.Library;
using MessageFramework.DataObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MediaPortalPlugin.PluginHelpers
{
    public class RockStarPlugin : PluginHelper
    {
        public RockStarPlugin(GUIWindow pluginindow, SupportedPluginSettings settings)
            : base(pluginindow, settings)
        {
        }
      
        public override bool IsPlaying(string filename, APIPlaybackType playtype)
        {
            if (IsEnabled)
            {
                var playerManager = ReflectionHelper.GetFieldValue(PluginWindow, "playerManager");
                if (playerManager != null)
                {
                    var players = ReflectionHelper.GetFieldValue<IDictionary>(playerManager, "players", null);
                    if (players != null)
                    {
                        foreach (var item in players.Values)
                        {
                            if (ReflectionHelper.GetPropertyValue<string>(item, "CurrentFile", string.Empty) == filename)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public override APIImage GetListItemImage1(GUIListItem item, APIListLayout layout)
        {
            if (item != null)
            {
                var image = ImageHelper.CreateImage(item.IconImage);
                if (!image.IsEmpty)
                {
                    return image;
                }
            }
            return base.GetListItemImage1(item, layout);
        }

        public override APIPlaybackType PlayType
        {
            get { return APIPlaybackType.Rockstar; }
        }
    }
}
