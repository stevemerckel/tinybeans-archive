namespace TBA.Common
{
    /// <summary>
    /// Structure for application logger
    /// </summary>
    public interface IAppLogger
    {
        /// <summary>
        /// Writes verbose message
        /// </summary>
        void Verbose (string message);

        /// <summary>
        /// Writes debug message
        /// </summary>
        void Debug(string message);

        /// <summary>
        /// Writes info message
        /// </summary>
        void Info(string message);

        /// <summary>
        /// Writes warning message
        /// </summary>
        void Warn(string message);

        /// <summary>
        /// Writes error message
        /// </summary>
        void Error(string message);

        /// <summary>
        /// Writes critical message
        /// </summary>
        void Critical(string message);
    }
}