using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Linq;
using Common.Settings;

namespace Common.Logging
{
    /// <summary>
    /// Logging manager class for easy handling of logs across classes
    /// </summary>
    public static class LoggingManager
    {
        private static Dictionary<string, Logger> _loggers = new Dictionary<string, Logger>();
        private static Dictionary<string, Dictionary<Type, Log>> _logs = new Dictionary<string, Dictionary<Type, Log>>();

        public static void AddLog(Logger logger, string logname = "Default")
        {
            if (_loggers.ContainsKey(logname))
            {
                _loggers[logname].Dispose();
                _loggers.Remove(logname);
            }

            _loggers.Add(logname, logger);
            _logs.Add(logname, new Dictionary<Type, Log>());
            logger.WriteHeader();
        }

        public static Log GetLog(Type owner, string logName = "Default")
        {
            if (_loggers.Count == 0)
            {
                AddLog(new ConsoleLogger(RegistrySettings.LogLevel));
            }

            if (_loggers.ContainsKey(logName) && _logs.ContainsKey(logName))
            {
                if (!_logs[logName].ContainsKey(owner))
                {
                    _logs[logName].Add(owner, new Log(owner, _loggers[logName].QueueLogMessage));
                }
            }
            return _logs[logName][owner];
        }

        /// <summary>
        /// Destroys the logging manager
        /// </summary>
        public static void Destroy()
        {
            if (_loggers.Any())
            {
                foreach (var logger in _loggers.Values)
                {
                    logger.Dispose();
                }
                _loggers.Clear();
                _logs.Clear();
            }
        }
    }
}