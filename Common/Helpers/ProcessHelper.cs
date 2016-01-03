using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Common.Log;

namespace Common.Helpers
{
    public class ProcessHelper
    {
        private static readonly Log.Log Log = LoggingManager.GetLog(typeof(ProcessHelper));

        // Sets the window to be foreground 
        [DllImport("User32")]
        private static extern int SetForegroundWindow(IntPtr hwnd);

        // Activate or minimize a window 
        [DllImport("User32.DLL")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        // ReSharper disable InconsistentNaming
        private const int SW_RESTORE = 9;
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_NOACTIVATE = 0x08000000;
        // ReSharper restore InconsistentNaming

        public static void ActivateApplication(string appName)
        {
            var procList = Process.GetProcessesByName(appName);
            if (procList.Length <= 0) return;
            ShowWindow(procList[0].MainWindowHandle, SW_RESTORE);
            SetForegroundWindow(procList[0].MainWindowHandle);
        }

        public static void SetWindowNoActivate(Window window)
        {
            //Set the window style to noactivate.
            var helper = new WindowInteropHelper(window);
            SetWindowLong(helper.Handle, GWL_EXSTYLE,
            GetWindowLong(helper.Handle, GWL_EXSTYLE) | WS_EX_NOACTIVATE);
        }


        public static void StartApplication(string filename, string args)
        {
            try
            {
                Process.Start(filename, args);
            }
            catch(Exception ex)
            {
                Log.Exception("[StartApplication] - An execption occured starting application, Filename: {0}, args: {1}", ex, filename, args);
            }
        }

        public static void KillApplication(string name, bool kill = false)
        {
            try
            {
                var processes = Process.GetProcessesByName(name);
                foreach (var process in processes)
                {
                    if (!string.IsNullOrEmpty(process.MainWindowTitle))
                    {
                        process.CloseMainWindow();
                    }
                    else
                    {
                        process.Close();
                    }
                }

                if (!kill)
                {
                    KillApplication(name, true);
                }
            }
            catch (Exception ex)
            {
                Log.Exception("[KillApplication] - An execption occured starting application, Name: {0}, IsKill: {1}", ex, name, kill);
            }
        }



    }
}
