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
    }
}