using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

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
        public List<IArchivedContent> ParseArchivedContent(string json)
        {
            //
            // note: string values are unicode encoded, but not sure whether little endian or big endian.
            // todo: find out the endian-ness of the strings
            //

            //
            // note: per-day entries are sorted where topmost in the list is the most recent.
            // todo: look for "sortOrder" attribute; 
            //         - if missing, consider topmost entry as the most recent in the day.
            //         - if found, then go by a "sortOrder" integer value (the lower it is, the more recent it is.)
            //

            var content = JObject.Parse(json);
            var result = new List<IArchivedContent>();
            foreach (var e in (JArray)content["entries"])
            {
                var parseMe = e.ToString();
                result.Add(JsonConvert.DeserializeObject<ArchivedContent>(parseMe));
            }

            _logger.Debug($"JSON response from {nameof(ParseArchivedContent)}:{Environment.NewLine}{json}");
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