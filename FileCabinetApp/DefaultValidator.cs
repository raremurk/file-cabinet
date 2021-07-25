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

        /// <summary>Name validation.</summary>
        /// <param name="name">Input string representing the name.</param>
        /// <returns>Returns false and exception message if name is incorrect, else returns true.</returns>
        public Tuple<bool, string> NameIsCorrect(string name)
        {
            string parameterName = nameof(name);
            string message = string.Empty;
            if (string.IsNullOrWhiteSpace(name))
            {
                message = $"{parameterName} cannot be null or whitespace only.";
                return new (false, message);
            }

            int stringLength = name.Length;
            if (stringLength < MinStringLength || stringLength > MaxStringLength)
            {
                message = $"Length of {parameterName} is less than {MinStringLength} or more than {MaxStringLength}.";
                return new (false, message);
            }

            return new (true, message);
        }

        /// <summary>Date of birth validation.</summary>
        /// <param name="dateOfBirth">Date of birth.</param>
        /// <returns>Returns false and exception message if date of birth is incorrect, else returns true.</returns>
        public Tuple<bool, string> DateOfBirthIsCorrect(DateTime dateOfBirth)
        {
            bool valid = DateTime.Compare(DateTime.Now, dateOfBirth) >= 0 && DateTime.Compare(MinDate, dateOfBirth) <= 0;
            string message = !valid ? $"{nameof(dateOfBirth)} is less than {MinDate.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture)} or more than current date." : string.Empty;
            return new (valid, message);
        }

        /// <summary>Work place number validation.</summary>
        /// <param name="workPlaceNumber">Work place number.</param>
        /// <returns>Returns false and exception message if work place number is incorrect, else returns true.</returns>
        public Tuple<bool, string> WorkPlaceNumberIsCorrect(short workPlaceNumber)
        {
            bool valid = workPlaceNumber >= WorkPlaceNumberMinValue;
            string message = !valid ? $"{nameof(workPlaceNumber)} is less than {WorkPlaceNumberMinValue}." : string.Empty;
            return new (valid, message);
        }

        /// <summary>Salary validation.</summary>
        /// <param name="salary">Salary.</param>
        /// <returns>Returns false and exception message if salary is incorrect, else returns true.</returns>
        public Tuple<bool, string> SalaryIsCorrect(decimal salary)
        {
            bool valid = salary >= SalaryMinValue;
            string message = !valid ? $"{nameof(salary)} cannot be less than {SalaryMinValue}." : string.Empty;
            return new (valid, message);
        }

        /// <summary>Department validation.</summary>
        /// <param name="department">Department.</param>
        /// <returns>Returns false and exception message if department is incorrect, else returns true.</returns>
        public Tuple<bool, string> DepartmentIsCorrect(char department)
        {
            bool valid = char.IsLetter(department) && char.IsUpper(department);
            string message = !valid ? $"{nameof(department)} can only be uppercase letter." : string.Empty;
            return new (valid, message);
        }

        /// <summary>Record validation.</summary>
        /// <param name="record">Object representing a record.</param>
        public void ValidateParameters(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            Tuple<bool, string>[] validationResults =
            {
                this.NameIsCorrect(record.FirstName),
                this.NameIsCorrect(record.LastName),
                this.DateOfBirthIsCorrect(record.DateOfBirth),
                this.WorkPlaceNumberIsCorrect(record.WorkPlaceNumber),
                this.SalaryIsCorrect(record.Salary),
                this.DepartmentIsCorrect(record.Department),
            };

            if (string.IsNullOrWhiteSpace(record.FirstName))
            {
                throw new ArgumentNullException(validationResults[0].Item2);
            }

            if (string.IsNullOrWhiteSpace(record.LastName))
            {
                throw new ArgumentNullException(validationResults[1].Item2);
            }

            foreach (var result in validationResults)
            {
                if (!result.Item1)
                {
                    throw new ArgumentException(result.Item2);
                }
            }
        }
    }
}