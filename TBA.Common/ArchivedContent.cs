using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TBA.Common
{
    /// <summary>
    /// Container class for all content journaled at Tinybeans
    /// </summary>
    public class ArchivedContent : IArchivedContent
    {
        [JsonConstructor]
        public ArchivedContent(long id, string type, int year, int month, int day, JObject blobs, long journalId, string caption, int sortOrder = -1)
            : this(id.ToString(), new DateTime(year, month, day), ConvertArchiveTextToEnum(type), caption, journalId.ToString(), sortOrder, null)
        {
        }

        /// <summary>
        /// Structured ctor
        /// </summary>
        /// <param name="id">The unique id for this entry</param>
        /// <param name="displayedOn">The date this was displayed on</param>
        /// <param name="type">The type of this archive</param>
        /// <param name="caption">The display caption for the item</param>
        /// <param name="journalId">The journal Id this entry belongs to</param>
        /// <param name="sortOrder">The optional sort order for showing an item on the page</param>
        /// <param name="sourceUrl">The source URL for the file -- not used for type <seealso cref="ArchiveType.Text"/></param>
        public ArchivedContent(string id, DateTime displayedOn, ArchiveType type, string caption, string journalId, int sortOrder = -1, string sourceUrl = null)
        {
            Id = id;
            DisplayedOn = displayedOn;
            ArchiveType = type;
            SourceUrl = type == ArchiveType.Text ? null : sourceUrl?.Trim();
            Caption = caption;
            JournalId = journalId;
            SortOverride = sortOrder >= 0 ? sortOrder : (int?)null;

            // todo: re-enable the section below once JsonConstructor pass-through is tested.
            //if (ArchiveType != ArchiveType.Text && SourceUrl == null)
            //    throw new ArgumentException($"You must provide a Source URL for {nameof(ArchiveType)}s of {nameof(ArchiveType.Image)} and {nameof(ArchiveType.Video)} !!");
        }

        /// <inheritdoc />
        public string Id { get; set; }

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
        public void Download(string destinationLocation)
        {
            if (File.Exists(destinationLocation))
                File.Delete(destinationLocation);

            if (ArchiveType == ArchiveType.Text)
            {
                File.WriteAllText(destinationLocation, Caption ?? string.Empty);
                return;
            }

            if (ArchiveType == ArchiveType.Image || ArchiveType == ArchiveType.Video)
            {
                WebClient wc = null;
                try
                {
                    wc = new WebClient();
                    Debug.WriteLine($"Began download of '{SourceUrl ?? "[NULL]"}' to '{destinationLocation}'");
                    wc.DownloadFile(SourceUrl, destinationLocation);
                    Debug.WriteLine($"Finished download of '{SourceUrl ?? "[NULL]"}' to '{destinationLocation}'");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"{nameof(Exception)} thrown trying to download '{SourceUrl ?? "[NULL]"}' -- details: {ex}");
                    throw;
                }
                finally
                {
                    wc?.Dispose();
                }

                return;
            }

            throw new NotSupportedException($"Archive type of {ArchiveType} is not yet supported!!");
        }

        private static ArchiveType ConvertArchiveTextToEnum(string type)
        {
            ArchiveType result;
            switch (type?.ToUpper())
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