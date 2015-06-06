using System;

namespace Common.Log
{
    public class Log
    {
        private Type _owner;
        private Action<LogLevel, string> _logAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="Log"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="logAction">The log action.</param>
        public Log(Type owner, Action<LogLevel, string> logAction)
        {
            _owner = owner;
            _logAction = logAction;
        }

        /// <summary>
        /// Gets the log time.
        /// </summary>
        public string LogTime
        {
            get { return string.Format("{0} {1} ", DateTime.Now.ToShortDateString(), TimeSpan.FromTicks(DateTime.Now.TimeOfDay.Ticks)); }
        }

        /// <summary>
        /// Messages the specified level.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The args.</param>
        public void Message(LogLevel level, string message, params object[] args)
        {
            var logMessage = args.Length > 0 ? string.Format(message, args) : message;
            var timeLevel = string.Format("{0}[{1}] ",LogTime,level);
            var logLine = string.Format("{0}[{1}] - {2}", timeLevel.PadRight(38), _owner.Name, logMessage);
            if (_logAction != null)
            {
                _logAction(level, logLine);
            }
        }

        /// <summary>
        /// Exceptions the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The ex.</param>
        /// <param name="args"></param>
        public void Exception(string message, Exception ex, params object[] args)
        {
            var logMessage = args.Length > 0 ? string.Format(message, args) : message;
            var timeLevel = string.Format("{0}[{1}] ", LogTime, LogLevel.Error);
            var logLine = string.Format("{0}[{1}] - {2}{3}{4}", timeLevel.PadRight(38), _owner.Name, logMessage, Environment.NewLine, ex);
            if (_logAction != null)
            {
                _logAction(LogLevel.Error, logLine);
            }
        }
    }
}
