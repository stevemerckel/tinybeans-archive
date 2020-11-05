using System;

namespace TBA.Common
{
    /// <summary>
    /// General functionality for directly interacting with Tiny Beans API
    /// </summary>
    public interface ITinybeansApiHelper
    {
        /// <summary>
        /// Gets JSON data for a specific date
        /// </summary>
        /// <param name="date">The target date (time is ignored)</param>
        string GetByDate(DateTime date);

        /// <summary>
        /// Fetching a text value's data + metadata for a specific id
        /// </summary>
        /// <param name="journalId">The archive's unique id</param>
        /// <returns>Text archive object</returns>
        ArchivedText GetTextData(string journalId);

        /// <summary>
        /// Fetching an image's data + metadata for a specific id
        /// </summary>
        /// <param name="journalId">The archive's unique id</param>
        /// <returns>Image archive object</returns>
        ArchivedImage GetImageDate(string journalId);

        /// <summary>
        /// Fetching a video's data + metadata for a specific id
        /// </summary>
        /// <param name="journalId">The archive's unique id</param>
        /// <returns>Video archive object</returns>
        ArchivedVideo GetVideoData(string journalId);
    }
}