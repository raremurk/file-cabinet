using System;
using FileCabinetApp.Validators;

namespace FileCabinetApp
{
    /// <summary>Provides functionality to data validation by default parameters.</summary>
    public class DefaultValidator : IRecordValidator
    {
        private const int MinNameLength = 2;
        private const int MaxNameLength = 60;
        private const short WorkPlaceNumberMinValue = 1;
        private const decimal SalaryMinValue = decimal.Zero;
        private static readonly DateTime MinDate = new (1950, 1, 1);

        /// <summary>First name validation.</summary>
        /// <param name="firstName">First name.</param>
        /// <returns>Returns true and exception message if parameter is incorrect, else returns false.</returns>
        public Tuple<bool, string> ValidateFirstName(string firstName) => new FirstNameValidator(MinNameLength, MaxNameLength).ValidateParameter(firstName);

        /// <summary>Last name validation.</summary>
        /// <param name="lastName">Last name.</param>
        /// <returns>Returns true and exception message if parameter is incorrect, else returns false.</returns>
        public Tuple<bool, string> ValidateLastName(string lastName) => new LastNameValidator(MinNameLength, MaxNameLength).ValidateParameter(lastName);

        /// <summary>Date of birth validation.</summary>
        /// <param name="dateOfBirth">Date of birth.</param>
        /// <returns>Returns true and exception message if parameter is incorrect, else returns false.</returns>
        public Tuple<bool, string> ValidateDateOfBirth(DateTime dateOfBirth) => new DateOfBirthValidator(MinDate).ValidateParameter(dateOfBirth);

        /// <summary>Workplace number validation.</summary>
        /// <param name="workPlaceNumber">Workplace number.</param>
        /// <returns>Returns true and exception message if parameter is incorrect, else returns false.</returns>
        public Tuple<bool, string> ValidateWorkPlaceNumber(short workPlaceNumber) => new WorkPlaceNumberValidator(WorkPlaceNumberMinValue).ValidateParameter(workPlaceNumber);

        /// <summary>Salary validation.</summary>
        /// <param name="salary">Salary.</param>
        /// <returns>Returns true and exception message if parameter is incorrect, else returns false.</returns>
        public Tuple<bool, string> ValidateSalary(decimal salary) => new SalaryValidator(SalaryMinValue).ValidateParameter(salary);

        /// <summary>Department validation.</summary>
        /// <param name="department">Department.</param>
        /// <returns>Returns true and exception message if parameter is incorrect, else returns false.</returns>
        public Tuple<bool, string> ValidateDepartment(char department) => DepartmentValidator.ValidateParameter(department);

        /// <summary>Record validation.</summary>
        /// <param name="record">Object representing a record.</param>
        /// <returns>Returns true and exception message if record is incorrect, else returns false.</returns>
        public Tuple<bool, string> ValidateRecord(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            bool recordIsInvalid = false;
            string message = string.Empty;

            Tuple<bool, string>[] validationResults =
            {
                this.ValidateFirstName(record.FirstName),
                this.ValidateLastName(record.LastName),
                this.ValidateDateOfBirth(record.DateOfBirth),
                this.ValidateWorkPlaceNumber(record.WorkPlaceNumber),
                this.ValidateSalary(record.Salary),
                this.ValidateDepartment(record.Department),
            };

            foreach (var result in validationResults)
            {
                recordIsInvalid = result.Item1;
                if (recordIsInvalid)
                {
                    message = $"Record #{record.Id} is invalid. {result.Item2}";
                    break;
                }
            }

            return new (recordIsInvalid, message);
        }

        /// <summary>Record validation.</summary>
        /// <param name="record">Object representing a record.</param>
        public void ValidateRecordWithExceptions(FileCabinetRecord record)
        {
            Tuple<bool, string> validationResult = this.ValidateRecord(record);
            if (!validationResult.Item1)
            {
                throw new ArgumentException(validationResult.Item2);
            }
        }
    }
}