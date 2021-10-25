using System;

namespace TBA.Common
{
    /// <summary>
    /// Base concept for a person that is involved in an archive entry
    /// </summary>
    public abstract class Person
    {
        public Person() // todo: implement pass-thru of values from sub-classes
        {

        }

        /// <summary>
        /// Unique ID for this person
        /// </summary>
        public ulong Id { get; set; }

        /// <summary>
        /// First name (given name)
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Last name (surname)
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Friendly presentation the full name
        /// </summary>
        public string FullName => $"{FirstName?.Trim() ?? string.Empty} {LastName?.Trim() ?? string.Empty}".Trim();

        /// <summary>
        /// The person's gender
        /// </summary>
        public Gender Gender { get; set; }

        /// <summary>
        /// Date when the person was born -- just assume it is local
        /// </summary>
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
            return firstCharacter switch
            {
                "M" => Gender.Male,
                "F" => Gender.Female,
                //_ => throw new ArgumentException($"Unable to convert value '{firstCharacter}' into type {nameof(Gender)} !!"),
                _ => Gender.Unknown
            };
        }
    }
}