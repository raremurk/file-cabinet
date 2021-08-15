using System;
using System.Globalization;

namespace FileCabinetApp.Validators
{
    /// <summary>Date of birth validator.</summary>
    public class DateOfBirthValidator
    {
        private readonly DateTime minDate;

        /// <summary>Initializes a new instance of the <see cref="DateOfBirthValidator"/> class.</summary>
        /// <param name="minDate">Min value of date of birth.</param>
        public DateOfBirthValidator(DateTime minDate)
        {
            this.minDate = minDate;
        }

        /// <summary>Date of birth validation.</summary>
        /// <param name="dateOfBirth">Date of birth.</param>
        /// <returns>Returns false and exception message if date of birth is incorrect, else returns true.</returns>
        public Tuple<bool, string> ValidateParameter(DateTime dateOfBirth)
        {
            string parameterName = nameof(dateOfBirth).Capitalize();
            bool valid = DateTime.Compare(DateTime.Now, dateOfBirth) >= 0 && DateTime.Compare(this.minDate, dateOfBirth) <= 0;
            string message = !valid ? $"{parameterName} is less than {this.minDate.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture)} or more than current date." : string.Empty;
            return new (valid, message);
        }
    }
}
