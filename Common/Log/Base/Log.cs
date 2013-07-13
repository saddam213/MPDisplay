using System;

namespace MPDisplay.Common.Log
{
    public class Log
    {
        private Type _owner { get; set; }
        private Action<LogLevel, string> _logAction { get; set; }

        public Log(Type owner, Action<LogLevel, string> logAction)
        {
            _owner = owner;
            _logAction = logAction;
        }

        public void Message(LogLevel level, string message, params object[] args)
        {
            string logMessage = args.Length > 0 ? string.Format(message, args) : message;
            string logLine = string.Format("[{0}] [{1}] - {2}", level, _owner.Name, logMessage);
            if (_logAction != null)
            {
                _logAction.Invoke(level, logLine);
            }
        }

        public void Exception(string message, Exception ex)
        {
            string logLine = string.Format("[{0}] [{1}] - {2}{3}{4}", LogLevel.Error, _owner.Name, message, Environment.NewLine, ex.ToString());
            if (_logAction != null)
            {
                _logAction.Invoke(LogLevel.Error, logLine);
            }
        }
    }
}
