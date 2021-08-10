using System;

namespace FileCabinetApp.Validators
{
    /// <summary>Validator builder.</summary>
    public class ValidatorBuilder
    {
        private readonly AllValidators allValidators = new ();

        /// <summary>Initializes firstNameValidator.</summary>
        /// <param name="minNameLength">minNameLength.</param>
        /// <param name="maxNameLength">maxNameLength.</param>
        /// <returns>Returns ValidatorBuilder.</returns>
        public ValidatorBuilder FirstNameValidator(int minNameLength, int maxNameLength)
        {
            this.allValidators.FirstNameValidator = new FirstNameValidator(minNameLength, maxNameLength);
            return this;
        }

        /// <summary>Initializes lastNameValidator.</summary>
        /// <param name="minNameLength">minNameLength.</param>
        /// <param name="maxNameLength">maxNameLength.</param>
        /// <returns>Returns ValidatorBuilder.</returns>
        public ValidatorBuilder LastNameValidator(int minNameLength, int maxNameLength)
        {
            this.allValidators.LastNameValidator = new LastNameValidator(minNameLength, maxNameLength);
            return this;
        }

        /// <summary>Initializes dateOfBirthValidator.</summary>
        /// <param name="minDate">minDate.</param>
        /// <returns>Returns ValidatorBuilder.</returns>
        public ValidatorBuilder DateOfBirthValidator(DateTime minDate)
        {
            this.allValidators.DateOfBirthValidator = new DateOfBirthValidator(minDate);
            return this;
        }

        /// <summary>Initializes workPlaceNumberValidator.</summary>
        /// <param name="workPlaceNumberMinValue">workPlaceNumberMinValue.</param>
        /// <returns>Returns ValidatorBuilder.</returns>
        public ValidatorBuilder WorkPlaceNumberValidator(short workPlaceNumberMinValue)
        {
            this.allValidators.WorkPlaceNumberValidator = new WorkPlaceNumberValidator(workPlaceNumberMinValue);
            return this;
        }

        /// <summary>Initializes salaryMinValue.</summary>
        /// <param name="salaryMinValue">salaryMinValue.</param>
        /// <returns>Returns ValidatorBuilder.</returns>
        public ValidatorBuilder SalaryValidator(decimal salaryMinValue)
        {
            this.allValidators.SalaryValidator = new SalaryValidator(salaryMinValue);
            return this;
        }

        /// <summary>Creates Validator.</summary>
        /// <returns>Returns Validator.</returns>
        public IRecordValidator Create() => new Validator(this.allValidators);
    }
}