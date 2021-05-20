using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TBA.Common
{
    /// <summary>
    /// Implementation of <see cref="IJournalManager"/>
    /// </summary>
    public sealed class JournalManager : IJournalManager
    {
        private readonly IAppLogger _logger;
        private readonly IRuntimeSettings _runtimeSettings;

        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="logger">Logger implementation</param>
        /// <param name="fileManager">File manager for working with file system</param>
        /// <param name="tinybeansApiHelper">Tinybeans API helper</param>
        /// <param name="runtimeSettingsProvider">Runtime settings provider</param>
        /// <param name="rootForRepo"></param>
        public JournalManager(IAppLogger logger, IFileManager fileManager, ITinybeansApiHelper tinybeansApiHelper, IRuntimeSettingsProvider runtimeSettingsProvider, string rootForRepo)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            FileManager = fileManager ?? throw new ArgumentNullException(nameof(fileManager));
            TinybeansApi = tinybeansApiHelper ?? throw new ArgumentNullException(nameof(tinybeansApiHelper));
            _runtimeSettings = runtimeSettingsProvider?.GetRuntimeSettings() ?? throw new ArgumentNullException(nameof(runtimeSettingsProvider));
            Root = rootForRepo;

            // validate runtime settings
            _runtimeSettings.ValidateSettings();

            if (string.IsNullOrWhiteSpace(Root))
                throw new ArgumentNullException(nameof(rootForRepo));

            // create directory if it does not already exist
            if (!FileManager.DirectoryExists(Root))
                FileManager.CreateDirectory(Root);
        }

        /// <inheritdoc />
        public IFileManager FileManager { get; private set; }

        /// <inheritdoc />
        public ITinybeansApiHelper TinybeansApi { get; private set; }

        /// <inheritdoc />
        public string Root { get; private set; }

        /// <inheritdoc />
        public EntryDownloadInfo Download(ITinybeansEntry content, string destinationLocation)
        {
            return TinybeansApi.Download(content, destinationLocation);
        }

        /// <inheritdoc />
        public List<DateTime> FindDatesWithRecentChanges(string journalId)
        {
            if (FileManager.DirectoryExists(Root))
                throw new Exception($"Cannot find root directory!  Tried looking here: '{Root}'");

            if (!long.TryParse(journalId, out long actualJournalId))
                throw new ArgumentException($"The value '{journalId}' cannot be parsed to {nameof(Int64)} !!");

            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public List<ITinybeansEntry> GetArchives(string journalId, DateTime start, DateTime end)
        {
            if (string.IsNullOrWhiteSpace(journalId))
                throw new ArgumentException("Journal ID cannot be null/empty/whitespace !!");

            if (start.Date > end.Date)
                throw new ArgumentOutOfRangeException($"The {nameof(start)} must be less than or equal to {nameof(end)} !!");

            if (!long.TryParse(journalId, out long actualJournalId))
                throw new ArgumentException($"The value '{journalId}' cannot be parsed to {nameof(Int64)} !!");

            // fetch the wider pool
            var pool = new List<ITinybeansEntry>(512); // init with a large net to avoid perf hit of early expansions of List<T>
            var currentYearMonth = new DateTime(start.Year, start.Month, 1, 0, 0, 0, DateTimeKind.Local); // first day of the "start" year-month
            do
            {
                // fetch current year-month combo of archive entries
                var currentMonth = TinybeansApi.GetEntriesByYearMonth(currentYearMonth.Date, actualJournalId);
                pool.AddRange(currentMonth);

                // add a month to "current" tracker
                currentYearMonth = currentYearMonth.AddMonths(1);
            }
            while (currentYearMonth <= end.Date);

            // trim off anything before/after the inclusive target date range
            var results = pool.Where(x => x.DisplayedOn.Date.IsBetween(start.Date, end.Date)).ToList();

            // return the final trimmed collection
            results.TrimExcess();
            return results;
        }

        /// <inheritdoc />
        public List<ITinybeansEntry> GetByDate(DateTime date, long journalId)
        {
            return TinybeansApi.GetByDate(date, journalId);
        }

        /// <inheritdoc />
        public List<ITinybeansEntry> GetEntriesByYearMonth(DateTime yearMonth, long journalId)
        {
            return TinybeansApi.GetEntriesByYearMonth(yearMonth, journalId);
        }

        /// <inheritdoc />
        public List<JournalSummary> GetJournalSummaries()
        {
            return TinybeansApi.GetJournalSummaries();
        }

        /// <inheritdoc />
        public void WriteArchivesToFileSystem(List<ITinybeansEntry> archives)
        {
            if (archives == null || !archives.Any())
            {
                _logger.Debug("No entries received!  Exiting method.");
                return;
            }

            if (!FileManager.DirectoryExists(Root))
                throw new Exception($"Cannot find root directory!  Tried looking here: '{Root}'");

            // pathing logic:
            //  root
            //      /journal-id (6 digit?)
            //          /year (4-digit)
            //              /month (2-digit)
            //                  yyyy-MM.json (year-month json summary)
            //                  /day
            //                      yyyy-MM-dd.json (daily summary)
            //                      *.[text|image|video]

            //
            // General Notes:
            //      Copy img/vid/txt files to disk first.  If that fails mid-stream, then recover/restart is easy.
            //      Then copy JSON files to disk.
            //          Start with monthly JSON summary.
            //              If a monthly fails, then detect/replace is minimal effort with file count of found vs expected.
            //          Then do daily within the month.
            //              If a daily fails, then detect/replace is minimal effort with file count of found vs expected.
            //          Then move on to next month, rinse-repeat.
            //

            // write the archives to destination system
            var localPathDictionary = new List<EntryDownloadInfo>(archives.Count);
            var downloadBehavior = new Func<ITinybeansEntry, EntryDownloadInfo>((archive) =>
            {
                var archiveLocalDirectory = DeterminePathToArchiveDirectory(archive, Root);
                var downloadResults = TinybeansApi.Download(archive, archiveLocalDirectory);
                return downloadResults;
            });

            var sw = new Stopwatch();
            sw.Start();
            if (_runtimeSettings.MaxThreadCount < 2)
            {
                // process serially
                foreach (var a in archives)
                {
                    var result = downloadBehavior.Invoke(a);
                    localPathDictionary.Add(result);
                }
            }
            else
            {
                // multithread the workload
                var poolSize = (int)Math.Ceiling(archives.Count / (double)_runtimeSettings.MaxThreadCount);
                var groups = SplitList(archives, poolSize);
                var threads = new List<Task>();
                var groupSplitInfo = new List<long>();
                foreach (var g in groups)
                {
                    var task = new Task(() =>
                    {
                        const int DelayInMs = 5000;
                        const int MaxAttemptCount = 3;

                        g.ForEach(x =>
                        {
                            var isProcessed = false;
                            for (var i = 0; i < MaxAttemptCount; i++)
                            {
                                if (isProcessed)
                                    break;

                                try
                                {
                                    var result = downloadBehavior.Invoke(x);
                                    localPathDictionary.Add(result);
                                    isProcessed = true;
                                }
                                catch (WebException webEx)
                                {
                                    var msg = $"[ThreadId={Thread.CurrentThread.ManagedThreadId}]  {webEx}";
                                    if (webEx.InnerException != null)
                                        msg += $"{Environment.NewLine}*** INNER EXCEPTION *** -- {webEx.InnerException}";
                                    _logger.Error($"{nameof(WebException)} thrown trying to download file '{x.SourceUrl}' for date {x.DisplayedOn.ToString("yyyy-MM-dd")} -- Details: {webEx}");
                                }
                                catch (Exception ex)
                                {
                                    var msg = $"[ThreadId={Thread.CurrentThread.ManagedThreadId}]  {ex}";
                                    if (ex.InnerException != null)
                                        msg += $"{Environment.NewLine}*** INNER EXCEPTION *** -- {ex.InnerException}";
                                    _logger.Error($"{nameof(Exception)} thrown trying to download file '{x.SourceUrl}' for date {x.DisplayedOn.ToString("yyyy-MM-dd")} -- Details: {ex}");
                                }

                                if (!isProcessed)
                                {
                                    var totalDelayInMs = (i * DelayInMs) + DelayInMs;
                                    _logger.Warn($"[ThreadId={Thread.CurrentThread.ManagedThreadId}]  Failed attempt #{i + 1} at downloading '{x.SourceUrl}'.  Going to wait {totalDelayInMs} milliseconds and try again.");
                                    Thread.Sleep(totalDelayInMs);
                                }
                            }

                            if (!isProcessed)
                                _logger.Error($"[ThreadId={Thread.CurrentThread.ManagedThreadId}]  Failed to download '{x.SourceUrl}' after {MaxAttemptCount} attempts.");
                        });
                    }
                    , TaskCreationOptions.LongRunning);

                    threads.Add(task);
                }
                threads.ForEach(x => x.Start());
                Task.WaitAll(threads.ToArray());
            }

            sw.Stop();
            _logger.Info($"Processing time for downloading {archives.Count} items using {_runtimeSettings.MaxThreadCount} thread(s) was {sw.ElapsedMilliseconds} ms");

            // write JSON metadata to file system
            var minDate = archives.Min(x => x.DisplayedOn);
            var maxDate = archives.Max(x => x.DisplayedOn);
            var currentYearMonth = new DateTime(minDate.Year, minDate.Month, 1, 0, 0, 0, DateTimeKind.Local); // first day of the "start" year-month
            sw.Reset();
            sw.Start();
            do
            {
                // fetch pool of archives for current year-month
                var currentMonthArchives = archives
                    .Where(x => x.DisplayedOn.Date >= currentYearMonth.Date && x.DisplayedOn.Date < currentYearMonth.AddMonths(1).Date)
                    .OrderBy(x => x.DisplayedOn)
                    .ThenBy(x => x.SortOverride ?? -1)
                    .ToList();

                if (!currentMonthArchives.Any())
                {
                    _logger.Debug($"For year-month of '{currentYearMonth.ToTinybeansMonthYearString()}' there are zero entries.  Moving on.");
                    return;
                }

                // write out each day's manifest
                _logger.Info($"For year-month of '{currentYearMonth.ToTinybeansMonthYearString()}' there are {currentMonthArchives.Count()} entries.");
                var daysToWrite = currentMonthArchives.Select(x => x.DisplayedOn.Date).Distinct();
                foreach (var day in daysToWrite)
                {
                    var currentDayEntries = currentMonthArchives
                        .Where(x => x.DisplayedOn.Date == day)
                        .OrderBy(x => x.SortOverride ?? -1)
                        .ToList();
                    var dayManifestLoc = DeterminePathToWriteDayJsonManifest(currentDayEntries.First(), Root);
                    FileManager.FileWriteText(dayManifestLoc, JsonConvert.SerializeObject(currentDayEntries, Formatting.Indented), System.Text.Encoding.Unicode);
                }

                //
                // todo:    find an archive image/video entry with an emoji
                //          then ensure the emoji is written as unicode (i.e. "\u" prefix)
                //

                _logger.Debug($"Finished writing manifest for days in '{currentMonthArchives.First().DisplayedOn.ToString("yyyy-MM")}'");

                // add a month to "current" tracker
                currentYearMonth = currentYearMonth.AddMonths(1);
            }
            while (currentYearMonth <= maxDate.Date);

            // write JSON manifests
            var uniqueJournalIds = archives.Select(x => x.JournalId).Distinct();
            foreach (var j in uniqueJournalIds)
            {
                var journalDir = FileManager.PathCombine(Root, j);

                // write yyyy-MM manifests in month directories
                var uniqueYearMonths = archives
                    .Select(x => new { Year = x.DisplayedOn.Year, Month = x.DisplayedOn.Month })
                    .Distinct();

                foreach(var ym in uniqueYearMonths)
                {
                    var yearDir = FileManager.PathCombine(journalDir, ym.Year.ToString());
                    var monthFolderName = ym.Month.ToString().PadLeft(2, '0');
                    var yearMonthDir = FileManager.PathCombine(yearDir, monthFolderName);
                    var yearMonthJsonLocation = FileManager.PathCombine(yearMonthDir, $"manifest_{ym.Year}-{monthFolderName}.json");
                    var pool = archives
                        .Where(x => x.JournalId == j && x.DisplayedOn.Year == ym.Year && x.DisplayedOn.Month == ym.Month)
                        .OrderBy(x => x.DisplayedOn)
                        .ToList();
                    var yearMonthJson = JsonConvert.SerializeObject(pool);
                    FileManager.FileWriteText(yearMonthJsonLocation, yearMonthJson);
                }

                // write yyyy manifests in year directories
                var uniqueYears = archives.Select(x => x.DisplayedOn.Year).Distinct();
                foreach (var y in uniqueYears)
                {
                    var yearDir = FileManager.PathCombine(journalDir, y.ToString());
                    var yearManifestLocation = FileManager.PathCombine(yearDir, $"manifest_{y}.json");
                    var pool = archives
                        .Where(x => x.JournalId == j && x.DisplayedOn.Year == y)
                        .OrderBy(x => x.DisplayedOn)
                        .ToList();
                    var yearJson = JsonConvert.SerializeObject(pool);
                    FileManager.FileWriteText(yearManifestLocation, yearJson);
                }
            }

            sw.Stop();
            _logger.Info($"Processing time for writing manifests for {archives.Count} entries was {sw.ElapsedMilliseconds} ms");
        }

        /// <summary>
        /// Determines the full file path to write the archive date's JSON manifest
        /// </summary>
        /// <param name="archive">The archive to write</param>
        /// <param name="root">The starting directory</param>
        /// <returns>The full path to where to write the file</returns>
        private string DeterminePathToWriteDayJsonManifest(ITinybeansEntry archive, string root)
        {
            var targetDirectory = DeterminePathToArchiveDirectory(archive, root);
            var manifestFileName = $"manifest.{archive.DisplayedOn.ToString("yyyy-MM-dd")}.json";
            return FileManager.PathCombine(targetDirectory, manifestFileName);
        }

        private string DeterminePathToArchiveDirectory(ITinybeansEntry archive, string root)
        {
            var directoryElements = new List<string>
            {
                root,
                archive.JournalId,
                archive.DisplayedOn.ToString("yyyy"),
                archive.DisplayedOn.ToString("MM"),
                archive.DisplayedOn.ToString("dd")
            };
            string result = string.Empty;
            directoryElements.ForEach(x => result = FileManager.PathCombine(result, x));
            return result;
        }

        /// <summary>
        /// Splits a pool of objects into sub-pools, with each pool containing up to the specified number of elements allowed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pool">The pool to split by <paramref name="size"/></param>
        /// <param name="size">The size of each group to create out of the entire <paramref name="pool"/> of content</param>
        /// <remarks>Found on SO: https://stackoverflow.com/questions/11463734/split-a-list-into-smaller-lists-of-n-size</remarks>
        private static IEnumerable<List<T>> SplitList<T>(List<T> pool, int size)
        {
            if (size < 1)
                throw new ArgumentOutOfRangeException($"{nameof(size)} must be greater than zero!");

            if (pool.Count <= size)
                yield return pool;

            for (int i = 0; i < pool.Count; i += size)
            {
                yield return pool.GetRange(i, Math.Min(size, pool.Count - i));
            }
        }

        /// <summary>
        /// Reads metadata from file system for the supplied path and journal ID, and returns the collection of POCO objects from that metadata.
        /// </summary>
        /// <param name="journalId">The journal ID</param>
        /// <param name="rootDirectory">The root directory for the entire repo</param>
        /// <returns></returns>
        private List<ITinybeansEntry> GetArchivesFromLocalFileSystem(long journalId)
        {
            // find and parse the monthly json files.
            // note: we only want the YYYY-MM.json files, not the YYYY-MM-DD.json files.
            var expectedJournalDirectory = FileManager.PathCombine(Root, journalId.ToString());
            const string SearchCriteria = "????-??.json";
            var jsonFiles = FileManager.FileSearch(SearchCriteria, expectedJournalDirectory, true);
            if (!jsonFiles.Any())
            {
                _logger.Warn($"No matching JSON files found, so returning NULL.  Used search criteria '{SearchCriteria}' in directory '{expectedJournalDirectory}'");
                return null;
            }

            // parse json content into POCOs
            var archives = new List<ITinybeansEntry>(jsonFiles.Count() * 2); // hack: init with a wide net by assuming 2 content entries per JSON file
            var fileProcessedCount = 0;
            try
            {
                foreach (var file in jsonFiles)
                {
                    var content = FileManager.FileReadAllText(file);
                    if (string.IsNullOrWhiteSpace(content))
                    {
                        _logger.Debug($"No JSON content was found in the file path: '{file}'");
                        continue;
                    }

                    archives.Add(JsonConvert.DeserializeObject<TinybeansEntry>(content));
                    fileProcessedCount++;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Unable to parse the local content library!  Details: {ex}");
                return null;
            }

            if (fileProcessedCount != jsonFiles.Count())
                _logger.Warn($"Not all of the JSON files were processed!  Look earlier in the log for details.  (expected {jsonFiles.Count()} but only completed {fileProcessedCount})");

            return archives;
        }
    }
}