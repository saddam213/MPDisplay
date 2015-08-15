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
        static Log _log = LoggingManager.GetLog(typeof(DirectoryHelpers));

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
                _log.Message(LogLevel.Error, string.Format("Error creating folder <{0}>, exception: {1}", folder, ex));

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
                _log.Message(LogLevel.Error, string.Format("Error deleting folder <{0}>, exception: {1}", folder, ex));
            }
        }

        public static string FolderBrowserDialog()
        {
            var dialog = new FolderBrowserDialog();
            return dialog.ShowDialog() == DialogResult.OK ? dialog.SelectedPath : string.Empty;
        }
    } 
}
