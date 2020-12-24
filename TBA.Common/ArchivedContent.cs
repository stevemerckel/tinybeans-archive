using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using Newtonsoft;
using Newtonsoft.Json;

namespace TBA.Common
{
    /// <summary>
    /// Container class for all content journaled at Tinybeans
    /// </summary>
    public class ArchivedContent : IArchivedContent
    {
        [JsonConstructor]
        public ArchivedContent(long id, string type, int year, int month, int day, List<string> blobs, long journalId)
        {
            Id = id.ToString();
            DisplayedOn = new DateTime(year, month, day);
            switch (type?.ToUpper())
            {
                case "PHOTO":
                    ArchiveType = ArchiveType.Image;
                    break;
                case "TEXT":
                    ArchiveType = ArchiveType.Text;
                    break;
                case "VIDEO":
                    ArchiveType = ArchiveType.Video;
                    break;
                default:
                    throw new ArgumentException($"Unable to determine type for {type?.ToUpper() ?? "[NULL]"}");
            }

        }

        /// <summary>
        /// Structured ctor
        /// </summary>
        /// <param name="id">The journal id</param>
        /// <param name="displayedOn">The date this was displayed on</param>
        /// <param name="orderDisplayed">The order-position this was displayed on <see cref="DisplayedOn"/> date</param>
        /// <param name="type">The type of this archive</param>
        /// <param name="sourceUrl">The source URL for the file -- not used for type <seealso cref="ArchiveType.Text"/></param>
        public ArchivedContent(string id, DateTime displayedOn, int orderDisplayed, ArchiveType type, string caption, string sourceUrl = null)
        {
            Id = id;
            DisplayedOn = displayedOn;
            OrderDisplayed = orderDisplayed;
            ArchiveType = type;
            SourceUrl = type == ArchiveType.Text ? null : sourceUrl?.Trim();

            if (ArchiveType != ArchiveType.Text && SourceUrl == null)
                throw new ArgumentException($"You must provide a Source URL for {nameof(ArchiveType)}s of {nameof(ArchiveType.Image)} and {nameof(ArchiveType.Video)} !!");
        }

        /// <inheritdoc />
        public string Id { get; set; }

        /// <inheritdoc />
        public DateTime DisplayedOn { get; set; }

        /// <inheritdoc />
        public int OrderDisplayed { get; set; }

        /// <inheritdoc />
        public ArchiveType ArchiveType { get; set; }

        /// <inheritdoc />
        public string SourceUrl { get; set; }

        /// <inheritdoc />
        public string Caption { get; set; }

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
    }
}