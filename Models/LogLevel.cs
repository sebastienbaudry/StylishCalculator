namespace StylishCalculator.Models
{
    /// <summary>
    /// Defines the available logging levels for the application
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// No logging output
        /// </summary>
        None = 0,

        /// <summary>
        /// Only critical errors that may cause application failure
        /// </summary>
        Error = 1,

        /// <summary>
        /// Warnings and errors
        /// </summary>
        Warning = 2,

        /// <summary>
        /// General information, warnings, and errors
        /// </summary>
        Info = 3,

        /// <summary>
        /// Detailed debugging information (includes all levels)
        /// </summary>
        Debug = 4,

        /// <summary>
        /// Very detailed tracing information (includes all levels)
        /// </summary>
        Trace = 5
    }
}