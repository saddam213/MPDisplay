using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Status
{
    public class SystemInfo
    {
        public string OSVersion { get; set; }
        public float CurrentCPUPercent { get; set; }
        public double TotalRAMMegaBytes { get; set; }
        public double FreeRAMMegaBytes { get; set; }
        public double TotalVirtualMemoryMegaBytes { get; set; }
        public double FreeVirtualMemoryMegaBytes { get; set; }
        public List<DriveInfo> DriveInformation { get; set; }

        public SystemInfo()
        {
            DriveInformation = new List<DriveInfo>();
        }
    }

    public class DriveInfo
    {
        public double TotalSpaceGigaBytes { get; set; }
        public double FreeSpaceGigaBytes { get; set; }
        public string DriveLetter { get; set; }
    }
}
