using System;
using FileCabinetApp.Models;

namespace FileCabinetApp.Validators
{
    /// <summary>Validator.</summary>
    /// <seealso cref="IRecordValidator" />
    public class Validator : IRecordValidator
    {
        private readonly AllValidators allValidators;

        /// <summary>Initializes a new instance of the <see cref="Validator"/> class.</summary>
        /// <param name="allValidators">AllValidators.</param>
        /// <exception cref="ArgumentNullException">Thrown when allValidators is null.</exception>
        public Validator(AllValidators allValidators)
        {
            this.allValidators = allValidators ?? throw new ArgumentNullException(nameof(allValidators));
        }

        /// <inheritdoc cref="IRecordValidator.ValidateFirstName(string)"/>
        public Tuple<bool, string> ValidateFirstName(string firstName) => this.allValidators.FirstNameValidator.ValidateParameter(firstName);

        /// <inheritdoc cref="IRecordValidator.ValidateLastName(string)"/>
        public Tuple<bool, string> ValidateLastName(string lastName) => this.allValidators.LastNameValidator.ValidateParameter(lastName);

        /// <inheritdoc cref="IRecordValidator.ValidateDateOfBirth(DateTime)"/>
        public Tuple<bool, string> ValidateDateOfBirth(DateTime dateOfBirth) => this.allValidators.DateOfBirthValidator.ValidateParameter(dateOfBirth);

        /// <inheritdoc cref="IRecordValidator.ValidateWorkPlaceNumber(short)"/>
        public Tuple<bool, string> ValidateWorkPlaceNumber(short workPlaceNumber) => this.allValidators.WorkPlaceNumberValidator.ValidateParameter(workPlaceNumber);

        /// <inheritdoc cref="IRecordValidator.ValidateSalary(decimal)"/>
        public Tuple<bool, string> ValidateSalary(decimal salary) => this.allValidators.SalaryValidator.ValidateParameter(salary);

        /// <inheritdoc cref="IRecordValidator.ValidateDepartment(char)"/>
        public Tuple<bool, string> ValidateDepartment(char department) => DepartmentValidator.ValidateParameter(department);

        /// <inheritdoc cref="IRecordValidator.ValidateRecord(FileCabinetRecord)"/>
        public Tuple<bool, string> ValidateRecord(FileCabinetRecord record)
        {
            _ = record ?? throw new ArgumentNullException(nameof(record));

            bool recordIsValid = false;
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
                recordIsValid = result.Item1;
                if (!recordIsValid)
                {
                    message = $"Record #{record.Id} is invalid. {result.Item2}";
                    break;
                }
            }

            return new (recordIsValid, message);
        }

        /// <inheritdoc cref="IRecordValidator.ValidateRecordWithExceptions(FileCabinetRecord)"/>
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