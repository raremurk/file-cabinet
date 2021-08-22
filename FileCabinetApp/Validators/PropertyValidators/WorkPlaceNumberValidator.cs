using System;
using FileCabinetApp.Helpers;

namespace FileCabinetApp.Validators
{
    /// <summary>Workplace number validator.</summary>
    public class WorkPlaceNumberValidator
    {
        private readonly short workPlaceNumberMinValue;

        /// <summary>Initializes a new instance of the <see cref="WorkPlaceNumberValidator"/> class.</summary>
        /// <param name="minvalue">Min value of Workplace number.</param>
        public WorkPlaceNumberValidator(short minvalue)
        {
            this.workPlaceNumberMinValue = minvalue;
        }

        /// <summary>Work place number validation.</summary>
        /// <param name="workPlaceNumber">Work place number.</param>
        /// <returns>Returns false and exception message if work place number is incorrect, else returns true.</returns>
        public Tuple<bool, string> ValidateParameter(short workPlaceNumber)
        {
            string parameterName = nameof(workPlaceNumber).Capitalize();
            bool valid = workPlaceNumber >= this.workPlaceNumberMinValue;
            string message = !valid ? $"{parameterName} is less than {this.workPlaceNumberMinValue}." : string.Empty;
            return new (valid, message);
        }
    }
}