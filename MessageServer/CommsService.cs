using System;
using System.IO;
using System.ServiceModel;
using System.ServiceProcess;
using Common.Log;
using Common.Settings;

namespace MessageServer
{
    partial class CommsService : ServiceBase
    {
        private ServiceHost _mpDisplayHost;
        private Log _log;
     

        /// <summary>
        /// Initializes a new instance of the <see cref="CommsService"/> class.
        /// </summary>
        public CommsService()
        {
            LoggingManager.AddLog(new FileLogger(RegistrySettings.ProgramDataPath + "Logs", "Server", RegistrySettings.LogLevel));
            _log = LoggingManager.GetLog(typeof(CommsService));
            InitializeComponent();
        }

        /// <summary>
        /// When implemented in a derived class, executes when a Start command is sent to the service by the Service Control Manager (SCM) or when the operating system starts (for a service that starts automatically). Specifies actions to take when the service starts.
        /// </summary>
        /// <param name="args">Data passed by the start command.</param>
        protected override void OnStart(string[] args)
        {
            _log.Message(LogLevel.Info, "[OnStart] - Starting MPDisplay server...");
            try
            {
                var settings = GetConnectionSettings(RegistrySettings.MPDisplaySettingsFile);
                if (settings != null)
                {
                    Uri uri;
                    string connectionString = string.Format("net.tcp://{0}:{1}/MPDisplayService", settings.IpAddress, settings.Port);
                    if (Uri.TryCreate(connectionString, UriKind.RelativeOrAbsolute, out uri))
                    {
                        _log.Message(LogLevel.Info, "[OnStart] - Connection settings loaded, Connection: {0}", connectionString);
                        _mpDisplayHost = new ServiceHost(typeof(MessageService), uri);

                        _log.Message(LogLevel.Info, "[OnStart] - Opening service host..");
                        _mpDisplayHost.Opened += (s, e) => _log.Message(LogLevel.Info, "[ServiceHost] - Service host successfuly opened.");
                        _mpDisplayHost.Closed += (s, e) => _log.Message(LogLevel.Info, "[ServiceHost] - Service host successfuly closed.");
                        _mpDisplayHost.Open();
                        return;
                    }
                    else
                    {
                        _log.Message(LogLevel.Error, "[OnStart] - Failed to create connection Uri, Uri: {0}", connectionString);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Exception("[OnStart] - An exception occured starting MPDisplay Server", ex);
            }
            Stop();
        }

        /// <summary>
        /// When implemented in a derived class, executes when a Stop command is sent to the service by the Service Control Manager (SCM). Specifies actions to take when a service stops running.
        /// </summary>
        protected override void OnStop()
        {
            _log.Message(LogLevel.Info, "[OnStop] - Stopping MPDisplay server...");
            if (_mpDisplayHost != null)
            {
                try
                {
                    _mpDisplayHost.Close();
                }
                catch
                {
                    // ignored
                }
            }
        }

        private ConnectionSettings GetConnectionSettings(string settingsFilename)
        {
            if (!File.Exists(settingsFilename))
            {
                _log.Message(LogLevel.Error, "[GetConnectionSettings] - File '{0}' does not exist, unable to load connection settings", settingsFilename);
                return null;
            }

            _log.Message(LogLevel.Info, "[GetConnectionSettings] - Loading settings file '{0}'", settingsFilename);
            var settings = SettingsManager.Load<MPDisplaySettings>(settingsFilename);
            if (settings != null && settings.PluginSettings != null && settings.PluginSettings.ConnectionSettings != null)
            {
                _log.Message(LogLevel.Info, "[GetConnectionSettings] - Sucessfully loaded settings file '{0}'", settingsFilename);
                return settings.PluginSettings.ConnectionSettings;
            }
            else
            {
                _log.Message(LogLevel.Error, "[GetConnectionSettings] - Failed to load settings file '{0}'", settingsFilename);
            }
            return null;
        }
    }
}
