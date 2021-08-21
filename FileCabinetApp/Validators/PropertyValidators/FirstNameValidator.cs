using System;
using FileCabinetApp.Helpers;

namespace FileCabinetApp.Validators
{
    /// <summary>First name validator.</summary>
    public class FirstNameValidator
    {
        private readonly int minLength;
        private readonly int maxLength;

        /// <summary>Initializes a new instance of the <see cref="FirstNameValidator"/> class.</summary>
        /// <param name="minLength">Min length of first name.</param>
        /// <param name="maxLength">Max length of first name.</param>
        public FirstNameValidator(int minLength, int maxLength)
        {
            this.minLength = minLength;
            this.maxLength = maxLength;
        }

        /// <summary>First name validation.</summary>
        /// <param name="firstName">First name.</param>
        /// <returns>Returns true and exception message if first name is incorrect, else returns false.</returns>
        public Tuple<bool, string> ValidateParameter(string firstName)
        {
            string parameterName = nameof(firstName).Capitalize();
            string message = string.Empty;
            if (string.IsNullOrWhiteSpace(firstName))
            {
                message = $"{parameterName} cannot be null or whitespace only.";
                return new (false, message);
            }

            int stringLength = firstName.Length;
            if (stringLength < this.minLength || stringLength > this.maxLength)
            {
                message = $"{parameterName} length is less than {this.minLength} or more than {this.maxLength}.";
                return new (false, message);
            }

            return new (true, message);
        }
    }
}
