using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Management;
using System.Diagnostics;
using System.Threading;
using Microsoft.VisualBasic.Devices;

namespace Common.Status
{
    /// <summary>
    /// Class that runs to get information from your system
    /// </summary>
    public class ServerChecker
    {
        private DateTime _lastFastUpdate = DateTime.MinValue;
        private DateTime _lastMediumUpdate = DateTime.MinValue;
        private DateTime _lastSlowUpdate = DateTime.MinValue;
        private System.Threading.Timer _updateTimer;
        private PerformanceCounter _cpuCounter;
        private ComputerInfo _computerInfo;

        public event Action<string, string> OnTextDataChanged;
        public event Action<string, double> OnNumberDataChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerChecker"/> class.
        /// This populates a SystemInfo object with data from the system
        /// </summary>
        public ServerChecker()
        {
           _computerInfo  = new ComputerInfo();
           _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
           _cpuCounter.NextValue();
        }

        /// <summary>
        /// Starts the monitoring.
        /// </summary>
        public void StartMonitoring()
        {
            if (_updateTimer == null)
            {
                _updateTimer = new System.Threading.Timer((o) => OnTimerTick(), null, 250, 250);
            }
        }

        /// <summary>
        /// Stops the monitoring.
        /// </summary>
        public void StopMonitoring()
        {
            if (_updateTimer != null)
            {
                _updateTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        /// <summary>
        /// Called when [timer tick].
        /// </summary>
        private void OnTimerTick()
        {
            if (DateTime.Now > _lastSlowUpdate.AddMinutes(1))
            {
                _lastSlowUpdate = DateTime.Now;
                UpdateDriveInfo();
                UpdateSystemInfo();
            }

            if (DateTime.Now > _lastMediumUpdate.AddSeconds(5))
            {
                _lastMediumUpdate = DateTime.Now;

                double physPercentFree = Math.Round(100 * (double)_computerInfo.AvailablePhysicalMemory / _computerInfo.TotalPhysicalMemory,2);
                NotifyTextDataChanged("#MPD.SystemInfo.Label.PhysicalTotal",  ((long)_computerInfo.TotalPhysicalMemory).ToPrettySize(2));
                NotifyTextDataChanged("#MPD.SystemInfo.Label.PhysicalFree", ((long)_computerInfo.AvailablePhysicalMemory).ToPrettySize(2));
                NotifyTextDataChanged("#MPD.SystemInfo.Label.PhysicalPercent", string.Format("{0:##0} %", physPercentFree));
                NotifyNumberDataChanged("#MPD.SystemInfo.Number.PhysicalPercent", physPercentFree);

                double vertPercentFree = Math.Round(100 * (double)_computerInfo.AvailableVirtualMemory / _computerInfo.TotalVirtualMemory,2);
                NotifyTextDataChanged("#MPD.SystemInfo.Label.VirtualTotal", ((long)_computerInfo.TotalVirtualMemory).ToPrettySize(2));
                NotifyTextDataChanged("#MPD.SystemInfo.Label.VirtualFree",  ((long)_computerInfo.AvailableVirtualMemory).ToPrettySize(2));
                NotifyTextDataChanged("#MPD.SystemInfo.Label.VirtualPercent", string.Format("{0:##0} %", vertPercentFree));
                NotifyNumberDataChanged("#MPD.SystemInfo.Number.VirtualPercent", vertPercentFree);
            }

            if (DateTime.Now > _lastFastUpdate.AddMilliseconds(250))
            {
                _lastFastUpdate = DateTime.Now;
                double value = Math.Round(_cpuCounter.NextValue(),2);
                NotifyTextDataChanged("#MPD.SystemInfo.Label.CPU", string.Format("{0:##0} %", value));
                NotifyNumberDataChanged("#MPD.SystemInfo.Number.CPU", value);
            }
        }

        /// <summary>
        /// Updates the system info.
        /// </summary>
        private void UpdateSystemInfo()
        {
            NotifyTextDataChanged("#MPD.SystemInfo.Label.OSFullName", _computerInfo.OSFullName);
            NotifyTextDataChanged("#MPD.SystemInfo.Label.OSPlatform", _computerInfo.OSPlatform);
            NotifyTextDataChanged("#MPD.SystemInfo.Label.OSVersion", _computerInfo.OSVersion);
        }

        /// <summary>
        /// Updates the drive info.
        /// </summary>
        private void UpdateDriveInfo()
        {
            var allDrives = System.IO.DriveInfo.GetDrives();
            int num = 0;
            foreach (var d in allDrives)
            {
                if ( d.IsReady && d.DriveType == DriveType.Fixed)
                {
                    NotifyTextDataChanged(string.Format("#MPD.SystemInfo.Label.Drive{0}.Name", num), d.Name);
                    NotifyTextDataChanged(string.Format("#MPD.SystemInfo.Label.Drive{0}.TotalSpace", num), d.TotalFreeSpace.ToPrettySize(2));
                    NotifyTextDataChanged(string.Format("#MPD.SystemInfo.Label.Drive{0}.FreeSpace", num), d.AvailableFreeSpace.ToPrettySize(2));
                    double percentFree = Math.Round(100 * (double)d.TotalFreeSpace / d.TotalSize,2);
                    NotifyTextDataChanged(string.Format("#MPD.SystemInfo.Label.Drive{0}.PercentFree", num), percentFree.ToString());
                    NotifyNumberDataChanged("#MPD.SystemInfo.Drive{0}.Number.PercentFree", percentFree);
                    num++;
                }
            }
        }

        /// <summary>
        /// Notifies the text data changed.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="tagValue">The tag value.</param>
        private void NotifyTextDataChanged(string tag, string tagValue)
        {
            if (OnTextDataChanged != null)
            {
                OnTextDataChanged(tag, tagValue);
            }
        }

        /// <summary>
        /// Notifies the number data changed.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="tagValue">The tag value.</param>
        private void NotifyNumberDataChanged(string tag, double tagValue)
        {
            if (OnNumberDataChanged != null)
            {
                OnNumberDataChanged(tag, tagValue);
            }
        }
    }
}
