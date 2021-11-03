using System;
using Newtonsoft.Json;

namespace TBA.Api
{
    public interface IMomentDto
    {
        /// <summary>
        /// The unique ID for the moment
        /// </summary>
        [JsonProperty("id")]
        public long Id { get; }

        /// <summary>
        /// The associated Journal ID
        /// </summary>
        [JsonProperty("journalId")]
        public long JournalId { get; }

        /// <summary>
        /// The content type -- TEXT, VIDEO, PHOTO
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; }

        /// <summary>
        /// Optional caption for the content
        /// </summary>
        /// <remarks>For the <see cref="Type"/> of "TEXT" content, this is where the TEXT value will be found</remarks>
        [JsonProperty("caption")]
        public string Caption { get; }

        /// <summary>
        /// The local date associated to this moment
        /// </summary>
        [JsonProperty("localDate")]
        public DateTime LocalDate { get; }

        /// <summary>
        /// The URL for the resource
        /// </summary>
        /// <remarks>Moments having a <see cref="Type"/> of "TEXT" will not have a URL</remarks>
        [JsonProperty("url")]
        public string Url { get; }
    }
}