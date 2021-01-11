﻿using System;

namespace TBA.Common
{
    /// <summary>
    /// Logger that only outputs to the Console on "stdout" stream, except for <see cref="Debug(string)"/> that writes to the "dbgout" stream.
    /// </summary>
    public sealed class ConsoleAppLogger : IAppLogger
    {
        /// <inheritdoc />
        public void Critical(string message)
        {
            Write(nameof(Critical), message);
        }

        /// <inheritdoc />
        public void Debug(string message)
        {
            System.Diagnostics.Debug.WriteLine($"[{nameof(Debug)}]  {message}");
        }

        /// <inheritdoc />
        public void Error(string message)
        {
            Write(nameof(Error), message);
        }

        /// <inheritdoc />
        public void Info(string message)
        {
            Write(nameof(Info), message);
        }

        /// <inheritdoc />
        public void Warn(string message)
        {
            Write(nameof(Warn), message);
        }

        private void Write(string level, string message)
        {
            Console.WriteLine($"[{level}]  {message}");
        }
    }
}