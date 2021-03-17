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
        List<JournalSummary> GetJournalSummaries();

        /// <summary>
        /// Fetches journal entries for the specific date given.
        /// </summary>
        /// <param name="date">The target date (time is ignored)</param>
        List<IArchivedContent> GetByDate(DateTime date, long journalId);

        /// <summary>
        /// Fetches journal entries for the Year + Month given.
        /// </summary>
        /// <param name="yearMonth">The target calendar year + month</param>
        List<IArchivedContent> GetEntriesByYearMonth(DateTime yearMonth, long journalId);

        /// <summary>
        /// Downloads the content to the returned byte array
        /// </summary>
        /// <param name="content">The archived content to download, specifically targetting the <seealso cref="IArchivedContent.SourceUrl"/> property</param>
        /// <param name="destinationLocation">The location file path to write the content</param>
        public void Download(IArchivedContent content, string destinationLocation);
    }
}