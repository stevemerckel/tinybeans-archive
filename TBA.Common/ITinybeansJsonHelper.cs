using System.Collections.Generic;

namespace TBA.Common
{
    /// <summary>
    /// Wraps up JSON (de)serialization to established POCOs
    /// </summary>
    public interface ITinybeansJsonHelper
    {
        /// <summary>
        /// Builds a collection of <see cref="JournalSummary"/> objects based on the received JSON content
        /// </summary>
        /// <param name="json">JSON content</param>
        List<JournalSummary> ParseJournalSummaries(string json);

        /// <summary>
        /// Builds a collection of <see cref="TinybeansArchivedContent"/> objects based on the received JSON content
        /// </summary>
        /// <param name="json">JSON content</param>
        List<ITinybeansArchivedContent> ParseArchivedContent(string json);
    }
}