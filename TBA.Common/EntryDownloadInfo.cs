namespace TBA.Common
{
    /// <summary>
    /// Placeholder container for returning meta about downloaded content from Tinybeans
    /// </summary>
    public sealed class EntryDownloadInfo
    {
        /// <summary>
        /// default ctor
        /// </summary>
        /// <param name="archiveId">The corresponding Archive ID</param>
        /// <param name="localPath">The local path to the primary content</param>
        /// <param name="thumbnailLocalPathRectangle">Optional - path to the rectangular thumbnail</param>
        /// <param name="thumbnailLocalPathSquare">Optional - path to the square thumbnail</param>
        public EntryDownloadInfo(ulong archiveId, string localPath, string thumbnailLocalPathRectangle, string thumbnailLocalPathSquare)
        {
            ArchiveId = archiveId;
            LocalUri = localPath;
            ThumbnailLocalUriRectangle = thumbnailLocalPathRectangle;
            ThumbnailLocalUriSquare = thumbnailLocalPathSquare;
        }

        /// <summary>
        /// The archive ID for the journal entry
        /// </summary>
        public ulong ArchiveId { get; }

        /// <summary>
        /// The path to the primary content locally
        /// </summary>
        public string LocalUri { get; }

        /// <summary>
        /// Optional - local path to the rectangle thumbnail image
        /// </summary>
        public string ThumbnailLocalUriRectangle { get; }

        /// <summary>
        /// Optional - local path to the square thumbnail image
        /// </summary>
        public string ThumbnailLocalUriSquare { get; }
    }
}