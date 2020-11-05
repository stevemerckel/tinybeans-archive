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
        /// <param name="videoBits">The byte array for the video</param>
        /// <param name="originalFilename">The original file name for the upload</param>
        /// <param name="displayedOn">The date that this was displayed on</param>
        /// <param name="orderDisplayed">The order-position this was shown on <seealso cref="DisplayedOn"/> date</param>
        public ArchivedVideo(string id, byte[] videoBits, string originalFilename, DateTime displayedOn, int orderDisplayed) : base(id, displayedOn, orderDisplayed, ArchiveType.Video)
        {
            VideoBits = videoBits;
            OriginalFilename = originalFilename;
        }

        /// <summary>
        /// The text archive entry
        /// </summary>
        public byte[] VideoBits { get; private set; }

        /// <summary>
        /// The original file name for this upload
        /// </summary>
        public string OriginalFilename { get; private set; }
    }
}