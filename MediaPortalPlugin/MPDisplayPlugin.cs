using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using MediaPortal.Configuration;
using MediaPortal.GUI.Library;
using MediaPortal.Player;
using MediaPortalPlugin.InfoManagers;
using MessageFramework.DataObjects;
using Microsoft.Win32;
using Common.Logging;
using Common.Settings;


namespace MediaPortalPlugin
{
    [PluginIcons("MediaPortalPlugin.Resources.Enabled.png", "MediaPortalPlugin.Resources.Disable.png")]
    public class MPDisplayPlugin : IPlugin, ISetupForm
    {
        private PluginSettings _settings;
        private AdvancedPluginSettings _advancedSettings;
        private Common.Logging.Log Log;

        public MPDisplayPlugin()
        {
            LoggingManager.AddLog(new FileLogger(RegistrySettings.ProgramDataPath + "Logs", "Plugin", RegistrySettings.LogLevel));
            Log = LoggingManager.GetLog(typeof(MPDisplayPlugin));

            Log.Message(LogLevel.Info, "[OnPluginStart] - Loading MPDisplay settings file: {0}", RegistrySettings.MPDisplaySettingsFile);
            var settings = SettingsManager.Load<MPDisplaySettings>(RegistrySettings.MPDisplaySettingsFile);
            if (settings == null)
            {
                Log.Message(LogLevel.Info, "[OnPluginStart] - Settings file not found, Loading defaults..");
                settings = new MPDisplaySettings();
                SettingsManager.Save<MPDisplaySettings>(settings, RegistrySettings.MPDisplaySettingsFile);
            }
            var advancedSettings = SettingsManager.Load<AdvancedPluginSettings>(Path.Combine(RegistrySettings.ProgramDataPath, "AdvancedPluginSettings.xml"));
            if (advancedSettings == null)
            {
                Log.Message(LogLevel.Info, "[OnPluginStart] - Settings file not found, Loading defaults..");
                advancedSettings = new AdvancedPluginSettings();
              //  SettingsManager.Save<AdvancedPluginSettings>(advancedSettings, Path.Combine(RegistrySettings.ProgramDataPath, "AdvancedPluginSettings.xml"));
            }
            _advancedSettings = advancedSettings;
            _settings = settings.PluginSettings;
        }

        private void OnPluginStart()
        {
            if (_settings != null)
            {
                Log.Message(LogLevel.Info, "[OnPluginStart] - Starting MPDisplay Plugin...");
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
                WindowManager.Instance.Initialize(_settings, _advancedSettings);
                TVServerManager.Instance.Initialize(_settings);
                Log.Message(LogLevel.Info, "[OnPluginStart] - MPDisplay Plugin started.");
              
            }
            else
            {
                Log.Message(LogLevel.Error, "[OnPluginStart] - Failed to create settings file, Stopping plugin.");
                LoggingManager.Destroy();
            }
        }

     

        private void OnPluginStop()
        {
         
            Log.Message(LogLevel.Info, "[OnPluginStop] - Stopping MPDisplay Plugin...");
            if (_settings.CloseMPDisplayOnExit)
            {
                Process.GetProcessesByName("MPDisplay").CloseAll(true);
            }
            MessageService.Instance.Shutdown();
            WindowManager.Instance.Shutdown();
        
            Log.Message(LogLevel.Info, "[OnPluginStop] - MPDisplay Plugin stopped.");
            LoggingManager.Destroy();
        }

        private void OnShowConfigForm()
        {
            if (File.Exists(RegistrySettings.MPDisplayConfigExePath))
            {
                Log.Message(LogLevel.Info, "[OnShowConfigForm] - Launching MPDisplay configuration...");
                Process.Start(RegistrySettings.MPDisplayConfigExePath);
            }
            else
            {
                Log.Message(LogLevel.Error, "[OnShowConfigForm] - Failed to start MPDisplay configuration, File not found: {0}", RegistrySettings.MPDisplayConfigExePath);
                MessageBox.Show(string.Format("Failed to start MPDisplay configuration, File not found: {0}", RegistrySettings.MPDisplayConfigExePath), "Error");
            }
        }

        private void OnCloseConfigForm()
        {

        }

        #region IPlugin Implementation

        /// <summary>
        /// Authors this instance.
        /// </summary>
        /// <returns></returns>
        public string Author()
        {
            return "Sa_ddam213(Developer), WonderMusic(Skin Designer)";
        }

        /// <summary>
        /// Determines whether this instance can enable.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance can enable; otherwise, <c>false</c>.
        /// </returns>
        public bool CanEnable()
        {
            return true;
        }

        /// <summary>
        /// Defaults the enabled.
        /// </summary>
        /// <returns></returns>
        public bool DefaultEnabled()
        {
            return true;
        }

        /// <summary>
        /// Descriptions this instance.
        /// </summary>
        /// <returns></returns>
        public string Description()
        {
            return "MPDisplay++ is s second GUI for MediaPortal for multiple external monitors/touchscreens, Customizable to show the information you want.";
        }

        /// <summary>
        /// Gets the home.
        /// </summary>
        /// <param name="strButtonText">The STR button text.</param>
        /// <param name="strButtonImage">The STR button image.</param>
        /// <param name="strButtonImageFocus">The STR button image focus.</param>
        /// <param name="strPictureImage">The STR picture image.</param>
        /// <returns></returns>
        public bool GetHome(out string strButtonText, out string strButtonImage, out string strButtonImageFocus, out string strPictureImage)
        {
            strButtonText = string.Empty;
            strButtonImage = string.Empty;
            strButtonImageFocus = string.Empty;
            strPictureImage = string.Empty;
            return false;
        }

        /// <summary>
        /// Gets the window id.
        /// </summary>
        /// <returns></returns>
        public int GetWindowId()
        {
            return 0;
        }

        /// <summary>
        /// Determines whether this instance has setup.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance has setup; otherwise, <c>false</c>.
        /// </returns>
        public bool HasSetup()
        {
            return true;
        }

        /// <summary>
        /// Plugins the name.
        /// </summary>
        /// <returns></returns>
        public string PluginName()
        {
            return "MPDisplay++";
        }

        /// <summary>
        /// Shows the plugin.
        /// </summary>
        public void ShowPlugin()
        {
            OnShowConfigForm();
            OnCloseConfigForm();
        }


        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            OnPluginStop();
        }

     

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            OnPluginStart();
        }

        #endregion
    }


  



 
}
