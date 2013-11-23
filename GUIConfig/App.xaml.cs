﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Common.Logging;
using Common.Settings;

namespace GUIConfig
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            LoggingManager.AddLog(new FileLogger(RegistrySettings.ProgramDataPath + "Logs", "Config",RegistrySettings.LogLevel));
        }
    }
}
