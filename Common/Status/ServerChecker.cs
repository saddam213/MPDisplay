using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Management;
using System.Diagnostics;

namespace Common.Status
{
    /// <summary>
    /// Class that runs to get information from your system
    /// </summary>
    public class ServerChecker
    {
        public SystemInfo SystemInformation { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerChecker"/> class.
        /// This populates a SystemInfo object with data from the system
        /// </summary>
        public ServerChecker()
        {
            SystemInformation = new SystemInfo();
            GetOSVersion();
            GetStatsForAllDrives();
            GetMemoryStats();
            SystemInformation.CurrentCPUPercent = GetCPUCounter();
        }

        /// <summary>
        /// Gets the CPU counter by using a performance counter.
        /// </summary>
        /// <returns></returns>
        public float GetCPUCounter()
        {
            PerformanceCounter cpuCounter = new PerformanceCounter();
            cpuCounter.CategoryName = "Processor";
            cpuCounter.CounterName = "% Processor Time";
            cpuCounter.InstanceName = "_Total";

            // will always start at 0
            cpuCounter.NextValue();
            System.Threading.Thread.Sleep(1000);
            // now matches task manager reading
            var secondValue = cpuCounter.NextValue();

            return secondValue;
        }

        /// <summary>
        /// Gets the OS version by querying system property data
        /// </summary>
        private void GetOSVersion()
        {
            var outputPropertyData = new List<PropertyData>();

            foreach (ManagementObject a in (new ManagementObjectSearcher("SELECT * FROM  Win32_OperatingSystem")).Get())
            {
                foreach (PropertyData d in a.Properties)
                {
                    // Yes, really; Caption is the OS version property...
                    if (d.Name.Equals("Caption", StringComparison.OrdinalIgnoreCase))
                    {
                        SystemInformation.OSVersion = (d.Value as string).Trim();
                    }
                }
            }
        }

        /// <summary>
        /// Gets the stats for all drives.
        /// This is the Drive letter, Free space (GB) and Total space (GB)
        /// </summary>
        private void GetStatsForAllDrives()
        {
            var allDrives = System.IO.DriveInfo.GetDrives();
            foreach (var d in allDrives)
            {
                if (d.DriveType == DriveType.Fixed || d.DriveType == DriveType.Removable && d.IsReady)
                {
                    var info = new DriveInfo
                    {
                        DriveLetter = d.Name
                        ,
                        FreeSpaceGigaBytes = ConvertBytesToGigaBytes(d.TotalFreeSpace)
                        ,
                        TotalSpaceGigaBytes = ConvertBytesToGigaBytes(d.TotalSize)
                    };
                    SystemInformation.DriveInformation.Add(info);
                }
            }
        }

        //Convert bytes to GB
        private double ConvertBytesToGigaBytes(long Bytes)
        {
            return (Bytes / 1024 / 1024 / 1024);
        }


        /// <summary>
        /// Gets the memory stats.
        /// This is how much RAM and Virtual memory is there total and how much is used
        /// </summary>
        private void GetMemoryStats()
        {
            var searcher = new ManagementObjectSearcher(new System.Management.ObjectQuery("SELECT * FROM Win32_OperatingSystem"));
            var results = searcher.Get();

            foreach (var result in results)
            {
                SystemInformation.TotalRAMMegaBytes = double.Parse(result["TotalVisibleMemorySize"].ToString()) / 1024;
                SystemInformation.FreeRAMMegaBytes = double.Parse(result["FreePhysicalMemory"].ToString()) / 1024;
                SystemInformation.TotalVirtualMemoryMegaBytes = double.Parse(result["TotalVirtualMemorySize"].ToString()) / 1024;
                SystemInformation.FreeVirtualMemoryMegaBytes = double.Parse(result["FreeVirtualMemory"].ToString()) / 1024;
            }
        }
    }
}
