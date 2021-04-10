namespace TBA.Common
{
    /// <summary>
    /// Runtime settings
    /// </summary>
    public interface IRuntimeSettings
    {
        /// <summary>
        /// Tinybeans API authorization header key name
        /// </summary>
        string AuthorizationHeaderKey { get; set; }

        /// <summary>
        /// Tinybeans API authorization header key's value
        /// </summary>
        string AuthorizationHeaderValue { get; set; }

        /// <summary>
        /// Tinybeans base URL, e.g. "https://tinybeans.com"
        /// </summary>
        string ApiBaseUrl { get; set; }

        /// <summary>
        /// The maximum number of threads to utilize for archive work
        /// </summary>
        int MaxThreadCount { get; set; }

        /// <summary>
        /// <para>Runs a simple check against each runtime setting to ensure it is a reasonable value.</para>
        /// <para>If any settings fail validation, a <seealso cref="SettingsFailureException"/> will be thrown</para>
        /// </summary>
        bool ValidateSettings();
    }
}