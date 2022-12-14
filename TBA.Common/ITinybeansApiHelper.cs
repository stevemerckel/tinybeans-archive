using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TBA.Common
{
    /// <summary>
    /// General functionality for directly interacting with Tiny Beans API
    /// </summary>
    public interface ITinybeansApiHelper
    {
        /// <summary>
        /// Returns summary information about each journal from the API
        /// </summary>
        /// <returns>List of Journal ID(s)</returns>
        Task<List<JournalSummary>> GetJournalSummariesAsync();

        /// <summary>
        /// Fetches journal entries for the specific date given.
        /// </summary>
        /// <param name="date">The target date (time is ignored)</param>
        Task<List<ITinybeansEntry>> GetByDateAsync(DateTime date, long journalId);

        /// <summary>
        /// Fetches journal entries for the Year + Month given.
        /// </summary>
        /// <param name="yearMonth">The target calendar year + month</param>
        Task<List<ITinybeansEntry>> GetEntriesByYearMonthAsync(DateTime yearMonth, long journalId);

        /// <summary>
        /// Downloads the content to the returned byte array
        /// </summary>
        /// <param name="content">The archived content to download, specifically targetting the <see cref="ITinybeansEntry.SourceUrl"/> property</param>
        /// <param name="destinationDirectory">The directory to be used for this specific archive</param>
        Task<EntryDownloadInfo> DownloadAsync(ITinybeansEntry content, string destinationDirectory);
    }
}