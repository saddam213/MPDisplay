using System.Diagnostics;
using System.IO;
using System.Linq;
using MediaPortal.Common.Messaging;
using MediaPortal.Common.PluginManager;
using MediaPortal.Common.Runtime;
using Common.Log;
using Common.Settings;
using MediaPortal2Plugin.InfoManagers;
using Log = Common.Log.Log;

namespace MediaPortal2Plugin
{
    public class MpDisplayPlugin2 : IPluginStateTracker
    {
    #region Protected fields

        protected AsynchronousMessageQueue MessageQueue;
        protected object SyncObj = new object();

        #endregion

        private readonly PluginSettings _settings;
        private readonly AdvancedPluginSettings _advancedSettings;
        private readonly MP2PluginSettings _mp2PluginSettings;
        private readonly Log _log;
        private bool _pluginStarted ;
    public MpDisplayPlugin2()
    {
            LoggingManager.AddLog(new FileLogger(RegistrySettings.ProgramDataPath + "Logs", "Plugin2", RegistrySettings.LogLevel));
            _log = LoggingManager.GetLog(typeof(MpDisplayPlugin2));

            _log.Message(LogLevel.Info, "[PluginConstructor] - Loading MPDisplay settings file: {0}", RegistrySettings.MPDisplaySettingsFile);
            var settings = SettingsManager.Load<MPDisplaySettings>(RegistrySettings.MPDisplaySettingsFile);
            if (settings == null)
            {
                _log.Message(LogLevel.Info, "[PluginConstructor] - Settings file for MPDisplay not found, Loading defaults..");
                settings = new MPDisplaySettings();
                SettingsManager.Save(settings, RegistrySettings.MPDisplaySettingsFile);
            }
            var advancedSettings = SettingsManager.Load<AdvancedPluginSettings>(Path.Combine(RegistrySettings.ProgramDataPath, "AdvancedPluginSettings.xml"));
            if (advancedSettings == null)
            {
                _log.Message(LogLevel.Info, "[PluginConstructor] - Settings file AdvancedPluginSettings not found, Loading defaults..");
                advancedSettings = new AdvancedPluginSettings();
            }
            var mp2PluginSettings = SettingsManager.Load<MP2PluginSettings>(Path.Combine(RegistrySettings.ProgramDataPath, "MP2PluginSettings.xml"));
            if (mp2PluginSettings == null)
            {
                _log.Message(LogLevel.Info, "[PluginConstructor] - Settings file MP2PluginSettings not found, Loading defaults..");
                mp2PluginSettings = new MP2PluginSettings();
            }

            _advancedSettings = advancedSettings;
            _mp2PluginSettings = mp2PluginSettings;
            _settings = settings.PluginSettings;

        }

        protected void DropMessageQueue()
            {
              lock (SyncObj)
              {
                  MessageQueue?.Terminate();
                  MessageQueue = null;
              }
            }

    #region Implementation of IPluginStateTracker

    public void Activated(PluginRuntime pluginRuntime)
    {
        if (_settings != null && !_pluginStarted)
        {
            _log.Message(LogLevel.Info, "[OnPluginActivated] - Starting MPDisplay Plugin...");
            if (_settings.LaunchMPDisplayOnStart)
            {
                var processes = Process.GetProcessesByName("MPDisplay");

                if (_settings.RestartMPDisplayOnStart)
                {
                    processes.CloseAll();
                }

                if (!processes.Any())
                {
                    Process.Start(RegistrySettings.MPDisplayExePath);
                }
            }

            MessageService.InitializeMessageService(_settings.ConnectionSettings);
            WindowManager.Instance.Initialize(_settings, _advancedSettings, _mp2PluginSettings);
            //TvServerManager.Instance.Initialize(_settings);
            _log.Message(LogLevel.Info, "[OnPluginStart] - MPDisplay Plugin started.");
            _pluginStarted = true;

            MessageQueue = new AsynchronousMessageQueue(this, new[] {SystemMessaging.CHANNEL});
            MessageQueue.MessageReceived += OnMessageReceived;
            MessageQueue.Start();
        }
        else
        {
            _log.Message(LogLevel.Error, "[OnPluginStart] - Failed to create settings file, Stopping plugin.");
            LoggingManager.Destroy();
        }

     }

        private void OnMessageReceived(AsynchronousMessageQueue queue, SystemMessage message)
        {
            if (message.ChannelName == SystemMessaging.CHANNEL)
            {

                SystemMessaging.MessageType messageType = (SystemMessaging.MessageType)message.MessageType;
                 _log.Message(LogLevel.Info, "[OnMessageReceived] - SystemMessaging Type <{0}> Data <{1}>", messageType, message.MessageData);
            }
        }

        public bool RequestEnd()
        {
            _log.Message(LogLevel.Info, "[PluginRequestEnd] - End requested...");
            return true;
        }

        public void Stop()
        {
            _log.Message(LogLevel.Info, "[OnPluginStop] - Stop requested...");
            PluginStop();
        }

        public void Continue()
        {
            _log.Message(LogLevel.Info, "[OnPluginContinue] - Continue requested...");
        }

        public void Shutdown()
        {
            _log.Message(LogLevel.Info, "[OnPluginShutdown] - Shutdown requested...");
            PluginStop();
         }

        #endregion

        /// <summary>
        /// Stop the plugin
        /// </summary>
        private void PluginStop()
        {
            if (!_pluginStarted) return;

            _pluginStarted = false;
            _log.Message(LogLevel.Info, "[PluginStop] - Stopping MPDisplay Plugin2...");
            if (_settings.CloseMPDisplayOnExit)
            {
                Process.GetProcessesByName("MPDisplay").CloseAll();
            }
            MessageService.Instance.Shutdown();
            WindowManager.Instance.Shutdown();
            DropMessageQueue();

            if (_mp2PluginSettings.IsModified)
            {
                SettingsManager.Save(_mp2PluginSettings, Path.Combine(RegistrySettings.ProgramDataPath, "MP2PluginSettings.xml"));
                _log.Message(LogLevel.Info, "[PluginStop] - Modified MP2Plugin Settings have been saved.");
            }
            _log.Message(LogLevel.Info, "[PluginStop] - MPDisplay Plugin2 stopped.");
            LoggingManager.Destroy();
        }
    }
}
