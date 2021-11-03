using System.Collections.Generic;
using Newtonsoft.Json;

namespace TBA.Api
{
    /// <summary>
    /// Summary information about the journal
    /// </summary>
    public interface IJournalSummaryDto
    {
        /// <summary>
        /// The children names
        /// </summary>
        [JsonProperty("children")]
        List<string> ChildrenNames { get; }

        /// <summary>
        /// The family name (i.e. last name)
        /// </summary>
        [JsonProperty("familyName")]
        string FamilyName { get; }

        /// <summary>
        /// The full names of the parents
        /// </summary>
        [JsonProperty("parents")]
        List<string> ParentNames { get; }

        /// <summary>
        /// The journal ID for this archive
        /// </summary>
        [JsonProperty("journalId")]
        long JournalId { get; }
    }
}