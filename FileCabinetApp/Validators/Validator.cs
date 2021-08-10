using System;
using System.Collections.Generic;

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

        /// <summary>First name validation.</summary>
        /// <param name="firstName">First name.</param>
        /// <returns>Returns true and exception message if parameter is incorrect, else returns false.</returns>
        public Tuple<bool, string> ValidateFirstName(string firstName) => this.allValidators.FirstNameValidator.ValidateParameter(firstName);

        /// <summary>Last name validation.</summary>
        /// <param name="lastName">Last name.</param>
        /// <returns>Returns true and exception message if parameter is incorrect, else returns false.</returns>
        public Tuple<bool, string> ValidateLastName(string lastName) => this.allValidators.LastNameValidator.ValidateParameter(lastName);

        /// <summary>Date of birth validation.</summary>
        /// <param name="dateOfBirth">Date of birth.</param>
        /// <returns>Returns true and exception message if parameter is incorrect, else returns false.</returns>
        public Tuple<bool, string> ValidateDateOfBirth(DateTime dateOfBirth) => this.allValidators.DateOfBirthValidator.ValidateParameter(dateOfBirth);

        /// <summary>Workplace number validation.</summary>
        /// <param name="workPlaceNumber">Workplace number.</param>
        /// <returns>Returns true and exception message if parameter is incorrect, else returns false.</returns>
        public Tuple<bool, string> ValidateWorkPlaceNumber(short workPlaceNumber) => this.allValidators.WorkPlaceNumberValidator.ValidateParameter(workPlaceNumber);

        /// <summary>Salary validation.</summary>
        /// <param name="salary">Salary.</param>
        /// <returns>Returns true and exception message if parameter is incorrect, else returns false.</returns>
        public Tuple<bool, string> ValidateSalary(decimal salary) => this.allValidators.SalaryValidator.ValidateParameter(salary);

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