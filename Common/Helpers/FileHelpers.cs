using System;
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
            if (string.IsNullOrEmpty(filename)) return null;
            try
            {
                // get image from url
                if (IsURL(filename) && ExistsURL(filename))
                {
                    using (var webClient = new WebClient())
                    {
                        var fileData = webClient.DownloadData(filename);
                        _log.Message(LogLevel.Verbose, "[ReadBytesFromFile] - Dowloaded data from URL {0}, received <{1}> bytes.", filename, fileData.Length);

                        return fileData;
                    }
                }
                if (File.Exists(filename))
                {
                    using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        var fileData = new byte[fs.Length];
                        fs.Read(fileData, 0, Convert.ToInt32(fs.Length));
                        return fileData;
                    }
                }
            }
            catch(Exception ex)
            {
                _log.Exception("[ReadBytesFromFile] - An exception occured reading file bytes, File or URL: {0}", ex, filename);
            }
            return null;
        }

        /// <summary>
		/// Checks if file points to an URL location
		/// </summary>
		/// <param name="file">The file.</param>
		public static bool IsURL(string file)
		{
				return file.StartsWith("http");
		}

		/// <summary>
		/// Checks if URL exists on server
		/// </summary>
		/// <param name="file">The file.</param>
		public static bool ExistsURL(string file)
		{
			var urlCheck = new Uri(file);
			var request = (HttpWebRequest)WebRequest.Create(urlCheck);					
			request.Timeout = 15000;
		    request.Method = "HEAD";
			HttpWebResponse response = null;
					
			try
			{
				response = (HttpWebResponse)request.GetResponse();
			    if (response.StatusCode != HttpStatusCode.OK)
			    {
                    _log.Message(LogLevel.Warn, "[ExistsURL] - Status for URL {0} not ok, Returned Status is <{1}>", file, response.StatusCode );
			    }
				return response.StatusCode == HttpStatusCode.OK;
			}
			catch (WebException we)
			{
			    if (response == null)
			    {
                    _log.Message(LogLevel.Error, "[ExistsURL] - An exception occurred checking URL {0}, no response received, WebException: {1}", file, we);
			    }
			    else
			    {
                    _log.Message(LogLevel.Error, "[ExistsURL] - An exception occurred checking URL {0}, Status Code <{1}>, WebException: {2}", file, response.StatusCode, we);			        
			    }
				return false; 
			}
			catch (Exception ex)
			{
                _log.Message(LogLevel.Error, "[ExistsURL] - An exception occurred checking URL {0}, Exception:{1}", file, ex);
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
            if (files == null) return;

            foreach (var file in files)
            {
                TryDelete(file);
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
            return dialog.ShowDialog() == DialogResult.OK ? dialog.FileName : string.Empty;
        }
    }
}
