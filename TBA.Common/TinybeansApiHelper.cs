using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
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
        private readonly IFileManager _fileManager;

        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="logger">Logging object</param>
        public TinybeansApiHelper(IAppLogger logger, IRuntimeSettingsProvider runtimeSettingsProvider, ITinybeansJsonHelper jsonHelper, IFileManager fileManager)
        {
            _logger = logger;
            _runtimeSettings = runtimeSettingsProvider.GetRuntimeSettings();
            _jsonHelper = jsonHelper;
            _fileManager = fileManager;

            // init http client
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri(_runtimeSettings.ApiBaseUrl)
            };
        }

        /// <inheritdoc />
        public async Task<List<ITinybeansEntry>> GetByDateAsync(DateTime date, long journalId)
        {
            _logger.Info($"Fetching day info for journal ID '{journalId}' for date '{date:MM/dd/yyyy}'");

            var partialUrl = $"/api/1/journals/{journalId}/entries?day={date.Day}&month={date.Month}&year={date.Year}&idsOnly=true";
            var json = await RestApiGetStringAsync(partialUrl, MediaTypeNames.Application.Json);
            if (string.IsNullOrWhiteSpace(json))
            {
                _logger.Warn($"No JSON returned from {nameof(GetByDateAsync)}");
                return null;
            }

            return _jsonHelper.ParseArchivedContent(json);
        }

        /// <inheritdoc />
        public async Task<List<ITinybeansEntry>> GetEntriesByYearMonthAsync(DateTime yearMonth, long journalId)
        {
            _logger.Info($"Fetching month info for journal ID '{journalId}' for date '{yearMonth:MMMM yyyy}'");

            var partialUrl = $"/api/1/journals/{journalId}/entries?month={yearMonth.Month}&year={yearMonth.Year}&idsOnly=true";
            var json = await RestApiGetStringAsync(partialUrl, MediaTypeNames.Application.Json);
            if (string.IsNullOrWhiteSpace(json))
            {
                _logger.Warn($"No JSON returned from {nameof(GetEntriesByYearMonthAsync)}");
                return null;
            }

            try
            {
                return _jsonHelper.ParseArchivedContent(json);
            }
            catch (Exception ex)
            {
                _logger.Warn($"{nameof(Exception)} thrown trying to fetch entries for '{yearMonth.Year}-{yearMonth.Month.ToString().PadLeft(2, '0')}' -- {ex.Message}");
            }

            return null;
        }

        /// <inheritdoc />
        public async Task<List<JournalSummary>> GetJournalSummariesAsync()
        {
            const string PartialUrl = "/api/1/journals";
            var json = await RestApiGetStringAsync(PartialUrl, MediaTypeNames.Application.Json);
            if (string.IsNullOrWhiteSpace(json))
            {
                _logger.Warn($"No JSON returned from {nameof(GetJournalSummariesAsync)}");
                return null;
            }

            return _jsonHelper.ParseJournalSummaries(json);
        }

        /// <inheritdoc />
        public async Task<EntryDownloadInfo> DownloadAsync(ITinybeansEntry archive, string destinationDirectory)
        {
            if (archive.ArchiveType == ArchiveType.Text)
            {
                //
                // todo: find an archive text entry that has an emoji
                //       then validate that the meta is written in unicode (i.e. "\u" prefix)

                var fileName = Guid.NewGuid().ToString("N");
                var destinationLocation = _fileManager.PathCombine(destinationDirectory, $"{fileName}.txt");
                await _fileManager.FileWriteTextAsync(destinationLocation, archive.Caption, System.Text.Encoding.Unicode);
                return new EntryDownloadInfo(archive.Id, destinationLocation, null, null);
            }

            if (archive.ArchiveType == ArchiveType.Image || archive.ArchiveType == ArchiveType.Video)
            {
                if (!archive.SourceUrl.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
                {
                    _logger.Debug($"The archive {nameof(archive.Id)} of '{archive.Id}' has a local path of '{archive.SourceUrl}', so we are not going to download it.");
                    return null;
                }

                var mainContentLocation = DetermineLocalFileLocation(archive.SourceUrl, destinationDirectory);
                var thumbRectLocation = DetermineLocalFileLocation(archive.ThumbnailUrlRectangle, destinationDirectory, "-tr");
                var thumbSquareLocation = DetermineLocalFileLocation(archive.ThumbnailUrlRectangle, destinationDirectory, "-ts");

                var downloadMe = new List<Tuple<string, string>>
                {
                    new Tuple<string, string>(archive.SourceUrl, mainContentLocation),
                    new Tuple<string, string>(archive.ThumbnailUrlRectangle, thumbRectLocation),
                    new Tuple<string, string>(archive.ThumbnailUrlSquare, thumbSquareLocation)
                };

                if (downloadMe.Select(x => x.Item2).All(string.IsNullOrWhiteSpace))
                {
                    _logger.Error($"There were zero files to download for archive '{archive.Id}' on {archive.DisplayedOn:yyyy-MM-dd}");
                    return null;
                }

                WebClient wc = null;
                string redirectUrl = null;
                try
                {
                    wc = new WebClient();
                    downloadMe.ForEach(async d =>
                    {
                        _logger.Debug($"Began download of '{d.Item1}' to '{d.Item2}'");
                        _fileManager.CreateDirectory(destinationDirectory);
                        await wc.DownloadFileTaskAsync(new Uri(d.Item1), d.Item2);
                        _fileManager.FileUnblock(d.Item2);
                        _logger.Debug($"Finished download of '{d.Item1 ?? "[NULL]"}' to '{d.Item2}'");
                    });
                }
                catch (WebException webEx)
                {
                    var response = webEx.Response as HttpWebResponse;
                    if (response != null)
                    {
                        var statusCode = (int)response.StatusCode;
                        if (statusCode >= 300 && statusCode < 400)
                        {
                            // capture redirect url
                            redirectUrl = response.Headers["Location"];
                        }
                    }

                    if (!string.IsNullOrEmpty(redirectUrl))
                    {
                        // no redirect url provided, so just throw the exception
                        _logger.Debug($"{nameof(WebException)} thrown trying to download '{archive.SourceUrl ?? "[NULL]"}' -- details: {webEx}");
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    _logger.Debug($"{nameof(Exception)} thrown trying to download '{archive.SourceUrl ?? "[NULL]"}' -- details: {ex}");
                    throw;
                }
                finally
                {
                    wc?.Dispose();
                }

                return new EntryDownloadInfo(archive.Id, mainContentLocation, thumbRectLocation, thumbSquareLocation);
            }

            throw new NotSupportedException($"Archive type of {archive.ArchiveType} is not yet supported!!");
        }

        /// <summary>
        /// Determines the full file path to write the received archive content
        /// </summary>
        /// <param name="remoteUrl">The remote URL of what to download</param>
        /// <param name="localDirectory">The directory for this specific archive</param>
        /// <param name="fileNameSuffix">OPTIONAL - an extra suffix to put after the filename, but before the file extension.</param>
        /// <returns>The full path to where to write the file</returns>
        private string DetermineLocalFileLocation(string remoteUrl, string localDirectory, string fileNameSuffix = "")
        {
            if (string.IsNullOrWhiteSpace(remoteUrl))
                return null;

            var destinationFileName = $"{_fileManager.FileGetNameWithoutExtension(remoteUrl)}{(string.IsNullOrWhiteSpace(fileNameSuffix) ? string.Empty : fileNameSuffix.Trim())}{_fileManager.FileGetExtension(remoteUrl)}";
            return _fileManager.PathCombine(localDirectory, destinationFileName);
        }

        /// <summary>
        /// Runs a GET call against the API
        /// </summary>
        /// <param name="partialUrl">The partial URL to send</param>
        /// <param name="mediaTypeName">The response's media type name.  Recommended to use the <see cref="System.Net.Mime.MediaTypeNames"/> static class' properties instead of hard-coding a value.</param>
        /// <returns>Response content if successful</returns>
        /// <remarks>A pointer in direction of HttpResponseMessage containing GZip content came from Rick Strahl's blog (https://weblog.west-wind.com/posts/2007/jun/29/httpwebrequest-and-gzip-http-responses), then did some tuning on my original decompress based on DotNetPerls article (https://www.dotnetperls.com/decompress)</remarks>
        private async Task<string> RestApiGetStringAsync(string partialUrl, string mediaTypeName = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, partialUrl);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(_runtimeSettings.AuthorizationHeaderValue);

            if (!string.IsNullOrWhiteSpace(mediaTypeName))
            {
                request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(mediaTypeName));
            }
            request.Headers.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("gzip"));
            request.Headers.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("deflate"));

            HttpResponseMessage response = await _httpClient.SendAsync(request);
            if (response.StatusCode != HttpStatusCode.OK)
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

                throw new Exception($"A non-200 status code of {code} was returned while fetching the partial URL of '{partialUrl}'");
            }

            // got an http-ok response
            string responseString;
            if (response.Content.Headers.ContentEncoding?.Contains("gzip") ?? false)
            {
                using (GZipStream stream = new GZipStream(new MemoryStream(await response.Content.ReadAsByteArrayAsync()), CompressionMode.Decompress))
                {
                    const int Size = 4096;
                    byte[] buffer = new byte[Size];
                    using (MemoryStream memory = new MemoryStream())
                    {
                        int count = 0;
                        do
                        {
                            count = stream.Read(buffer, 0, Size);
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
                responseString = await response.Content.ReadAsStringAsync();
            }

            // need to check the inner json "status" field to look for "ok"
            var internalStatus = JObject.Parse(responseString).GetValue("status").ToString();
            if (internalStatus?.ToLower().Trim() != "ok")
                throw new Exception($"Tinybeans API returned a non-ok status code of '{internalStatus ?? "[NULL]"}'");

            return responseString;
        }
    }
}