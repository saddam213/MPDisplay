namespace Common.Log
{
    /// <summary>
    /// Message type
    /// </summary>
    public enum LogLevel
    {
        Verbose = 0,
        /// <summary>
        /// Debug message
        /// </summary>
        Debug = 1,
        /// <summary>
        /// Info message
        /// </summary>
        Info = 2,
        /// <summary>
        /// Warn message
        /// </summary>
        Warn = 3,
        /// <summary>
        /// Error message
        /// </summary>
        Error = 4,
        /// <summary>
        /// No Logging
        /// </summary>
        None = 5,
    }
}
