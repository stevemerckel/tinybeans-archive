using System;
using System.Collections.Generic;

namespace TBA.Common
{
    /// <summary>
    /// General functionality for directly interacting with Tiny Beans API
    /// </summary>
    public interface ITinybeansApiHelper
    {
        /// <summary>
        /// Returns a list of journal ID(s) from the API
        /// </summary>
        /// <returns>List of Journal ID(s)</returns>
        List<int> GetJournalIds();

        /// <summary>
        /// Gets JSON data for a specific date
        /// </summary>
        /// <param name="date">The target date (time is ignored)</param>
        string GetByDate(DateTime date);

        /// <summary>
        /// Fetching a text value's data + metadata for a specific id
        /// </summary>
        /// <param name="journalId">The journal ID</param>
        /// <param name="archiveId">The archive's unique id</param>
        /// <returns>Text archive object</returns>
        ArchivedText GetTextData(int journalId, string archiveId);

        /// <summary>
        /// Fetching an image's data + metadata for a specific id
        /// </summary>
        /// <param name="journalId">The journal ID</param>
        /// <param name="archiveId">The archive's unique id</param>
        /// <returns>Image archive object</returns>
        ArchivedImage GetImageData(int journalId, string archiveId);

        /// <summary>
        /// Fetching a video's data + metadata for a specific id
        /// </summary>
        /// <param name="journalId">The journal ID</param>
        /// <param name="archiveId">The archive's unique id</param>
        /// <returns>Video archive object</returns>
        ArchivedVideo GetVideoData(int journalId, string archiveId);
    }
}