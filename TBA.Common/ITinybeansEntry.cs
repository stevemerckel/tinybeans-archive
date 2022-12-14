using System;

namespace TBA.Common
{
    /// <summary>
    /// General contract shared across all archived content
    /// </summary>
    public interface ITinybeansEntry
    {
        /// <summary>
        /// The unique id for this archive entry
        /// </summary>
        public ulong Id { get; }

        /// <summary>
        /// Some (possibly) arbitrary value that indicates a version of the entry.
        /// </summary>
        /// <remarks>
        /// When looking to see if an entry has changed or not, compare what is stored against this value from remote.
        /// </remarks>
        public ulong Timestamp { get; }

        /// <summary>
        /// The date in the archive that this is to be shown on
        /// </summary>
        public DateTime DisplayedOn { get; }

        /// <summary>
        /// Optional: If not null, then use this as the sort order for displaying items on the specified <see cref="DisplayedOn"/> date
        /// </summary>
        public int? SortOverride { get; set; }

        /// <summary>
        /// If this is set to <c>true</c>, then use the sort ordering specified by <see cref="SortOverride"/> property
        /// </summary>
        public bool IsSortOverridePresent { get; }

        /// <summary>
        /// The type of this archive entry
        /// </summary>
        public ArchiveType ArchiveType { get; }

        /// <summary>
        /// URL to fetch the resource
        /// </summary>
        public string SourceUrl { get; }

        /// <summary>
        /// Url to fetch the rectangle-dimensioned thumbnail -- note this could mean it is oriented tall or wide.
        /// </summary>
        public string ThumbnailUrlRectangle { get; }

        /// <summary>
        /// Url to fetch the square-dimensioned thumbnail.
        /// </summary>
        public string ThumbnailUrlSquare { get; }

        /// <summary>
        /// The optional text caption for the content
        /// </summary>
        /// <remarks>Types <see cref="ArchiveType.Video"/> and <see cref="ArchiveType.Image"/> *may* have a caption, but <see cref="ArchiveType.Text"/> will always have a value</remarks>
        public string Caption { get; }

        /// <summary>
        /// The parent's journal ID
        /// </summary>
        public string JournalId { get; }
    }
}