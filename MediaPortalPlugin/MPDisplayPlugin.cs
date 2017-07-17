using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Common.Log;
using Common.Settings;
using MediaPortal.Configuration;
using MediaPortal.GUI.Library;
using MediaPortalPlugin.InfoManagers;
using Log = Common.Log.Log;



namespace MediaPortalPlugin
{
    [PluginIcons("MediaPortalPlugin.Resources.Enabled.png", "MediaPortalPlugin.Resources.Disable.png")]
    public class MpDisplayPlugin : IPlugin, ISetupForm
    {
        private readonly PluginSettings _settings;
        private readonly AdvancedPluginSettings _advancedSettings;
        private readonly AddImageSettings _addImageSettings;
        private readonly Log _log;

        public MpDisplayPlugin()
        {

            LoggingManager.AddLog(new FileLogger(RegistrySettings.ProgramDataPath + "Logs", "Plugin", RegistrySettings.LogLevel));
            _log = LoggingManager.GetLog(typeof(MpDisplayPlugin));

            AppDomain.CurrentDomain.AssemblyResolve += delegate(object sender, ResolveEventArgs e)
            {
                try
                {
                    string partialName = e.Name.Substring(0, e.Name.IndexOf(','));
                    return Assembly.Load(new AssemblyName(partialName));

                }
                catch 
                {
                    _log.Message(LogLevel.Info, "[AssemblyResover] - Cannot redirect assembly: {0}", e.Name);
                    return null;
                }

            };

            _log.Message(LogLevel.Info, "[OnPluginStart] - Loading MPDisplay settings file: {0}", RegistrySettings.MPDisplaySettingsFile);
            var settings = SettingsManager.Load<MPDisplaySettings>(RegistrySettings.MPDisplaySettingsFile);
            if (settings == null)
            {
                _log.Message(LogLevel.Info, "[OnPluginStart] - Settings file for MPDisplay not found, Loading defaults..");
                settings = new MPDisplaySettings();
                SettingsManager.Save(settings, RegistrySettings.MPDisplaySettingsFile);
            }
            var advancedSettings = SettingsManager.Load<AdvancedPluginSettings>(Path.Combine(RegistrySettings.ProgramDataPath, "AdvancedPluginSettings.xml"));
            if (advancedSettings == null)
            {
                _log.Message(LogLevel.Info, "[OnPluginStart] - Settings file AdvancedPluginSettings not found, Loading defaults..");
                advancedSettings = new AdvancedPluginSettings();
            }
            var addImageSettings = SettingsManager.Load<AddImageSettings>(Path.Combine(RegistrySettings.ProgramDataPath, "AddImageSettings.xml"));
            if (addImageSettings == null)
            {
                _log.Message(LogLevel.Info, "[OnPluginStart] - Settings file AddImageSettings not found, Loading defaults..");
                addImageSettings = new AddImageSettings();
            }

            _advancedSettings = advancedSettings;
            _addImageSettings = addImageSettings;
            _settings = settings.PluginSettings;

            _addImageSettings.InitAddImagePropertySettings(Config.GetFolder(Config.Dir.Thumbs));
        }

        private void OnPluginStart()
        {
            if (_settings != null)
            {
                _log.Message(LogLevel.Info, "[OnPluginStart] - Starting MPDisplay Plugin...");
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
                WindowManager.Instance.Initialize(_settings, _advancedSettings, _addImageSettings);
                TvServerManager.Instance.Initialize(_settings);
                _log.Message(LogLevel.Info, "[OnPluginStart] - MPDisplay Plugin started.");
              
            }
            else
            {
                _log.Message(LogLevel.Error, "[OnPluginStart] - Failed to create settings file, Stopping plugin.");
                LoggingManager.Destroy();
            }
        }

     

        private void OnPluginStop()
        {
         
            _log.Message(LogLevel.Info, "[OnPluginStop] - Stopping MPDisplay Plugin...");
            if (_settings.CloseMPDisplayOnExit)
            {
                Process.GetProcessesByName("MPDisplay").CloseAll();
            }
            MessageService.Instance.Shutdown();
            WindowManager.Instance.Shutdown();
        
            _log.Message(LogLevel.Info, "[OnPluginStop] - MPDisplay Plugin stopped.");
            LoggingManager.Destroy();
        }

        private void OnShowConfigForm()
        {
            if (File.Exists(RegistrySettings.MPDisplayConfigExePath))
            {
                _log.Message(LogLevel.Info, "[OnShowConfigForm] - Launching MPDisplay configuration...");
                Process.Start(RegistrySettings.MPDisplayConfigExePath);
            }
            else
            {
                _log.Message(LogLevel.Error, "[OnShowConfigForm] - Failed to start MPDisplay configuration, File not found: {0}", RegistrySettings.MPDisplayConfigExePath);
                MessageBox.Show(
                    $"Failed to start MPDisplay configuration, File not found: {RegistrySettings.MPDisplayConfigExePath}", "Error");
            }
        }

        private static void OnCloseConfigForm()
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
