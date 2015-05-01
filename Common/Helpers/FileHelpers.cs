﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Common.Log;
using System.Net;

namespace Common.Helpers
{
    public static class FileHelpers
    {
        private static Log.Log _log = LoggingManager.GetLog(typeof(FileHelpers));

        /// <summary>
        /// Reads the bytes from file.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        public static byte[] ReadBytesFromFile(string filename)
        {
            if (!string.IsNullOrEmpty(filename))
            {

                try
                {
				    // get image from url
				    if (IsURL(filename) && ExistsURL(filename))
				    {
					    using (WebClient webClient = new WebClient())
					    {
						    byte[] fileData = webClient.DownloadData(filename);
						    return fileData;
					    }
				    }									
				    else if (File.Exists(filename))
					{
						using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
						{
							byte[] fileData = new byte[fs.Length];
							fs.Read(fileData, 0, Convert.ToInt32(fs.Length));
							return fileData;
						}
					}
                 }
                catch(Exception ex)
                {
                    _log.Exception("[ReadBytesFromFile] - An exception occured reading file bytes, File: {0}", ex, filename);
                }
            }
            return null;
        }

        /// <summary>
		/// Checks if file points to an URL location
		/// </summary>
		/// <param name="file">The file.</param>
		public static bool IsURL(String file)
		{
				return file.StartsWith("http");
		}

		/// <summary>
		/// Checks if URL exists on server
		/// </summary>
		/// <param name="file">The file.</param>
		public static bool ExistsURL(String file)
		{
			Uri urlCheck = new Uri(file);
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlCheck);					
			request.Timeout = 1000;
			HttpWebResponse response = null;
					
			try
			{
				response = (HttpWebResponse)request.GetResponse();
				return response.StatusCode == HttpStatusCode.OK;
			}
			catch (Exception)
			{
				return false; 
			}
			finally 
			{
			    if (response != null) response.Dispose();
			}
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
                _log.Exception("[TryDelete] - An exception occured deleting file, File: {0}", ex, file);
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
                _log.Exception("[CopyFile] - An exception occured copying file, Source: {0}, Destination: {1}", ex, source, destination);
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
            var dialog = new OpenFileDialog {Filter = filter, InitialDirectory = initial};
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return dialog.FileName;
            }
            return string.Empty;
        }
    }
}
