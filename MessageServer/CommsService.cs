using System;
using System.IO;
using System.ServiceModel;
using System.ServiceProcess;
using Microsoft.Win32;
using MPDisplay.Common.Log;
using MPDisplay.Common.Settings;

namespace MessageServer
{
    partial class CommsService : ServiceBase
    {
        private ServiceHost MPDisplayHost;
        private Log Log = LoggingManager.GetLog(typeof(CommsService));
     

        /// <summary>
        /// Initializes a new instance of the <see cref="CommsService"/> class.
        /// </summary>
        public CommsService()
        {
            LoggingManager.AddLog(new FileLogger(RegistrySettings.ProgramDataPath + "Logs", "Server"));
            InitializeComponent();
        }

        /// <summary>
        /// When implemented in a derived class, executes when a Start command is sent to the service by the Service Control Manager (SCM) or when the operating system starts (for a service that starts automatically). Specifies actions to take when the service starts.
        /// </summary>
        /// <param name="args">Data passed by the start command.</param>
        protected override void OnStart(string[] args)
        {
            Log.Message(LogLevel.Info, "[OnStart] - Starting MPDisplay server...");
            try
            {
                var settings = GetConnectionSettings(RegistrySettings.MPDisplaySettingsFile);
                if (settings != null)
                {
                    Uri uri = null;
                    string connectionString = string.Format("net.tcp://{0}:{1}/MPDisplayService", settings.IpAddress, settings.Port);
                    if (Uri.TryCreate(connectionString, UriKind.RelativeOrAbsolute, out uri))
                    {
                        Log.Message(LogLevel.Info, "[OnStart] - Connection settings loaded, Connection: {0}", connectionString);
                        MPDisplayHost = new ServiceHost(typeof(MessageService), uri);

                        Log.Message(LogLevel.Info, "[OnStart] - Opening service host..");
                        MPDisplayHost.Opened += (s, e) => Log.Message(LogLevel.Info, "[ServiceHost] - Service host successfuly opened.");
                        MPDisplayHost.Closed += (s, e) => Log.Message(LogLevel.Info, "[ServiceHost] - Service host successfuly closed.");
                        MPDisplayHost.Open();
                        return;
                    }
                    else
                    {
                        Log.Message(LogLevel.Error, "[OnStart] - Failed to create connection Uri, Uri: {0}", connectionString);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception("[OnStart] - An exception occured starting MPDisplay Server", ex);
            }
            Stop();
        }

        /// <summary>
        /// When implemented in a derived class, executes when a Stop command is sent to the service by the Service Control Manager (SCM). Specifies actions to take when a service stops running.
        /// </summary>
        protected override void OnStop()
        {
            Log.Message(LogLevel.Info, "[OnStop] - Stopping MPDisplay server...");
            if (MPDisplayHost != null)
            {
                try
                {
                    MPDisplayHost.Close();
                }
                catch
                {

                }
            }
        }

        private ConnectionSettings GetConnectionSettings(string settingsFilename)
        {
            if (!File.Exists(settingsFilename))
            {
                Log.Message(LogLevel.Error, "[GetConnectionSettings] - File '{0}' does not exist, unable to load connection settings", settingsFilename);
                return null;
            }

            Log.Message(LogLevel.Info, "[GetConnectionSettings] - Loading settings file '{0}'", settingsFilename);
            var settings = SettingsManager.Load<MPDisplaySettings>(settingsFilename);
            if (settings != null && settings.PluginSettings != null && settings.PluginSettings.ConnectionSettings != null)
            {
                Log.Message(LogLevel.Info, "[GetConnectionSettings] - Sucessfully loaded settings file '{0}'", settingsFilename);
                return settings.PluginSettings.ConnectionSettings;
            }
            else
            {
                Log.Message(LogLevel.Error, "[GetConnectionSettings] - Failed to load settings file '{0}'", settingsFilename);
            }
            return null;
        }
    }
}
