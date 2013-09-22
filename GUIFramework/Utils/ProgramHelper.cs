using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace GUIFramework.Utils
{
    public class ProgramHelper
    {

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

        private const int SW_SHOW = 5;
        private const int SW_MINIMIZE = 6;
        private const int SW_RESTORE = 9;
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_NOACTIVATE = 0x08000000;

        public static void ActivateApplication(string appName)
        {
            Process[] procList = Process.GetProcessesByName(appName);
            if (procList.Length > 0)
            {
                ShowWindow(procList[0].MainWindowHandle, SW_RESTORE);
                SetForegroundWindow(procList[0].MainWindowHandle);
            }
        }

        public static void SetWindowNoActivate(Window window)
        {
            //Set the window style to noactivate.
            WindowInteropHelper helper = new WindowInteropHelper(window);
            SetWindowLong(helper.Handle, GWL_EXSTYLE,
            GetWindowLong(helper.Handle, GWL_EXSTYLE) | WS_EX_NOACTIVATE);
        }


        public static void StartApplication(string filename, string args)
        {
            try
            {
                Process.Start(filename, args);
            }
            catch 
            {
            }
        }

        public static void KillApplication(string name, bool kill = false)
        {
            try
            {
                var processes = Process.GetProcessesByName(name);
                if (processes != null)
                {
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
            }
            catch 
            {
                
            }
        }



    }
}
