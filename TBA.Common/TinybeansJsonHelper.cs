using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TBA.Common
{
    /// <inheritdoc />
    public class TinybeansJsonHelper : ITinybeansJsonHelper
    {
        private readonly IAppLogger _logger;

        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="logger">Logger implementation</param>
        public TinybeansJsonHelper(IAppLogger logger)
        {
            _logger = logger;
        }

        /// <inheritdoc />
        public List<ITinybeansArchivedContent> ParseArchivedContent(string json)
        {
            _logger.Debug($"JSON response from {nameof(ParseArchivedContent)}:{Environment.NewLine}{json}");

            var content = JObject.Parse(json);
            var entryCount = ((JArray)content["entries"]).Count();
            var result = new List<ITinybeansArchivedContent>(entryCount);
            foreach (var e in (JArray)content["entries"])
            {
                var id = (int)e["id"];

                // ensure this is not deleted -- skip it if it was deleted.
                var isDeleted = (bool)e["deleted"];
                if (isDeleted)
                {
                    _logger.Info($"Skipping content id '{id}' because it was deleted.");
                    continue;
                }

                // if this is a video upload, then make sure the upload and/or encode succeeded.
                // if it is a video upload and failed processing, then skip it.
                var isEncodeFailed = e["attachmentType"] != null 
                    && string.Equals((string)e["attachmentType"], "VIDEO", StringComparison.InvariantCultureIgnoreCase)
                    && (bool)e["attachmentEncodingFailed"];

                if (isEncodeFailed)
                {
                    _logger.Info($"Skipping content id '{id}' because it's video encode failed.");
                    continue;
                }

                // all clear!
                var parseMe = e.ToString();
                result.Add(JsonConvert.DeserializeObject<TinybeansArchivedContent>(parseMe));
            }

            if (!result.Any())
                return result;

            var isSortOverrideFound = ((JArray)content["entries"]).Any(x => x["sortOrder"] != null);
            if (!isSortOverrideFound)
            {
                return result;
            }

            // note: group by day first, then look for presence of "sortOrder" on a per-day basis.
            //       only those overridden "sortOrder" days should utilize the preferred sortorder
            _logger.Debug($"JSON response needs SORTING");
            var daysWithSortOverride = result.Where(x => x.IsSortOverridePresent).Select(x => x.DisplayedOn.Date).Distinct();
            var uniqueDays = result.Select(x => x.DisplayedOn.Date).Distinct();
            foreach (var day in uniqueDays)
            {
                // the total number of items on a "sorted" day's entries is "N"
                // tinybeans displays content in a date by the highest "sortOrder" value (i.e. N --> 1)
                // we need to reverse this logic and adjust for zero-based incrementing (i.e. 0 --> N-1)
                var matches = result.Where(x => x.DisplayedOn == day);
                var hasOverriddenSortOrder = daysWithSortOverride.Contains(day);
                var currentIndex = 0;
                if (!hasOverriddenSortOrder)
                {
                    // there is no existing override so order them normally: 0 --> N
                    foreach (var m in matches)
                    {
                        m.SortOverride = currentIndex;
                    }
                    continue;
                }

                // an override was found, so flip the ordering so it becomes 0 --> N
                var highestValue = matches.Max(x => x.SortOverride.Value);
                foreach (var m in matches.OrderByDescending(x => x.SortOverride))
                {
                    m.SortOverride = currentIndex;
                    currentIndex++;
                }
            }

            return result;
        }

        /// <inheritdoc />
        public List<JournalSummary> ParseJournalSummaries(string json)
        {
            _logger.Debug($"JSON response from {nameof(ParseJournalSummaries)}:{Environment.NewLine}{json}");
            var content = JObject.Parse(json);
            var journalEntities = (JArray)content["journals"];
            var journals = new List<JournalSummary>();
            foreach (var j in journalEntities)
            {
                var children = new List<Child>();
                foreach (var c in (JArray)j["children"])
                {
                    var child = new Child
                    {
                        Id = (long)c["id"],
                        Url = (string)c["URL"],
                        FirstName = (string)c["firstName"],
                        LastName = (string)c["lastName"],
                        BornOn = DateTime.ParseExact((string)c["dob"], "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
                        Gender = (string)c["gender"]
                    };
                    children.Add(child);
                }

                var journal = new JournalSummary
                {
                    Id = (long)j["id"],
                    CreatedOnEpoch = (long)j["timestamp"],
                    Title = (string)j["title"],
                    Url = (string)j["URL"],
                    Children = children
                };

                journals.Add(journal);
            }

            return journals;
        }
    }
}