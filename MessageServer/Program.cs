using System.ServiceProcess;

namespace MessageServer
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
			{ 
				new CommsService() 
			};
            ServiceBase.Run(ServicesToRun);
        }
    }
}
