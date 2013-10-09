﻿using System;
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
        private DateTime _lastTimeUpdate = DateTime.MinValue;
        private System.Threading.Timer _updateTimer;
        private PerformanceCounter _cpuCounter;
        private ComputerInfo _computerInfo;
        private string _tagPrefix = "MPD";

        public event Action<string, string> OnTextDataChanged;
        public event Action<string, double> OnNumberDataChanged;

        public string TagPrefix
        {
            get { return _tagPrefix; }
            set { _tagPrefix = value; }
        }

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
                NotifyTextDataChanged("PhysicalTotal",  ((long)_computerInfo.TotalPhysicalMemory).ToPrettySize(2));
                NotifyTextDataChanged("PhysicalFree", ((long)_computerInfo.AvailablePhysicalMemory).ToPrettySize(2));
                NotifyTextDataChanged("PhysicalPercent", string.Format("{0:##0} %", physPercentFree));
                NotifyNumberDataChanged("PhysicalPercent", physPercentFree);

                double vertPercentFree = Math.Round(100 * (double)_computerInfo.AvailableVirtualMemory / _computerInfo.TotalVirtualMemory,2);
                NotifyTextDataChanged("VirtualTotal", ((long)_computerInfo.TotalVirtualMemory).ToPrettySize(2));
                NotifyTextDataChanged("VirtualFree",  ((long)_computerInfo.AvailableVirtualMemory).ToPrettySize(2));
                NotifyTextDataChanged("VirtualPercent", string.Format("{0:##0} %", vertPercentFree));
                NotifyNumberDataChanged("VirtualPercent", vertPercentFree);
            }

            if (DateTime.Now > _lastFastUpdate.AddMilliseconds(250))
            {
                _lastFastUpdate = DateTime.Now;
                double value = Math.Round(_cpuCounter.NextValue(),2);
                NotifyTextDataChanged("CPU", string.Format("{0:##0} %", value));
                NotifyNumberDataChanged("CPU", value);
                UpdateTime(_lastFastUpdate);
            }
        }

        /// <summary>
        /// Updates the system info.
        /// </summary>
        private void UpdateSystemInfo()
        {
            NotifyTextDataChanged("OSFullName", _computerInfo.OSFullName);
            NotifyTextDataChanged("OSPlatform", _computerInfo.OSPlatform);
            NotifyTextDataChanged("OSVersion", _computerInfo.OSVersion);
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
                    NotifyTextDataChanged(string.Format("Drive{0}.Name", num), d.Name);
                    NotifyTextDataChanged(string.Format("Drive{0}.TotalSpace", num), d.TotalFreeSpace.ToPrettySize(2));
                    NotifyTextDataChanged(string.Format("Drive{0}.FreeSpace", num), d.AvailableFreeSpace.ToPrettySize(2));
                    double percentFree = Math.Round(100 * (double)d.TotalFreeSpace / d.TotalSize,2);
                    NotifyTextDataChanged(string.Format("Drive{0}.PercentFree", num), percentFree.ToString());
                    NotifyNumberDataChanged("Drive{0}.PercentFree", percentFree);
                    num++;
                }
            }
        }

        /// <summary>
        /// Updates the time.
        /// </summary>
        /// <param name="datetime">The datetime.</param>
        private void UpdateTime(DateTime datetime)
        {
            if (_lastTimeUpdate.ToLongDateString() != datetime.ToLongDateString())
            {
                NotifyTextDataChanged("Date", datetime.ToLongDateString());
                NotifyTextDataChanged("Date1", datetime.ToString("ddd d MMM"));
                NotifyTextDataChanged("Date2", datetime.ToString("dddd dd MMMM"));
                NotifyTextDataChanged("Date3", datetime.ToString("MM/dd/yyyy"));
                NotifyTextDataChanged("DateMonth", datetime.Month.ToString());
                NotifyTextDataChanged("DateMonthShort", datetime.ToString("MMM"));
                NotifyTextDataChanged("DateMonthLong", datetime.ToString("MMMM"));
                NotifyTextDataChanged("DateDay", datetime.Day.ToString());
                NotifyTextDataChanged("DateDayShort", datetime.ToString("dddd"));
                NotifyTextDataChanged("DateDayLong", datetime.ToString("ddd"));
                NotifyTextDataChanged("DateYear", datetime.ToString("yyyy"));
                NotifyTextDataChanged("DateShort", datetime.ToString("yy"));
            }
            if (_lastTimeUpdate.Second != datetime.Second)
            {
                NotifyTextDataChanged("Time", datetime.ToString("H:mm tt"));
                NotifyTextDataChanged("Time2", datetime.ToString("HH:mm"));
                NotifyTextDataChanged("Time3", datetime.ToString("H:mm:ss tt"));
                NotifyTextDataChanged("Time4", datetime.ToString("HH:mm:ss"));
            }
            _lastTimeUpdate = datetime;
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
                OnTextDataChanged(string.Format("#{0}.SystemInfo.Label.{1}", _tagPrefix, tag), tagValue);
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
                OnNumberDataChanged(string.Format("#{0}.SystemInfo.Number.{1}", _tagPrefix, tag), tagValue);
            }
        }

      
    }
}
