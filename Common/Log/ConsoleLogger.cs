using System;

namespace MPDisplay.Common.Log
{
    /// <summary>
    /// Outputs messages to the console
    /// </summary>
    public class ConsoleLogger : Logger
    {
        protected override void LogQueuedMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}