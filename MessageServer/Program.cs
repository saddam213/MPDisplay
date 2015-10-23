using System.ServiceProcess;

namespace MessageServer
{
    class Program
    {
        static void Main()
        {
            var servicesToRun = new ServiceBase[] 
            { 
                new CommsService() 
            };
            ServiceBase.Run(servicesToRun);
        }
    }
}
