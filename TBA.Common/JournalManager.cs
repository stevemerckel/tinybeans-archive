using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace TBA.Common
{
    /// <summary>
    /// Implementation of <see cref="IJournalManager"/>
    /// </summary>
    public sealed class JournalManager : IJournalManager
    {
        private readonly IAppLogger _logger;

        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="fileManager">File manager for working with file system</param>
        /// <param name="tinybeansApiHelper">Tinybeans API helper</param>
        /// <param name="rootForRepo"></param>
        public JournalManager(IAppLogger logger, IFileManager fileManager, ITinybeansApiHelper tinybeansApiHelper, string rootForRepo)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            FileManager = fileManager ?? throw new ArgumentNullException(nameof(fileManager));
            TinybeansApi = tinybeansApiHelper ?? throw new ArgumentNullException(nameof(tinybeansApiHelper));
            Root = rootForRepo;

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
        public List<DateTime> FindDatesWithRecentChanges(string journalId)
        {
            if (FileManager.DirectoryExists(Root))
                throw new Exception($"Cannot find root directory!  Tried looking here: '{Root}'");

            if (!long.TryParse(journalId, out long actualJournalId))
                throw new ArgumentException($"The value '{journalId}' cannot be parsed to {nameof(Int64)} !!");

            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public List<IArchivedContent> GetArchives(string journalId, DateTime start, DateTime end)
        {
            if (string.IsNullOrWhiteSpace(journalId))
                throw new ArgumentException("Journal ID cannot be null/empty/whitespace !!");

            if (start.Date > end.Date)
                throw new ArgumentOutOfRangeException($"The {nameof(start)} must be less than or equal to {nameof(end)} !!");

            if (!long.TryParse(journalId, out long actualJournalId))
                throw new ArgumentException($"The value '{journalId}' cannot be parsed to {nameof(Int64)} !!");

            // fetch the wider pool
            var pool = new List<IArchivedContent>(512); // init to a large net to avoid early expansions of List<T>
            var currentYearMonth = new DateTime(start.Year, start.Month, 1); // first day of the "start" year-month
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
        public void WriteArchivesToFileSystem(List<IArchivedContent> archives)
        {
            if (FileManager.DirectoryExists(Root))
                throw new Exception($"Cannot find root directory!  Tried looking here: '{Root}'");

            // pathing logic:
            //  root
            //      /journal-id (6 digit?)
            //          /year (4-digit)
            //              /month (2-digit)
            //                  YYYY-MM.json (year-month json summary)
            //                  /day
            //                      YYYY-MM-DD-json (daily summary)
            //                      *.[text|image|video]

            throw new NotImplementedException();
        }

        /// <summary>
        /// Reads metadata from file system for the supplied path and journal ID, and returns the collection of POCO objects from that metadata.
        /// </summary>
        /// <param name="journalId">The journal ID</param>
        /// <param name="rootDirectory">The root directory for the entire repo</param>
        /// <returns></returns>
        private List<IArchivedContent> GetArchivesFromLocalFileSystem(long journalId, string rootDirectory)
        {
            var expectedJournalDirectory = FileManager.PathCombine(rootDirectory, journalId.ToString());
            if (!FileManager.DirectoryExists(expectedJournalDirectory))
            {
                _logger.Error($"The received directory does not exist or is inaccessible!  '{rootDirectory}'");
                return null;
            }

            // find and parse the monthly json files.
            // note: we only want the YYYY-MM.json files, not the YYYY-MM-DD.json files.
            const string SearchCriteria = "????-??.json";
            var jsonFiles = FileManager.FileSearch(SearchCriteria, expectedJournalDirectory, true);
            if (!jsonFiles.Any())
            {
                _logger.Warn($"No matching JSON files found, so returning NULL.  Used search criteria '{SearchCriteria}' in directory '{expectedJournalDirectory}'");
                return null;
            }

            // parse json content into POCOs
            var archives = new List<IArchivedContent>(jsonFiles.Count() * 2); // hack: init with a wide net by assuming 2 content entries per JSON file
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

                    archives.Add(JsonConvert.DeserializeObject<ArchivedContent>(content));
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