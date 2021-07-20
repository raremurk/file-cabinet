using System;

namespace FileCabinetApp
{
    /// <summary>Data validation by parameters.</summary>
    public static class Guard
    {
        /// <summary>Min length of string.</summary>
        public const int MinStringLength = 2;

        /// <summary>Max length of string.</summary>
        public const int MaxStringLength = 60;

        /// <summary>Min value of work place number.</summary>
        public const short WorkPlaceNumberMinValue = 1;

        /// <summary>Min value of salary.</summary>
        public const decimal SalaryMinValue = decimal.Zero;

        /// <summary>Min value of date of birth.</summary>
        public static readonly DateTime MinDate = new (1950, 1, 1);

        /// <summary>String validation.</summary>
        /// <param name="argument">Input string.</param>
        /// <returns>Returns true if string is incorrect, else false.</returns>
        public static bool StringIsIncorrect(string argument)
        {
            return string.IsNullOrWhiteSpace(argument) || argument.Length < MinStringLength || argument.Length > MaxStringLength;
        }

        /// <summary>Date of birth validation.</summary>
        /// <param name="argument">Date of birth.</param>
        /// <returns>Returns true if date of birth is incorrect, else false.</returns>
        public static bool DateTimeRangeIsIncorrect(DateTime argument)
        {
            return DateTime.Compare(DateTime.Now, argument) < 0 || DateTime.Compare(MinDate, argument) > 0;
        }

        /// <summary>Work place number validation.</summary>
        /// <param name="argument">Work place number.</param>
        /// <returns>Returns true if work place number is incorrect, else false.</returns>
        public static bool WorkPlaceNumberIsLessThanMinValue(short argument)
        {
            return argument < WorkPlaceNumberMinValue;
        }

        /// <summary>Salary validation.</summary>
        /// <param name="argument">Salary.</param>
        /// <returns>Returns true if salary is incorrect, else false.</returns>
        public static bool SalaryIsLessThanThanMinValue(decimal argument)
        {
            return argument < SalaryMinValue;
        }

        /// <summary>Department validation.</summary>
        /// <param name="argument">Department.</param>
        /// <returns>Returns true if department is incorrect, else false.</returns>
        public static bool DepartmentValueIsIncorrect(char argument)
        {
            return !char.IsLetter(argument) || !char.IsUpper(argument);
        }
    }
}
