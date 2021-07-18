using System;

namespace FileCabinetApp
{
    public static class Guard
    {
        public const int MinStringLength = 2;
        public const int MaxStringLength = 60;
        public const short WorkPlaceNumberMinValue = 1;
        public const decimal SalaryMinValue = decimal.Zero;
        public static readonly DateTime MinDate = new (1950, 1, 1);

        public static bool StringIsIncorrect(string argument)
        {
            return string.IsNullOrWhiteSpace(argument) || argument.Length < MinStringLength || argument.Length > MaxStringLength;
        }

        public static bool DateTimeRangeIsIncorrect(DateTime argument)
        {
            return DateTime.Compare(DateTime.Now, argument) < 0 || DateTime.Compare(MinDate, argument) > 0;
        }

        public static bool WorkPlaceNumberIsLessThanMinValue(short argument)
        {
            return argument < WorkPlaceNumberMinValue;
        }

        public static bool SalaryIsLessThanThanMinValue(decimal argument)
        {
            return argument < SalaryMinValue;
        }

        public static bool DepartmentValueIsIncorrect(char argument)
        {
            return !char.IsLetter(argument) || !char.IsUpper(argument);
        }
    }
}
