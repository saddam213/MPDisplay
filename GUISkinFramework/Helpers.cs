using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace GUISkinFramework
{
    public  static class DirectoryHelpers
    {
        public static void CreateIfNotExists(string folder)
        {
            try
            {
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
            }
            catch (Exception)
            {
            
            }
        }

        public static List<string> GetFiles(string folder, string ext)
        {
            if (Directory.Exists(folder))
            {
                return Directory.GetFiles(folder, ext, SearchOption.TopDirectoryOnly).ToList();
            }
            return new List<string>();
        }

        public static void TryDelete(string folder)
        {
            try
            {
                Directory.Delete(folder, true);
            }
            catch (Exception)
            {
              
            }
        }

        public static string FolderBrowserDialog()
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return dialog.SelectedPath;
            }
            return string.Empty;
        }
    }

  
}
