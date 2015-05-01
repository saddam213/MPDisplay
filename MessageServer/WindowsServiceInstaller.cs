using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace MessageServer
{
    [RunInstaller(true)]
    public class WindowsServiceInstaller : Installer
    {
        /// <summary>
        /// Public Constructor for WindowsServiceInstaller.
        /// - Put all of your Initialization code here.
        /// </summary>
        public WindowsServiceInstaller()
        {
            ServiceProcessInstaller serviceProcessInstaller = new ServiceProcessInstaller();
            ServiceInstaller serviceInstaller = new ServiceInstaller();

            //# Service Account Information
            serviceProcessInstaller.Account = ServiceAccount.LocalService;
          
            //# Service Information
            serviceInstaller.DisplayName = "MPDisplayServer";
            serviceInstaller.StartType = ServiceStartMode.Automatic;
            serviceInstaller.Description = "MPDisplay Communication Server";
            //# This must be identical to the WindowsService.ServiceBase name
            //# set in the constructor of WindowsService.cs
            serviceInstaller.ServiceName = "MPDisplayServer";
            Installers.Add(serviceProcessInstaller);
            Installers.Add(serviceInstaller);
        }
    }
}