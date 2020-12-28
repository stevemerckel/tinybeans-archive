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
        /// Gets JSON data for a specific date
        /// </summary>
        /// <param name="date">The target date (time is ignored)</param>
        List<ArchivedContent> GetByDate(DateTime date, long journalId);

        /// <summary>
        /// Fetches journal entries for the Year + Month given.
        /// </summary>
        /// <param name="yearMonth">The target calendar year + month</param>
        List<ArchivedContent> GetEntriesByYearMonth(DateTime yearMonth, long journalId);
    }
}