using System;
using System.Net.Http;

namespace TBA.Common
{
    /// <inheritdoc />
    public class TinybeansApiHelper : ITinybeansApiHelper
    {
        private readonly HttpClient _client;
        private readonly IAppLogger _logger;
        private readonly IRuntimeSettings _runtimeSettings;

        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="logger">Logging object</param>
        public TinybeansApiHelper(IAppLogger logger, IRuntimeSettings runtimeSettings)
        {
            _logger = logger;
            if (logger == null)
                throw new NullReferenceException(nameof(logger));

            _runtimeSettings = runtimeSettings;
            if (runtimeSettings == null)
                throw new NullReferenceException(nameof(runtimeSettings));

            _client = new HttpClient()
            {
                BaseAddress = new Uri(_runtimeSettings.ApiBaseUrl)
            };
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(_runtimeSettings.AuthorizationHeaderKey, _runtimeSettings.AuthorizationHeaderValue);
        }

        /// <inheritdoc />
        public string GetByDate(DateTime date)
        {
            var partialUrlFormat = $"";
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public ArchivedImage GetImageDate(string journalId)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public ArchivedText GetTextData(string journalId)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public ArchivedVideo GetVideoData(string journalId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Runs a GET call against the API
        /// </summary>
        /// <param name="partialUrl">The partial URL to send</param>
        /// <returns>JSON response</returns>
        private string RestApiGetString(string partialUrl)
        {
            HttpResponseMessage response = _client.GetAsync(partialUrl).Result;
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                var code = (int)response.StatusCode;
                // some failure happened
                if (code >= 400)
                {
                    _logger.Critical($"A {code} status code was returned -- {response.Content?.ToString() ?? "(no content)"}");
                }
                else if (code >= 300)
                {
                    _logger.Error($"A {code} status code was returned, but it's unclear why -- {response.Content?.ToString() ?? "(no content)"}");
                }
            }

            // got an ok response, proceed as normal
            throw new NotImplementedException();
        }
    }
}