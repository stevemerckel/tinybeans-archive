namespace TBA.Common
{
    /// <summary>
    /// Serilog implementation of <see cref="IAppLogger"/>
    /// </summary>
    /// <remarks>
    /// Need to actually wire it up to Serilog, for now just doing the same thing as the <see cref="ConsoleAppLogger"/>
    /// </remarks>
    public sealed class SerilogAppLogger : IAppLogger
    {
        private readonly ConsoleAppLogger _console = new ConsoleAppLogger();

        /// <inheritdoc />
        public void Critical(string message)
        {
            _console.Critical(message);
        }

        /// <inheritdoc />
        public void Debug(string message)
        {
            System.Diagnostics.Debug.WriteLine($"[{nameof(Debug)}]  {message}");
        }

        /// <inheritdoc />
        public void Error(string message)
        {
            _console.Error(message);
        }

        /// <inheritdoc />
        public void Info(string message)
        {
            _console.Info(message);
        }

        /// <inheritdoc />
        public void Warn(string message)
        {
            _console.Warn(message);
        }
    }
}