using System;

namespace TBA.Common
{
    /// <summary>
    /// Abstract class for all content journaled at Tinybeans
    /// </summary>
    public abstract class ArchivedContent : IArchivedContent
    {
        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="id">The journal id</param>
        /// <param name="displayedOn">The date this was displayed on</param>
        /// <param name="orderDisplayed">The order-position this was displayed on <see cref="DisplayedOn"/> date</param>
        /// <param name="type">The type of this archive</param>
        /// <param name="sourceUrl">The source URL for the file -- not used for type <seealso cref="ArchiveType.Text"/></param>
        public ArchivedContent(string id, DateTime displayedOn, int orderDisplayed, ArchiveType type, string sourceUrl = null)
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
        public string Id { get; private set; }

        /// <inheritdoc />
        public DateTime DisplayedOn { get; private set; }

        /// <inheritdoc />
        public int OrderDisplayed { get; private set; }

        /// <inheritdoc />
        public ArchiveType ArchiveType { get; private set; }

        /// <inheritdoc />
        public string SourceUrl { get; private set; }
    }
}