using System;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>Provides functionality to data validation by default parameters.</summary>
    public class DefaultValidator : IRecordValidator
    {
        private const int MinStringLength = 2;
        private const int MaxStringLength = 60;
        private const short WorkPlaceNumberMinValue = 1;
        private const decimal SalaryMinValue = decimal.Zero;
        private static readonly DateTime MinDate = new (1950, 1, 1);

        /// <summary>String validation.</summary>
        /// <param name="stringLength">Length of input string.</param>
        /// <returns>Returns true if string is incorrect, else false.</returns>
        public bool CheckString(int stringLength)
        {
            return stringLength < MinStringLength || stringLength > MaxStringLength;
        }

        /// <summary>Date of birth validation.</summary>
        /// <param name="argument">Date of birth.</param>
        /// <returns>Returns true if date of birth is incorrect, else false.</returns>
        public bool CheckDateTimeRange(DateTime argument)
        {
            return DateTime.Compare(DateTime.Now, argument) < 0 || DateTime.Compare(MinDate, argument) > 0;
        }

        /// <summary>Work place number validation.</summary>
        /// <param name="argument">Work place number.</param>
        /// <returns>Returns true if work place number is incorrect, else false.</returns>
        public bool CheckWorkPlaceNumber(short argument)
        {
            return argument < WorkPlaceNumberMinValue;
        }

        /// <summary>Salary validation.</summary>
        /// <param name="argument">Salary.</param>
        /// <returns>Returns true if salary is incorrect, else false.</returns>
        public bool CheckSalary(decimal argument)
        {
            return argument < SalaryMinValue;
        }

        /// <summary>Department validation.</summary>
        /// <param name="argument">Department.</param>
        /// <returns>Returns true if department is incorrect, else false.</returns>
        public bool CheckDepartment(char argument)
        {
            return !char.IsLetter(argument) || !char.IsUpper(argument);
        }

        /// <summary>Record validation.</summary>
        /// <param name="record">Object representing a record.</param>
        public void ValidateParameters(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            if (string.IsNullOrWhiteSpace(record.FirstName))
            {
                throw new ArgumentNullException(nameof(record.FirstName), "Input string cannot be null or whitespace only.");
            }

            if (this.CheckString(record.FirstName.Length))
            {
                throw new ArgumentException($"Length of {nameof(record.FirstName)} is less than {MinStringLength} or more than {MaxStringLength}.");
            }

            if (string.IsNullOrWhiteSpace(record.LastName))
            {
                throw new ArgumentNullException(nameof(record.LastName), "Input string cannot be null or whitespace only.");
            }

            if (this.CheckString(record.LastName.Length))
            {
                throw new ArgumentException($"Length of {nameof(record.LastName)} is less than {MinStringLength} or more than {MaxStringLength}.");
            }

            if (this.CheckDateTimeRange(record.DateOfBirth))
            {
                throw new ArgumentException($"{nameof(record.DateOfBirth)} is less than {MinDate.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture)} or more than current date.");
            }

            if (this.CheckWorkPlaceNumber(record.WorkPlaceNumber))
            {
                throw new ArgumentException($"{nameof(record.WorkPlaceNumber)} is less than {WorkPlaceNumberMinValue}.");
            }

            if (this.CheckSalary(record.Salary))
            {
                throw new ArgumentException($"{nameof(record.Salary)} cannot be less than {SalaryMinValue}.");
            }

            if (this.CheckDepartment(record.Department))
            {
                throw new ArgumentException($"{nameof(record.Department)} can only be uppercase letter.");
            }
        }
    }
}
