﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Common.Helpers;
using Common.Log;
using Microsoft.VisualBasic.Devices;

namespace Common.Status
{
    /// <summary>
    /// Class that runs to get information from your system
    /// </summary>
    public class SystemStatusInfo
    {
        private readonly Log.Log _log = LoggingManager.GetLog(typeof(SystemStatusInfo));
        private DateTime _lastFastUpdate = DateTime.MinValue;
        private DateTime _lastMediumUpdate = DateTime.MinValue;
        private DateTime _lastSlowUpdate = DateTime.MinValue;
        private DateTime _lastTimeUpdate = DateTime.MinValue;
        private Timer _updateTimer;
        private readonly PerformanceCounter _cpuCounter;
        private readonly ComputerInfo _computerInfo;
        private string _tagPrefix = "MPD";

        public event Action<string, string> OnTextDataChanged;
        public event Action<string, double> OnNumberDataChanged;

        public string TagPrefix
        {
            get { return _tagPrefix; }
            set { _tagPrefix = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemStatusInfo"/> class.
        /// This populates a SystemInfo object with data from the system
        /// </summary>
        public SystemStatusInfo()
        {
           _computerInfo  = new ComputerInfo();
           try
           {
               _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
               _cpuCounter.NextValue();
           }
           catch (Exception ex)
           {
               _log.Message(LogLevel.Error, "Unable to start CPU performance counter, MPDisplay does not have the required permission/rights{0}{1}", Environment.NewLine, ex.Message);
           }
        }

        /// <summary>
        /// Starts the monitoring.
        /// </summary>
        public void StartMonitoring()
        {
            if (_updateTimer == null)
            {
                _updateTimer = new Timer(o => OnTimerTick(), null, 250, 250);
            }
        }

        /// <summary>
        /// Stops the monitoring.
        /// </summary>
        public void StopMonitoring()
        {
            _updateTimer?.Change(Timeout.Infinite, Timeout.Infinite);
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

                var physPercentFree = Math.Round(100 * (double)_computerInfo.AvailablePhysicalMemory / _computerInfo.TotalPhysicalMemory,2);
                NotifyTextDataChanged("PhysicalTotal",  ((long)_computerInfo.TotalPhysicalMemory).ToPrettySize(2));
                NotifyTextDataChanged("PhysicalFree", ((long)_computerInfo.AvailablePhysicalMemory).ToPrettySize(2));
                NotifyTextDataChanged("PhysicalPercent", $"{physPercentFree:##0} %");
                NotifyNumberDataChanged("PhysicalPercent", physPercentFree);

                var vertPercentFree = Math.Round(100 * (double)_computerInfo.AvailableVirtualMemory / _computerInfo.TotalVirtualMemory,2);
                NotifyTextDataChanged("VirtualTotal", ((long)_computerInfo.TotalVirtualMemory).ToPrettySize(2));
                NotifyTextDataChanged("VirtualFree",  ((long)_computerInfo.AvailableVirtualMemory).ToPrettySize(2));
                NotifyTextDataChanged("VirtualPercent", $"{vertPercentFree:##0} %");
                NotifyNumberDataChanged("VirtualPercent", vertPercentFree);
            }

            if (DateTime.Now <= _lastFastUpdate.AddMilliseconds(500)) return;
            _lastFastUpdate = DateTime.Now;
            var value = _cpuCounter != null ? Math.Round(_cpuCounter.NextValue(),2) : 0.0;
            NotifyTextDataChanged("CPU", $"{value:##0} %");
            NotifyNumberDataChanged("CPU", value);
            UpdateTime(_lastFastUpdate);
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
            var num = 0;
            foreach (var d in allDrives.Where(d => d.IsReady && d.DriveType == DriveType.Fixed))
            {
                NotifyTextDataChanged($"Drive{num}.Name", d.Name);
                NotifyTextDataChanged($"Drive{num}.VolumeLabel", d.VolumeLabel);
                NotifyTextDataChanged($"Drive{num}.TotalSpace", d.TotalSize.ToPrettySize(2));
                NotifyTextDataChanged($"Drive{num}.FreeSpace", d.AvailableFreeSpace.ToPrettySize(2));
                var percentFree = Math.Round(100 * (double)d.TotalFreeSpace / d.TotalSize,2);
                NotifyTextDataChanged($"Drive{num}.PercentFree", percentFree.ToString(CultureInfo.InvariantCulture) + " %");
                NotifyNumberDataChanged($"Drive{num}.PercentFree", percentFree);
                num++;
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
                NotifyTextDataChanged("Date2", datetime.ToString("ddd d MMM"));
                NotifyTextDataChanged("Date3", datetime.ToString("dddd dd MMMM"));
                NotifyTextDataChanged("Date4", datetime.ToString("MM/dd/yyyy"));
                NotifyTextDataChanged("DateMonth", datetime.Month.ToString());
                NotifyTextDataChanged("DateMonthShort", datetime.ToString("MMM"));
                NotifyTextDataChanged("DateMonthLong", datetime.ToString("MMMM"));
                NotifyTextDataChanged("DateDay", datetime.Day.ToString());
                NotifyTextDataChanged("DateDayShort", datetime.ToString("ddd"));
                NotifyTextDataChanged("DateDayLong", datetime.ToString("dddy"));
                NotifyTextDataChanged("DateYear", datetime.ToString("yyyy"));
                NotifyTextDataChanged("DateYearShort", datetime.ToString("yy"));
            }
            if (_lastTimeUpdate.Second != datetime.Second)
            {
                NotifyTextDataChanged("Time", datetime.ToString("h:mm tt"));
                NotifyTextDataChanged("Time2", datetime.ToString("HH:mm"));
                NotifyTextDataChanged("Time3", datetime.ToString("h:mm:ss tt"));
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
            OnTextDataChanged?.Invoke($"#{_tagPrefix}.SystemInfo.Label.{tag}", tagValue);
        }

        /// <summary>
        /// Notifies the number data changed.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="tagValue">The tag value.</param>
        private void NotifyNumberDataChanged(string tag, double tagValue)
        {
            OnNumberDataChanged?.Invoke($"#{_tagPrefix}.SystemInfo.Number.{tag}", tagValue);
        }
    }
}
