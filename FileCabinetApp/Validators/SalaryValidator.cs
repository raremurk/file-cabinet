using System;

namespace FileCabinetApp.Validators
{
    /// <summary>Salary validator.</summary>
    public class SalaryValidator
    {
        private readonly decimal salaryMinValue;

        /// <summary>Initializes a new instance of the <see cref="SalaryValidator"/> class.</summary>
        /// <param name="salaryMinValue">Min value of salary.</param>
        public SalaryValidator(decimal salaryMinValue)
        {
            this.salaryMinValue = salaryMinValue;
        }

        /// <summary>Salary validation.</summary>
        /// <param name="salary">Salary.</param>
        /// <returns>Returns false and exception message if salary is incorrect, else returns true.</returns>
        public Tuple<bool, string> ValidateParameter(decimal salary)
        {
            bool valid = salary >= this.salaryMinValue;
            string message = !valid ? $"{nameof(salary)} cannot be less than {this.salaryMinValue}." : string.Empty;
            return new (valid, message);
        }
    }
}