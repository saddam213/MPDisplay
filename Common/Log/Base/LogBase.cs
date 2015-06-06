using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace Common.Log
{
    /// <summary>
    /// Base class for logs
    /// </summary>
    public abstract class Logger : IDisposable
    {
        private Queue<Action> _queue = new Queue<Action>();
        private ManualResetEvent _hasNewItems = new ManualResetEvent(false);
        private ManualResetEvent _terminate = new ManualResetEvent(false);
        private ManualResetEvent _waiting = new ManualResetEvent(false);
        private Thread _loggingThread;
        private volatile LogLevel _logLevel;

        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class.
        /// </summary>
        protected Logger(LogLevel level)
        {
            _logLevel = level;
            // Create background thread, to ensure the queue is serviced from a single thread
            _loggingThread = new Thread(ProcessQueue) {IsBackground = true};
            _loggingThread.Start();
        }

        public virtual void WriteHeader()
        {
            foreach (var line in StartHeader)
            {
                QueueLogMessage(_logLevel, line);
            }
        }

        public virtual void WriteFooter()
        {
            foreach (var line in EndFooter)
            {
                QueueLogMessage(_logLevel, line);
            }
        }

        /// <summary>
        /// Processes the queue.
        /// </summary>
        private void ProcessQueue()
        {
            while (true)
            {
                _waiting.Set();
                if (WaitHandle.WaitAny(new WaitHandle[] { _hasNewItems, _terminate }) == 1)
                {
                    return;
                }

                _hasNewItems.Reset();
                _waiting.Reset();

                // create a copy of the current queue so we can continue
                // to queue up log lines while we process the currnt queue
                Queue<Action> queueCopy;
                lock (_queue)
                {
                    queueCopy = new Queue<Action>(_queue);
                    _queue.Clear();
                }

                // Write queue
                while (queueCopy.Count > 0)
                {
                    queueCopy.Dequeue()();
                }
            }
        }


        /// <summary>
        /// Queues the log message.
        /// </summary>
        /// <param name="level">The log level</param>
        /// <param name="message">The message.</param>
        public void QueueLogMessage(LogLevel level, string message)
        {
            if (_logLevel == LogLevel.None || level < _logLevel)
            {
                return;
            }

            lock (_queue)
            {
                _queue.Enqueue(() => LogQueuedMessage(message));
            }
            _hasNewItems.Set();
        }

        /// <summary>
        /// Logs the queued message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected abstract void LogQueuedMessage(string message);


        public virtual IEnumerable<string> StartHeader
        {
            get
            {
                yield return "Log Started at: " + DateTime.Now;
                yield return Environment.OSVersion.VersionString;
                yield return Assembly.GetExecutingAssembly().GetName().ToString();
            }
        }

        public virtual IEnumerable<string> EndFooter
        {
            get
            {
                yield return "Log Ended at: " + DateTime.Now;
            }
        }

        /// <summary>
        /// Flushes this instance.
        /// </summary>
        private void Flush()
        {
            WriteFooter();
            _waiting.WaitOne();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            Flush();
            _terminate.Set();
            _loggingThread.Join();
        }
    }
}