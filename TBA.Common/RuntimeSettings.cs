using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace TBA.Common
{
    /// <inheritdoc />
    [DebuggerDisplay(nameof(ApiBaseUrl) + "={ApiBaseUrl} -- " + nameof(MaxThreadCount) + "={MaxThreadCount}")]
    public sealed class RuntimeSettings : IRuntimeSettings
    {
        private const int MinAllowedThreadCount = 1;
        private const int MaxAllowedThreadCount = 8;
        private int _maxThreadCount;
        private string _authorizationHeaderKey;
        private string _authorizationHeaderValue;
        private string _apiBaseUrl;

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
            AuthorizationHeaderKey = authorizationHeaderKey?.Trim() ?? string.Empty;
            AuthorizationHeaderValue = authorizationHeaderValue.Trim() ?? string.Empty;
            ApiBaseUrl = apiBaseUrl.Trim() ?? string.Empty;
            MaxThreadCount = threadCount;
        }

        /// <inheritdoc />
        [JsonProperty("api/auth-header-name")]
        public string AuthorizationHeaderKey
        {
            get => _authorizationHeaderKey;
            set => _authorizationHeaderKey = value?.Trim() ?? string.Empty;
        }

        /// <inheritdoc />
        [JsonProperty("api/auth-header-value")]
        public string AuthorizationHeaderValue
        {
            get => _authorizationHeaderValue;
            set => _authorizationHeaderValue = value?.Trim() ?? string.Empty;
        }

        /// <inheritdoc />
        [JsonProperty("api/base-url")]
        public string ApiBaseUrl 
        { 
            get => _apiBaseUrl; 
            set => _apiBaseUrl = value?.Trim() ?? string.Empty; 
        }

        /// <inheritdoc />
        [JsonProperty("max-thread-count")]
        public int MaxThreadCount
        {
            get
            {
                if (_maxThreadCount <= MinAllowedThreadCount)
                    return MinAllowedThreadCount; // minimum enforcement

                return _maxThreadCount > MaxAllowedThreadCount ? MaxAllowedThreadCount : _maxThreadCount;
            }
            set
            {
                _maxThreadCount = value;
            }
        }

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