using System;

namespace TBA.Common
{
    /// <summary>
    /// Representation of the video archive
    /// </summary>
    public sealed class ArchivedVideo : ArchivedContent
    {
        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="id">The unique archive id</param>
        /// <param name="displayedOn">The date that this was displayed on</param>
        /// <param name="orderDisplayed">The order-position this was shown on <seealso cref="DisplayedOn"/> date</param>
        /// <param name="sourceUrl">The source URL to find the file</param>
        public ArchivedVideo(string id, DateTime displayedOn, int orderDisplayed, string sourceUrl) : base(id, displayedOn, orderDisplayed, ArchiveType.Video, sourceUrl)
        {
        }
    }
}