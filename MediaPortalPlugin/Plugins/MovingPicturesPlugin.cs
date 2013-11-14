using Common.Helpers;
using Common.Settings.SettingsObjects;
using MediaPortal.GUI.Library;
using MessageFramework.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MediaPortalPlugin.PluginHelpers
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
                var moviePlayer = ReflectionHelper.GetFieldValue(PluginWindow, "moviePlayer", null);
                if (moviePlayer != null)
                {
                    var currentMovie = ReflectionHelper.GetPropertyValue<object>(moviePlayer, "CurrentMedia",null);
                    if (currentMovie != null)
                    {
                        if (ReflectionHelper.GetPropertyValue<string>(currentMovie, "FullPath", string.Empty) == filename)
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
