using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Common.Log;

namespace GUISkinFramework
{
    public  static class DirectoryHelpers
    {
        static readonly Log Log = LoggingManager.GetLog(typeof(DirectoryHelpers));

        public static void CreateIfNotExists(string folder)
        {
            try
            {
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
            }
            catch (Exception ex)
            {
                Log.Message(LogLevel.Error, $"Error creating folder <{folder}>, exception: {ex}");

            }
        }

        public static List<string> GetFiles(string folder, string ext)
        {
            return Directory.Exists(folder) ? Directory.GetFiles(folder, ext, SearchOption.TopDirectoryOnly).ToList() : new List<string>();
        }

        public static void TryDelete(string folder)
        {
            try
            {
                Directory.Delete(folder, true);
            }
            catch (Exception ex)
            {
                Log.Message(LogLevel.Error, $"Error deleting folder <{folder}>, exception: {ex}");
            }
        }

        public static string FolderBrowserDialog()
        {
            var dialog = new FolderBrowserDialog();
            return dialog.ShowDialog() == DialogResult.OK ? dialog.SelectedPath : string.Empty;
        }
    } 
}
