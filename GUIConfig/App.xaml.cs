using System.Windows;
using Common.Log;
using Common.Settings;

namespace GUIConfig
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            LoggingManager.AddLog(new FileLogger(RegistrySettings.ProgramDataPath + "Logs", "Config",RegistrySettings.LogLevel));
        }
    }
}
