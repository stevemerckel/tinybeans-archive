using Newtonsoft.Json;
using System;
using System.Diagnostics;

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
            {
                Console.WriteLine($"Failed on {nameof(AuthorizationHeaderKey)} -- value = '{(string.IsNullOrWhiteSpace(AuthorizationHeaderKey) ? "[NULL/EMPTY]" : AuthorizationHeaderKey)}'");
                isValid = false;
            }

            // header value
            if (string.IsNullOrWhiteSpace(AuthorizationHeaderValue))
            {
                Console.WriteLine($"Failed on {nameof(AuthorizationHeaderValue)} -- value = '{(string.IsNullOrWhiteSpace(AuthorizationHeaderValue) ? "[NULL/EMPTY]" : AuthorizationHeaderValue)}'");
                isValid = false;
            }

            // api url
            if (string.IsNullOrWhiteSpace(ApiBaseUrl))
            {
                Console.WriteLine($"Failed on string parse of {nameof(ApiBaseUrl)} -- value = '{(string.IsNullOrWhiteSpace(ApiBaseUrl) ? "[NULL/EMPTY]" : ApiBaseUrl)}'");
                isValid = false;
            }

            // max thread count
            if (MaxThreadCount < 1 || MaxThreadCount > 8)
            {
                Console.WriteLine($"Failed on {nameof(MaxThreadCount)} -- value = {MaxThreadCount}");
                isValid = false;
            }
            
            try
            {
                new Uri(ApiBaseUrl);
            }
            catch
            {
                Console.WriteLine($"Failed on init of {nameof(Uri)} using {nameof(ApiBaseUrl)} with value of '{(string.IsNullOrWhiteSpace(ApiBaseUrl) ? "[NULL/EMPTY]" : ApiBaseUrl)}'");
                isValid = false;
            }

            if (isValid)
                return true;

            throw new SettingsFailureException($"{nameof(ValidateSettings)} failed!");
        }
    }
}