namespace TBA.Common
{
    public abstract class Person
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => FirstName + " " + LastName;
        public Gender Gender { get; set; }
    }
}