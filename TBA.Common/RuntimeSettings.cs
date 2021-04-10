using Newtonsoft.Json;
using System;

namespace TBA.Common
{
    /// <inheritdoc />
    public class RuntimeSettings : IRuntimeSettings
    {
        /// <summary>
        /// Creates an empty runtime settings object
        /// </summary>
        public RuntimeSettings()
        {
        }

        /// <summary>
        /// Loads the runtime settings object
        /// </summary>
        /// <param name="authorizationHeaderKey">API header key for authorization</param>
        /// <param name="authorizationHeaderValue">API header value for authorization</param>
        /// <param name="apiBaseUrl">The bse URL for the API</param>
        /// <param name="threadCount"></param>
        public RuntimeSettings(string authorizationHeaderKey, string authorizationHeaderValue, string apiBaseUrl, int threadCount)
        {
            AuthorizationHeaderKey = authorizationHeaderKey;
            AuthorizationHeaderValue = authorizationHeaderValue;
            ApiBaseUrl = apiBaseUrl;
        }

        /// <inheritdoc />
        [JsonProperty("api/auth-header-name")]
        public string AuthorizationHeaderKey { get; set; }

        /// <inheritdoc />
        [JsonProperty("api/auth-header-value")]
        public string AuthorizationHeaderValue { get; set; }

        /// <inheritdoc />
        [JsonProperty("api/base-url")]
        public string ApiBaseUrl { get; set; }

        [JsonProperty("max-thread-count")]
        public int MaxThreadCount { get; set; }

        /// <inheritdoc />
        public bool ValidateSettings()
        {
            var isValid = true;

            // header key
            if (string.IsNullOrWhiteSpace(AuthorizationHeaderKey))
                isValid = false;

            // header value
            if (string.IsNullOrWhiteSpace(AuthorizationHeaderValue))
                isValid = false;

            // api url
            if (string.IsNullOrWhiteSpace(ApiBaseUrl))
                isValid = false;

            // max thread count
            if (MaxThreadCount < 1 || MaxThreadCount > 8)
                isValid = false;
            
            try
            {
                new Uri(ApiBaseUrl);
            }
            catch
            {
                isValid = false;
            }

            if (isValid)
                return true;

            throw new SettingsFailureException($"{nameof(ValidateSettings)} failed!");
        }
    }
}