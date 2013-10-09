using Common.Settings.SettingsObjects;
using MediaPortal.GUI.Library;
using MessageFramework.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MediaPortalPlugin.PluginHelpers
{
    public interface IPluginHelper
    {
        SupportedPluginSettings Settings { get; }
        GUIWindow PluginWindow { get; }
        int WindowId { get; }
        bool IsEnabled { get; }
        bool IsPlaying(string filename, APIPlaybackType playtype);
        APIPlaybackType PlayType { get; }
        string GetListItemThumb(GUIListItem item, APIListLayout layout);
    }
}
