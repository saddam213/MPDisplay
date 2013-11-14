using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MPDisplay.Common.Log;
using System.IO;

namespace SkinEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static string _startupSkinInfoFilename = string.Empty;
        public static string StartupSkinInfoFilename
        {
            get { return _startupSkinInfoFilename; }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
         
            base.OnStartup(e);
            if (e.Args != null && e.Args.Any())
            {
                string skinInfoFile = e.Args[0];
                if (File.Exists(skinInfoFile))
                {
                    _startupSkinInfoFilename = skinInfoFile;
                }
            }
          
        }

        protected override void OnExit(ExitEventArgs e)
        {
           
            base.OnExit(e);
        }
    }
}
