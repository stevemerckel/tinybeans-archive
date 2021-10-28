using System.Collections.Generic;
using Newtonsoft.Json;

namespace TBA.Common
{
    /// <summary>
    /// General family information
    /// </summary>
    public sealed class FamilySummaryDto
    {
        /// <summary>
        /// default ctor
        /// </summary>
        /// <param name="familyName">Family name</param>
        /// <param name="parentNames">Parent names</param>
        /// <param name="childrenNames">Children names</param>
        public FamilySummaryDto(string familyName, List<string> parentNames, List<string> childrenNames)
        {
            FamilyName = familyName;
            ParentNames = parentNames;
            ChildrenNames = childrenNames;
        }

        [JsonProperty("familyName")]
        public string FamilyName { get; }

        [JsonProperty("parents")]
        public List<string> ParentNames { get; }

        [JsonProperty("children")]
        public List<string> ChildrenNames { get; }
    }
}