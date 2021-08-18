using System;

namespace FileCabinetApp.Validators
{
    /// <summary>Last name validator.</summary>
    public class LastNameValidator
    {
        private readonly int minLength;
        private readonly int maxLength;

        /// <summary>Initializes a new instance of the <see cref="LastNameValidator"/> class.</summary>
        /// <param name="minLength">Min length of last name.</param>
        /// <param name="maxLength">Max length of last name.</param>
        public LastNameValidator(int minLength, int maxLength)
        {
            this.minLength = minLength;
            this.maxLength = maxLength;
        }

        /// <summary>Last name validation.</summary>
        /// <param name="lastName">Last name.</param>
        /// <returns>Returns true and exception message if last name is incorrect, else returns false.</returns>
        public Tuple<bool, string> ValidateParameter(string lastName)
        {
            string parameterName = nameof(lastName).Capitalize();
            string message = string.Empty;
            if (string.IsNullOrWhiteSpace(lastName))
            {
                message = $"{parameterName} cannot be null or whitespace only.";
                return new (false, message);
            }

            int stringLength = lastName.Length;
            if (stringLength < this.minLength || stringLength > this.maxLength)
            {
                message = $"{parameterName} length is less than {this.minLength} or more than {this.maxLength}.";
                return new (false, message);
            }

            return new (true, message);
        }
    }
}
