using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TBA.Common
{
    /// <summary>
    /// Container class for all content journaled at Tinybeans
    /// </summary>
    public class TinybeansEntry : ITinybeansEntry
    {
        [JsonConstructor]
        public TinybeansEntry(ulong id, string type, int year, int month, int day, JObject blobs, long journalId, string caption, string attachmentType, string attachmentUrl, ulong timestamp, int sortOrder = -1)
            : this(id, new DateTime(year, month, day), ConvertArchiveTextToEnum(type, attachmentType), caption, journalId.ToString(), sortOrder, null, timestamp)
        {
            if (ArchiveType == ArchiveType.Text)
                return; // nothing extra to do

            if (ArchiveType == ArchiveType.Image)
            {
                // from the JObject, grab the "o" entry as that is the "original" file upload
                SourceUrl = ((string)blobs["o"]).Trim();
                ThumbnailUrlRectangle = ((string)blobs["t"]).Trim();
                ThumbnailUrlSquare = ((string)blobs["o2"]).Trim();

                if (string.IsNullOrWhiteSpace(SourceUrl))
                    throw new ArgumentException($"Unsure how to handle an archive type of {Enum.GetName(typeof(ArchiveType), ArchiveType)} that is missing a value for {nameof(SourceUrl)} !!");

                return;
            }

            if (ArchiveType == ArchiveType.Video)
            {
                if (string.IsNullOrWhiteSpace(attachmentUrl))
                    throw new ArgumentException($"Unsure how to handle an archive type of {Enum.GetName(typeof(ArchiveType), ArchiveType)} that is missing a value for {nameof(attachmentUrl)} !!");

                // use the "attachmentUrl" property as the source url
                // then grab "t" as the scaled thumbnail and "o2" as the square-ish thumbnail
                SourceUrl = attachmentUrl.Trim();
                ThumbnailUrlRectangle = ((string)blobs["t"]).Trim();
                ThumbnailUrlSquare = ((string)blobs["o2"]).Trim();

                return;
            }

            throw new ArgumentException($"Received an unknown archive type of {Enum.GetName(typeof(ArchiveType), ArchiveType)} !!");
        }

        /// <summary>
        /// Structured ctor
        /// </summary>
        /// <param name="id">The id for this entry</param>
        /// <param name="displayedOn">The date this was displayed on</param>
        /// <param name="type">The type of this archive</param>
        /// <param name="caption">The display caption for the item</param>
        /// <param name="journalId">The journal Id this entry belongs to</param>
        /// <param name="sortOrder">The optional sort order for showing an item on the page</param>
        /// <param name="sourceUrl">The source URL for the file -- not used for type <see cref="ArchiveType.Text"/></param>
        /// <param name="timestamp">The arbitrary version number for the content in this entry</param>
        public TinybeansEntry(ulong id, DateTime displayedOn, ArchiveType type, string caption, string journalId, int sortOrder = -1, string sourceUrl = null, ulong timestamp = 0)
        {
            Id = id;
            DisplayedOn = displayedOn;
            ArchiveType = type;
            SourceUrl = type == ArchiveType.Text ? null : sourceUrl?.Trim();
            Caption = caption;
            JournalId = journalId;
            SortOverride = sortOrder >= 0 ? sortOrder : (int?)null;
            Timestamp = timestamp;
        }

        /// <inheritdoc />
        public ulong Id { get; set; }

        /// <inheritdoc />
        public DateTime DisplayedOn { get; set; }

        /// <inheritdoc />
        public ArchiveType ArchiveType { get; set; }

        /// <inheritdoc />
        public string SourceUrl { get; set; }

        /// <inheritdoc />
        public string Caption { get; set; }

        /// <inheritdoc />
        public string JournalId { get; set; }

        /// <inheritdoc />
        public int? SortOverride { get; set; }

        /// <inheritdoc />
        public bool IsSortOverridePresent => SortOverride != null && SortOverride > -1;

        /// <inheritdoc />
        public string ThumbnailUrlRectangle { get; set; }

        /// <inheritdoc />
        public string ThumbnailUrlSquare { get; set; }

        /// <inheritdoc />
        public ulong Timestamp { get; set; }

        /// <summary>
        /// Reads in the two possible strings, weighs against some business rules, and returns the matching <see cref="ArchiveType">ArchiveType</see> enum
        /// </summary>
        private static ArchiveType ConvertArchiveTextToEnum(string type, string attachmentType)
        {
            var valueToConsider = string.IsNullOrWhiteSpace(attachmentType) ? type : attachmentType;

            ArchiveType result;
            switch (valueToConsider?.Trim().ToUpper())
            {
                case "PHOTO":
                    result = ArchiveType.Image;
                    break;
                case "TEXT":
                    result = ArchiveType.Text;
                    break;
                case "VIDEO":
                    result = ArchiveType.Video;
                    break;
                default:
                    throw new ArgumentException($"Unable to determine type for '{type?.ToUpper() ?? "[NULL]"}'");
            }

            return result;
        }

        public override string ToString()
        {
            return $"[{Id}]: {ArchiveType} on {DisplayedOn.ToString("yyyy-MM-dd")}";
        }
    }
}