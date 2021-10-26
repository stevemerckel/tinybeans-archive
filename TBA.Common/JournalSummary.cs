using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TBA.Common
{
    /// <summary>
    /// Summary info for the journal
    /// </summary>
    [DebuggerDisplay("[{Id}]  {Title} with {Children?.Count ?? 0} children ({Url})")]
    public sealed class JournalSummary
    {
        /// <summary>
        /// The journal ID at Tinybeans
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Title of this journal
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// When this journal was first created, expressed as UTC
        /// </summary>
        public DateTime CreatedOnUtc => DateTimeExtensions.FromUnixEpochUtc(CreatedOnEpoch);

        /// <summary>
        /// When this journal was first created, expressed as Unix Epoch numeric
        /// </summary>
        public long CreatedOnEpoch { get; set; }

        /// <summary>
        /// URL for this journal home in Tinybeans
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The child(ren) associated to the journal
        /// </summary>
        public List<Child> Children { get; set; }

        public override string ToString()
        {
            return $"[{Id}]  {Title} with {Children?.Count ?? 0} children ({Url})";
        }
    }
}