﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using Newtonsoft.Json.Linq;

namespace TBA.Common
{
    /// <inheritdoc />
    public class TinybeansApiHelper : ITinybeansApiHelper
    {
        private readonly IAppLogger _logger;
        private readonly IRuntimeSettings _runtimeSettings;
        private readonly HttpClient _httpClient;
        private readonly ITinybeansJsonHelper _jsonHelper;
        private readonly WebClient _webClient;

        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="logger">Logging object</param>
        public TinybeansApiHelper(IAppLogger logger, IRuntimeSettingsProvider runtimeSettingsProvider, ITinybeansJsonHelper jsonHelper)
        {
            _logger = logger;
            if (logger == null)
                throw new NullReferenceException(nameof(logger));

            _runtimeSettings = runtimeSettingsProvider.GetRuntimeSettings();
            if (_runtimeSettings == null)
                throw new NullReferenceException(nameof(_runtimeSettings));

            // validate runtime settings
            _runtimeSettings.ValidateSettings();

            _jsonHelper = jsonHelper;

            // init http client
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri(_runtimeSettings.ApiBaseUrl)
            };

            // init web client
            _webClient = new WebClient();
        }

        /// <inheritdoc />
        public bool DownloadFile(string remoteUrl, string localPath)
        {
            try
            {
                if (File.Exists(localPath))
                    File.Delete(localPath);

                _webClient.DownloadFile(remoteUrl, localPath);
            }
            catch (Exception ex)
            {
                var message = ex.ToString();
                if (ex.InnerException != null)
                    message += $"{Environment.NewLine}Inner Exception type '{ex.InnerException.GetType()}' -- Details: {ex.InnerException}";
                _logger.Error(message);
                return false;
            }

            return true;
        }

        /// <inheritdoc />
        public List<IArchivedContent> GetByDate(DateTime date, long journalId)
        {
            _logger.Info($"Fetching day info for journal ID '{journalId}' for date '{date.ToString("MM/dd/yyyy")}'");

            var partialUrl = $"/api/1/journals/{journalId}/entries?day={date.Day}&month={date.Month}&year={date.Year}&idsOnly=true";
            var json = RestApiGetString(partialUrl, MediaTypeNames.Application.Json);
            if (string.IsNullOrWhiteSpace(json))
            {
                _logger.Warn($"No JSON returned from {nameof(GetJournalSummaries)}");
                return null;
            }

            return _jsonHelper.ParseArchivedContent(json);
        }

        /// <inheritdoc />
        public List<IArchivedContent> GetEntriesByYearMonth(DateTime yearMonth, long journalId)
        {
            _logger.Info($"Fetching month info for journal ID '{journalId}' for date '{yearMonth.ToString("MMMM yyyy")}'");

            var partialUrl = $"/api/1/journals/{journalId}/entries?month={yearMonth.Month}&year={yearMonth.Year}&idsOnly=true";
            var json = RestApiGetString(partialUrl, MediaTypeNames.Application.Json);
            if (string.IsNullOrWhiteSpace(json))
            {
                _logger.Warn($"No JSON returned from {nameof(GetJournalSummaries)}");
                return null;
            }

            return _jsonHelper.ParseArchivedContent(json);
        }

        /// <inheritdoc />
        public List<JournalSummary> GetJournalSummaries()
        {
            const string PartialUrl = "/api/1/journals";
            var json = RestApiGetString(PartialUrl, MediaTypeNames.Application.Json);
            if (string.IsNullOrWhiteSpace(json))
            {
                _logger.Warn($"No JSON returned from {nameof(GetJournalSummaries)}");
                return null;
            }

            return _jsonHelper.ParseJournalSummaries(json);
        }

        /// <summary>
        /// Runs a GET call against the API
        /// </summary>
        /// <param name="partialUrl">The partial URL to send</param>
        /// <param name="mediaTypeName">The response's media type name.  Recommended to use the <see cref="System.Net.Mime.MediaTypeNames"/> static class' properties instead of hard-coding a value.</param>
        /// <returns>Response content if successful</returns>
        /// <remarks>A pointer in direction of HttpResponseMessage containing GZip content came from Rick Strahl's blog (https://weblog.west-wind.com/posts/2007/jun/29/httpwebrequest-and-gzip-http-responses), then did some tuning on my original decompress based on DotNetPerls article (https://www.dotnetperls.com/decompress)</remarks>
        private string RestApiGetString(string partialUrl, string mediaTypeName = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, partialUrl);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(_runtimeSettings.AuthorizationHeaderValue);

            if (!string.IsNullOrWhiteSpace(mediaTypeName))
            {
                request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(mediaTypeName));
            }
            request.Headers.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("gzip"));
            request.Headers.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("deflate"));

            HttpResponseMessage response = _httpClient.SendAsync(request).Result;
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

            // got an http-ok response
            string responseString;
            if (response.Content.Headers.ContentEncoding?.Contains("gzip") ?? false)
            {
                using (GZipStream stream = new GZipStream(new MemoryStream(response.Content.ReadAsByteArrayAsync().Result), CompressionMode.Decompress))
                {
                    const int size = 4096;
                    byte[] buffer = new byte[size];
                    using (MemoryStream memory = new MemoryStream())
                    {
                        int count = 0;
                        do
                        {
                            count = stream.Read(buffer, 0, size);
                            if (count > 0)
                            {
                                memory.Write(buffer, 0, count);
                            }
                        }
                        while (count > 0);
                        responseString = System.Text.Encoding.UTF8.GetString(memory.ToArray());
                    }
                }
            }
            else
            {
                responseString = response.Content.ReadAsStringAsync().Result;
            }

            // need to check the inner json "status" field to look for "ok"
            var internalStatus = JObject.Parse(responseString).GetValue("status").ToString();
            if (internalStatus?.ToLower() != "ok")
                throw new Exception($"Tinybeans API returned a non-ok status code of '{internalStatus ?? "[NULL]"}'");

            return responseString;
        }
    }
}