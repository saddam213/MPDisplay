using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Common.Helpers
{
    public static class FileHelpers
    {

        public static byte[] ReadBytesFromFile(string filename)
        {
            if (!string.IsNullOrEmpty(filename) && File.Exists(filename))
            {

                try
                {
                    using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                    {
                        byte[] fileData = new byte[fs.Length];
                        fs.Read(fileData, 0, System.Convert.ToInt32(fs.Length));
                        return fileData;
                    }
                }
                catch
                {
                  
                }
            }
            return null;
        }

        public static void TryDelete(string file)
        {
            try
            {
                File.Delete(file);
            }
            catch (Exception)
            {
            }
        }

        public static void TryDelete(IEnumerable<string> files)
        {
            if (files != null)
            {
                foreach (var file in files)
                {
                    TryDelete(file);
                }
            }
        }

        public static void CopyFile(string source, string destination)
        {
            try
            {
                if (File.Exists(source))
                {
                    File.Copy(source, destination);
                }
            }
            catch (Exception)
            {
            }
        }

        public static string OpenFileDialog(string initial, string filter)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = filter;
            dialog.InitialDirectory = initial;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return dialog.FileName;
            }
            return string.Empty;
        }
    }
}
