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

        public string LogTime
        {
            get { return string.Format("{0} {1} ", DateTime.Now.ToShortDateString(), TimeSpan.FromTicks(DateTime.Now.TimeOfDay.Ticks)); }
        }

        public void Message(LogLevel level, string message, params object[] args)
        {
            string logMessage = args.Length > 0 ? string.Format(message, args) : message;
            string timeLevel = string.Format("{0}[{1}] ",LogTime,level);
            string logLine = string.Format("{0}[{1}] - {2}", timeLevel.PadRight(37), _owner.Name, logMessage);
            if (_logAction != null)
            {
                _logAction(level, logLine);
            }
        }

        public void Exception(string message, Exception ex)
        {
            string timeLevel = string.Format("{0}[{1}] ", LogTime, LogLevel.Error);
            string logLine = string.Format("{0}[{1}] - {2}{3}{4}", timeLevel.PadRight(37), _owner.Name, message, Environment.NewLine, ex.ToString());
            if (_logAction != null)
            {
                _logAction(LogLevel.Error, logLine);
            }
        }
    }
}
