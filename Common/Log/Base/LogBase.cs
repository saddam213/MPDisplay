using System;
using System.Collections.Generic;
using System.Threading;

namespace MPDisplay.Common.Log
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

        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class.
        /// </summary>
        public Logger()
        {
            // Create background thread, to ensure the queue is serviced from a single thread
            loggingThread = new Thread(new ThreadStart(ProcessQueue));
            loggingThread.IsBackground = true;
            loggingThread.Start();
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
                    queueCopy.Dequeue().Invoke();
                }
            }
        }

        /// <summary>
        /// Gets the log time.
        /// </summary>
        public string LogTime
        {
            get { return string.Format("{0} {1} ", DateTime.Now.ToShortDateString(), TimeSpan.FromTicks(DateTime.Now.TimeOfDay.Ticks)); }
        }

        /// <summary>
        /// Queues the log message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void QueueLogMessage(string message)
        {
            lock (queue)
            {
                queue.Enqueue(() => LogQueuedMessage(LogTime + message));
            }
            hasNewItems.Set();
        }

        /// <summary>
        /// Logs the queued message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected abstract void LogQueuedMessage(string message);

        /// <summary>
        /// Flushes this instance.
        /// </summary>
        public void Flush()
        {
            waiting.WaitOne();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            terminate.Set();
          //  loggingThread.Join();
        }
    }
}