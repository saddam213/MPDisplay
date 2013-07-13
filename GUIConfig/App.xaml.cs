using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using MPDisplay.Common.Log;

namespace GUIConfig
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
#if DEBUG
            LoggingManager.AddLog(new WindowLogger("Config"));
#else
            LoggingManager.AddLog(new FileLogger(Environment.CurrentDirectory + "\\Logs", "Config"));
#endif
            base.OnStartup(e);
        }
    }
}
