using System.Windows;
using Common.Log;
using Common.Settings;

namespace GUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);


            LoggingManager.AddLog(new FileLogger(RegistrySettings.ProgramDataPath + "Logs", "MPDisplay", RegistrySettings.LogLevel));

        }
    }
}
