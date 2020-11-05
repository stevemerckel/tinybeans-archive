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
        /// <param name="archiveType">The type of this archive</param>
        public ArchivedContent(string id, DateTime displayedOn, int orderDisplayed, ArchiveType type)
        {
            Id = id;
            DisplayedOn = displayedOn;
            OrderDisplayed = orderDisplayed;
            ArchiveType = type;
        }

        /// <inheritdoc />
        public string Id { get; private set; }

        /// <inheritdoc />
        public DateTime DisplayedOn { get; private set; }

        /// <inheritdoc />
        public int OrderDisplayed { get; private set; }

        /// <inheritdoc />
        public ArchiveType ArchiveType { get; private set; }
    }
}