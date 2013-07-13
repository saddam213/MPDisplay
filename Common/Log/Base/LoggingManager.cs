using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Linq;

namespace MPDisplay.Common.Log
{
    /// <summary>
    /// Logging manager class for easy handling of logs across classes
    /// </summary>
    public static class LoggingManager
    {
        private static Logger _logger = new ConsoleLogger();
        private static Dictionary<Type, Log> _logs = new Dictionary<Type, Log>();
        private static List<LogLevel> _levels = new List<LogLevel>();

        static LoggingManager()
        {
            string levels = System.Configuration.ConfigurationManager.AppSettings["LogLevel"] ?? "Verbose,Debug,Info,Warn,Error";
            var logLevels = levels.Contains(',')
                ? Enum.GetValues(typeof(LogLevel)).Cast<LogLevel>().Where(e => levels.Split(',').Select(x => x.Trim()).Contains(e.ToString()))
                : Enum.GetValues(typeof(LogLevel)).Cast<LogLevel>().Where(x => x.ToString().Equals(levels))
                ?? Enum.GetValues(typeof(LogLevel)).Cast<LogLevel>();
            _levels = new List<LogLevel>(logLevels);
        }

        public static void AddLog(Logger logger)
        {
            Destroy();
            _logger = logger;
        }

        /// <summary>
        /// Gets/Creates the log for this class.
        /// </summary>
        /// <param name="owner">The class that owns this Log instance</param>
        /// <returns></returns>
        public static Log GetLog(Type owner)
        {
            if (!_logs.ContainsKey(owner))
            {
                _logs.Add(owner, new Log(owner, (l, s) =>
                {
                    if (_levels.Contains(l))
                    {
                        _logger.QueueLogMessage(s);
                    }
                }));
            }
            return _logs[owner];
        }

        /// <summary>
        /// Destroys the logging manager
        /// </summary>
        public static void Destroy()
        {
            if (_logger != null)
            {
                _logger.Dispose();
            }
        }
    }
}