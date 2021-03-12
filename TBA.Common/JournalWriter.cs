using System;
using System.Collections.Generic;

namespace TBA.Common
{
    /// <summary>
    /// Implementation of <see cref="IJournalWriter"/>
    /// </summary>
    public sealed class JournalWriter : IJournalWriter
    {
        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="fileManager">File manager for working with file system</param>
        /// <param name="tinybeansApiHelper">Tinybeans API helper</param>
        /// <param name="rootForRepo"></param>
        public JournalWriter(IFileManager fileManager, ITinybeansApiHelper tinybeansApiHelper, string rootForRepo)
        {
            FileManager = fileManager ?? throw new ArgumentNullException(nameof(fileManager));
            TinybeansApi = tinybeansApiHelper ?? throw new ArgumentNullException(nameof(tinybeansApiHelper));
            Root = rootForRepo;

            if (string.IsNullOrWhiteSpace(Root))
                throw new ArgumentNullException(nameof(rootForRepo));

            // create directory if it does not already exist
            if (!FileManager.DirectoryExists(Root))
                FileManager.CreateDirectory(Root);
        }

        /// <inheritdoc />
        public IFileManager FileManager { get; private set; }

        /// <inheritdoc />
        public ITinybeansApiHelper TinybeansApi { get; private set; }
        
        /// <inheritdoc />
        public string Root { get; private set; }

        /// <inheritdoc />
        public List<DateTime> FindDatesWithRecentChanges()
        {
            if (FileManager.DirectoryExists(Root))
                throw new Exception($"Cannot find root directory!  Tried looking here: '{Root}'");

            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void WriteArchivesToFileSystem(List<IArchivedContent> archives)
        {
            if (FileManager.DirectoryExists(Root))
                throw new Exception($"Cannot find root directory!  Tried looking here: '{Root}'");

            // pathing logic:
            //  root
            //      /journal-id (6 digit?)
            //          /year (4-digit)
            //              /month (2-digit)
            //                  YYYY-MM.json (year-month json summary)
            //                  /day
            //                      YYYY-MM-DD-json (daily summary)
            //                      *.[text|image|video]
        }
    }
}