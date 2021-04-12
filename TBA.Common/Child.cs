using System;

namespace TBA.Common
{
    public class Child : Person
    {
        public string Url { get; set; }
        public DateTime BornOn { get; set; }
        public Gender GetGender(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{nameof(value)} is missing!");

            var firstCharacter = value.Trim().Substring(0, 1).ToUpper();
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