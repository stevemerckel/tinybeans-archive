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
        string AuthorizationHeaderKey { get; }

        /// <summary>
        /// Tinybeans API authorization header key's value
        /// </summary>
        string AuthorizationHeaderValue { get; }

        /// <summary>
        /// Tinybeans base URL, e.g. "https://tinybeans.com"
        /// </summary>
        string ApiBaseUrl { get; }
    }
}