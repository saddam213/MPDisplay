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
        private static readonly Dictionary<SupportedPlugin, PluginHelper> SupportedPlugins = new Dictionary<SupportedPlugin, PluginHelper>();
        private static readonly Dictionary<int, SupportedPlugin> SupportedPluginMap = new Dictionary<int, SupportedPlugin>();
        private static readonly Dictionary<int, GUIWindow> InstalledPlugins = new Dictionary<int, GUIWindow>();
        private static AdvancedPluginSettings _supportedPluginSettings;

        public static void LoadPlugins(AdvancedPluginSettings settings)
        {
            _supportedPluginSettings = settings;

            foreach (var plugin in PluginManager.GUIPlugins.Cast<GUIWindow>().Where(plugin => !InstalledPlugins.ContainsKey(plugin.GetID)))
            {
                InstalledPlugins.Add(plugin.GetID, plugin);
            }

            if (_supportedPluginSettings != null)
            {
                foreach (var plugin in _supportedPluginSettings.SupportedPlugins)
                {
                    foreach (var id in plugin.WindowIds.Where(id => !SupportedPluginMap.ContainsKey(id)))
                    {
                        SupportedPluginMap.Add(id, plugin.PluginType);
                    }
                }
            }

            SupportedPlugins.Add(SupportedPlugin.MovingPictures, new MovingPicturesPlugin(GetPluginWindow(SupportedPlugin.MovingPictures), GetPluginSettings(SupportedPlugin.MovingPictures)));
            SupportedPlugins.Add(SupportedPlugin.MyFilms, new MyFilmsPlugin(GetPluginWindow(SupportedPlugin.MyFilms), GetPluginSettings(SupportedPlugin.MyFilms)));
            SupportedPlugins.Add(SupportedPlugin.MPTVSeries, new MpTvSeriesPlugin(GetPluginWindow(SupportedPlugin.MPTVSeries), GetPluginSettings(SupportedPlugin.MPTVSeries)));
            SupportedPlugins.Add(SupportedPlugin.mvCentral, new MvCentralPlugin(GetPluginWindow(SupportedPlugin.mvCentral), GetPluginSettings(SupportedPlugin.mvCentral)));
            SupportedPlugins.Add(SupportedPlugin.OnlineVideos, new OnlineVideosPlugin(GetPluginWindow(SupportedPlugin.OnlineVideos), GetPluginSettings(SupportedPlugin.OnlineVideos)));
            SupportedPlugins.Add(SupportedPlugin.MyAnime, new MyAnimePlugin(GetPluginWindow(SupportedPlugin.MyAnime), GetPluginSettings(SupportedPlugin.MyAnime)));
            SupportedPlugins.Add(SupportedPlugin.Rockstar, new RockStarPlugin(GetPluginWindow(SupportedPlugin.Rockstar), GetPluginSettings(SupportedPlugin.Rockstar)));
            SupportedPlugins.Add(SupportedPlugin.TuneIn, new TuneInPlugin(GetPluginWindow(SupportedPlugin.TuneIn), GetPluginSettings(SupportedPlugin.TuneIn)));
 
        }

        public static SupportedPluginSettings GetPluginSettings(SupportedPlugin plugin)
        {
            return _supportedPluginSettings?.SupportedPlugins.FirstOrDefault(s => s.PluginType == plugin);
        }


        public static PluginHelper GetPluginHelper(SupportedPlugin plugin)
        {
            return SupportedPlugins[plugin];
        }

        public static PluginHelper GetPluginHelper(int windowId)
        {
            return SupportedPluginMap.ContainsKey(windowId) ? SupportedPlugins[SupportedPluginMap[windowId]] : null;
        }

        public static GUIWindow GetPluginWindow(int plugin)
        {
            return InstalledPlugins.ContainsKey(plugin) ? InstalledPlugins[plugin] : null;
        }

        public static GUIWindow GetPluginWindow(SupportedPlugin plugin)
        {
            return GetPluginWindow((int)plugin);
        }

        public static APIPlaybackType GetPluginPlayerType(APIPlaybackType playtype, string filename)
        {
            var plugin = SupportedPlugins.Values.FirstOrDefault(p => p.IsPlaying(filename, playtype));
            return plugin?.PlayType ?? playtype;
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
