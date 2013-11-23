using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Common.Logging;

namespace Common.Helpers
{
    public static class ServiceHelper
    {
        private static Log Log = LoggingManager.GetLog(typeof(ServiceHelper));

        /// <summary>
        /// Starts the customers interface if its not running
        /// </summary>
        /// <param name="customerName">The customer name of the interface to start</param>
        /// <returns>true if started, else false</returns>
        public static bool StartService(string serviceName, int timeout)
        {
            try
            {
                using (var service = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == serviceName))
                {
                    if (service != null)
                    {
                        if (service.Status != ServiceControllerStatus.Running)
                        {
                            service.Start();
                            service.WaitForStatus(ServiceControllerStatus.Running, new TimeSpan(0, 0, timeout));
                            if (service.Status == ServiceControllerStatus.Running)
                            {
                                return true;
                            }
                        }
                        else
                        {
                            service.Stop();
                            service.WaitForStatus(ServiceControllerStatus.Stopped, new TimeSpan(0, 0, timeout));

                            service.Start();
                            service.WaitForStatus(ServiceControllerStatus.Running, new TimeSpan(0, 0, timeout));
                            if (service.Status == ServiceControllerStatus.Running)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception("[StartService] - An exception occured starting service, Service: "+serviceName, ex);
            }
            return false;
        }

        public static bool StopService(string serviceName, int timeout)
        {
            using (var service = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == serviceName))
            {
                if (service != null)
                {
                    if (service.Status != ServiceControllerStatus.Stopped)
                    {
                        service.Stop();
                        service.WaitForStatus(ServiceControllerStatus.Stopped, new TimeSpan(0, 0, timeout));
                        return service.Status == ServiceControllerStatus.Stopped;
                    }
                }
                return true;
            }
        }


        /// <summary>
        /// Check if the customers interface is running
        /// </summary>
        /// <param name="customerName">The customer name to check</param>
        /// <returns>true if running, else false</returns>
        public static bool IsServiceRunning(string serviceName)
        {
            try
            {
                using (var service = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == serviceName))
                {
                    if (service != null)
                    {
                        return service.Status == ServiceControllerStatus.Running;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception("[IsServiceRunning] - An exception occured checking service status, Service: " + serviceName, ex);
            }
            return false;
        }

        public static ServiceStatus GetServiceStatus(string serviceName)
        {
            try
            {
                using (var service = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == serviceName))
                {
                    if (service != null)
                    {
                        return (ServiceStatus)Enum.Parse(typeof(ServiceStatus), service.Status.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception("[GetServiceStatus] - An exception occured fetching service status, Service: " + serviceName, ex);
            }
            return ServiceStatus.Stopped;
        }


      

        /// <summary>
        /// Check if the customer has an interface instace installed
        /// </summary>
        /// <param name="customerName">The customer name to check</param>
        /// <returns>True if interface exists, else false</returns>
        public static bool CheckIfServiceExists(string serviceName)
        {
            try
            {
                return ServiceController.GetServices().Any(s => s.ServiceName == serviceName);
            }
            catch (Exception ex)
            {
                Log.Exception("[CheckIfServiceExists] - An exception occured checking service, Service: " + serviceName, ex);
            }
            return false;
        }

     
    }


    // Summary:
    //     Indicates the current state of the service.
    public enum ServiceStatus
    {
        // Summary:
        //     The service is not running. This corresponds to the Win32 SERVICE_STOPPED
        //     constant, which is defined as 0x00000001.
        Stopped = 1,
        //
        // Summary:
        //     The service is starting. This corresponds to the Win32 SERVICE_START_PENDING
        //     constant, which is defined as 0x00000002.
        StartPending = 2,
        //
        // Summary:
        //     The service is stopping. This corresponds to the Win32 SERVICE_STOP_PENDING
        //     constant, which is defined as 0x00000003.
        StopPending = 3,
        //
        // Summary:
        //     The service is running. This corresponds to the Win32 SERVICE_RUNNING constant,
        //     which is defined as 0x00000004.
        Running = 4,
        //
        // Summary:
        //     The service continue is pending. This corresponds to the Win32 SERVICE_CONTINUE_PENDING
        //     constant, which is defined as 0x00000005.
        ContinuePending = 5,
        //
        // Summary:
        //     The service pause is pending. This corresponds to the Win32 SERVICE_PAUSE_PENDING
        //     constant, which is defined as 0x00000006.
        PausePending = 6,
        //
        // Summary:
        //     The service is paused. This corresponds to the Win32 SERVICE_PAUSED constant,
        //     which is defined as 0x00000007.
        Paused = 7,
    }
}
