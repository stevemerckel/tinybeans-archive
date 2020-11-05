using System;

namespace TBA.Common
{
    /// <summary>
    /// Representation of the image archive
    /// </summary>
    public sealed class ArchivedImage : ArchivedContent
    {
        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="id">The unique archive id</param>
        /// <param name="imageBits">The byte array for the image</param>
        /// <param name="originalFilename">The original file name for the upload</param>
        /// <param name="displayedOn">The date that this was displayed on</param>
        /// <param name="orderDisplayed">The order-position this was shown on <seealso cref="DisplayedOn"/> date</param>
        public ArchivedImage(string id, byte[] imageBits, string originalFilename, DateTime displayedOn, int orderDisplayed) : base(id, displayedOn, orderDisplayed, ArchiveType.Image)
        {
            ImageBits = imageBits;
            OriginalFilename = originalFilename;
        }

        /// <summary>
        /// The text archive entry
        /// </summary>
        public byte[] ImageBits { get; private set; }

        /// <summary>
        /// The original file name for this upload
        /// </summary>
        public string OriginalFilename { get; private set; }
    }
}