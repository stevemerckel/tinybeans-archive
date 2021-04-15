using System;

namespace TBA.Common
{
    /// <summary>
    /// Base concept for a person that is involved in an archive entry
    /// </summary>
    public abstract class Person
    {
        public ulong Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => FirstName + " " + LastName;
        public Gender Gender { get; set; }

        public DateTime BornOn { get; set; }

        /// <summary>
        /// Parses a gender string and returns an <see cref="Gender"/> enum
        /// </summary>
        /// <param name="genderString">The string to parse -- i.e. "male" or "female"</param>
        public static Gender GetGender(string genderString)
        {
            if (string.IsNullOrWhiteSpace(genderString))
                throw new ArgumentException($"{nameof(genderString)} is missing!");

            var firstCharacter = genderString.Trim().Substring(0, 1).ToUpper();
            switch (firstCharacter)
            {
                case "M":
                    return Gender.Male;
                case "F":
                    return Gender.Female;
                default:
                    throw new ArgumentException($"Unable to convert value '{firstCharacter}' into type {nameof(Gender)} !!");
            }
        }
    }
}