using System;
using System.Collections.Generic;

namespace TBA.Common
{
    /// <summary>
    /// Manages logic for writing journal archives to the file system
    /// </summary>
    public interface IJournalManager
    {
        /// <summary>
        /// Writes the received archives to the file system
        /// </summary>
        /// <param name="archives">The archives to write</param>
        void WriteArchivesToFileSystem(List<IArchivedContent> archives);

        /// <summary>
        /// Looks for dates at the Tinybeans API having at least one change compared to the local copy
        /// </summary>
        /// <returns></returns>
        List<DateTime> FindDatesWithRecentChanges(string journalId);

        /// <summary>
        /// Fetches archive data within the specified date range
        /// </summary>
        /// <param name="journalId">The target journal ID</param>
        /// <param name="start">The inclusive start -- only the <see cref="DateTime.Date"/> property is considered, and assumes "midnight" of the date.</param>
        /// <param name="end">The inclusive end -- only the <see cref="DateTime.Date"/> property is considered, and will go through the entire day</param>
        /// <returns>List of all archives found within the range</returns>
        List<IArchivedContent> GetArchives(string journalId, DateTime start, DateTime end);

        /// <summary>
        /// Get the instance of the <see cref="IFileManager"/> object
        /// </summary>
        IFileManager FileManager { get; }

        /// <summary>
        /// Get the instance of the <see cref="ITinybeansApiHelper"/> object
        /// </summary>
        ITinybeansApiHelper TinybeansApi { get; }

        /// <summary>
        /// Get the root path for reading/writing the archived content on the file system
        /// </summary>
        string Root { get; }
    }
}