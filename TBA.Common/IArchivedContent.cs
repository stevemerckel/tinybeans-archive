using System;

namespace TBA.Common
{
    /// <summary>
    /// General contract shared across all archived content
    /// </summary>
    public interface IArchivedContent
    {
        /// <summary>
        /// The unique id for this archive entry
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// The date in the archive that this is to be shown on
        /// </summary>
        public DateTime DisplayedOn { get; }

        /// <summary>
        /// Optional: If not null, then use this as the sort order for displaying items on the specified <see cref="DisplayedOn"/> date
        /// </summary>
        public int? SortOverride { get; set; }

        /// <summary>
        /// If this is set to <c>true</c>, then use the sort ordering specified by <seealso cref="SortOverride"/>
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