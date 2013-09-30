using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.GUI.Library;
using System.Collections;
using Common.Helpers;
using System.Reflection;
using Common.Settings.SettingsObjects;
using MediaPortalPlugin.PluginHelpers;
using MPDisplay.Common.Settings;
using MessageFramework.DataObjects;

namespace MediaPortalPlugin
{
    public static class SupportedPluginManager
    {
        private static Dictionary<SupportedPlugin, IPluginHelper> _supportedPlugins = new Dictionary<SupportedPlugin, IPluginHelper>();
        private static Dictionary<int, SupportedPlugin> _supportedPluginMap = new Dictionary<int, SupportedPlugin>();
        private static Dictionary<int, GUIWindow> _installedPlugins = new Dictionary<int, GUIWindow>();
        private static AdvancedPluginSettings _supportedPluginSettings;

        public static void LoadPlugins(AdvancedPluginSettings settings)
        {
            _supportedPluginSettings = settings;

            foreach (var plugin in  PluginManager.GUIPlugins.Cast<GUIWindow>())
            {
                if (!_installedPlugins.ContainsKey(plugin.GetID))
                {
                    _installedPlugins.Add(plugin.GetID, plugin);
                }
            }

         

            if (_supportedPluginSettings != null)
            {
                foreach (var plugin in _supportedPluginSettings.SupportedPlugins)
                {
                    foreach (var id in plugin.WindowIds)
                    {
                        if (!_supportedPluginMap.ContainsKey(id))
                        {
                            _supportedPluginMap.Add(id, plugin.PluginType);
                        }
                    }
                }
            }

            _supportedPlugins.Add(SupportedPlugin.MovingPictures, new MovingPicturesPlugin(GetPluginWindow(SupportedPlugin.MovingPictures), GetPluginSettings(SupportedPlugin.MovingPictures)));
            _supportedPlugins.Add(SupportedPlugin.MyFilms, new MyFilmsPlugin(GetPluginWindow(SupportedPlugin.MyFilms), GetPluginSettings(SupportedPlugin.MyFilms)));
            _supportedPlugins.Add(SupportedPlugin.MPTVSeries, new MPTvSeriesPlugin(GetPluginWindow(SupportedPlugin.MPTVSeries), GetPluginSettings(SupportedPlugin.MPTVSeries)));
            _supportedPlugins.Add(SupportedPlugin.mvCentral, new MvCentralPlugin(GetPluginWindow(SupportedPlugin.mvCentral), GetPluginSettings(SupportedPlugin.mvCentral)));
            _supportedPlugins.Add(SupportedPlugin.OnlineVideos, new OnlineVideosPlugin(GetPluginWindow(SupportedPlugin.OnlineVideos), GetPluginSettings(SupportedPlugin.OnlineVideos)));
            _supportedPlugins.Add(SupportedPlugin.MyAnime, new MovingPicturesPlugin(GetPluginWindow(SupportedPlugin.MyAnime), GetPluginSettings(SupportedPlugin.MyAnime)));
            _supportedPlugins.Add(SupportedPlugin.Rockstar, new RockStarPlugin(GetPluginWindow(SupportedPlugin.Rockstar), GetPluginSettings(SupportedPlugin.Rockstar)));

         

           
        }

        public static SupportedPluginSettings GetPluginSettings(SupportedPlugin plugin)
        {
            if (_supportedPluginSettings != null)
            {
              return   _supportedPluginSettings.SupportedPlugins.FirstOrDefault(s => s.PluginType == plugin);
            }
            return null;
        }
     

        public static IPluginHelper GetPluginHelper(SupportedPlugin plugin)
        {
            return _supportedPlugins[plugin];
        }

        public static IPluginHelper GetPluginHelper(int windowId)
        {
            if (_supportedPluginMap.ContainsKey(windowId))
            {
                return _supportedPlugins[_supportedPluginMap[windowId]];
            }
            return null;
        }

        public static GUIWindow GetPluginWindow(int plugin)
        {
            if (_installedPlugins.ContainsKey(plugin))
            {
                return _installedPlugins[plugin];
            }
            return null;
        }

        public static GUIWindow GetPluginWindow(SupportedPlugin plugin)
        {
            return GetPluginWindow((int)plugin);
        }

        public static APIPlaybackType GetPluginPlayerType(APIPlaybackType playtype, string filename)
        {
            var plugin = _supportedPlugins.Values.FirstOrDefault(p => p.IsPlaying(filename));
            if (plugin != null)
            {
                return plugin.PlayType;
            }
            return playtype;
        }

        public static void GUISafeInvoke(System.Action action)
        {
            try
            {
                GUIGraphicsContext.form.Invoke(action);
            }
            catch { }
        }

        public static string GetListItemImage(SupportedPluginSettings settings, GUIListItem item, APIListLayout layout)
        {
            if (settings != null && item != null)
            {
                switch (layout)
                {
                    case APIListLayout.Vertical:
                        return ReflectionHelper.GetPropertyPath<string>(item, settings.VerticalListItemThumbPath, string.Empty);
                    case APIListLayout.Horizontal:
                        return ReflectionHelper.GetPropertyPath<string>(item, settings.HorizontalListItemThumbPath, string.Empty);
                    case APIListLayout.CoverFlow:
                        return ReflectionHelper.GetPropertyPath<string>(item, settings.VerticalListItemThumbPath, string.Empty);
                    default:
                        break;
                }

            }
            return string.Empty;
        }
   
    }

   
}
