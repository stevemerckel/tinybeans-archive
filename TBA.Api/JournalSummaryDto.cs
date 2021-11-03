using System.Collections.Generic;

namespace TBA.Api
{
    public class JournalSummaryDto : IJournalSummaryDto
    {
        /// <summary>
        /// default ctor
        /// </summary>
        /// <param name="familyName">Family name, i.e. last name</param>
        /// <param name="parentNames">Parent Names</param>
        /// <param name="childrenNames">Children names</param>
        /// <param name="journalId">The Journal ID for this archive</param>
        public JournalSummaryDto(string familyName, List<string> parentNames, List<string> childrenNames, long journalId)
        {
            FamilyName = familyName?.Trim() ?? string.Empty;
            ParentNames = parentNames ?? new List<string>();
            ChildrenNames = childrenNames ?? new List<string>();
            JournalId = journalId;
        }

        /// <inheritdoc />
        public string FamilyName { get; }

        /// <inheritdoc />
        public List<string> ParentNames { get; }

        /// <inheritdoc />
        public List<string> ChildrenNames { get; }

        /// <inheritdoc />
        public long JournalId { get; }
    }
}