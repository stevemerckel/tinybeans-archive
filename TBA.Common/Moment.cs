using System;
using Newtonsoft.Json;

namespace TBA.Common
{
    /// <summary>
    /// An uploaded moment
    /// </summary>
    public sealed class Moment
    {
        /// <summary>
        /// default ctor
        /// </summary>
        /// <param name="id">ID for this moment</param>
        /// <param name="journalId">Associated Journal ID</param>
        /// <param name="type">Content type</param>
        /// <param name="caption">Optional caption</param>
        /// <param name="localDate">Local date for this content, excluding timezone</param>
        /// <param name="url"></param>
        public Moment(long id, long journalId, string type, string caption, DateTime localDate, string url)
        {
            Id = id;
            JournalId = journalId;
            Type = type?.Trim() ?? string.Empty;
            Caption = caption?.Trim() ?? string.Empty;
            LocalDate = localDate;
            Url = url?.Trim() ?? string.Empty;
        }

        [JsonProperty("id")]
        public long Id { get; }

        [JsonProperty("journalId")]
        public long JournalId { get; }

        [JsonProperty("type")]
        public string Type { get; }

        [JsonProperty("caption")]
        public string Caption { get; }

        [JsonProperty("localDate")]
        public DateTime LocalDate { get; }

        [JsonProperty("url")]
        public string Url { get; }
    }
}