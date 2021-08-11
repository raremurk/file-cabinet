using FileCabinetApp.JsonConfiguration;

namespace FileCabinetApp
{
    /// <summary>Validation rules.</summary>
    public class ValidationRules
    {
        /// <summary>Gets or sets FirstNameValidationRules.</summary>
        /// <value>FirstNameValidationRules.</value>
        public FirstNameValidationRules FirstName { get; set; }

        /// <summary>Gets or sets LastNameValidationRules.</summary>
        /// <value>LastNameValidationRules.</value>
        public LastNameValidationRules LastName { get; set; }

        /// <summary>Gets or sets DateOfBirthValidationRules.</summary>
        /// <value>DateOfBirthValidationRules.</value>
        public DateOfBirthValidationRules DateOfBirth { get; set; }

        /// <summary>Gets or sets WorkPlaceNumberValidationRules.</summary>
        /// <value>WorkPlaceNumberValidationRules.</value>
        public WorkPlaceNumberValidationRules WorkPlaceNumber { get; set; }

        /// <summary>Gets or sets SalaryValidationRules.</summary>
        /// <value>SalaryValidationRules.</value>
        public SalaryValidationRules Salary { get; set; }
    }
}
