using System;
using System.Collections.Generic;

namespace TBA.Common
{
    public class JournalSummary
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedOnUtc => DateTimeExtensions.FromUnixEpochUtc(CreatedOnEpoch);
        public long CreatedOnEpoch { get; set; }
        public string Url { get; set; }
        public List<Child> Children { get; set; }

        public override string ToString()
        {
            return $"[{Id}]  {Title} with {Children?.Count ?? 0} children ({Url})";
        }
    }
}