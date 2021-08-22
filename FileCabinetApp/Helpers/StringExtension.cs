using System;
using System.Globalization;

namespace FileCabinetApp.Helpers
{
    /// <summary>StringExtension class.</summary>
    public static class StringExtension
    {
        /// <summary>Capitalize string.</summary>
        /// <param name="input">Input string.</param>
        /// <returns>Returns string with uppercase first letter.</returns>
        public static string Capitalize(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentNullException(nameof(input));
            }

            string firstLetter = input[0..1].ToUpper(CultureInfo.InvariantCulture);
            return input.Length == 1 ? firstLetter : $"{firstLetter}{input[1..]}";
        }
    }
}
