using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using MediaPortal.Configuration;
using MediaPortal.GUI.Library;
using MediaPortal.Player;
using MediaPortalPlugin.InfoManagers;
using MessageFramework.DataObjects;
using MPDisplay.Common.Log;
using MPDisplay.Common.Settings;

namespace MediaPortalPlugin
{
    [PluginIcons("MediaPortalPlugin.Resources.Enabled.png", "MediaPortalPlugin.Resources.Disable.png")]
    public class MPDisplayPlugin : IPlugin, ISetupForm
    {
        private PluginSettings _settings;
        private MPDisplay.Common.Log.Log Log;

        private void OnPluginStart()
        {
          
            LoggingManager.AddLog(new FileLogger(RegistrySettings.ProgramDataPath + "Logs", "Plugin"));
            Log = LoggingManager.GetLog(typeof(MPDisplayPlugin));
            var settings = SettingsManager.Load<MPDisplaySettings>(RegistrySettings.MPDisplaySettingsFile);
            if (settings == null)
            {
                settings = new MPDisplaySettings();
                SettingsManager.Save<MPDisplaySettings>(settings, RegistrySettings.MPDisplaySettingsFile);
            }
            _settings = settings.PluginSettings;

            if (_settings != null)
            {
                MessageService.Instance.InitializeConnection(_settings.ConnectionSettings);
                WindowManager.Instance.Initialize(_settings);

             
            }
        }

        private void OnPluginStop()
        {
          
            WindowManager.Instance.Shutdown();
            MessageService.Instance.Disconnect();
            LoggingManager.Destroy();
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
