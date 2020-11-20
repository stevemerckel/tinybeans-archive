using System;

namespace TBA.Common
{
    public class Child
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => FirstName + " " + LastName;
        public string Url { get; set; }
        public DateTime BornOn { get; set; }
        public string Gender { get; set; }
    }
}