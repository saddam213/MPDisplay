using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Linq;

namespace Common.Logging
{
    /// <summary>
    /// Outputs messages to a file
    /// </summary>
    public class FileLogger : Logger
    {
        private string _filename;
        private string _directory;
        private string _fullPath;
        private int _linesLogged = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileLogger"/> class.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="filename">The filename.</param>
        public FileLogger(string directory, string filename, LogLevel level)
            : base(level)
        {
            _directory = directory;
            _filename = filename;
            _fullPath = System.IO.Path.Combine(_directory, _filename) + ".log";

            if (!Directory.Exists(_directory))
            {
                Directory.CreateDirectory(_directory);
            }
            RollLogs();
        }

        /// <summary>
        /// Writes the message to file.
        /// </summary>
        /// <param name="message">The message.</param>
        protected override void LogQueuedMessage(string message)
        {
            if (_linesLogged > 500)
            {
                _linesLogged = 0;
                if (File.Exists(_fullPath) && new FileInfo(_fullPath).Length > 52428800)
                {
                    RollLogs();
                }
            }

            using (StreamWriter writer = new StreamWriter(_fullPath, true))
            {
                writer.WriteLine(message);
            }
            _linesLogged++;
        }

        /// <summary>
        /// Rolls the log files.
        /// </summary>
        private void RollLogs()
        {
            try
            {
                // if any older files exist cycle the file number
                for (int i = 10; i > 0; i--)
                {
                    string current = string.Format("{0}_{1}.log", System.IO.Path.Combine(_directory, _filename), i);
                    if (File.Exists(current))
                    {
                        if (i == 10)
                        {
                            File.Delete(current);
                            continue;
                        }
                        File.Move(current, string.Format("{0}_{1}.log", System.IO.Path.Combine(_directory, _filename), i + 1));
                    }
                }

                // If the current log exists cycle its number
                if (File.Exists(_fullPath))
                {
                    File.Move(_fullPath, string.Format("{0}_1.log", System.IO.Path.Combine(_directory, _filename)));
                }
            }
            catch 
            {
                // should I log this error, LMAO!!!!
            }
        }
    }
}