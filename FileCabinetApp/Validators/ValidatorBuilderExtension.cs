using System;

namespace FileCabinetApp.Validators
{
    /// <summary>Validator builder.</summary>
    public static class ValidatorBuilderExtension
    {
        /// <summary>Creates default validator.</summary>
        /// <param name="builder">Builder.</param>
        /// <returns>Returns <see cref="IRecordValidator"/> object.</returns>
        /// <exception cref="ArgumentNullException">Thrown when builder is null.</exception>
        public static IRecordValidator CreateDefault(this ValidatorBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder
                .FirstNameValidator(2, 60)
                .LastNameValidator(2, 60)
                .DateOfBirthValidator(new (1950, 1, 1))
                .WorkPlaceNumberValidator(1)
                .SalaryValidator(decimal.Zero)
                .Create();
        }

        /// <summary>Creates custom validator.</summary>
        /// <param name="builder">Builder.</param>
        /// <returns>Returns <see cref="IRecordValidator"/> object.</returns>
        /// <exception cref="ArgumentNullException">Thrown when builder is null.</exception>
        public static IRecordValidator CreateCustom(this ValidatorBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder
                .FirstNameValidator(5, 30)
                .LastNameValidator(5, 30)
                .DateOfBirthValidator(new (1980, 1, 1))
                .WorkPlaceNumberValidator(10)
                .SalaryValidator(200m)
                .Create();
        }
    }
}
