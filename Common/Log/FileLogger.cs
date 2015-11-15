using System.IO;

namespace Common.Log
{
    /// <summary>
    /// Outputs messages to a file
    /// </summary>
    public class FileLogger : Logger
    {
        private readonly string _filename;
        private readonly string _directory;
        private readonly string _fullPath;
        private int _linesLogged;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileLogger"/> class.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="level"> Log Level</param>
        public FileLogger(string directory, string filename, LogLevel level)
            : base(level)
        {
            _directory = directory;
            _filename = filename;
            _fullPath = Path.Combine(_directory, _filename) + ".log";

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

            using (var writer = new StreamWriter(_fullPath, true))
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
                for (var i = 10; i > 0; i--)
                {
                    var current = $"{Path.Combine(_directory, _filename)}_{i}.log";
                    if (!File.Exists(current)) continue;
                    if (i == 10)
                    {
                        File.Delete(current);
                        continue;
                    }
                    File.Move(current, $"{Path.Combine(_directory, _filename)}_{i + 1}.log");
                }

                // If the current log exists cycle its number
                if (File.Exists(_fullPath))
                {
                    File.Move(_fullPath, $"{Path.Combine(_directory, _filename)}_1.log");
                }
            }
            catch 
            {
                // should I log this error, LMAO!!!!
            }
        }
    }
}