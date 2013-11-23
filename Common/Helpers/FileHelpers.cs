using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Logging;

namespace Common.Helpers
{
    public static class FileHelpers
    {
        private static Log Log = LoggingManager.GetLog(typeof(FileHelpers));

        /// <summary>
        /// Reads the bytes from file.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
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
                catch(Exception ex)
                {
                    Log.Exception("[ReadBytesFromFile] - An exception occured reading file bytes, File: {0}", ex, filename);
                }
            }
            return null;
        }

        /// <summary>
        /// Tries to delete a file.
        /// </summary>
        /// <param name="file">The file.</param>
        public static void TryDelete(string file)
        {
            try
            {
                File.Delete(file);
            }
            catch (Exception ex)
            {
                Log.Exception("[TryDelete] - An exception occured deleting file, File: {0}", ex, file);
            }
        }

        /// <summary>
        /// Tries to delete files.
        /// </summary>
        /// <param name="files">The files.</param>
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

        /// <summary>
        /// Copies the file.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        public static void CopyFile(string source, string destination)
        {
            try
            {
                if (File.Exists(source))
                {
                    File.Copy(source, destination);
                }
            }
            catch (Exception ex)
            {
                Log.Exception("[CopyFile] - An exception occured copying file, Source: {0}, Destination: {1}", ex, source, destination);
            }
        }

        /// <summary>
        /// Opens the file dialog.
        /// </summary>
        /// <param name="initial">The initial.</param>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
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
