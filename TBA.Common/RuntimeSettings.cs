namespace TBA.Common
{
    /// <inheritdoc />
    public class RuntimeSettings : IRuntimeSettings
    {
        /// <summary>
        /// Loads the runtime settings object
        /// </summary>
        /// <param name="authorizationHeaderKey">API header key for authorization</param>
        /// <param name="authorizationHeaderValue">API header value for authorization</param>
        /// <param name="apiBaseUrl">The bse URL for the API</param>
        public RuntimeSettings(string authorizationHeaderKey, string authorizationHeaderValue, string apiBaseUrl)
        {
            AuthorizationHeaderKey = authorizationHeaderKey;
            AuthorizationHeaderValue = authorizationHeaderValue;
            ApiBaseUrl = apiBaseUrl;
        }

        /// <inheritdoc />
        public string AuthorizationHeaderKey { get; }

        /// <inheritdoc />
        public string AuthorizationHeaderValue { get; }

        /// <inheritdoc />
        public string ApiBaseUrl { get; }
    }
}