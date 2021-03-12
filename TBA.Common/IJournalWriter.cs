using System;
using System.Collections.Generic;
using System.Text;

namespace TBA.Common
{
    /// <summary>
    /// Manages logic for writing journal archives to the file system
    /// </summary>
    public interface IJournalWriter
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
        List<DateTime> FindDatesWithRecentChanges();

        /// <summary>
        /// Get/Set the instance of the <see cref="IFileManager"/> object
        /// </summary>
        IFileManager FileManager { get; }

        /// <summary>
        /// Get/Set the instance of the <see cref="ITinybeansApiHelper"/> object
        /// </summary>
        ITinybeansApiHelper TinybeansApi { get; }

        /// <summary>
        /// This is the root path for reading/writing the archived content on the file system
        /// </summary>
        string Root { get; }
    }
}