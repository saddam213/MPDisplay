using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MessageFramework.DataObjects;
using MPDisplay.Common.Log;
using MPDisplay.Common.Settings;

namespace MediaPortalPlugin.InfoManagers
{
    public class DialogManager
    {
        #region Singleton Implementation

        private static DialogManager instance;

        private DialogManager()
        {
            Log = MPDisplay.Common.Log.LoggingManager.GetLog(typeof(DialogManager));
        }

        public static DialogManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DialogManager();
                }
                return instance;
            }
        }

        #endregion

        private MPDisplay.Common.Log.Log Log;
        private PluginSettings _settings;

        public void Initialize(PluginSettings settings)
        {
            _settings = settings;
        }

        public void Shutdown()
        {
        }

        public void RegisterDialogInfo(APIWindowInfoMessage message)
        {
            
        }
    }
}
