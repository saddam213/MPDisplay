using System;
using System.Collections.Generic;
using System.Linq;
using Common.Settings;

namespace Common.Log
{
    /// <summary>
    /// Logging manager class for easy handling of logs across classes
    /// </summary>
    public static class LoggingManager
    {
        private static readonly Dictionary<string, Logger> Loggers = new Dictionary<string, Logger>();
        private static readonly Dictionary<string, Dictionary<Type, Log>> Logs = new Dictionary<string, Dictionary<Type, Log>>();

        public static void AddLog(Logger logger, string logname = "Default")
        {
            if (Loggers.ContainsKey(logname))
            {
                Loggers[logname].Dispose();
                Loggers.Remove(logname);
            }

            Loggers.Add(logname, logger);
            Logs.Add(logname, new Dictionary<Type, Log>());
            logger.WriteHeader();
        }

        public static Log GetLog(Type owner, string logName = "Default")
        {
            if (Loggers.Count == 0)
            {
                AddLog(new ConsoleLogger(RegistrySettings.LogLevel));
            }

            if (!Loggers.ContainsKey(logName) || !Logs.ContainsKey(logName)) return Logs[logName][owner];

            if (!Logs[logName].ContainsKey(owner))
            {
                Logs[logName].Add(owner, new Log(owner, Loggers[logName].QueueLogMessage));
            }
            return Logs[logName][owner];
        }

        /// <summary>
        /// Destroys the logging manager
        /// </summary>
        public static void Destroy()
        {
            if (!Loggers.Any()) return;
            foreach (var logger in Loggers.Values)
            {
                logger.Dispose();
            }
            Loggers.Clear();
            Logs.Clear();
        }
    }
}