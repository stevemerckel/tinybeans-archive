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
        /// The order-position that the content was presented on <see cref="DisplayedOn"/> date
        /// </summary>
        public int OrderDisplayed { get; }

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
        /// Downloads the content to the returned byte array
        /// </summary>
        /// <param name="destinationLocation">The location file path to write the content</param>
        public void Download(string destinationLocation);
    }
}