using System;

namespace TBA.Common
{
    public class Child : Person
    {
        public string Url { get; set; }

        public DateTime BornOn { get; set; }

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