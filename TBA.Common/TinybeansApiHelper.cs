using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;

namespace TBA.Common
{
    /// <inheritdoc />
    public class TinybeansApiHelper : ITinybeansApiHelper
    {
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

            // validate runtime settings
        }

        /// <inheritdoc />
        public string GetByDate(DateTime date)
        {
            var partialUrlFormat = $"";
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public ArchivedImage GetImageDate(int journalId, string archiveId)
        {
            throw new NotImplementedException();
        }

        public List<int> GetJournalIds()
        {
            const string PartialUrl = "/api/1/journals";
            var json = RestApiGetString(PartialUrl, System.Net.Mime.MediaTypeNames.Application.Json);
            _logger.Info($"JSON = {json}");
            
            // todo: parse JSON, grab journal id(s), return collection
            return null;
        }

        /// <inheritdoc />
        public ArchivedText GetTextData(int journalId, string archiveId)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public ArchivedVideo GetVideoData(int journalId, string archiveId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Runs a GET call against the API
        /// </summary>
        /// <param name="partialUrl">The partial URL to send</param>
        /// <param name="mediaTypeName">The response's media type name.  Recommended to use the <see cref="System.Net.Mime.MediaTypeNames"/> static class' properties instead of hard-coding a value.</param>
        /// <returns>Response content if successful</returns>
        private string RestApiGetString(string partialUrl, string mediaTypeName = null)
        {
            var client = new HttpClient()
            {
                BaseAddress = new Uri(_runtimeSettings.ApiBaseUrl)
            };
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(_runtimeSettings.AuthorizationHeaderKey, _runtimeSettings.AuthorizationHeaderValue);

            if (!string.IsNullOrWhiteSpace(mediaTypeName))
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(mediaTypeName));

            HttpResponseMessage response = client.GetAsync(partialUrl).Result;
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                // some failure happened
                var code = (int)response.StatusCode;
                if (code >= 400)
                {
                    _logger.Critical($"A {code} status code was returned -- {response.Content?.ToString() ?? "(no content)"}");
                }
                else if (code >= 300)
                {
                    _logger.Error($"A {code} status code was returned, but it's unclear why -- {response.Content?.ToString() ?? "(no content)"}");
                }
                else
                {
                    _logger.Error($"A {code} status code was returned, but honestly... WTH?? -- {response.Content?.ToString() ?? "(no content)"}");
                }

                // todo: early exit ??
            }

            // todo: got an ok response, proceed as normal
            throw new NotImplementedException();
        }
    }
}