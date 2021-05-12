using System;

namespace TBA.Common
{
    /// <summary>
    /// Exception for indicating a failure in some aspect of <see cref="IRuntimeSettings"/>
    /// </summary>
    public sealed class SettingsFailureException : ApplicationException
    {
        /// <summary>
        /// Information about failure related to the "settings" file or the <see cref="IRuntimeSettings"/> object.
        /// </summary>
        /// <param name="message">The details of the error</param>
        public SettingsFailureException(string message) : base(message)
        {
        }
    }
}