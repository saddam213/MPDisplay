using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace Common.Logging
{
    /// <summary>
    /// Base class for logs
    /// </summary>
    /// <typeparam name="LogType">Log type</typeparam>
    public abstract class Logger : IDisposable
    {
        private Queue<Action> queue = new Queue<Action>();
        private ManualResetEvent hasNewItems = new ManualResetEvent(false);
        private ManualResetEvent terminate = new ManualResetEvent(false);
        private ManualResetEvent waiting = new ManualResetEvent(false);
        private Thread loggingThread;
        private volatile LogLevel _logLevel = LogLevel.Verbose;

        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class.
        /// </summary>
        public Logger(LogLevel level)
        {
            _logLevel = level;
            // Create background thread, to ensure the queue is serviced from a single thread
            loggingThread = new Thread(new ThreadStart(ProcessQueue));
            loggingThread.IsBackground = true;
            loggingThread.Start();
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
                waiting.Set();
                if (ManualResetEvent.WaitAny(new WaitHandle[] { hasNewItems, terminate }) == 1)
                {
                    return;
                }

                hasNewItems.Reset();
                waiting.Reset();

                // create a copy of the current queue so we can continue
                // to queue up log lines while we process the currnt queue
                Queue<Action> queueCopy;
                lock (queue)
                {
                    queueCopy = new Queue<Action>(queue);
                    queue.Clear();
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
        /// <param name="message">The message.</param>
        public void QueueLogMessage(LogLevel level, string message)
        {
            if (_logLevel == LogLevel.None)
            {
                return;
            }

            if (level >= _logLevel)
            {
                lock (queue)
                {
                    queue.Enqueue(() => LogQueuedMessage(message));
                }
                hasNewItems.Set();
            }
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
            waiting.WaitOne();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            Flush();
            terminate.Set();
            loggingThread.Join();
        }
    }
}