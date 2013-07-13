using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MPDisplay.Common.Log;

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

        public void Initialize()
        {
           
        }



        public void Shutdown()
        {
        }
    }
}
