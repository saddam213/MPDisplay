using System.Collections.Generic;
using System.Linq;
using Common.Settings;
using MediaPortal.GUI.Library;
using MediaPortalPlugin.Plugins;
using MessageFramework.DataObjects;
using Action = System.Action;

namespace MediaPortalPlugin.InfoManagers
{
    public static class SupportedPluginManager
    {
        private static Dictionary<SupportedPlugin, PluginHelper> _supportedPlugins = new Dictionary<SupportedPlugin, PluginHelper>();
        private static Dictionary<int, SupportedPlugin> _supportedPluginMap = new Dictionary<int, SupportedPlugin>();
        private static Dictionary<int, GUIWindow> _installedPlugins = new Dictionary<int, GUIWindow>();
        private static AdvancedPluginSettings _supportedPluginSettings;

        public static void LoadPlugins(AdvancedPluginSettings settings)
        {
            _supportedPluginSettings = settings;

            foreach (var plugin in PluginManager.GUIPlugins.Cast<GUIWindow>().Where(plugin => !_installedPlugins.ContainsKey(plugin.GetID)))
            {
                _installedPlugins.Add(plugin.GetID, plugin);
            }

            if (_supportedPluginSettings != null)
            {
                foreach (var plugin in _supportedPluginSettings.SupportedPlugins)
                {
                    foreach (var id in plugin.WindowIds.Where(id => !_supportedPluginMap.ContainsKey(id)))
                    {
                        _supportedPluginMap.Add(id, plugin.PluginType);
                    }
                }
            }

            _supportedPlugins.Add(SupportedPlugin.MovingPictures, new MovingPicturesPlugin(GetPluginWindow(SupportedPlugin.MovingPictures), GetPluginSettings(SupportedPlugin.MovingPictures)));
            _supportedPlugins.Add(SupportedPlugin.MyFilms, new MyFilmsPlugin(GetPluginWindow(SupportedPlugin.MyFilms), GetPluginSettings(SupportedPlugin.MyFilms)));
            _supportedPlugins.Add(SupportedPlugin.MPTVSeries, new MpTvSeriesPlugin(GetPluginWindow(SupportedPlugin.MPTVSeries), GetPluginSettings(SupportedPlugin.MPTVSeries)));
            _supportedPlugins.Add(SupportedPlugin.mvCentral, new MvCentralPlugin(GetPluginWindow(SupportedPlugin.mvCentral), GetPluginSettings(SupportedPlugin.mvCentral)));
            _supportedPlugins.Add(SupportedPlugin.OnlineVideos, new OnlineVideosPlugin(GetPluginWindow(SupportedPlugin.OnlineVideos), GetPluginSettings(SupportedPlugin.OnlineVideos)));
            _supportedPlugins.Add(SupportedPlugin.MyAnime, new MyAnimePlugin(GetPluginWindow(SupportedPlugin.MyAnime), GetPluginSettings(SupportedPlugin.MyAnime)));
            _supportedPlugins.Add(SupportedPlugin.Rockstar, new RockStarPlugin(GetPluginWindow(SupportedPlugin.Rockstar), GetPluginSettings(SupportedPlugin.Rockstar)));
            _supportedPlugins.Add(SupportedPlugin.TuneIn, new TuneInPlugin(GetPluginWindow(SupportedPlugin.TuneIn), GetPluginSettings(SupportedPlugin.TuneIn)));
 
        }

        public static SupportedPluginSettings GetPluginSettings(SupportedPlugin plugin)
        {
            return _supportedPluginSettings != null ? _supportedPluginSettings.SupportedPlugins.FirstOrDefault(s => s.PluginType == plugin) : null;
        }


        public static PluginHelper GetPluginHelper(SupportedPlugin plugin)
        {
            return _supportedPlugins[plugin];
        }

        public static PluginHelper GetPluginHelper(int windowId)
        {
            return _supportedPluginMap.ContainsKey(windowId) ? _supportedPlugins[_supportedPluginMap[windowId]] : null;
        }

        public static GUIWindow GetPluginWindow(int plugin)
        {
            return _installedPlugins.ContainsKey(plugin) ? _installedPlugins[plugin] : null;
        }

        public static GUIWindow GetPluginWindow(SupportedPlugin plugin)
        {
            return GetPluginWindow((int)plugin);
        }

        public static APIPlaybackType GetPluginPlayerType(APIPlaybackType playtype, string filename)
        {
            var plugin = _supportedPlugins.Values.FirstOrDefault(p => p.IsPlaying(filename, playtype));
            return plugin != null ? plugin.PlayType : playtype;
        }

        public static void GuiSafeInvoke(Action action)
        {
            try
            {
                GUIGraphicsContext.form.Invoke(action);
            }
            catch
            {
                // ignored
            }
        }

     
   
    }

   
}
