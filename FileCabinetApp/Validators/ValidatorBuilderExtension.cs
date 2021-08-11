using System;
using Microsoft.Extensions.Configuration;

namespace FileCabinetApp.Validators
{
    /// <summary>Validator builder.</summary>
    public static class ValidatorBuilderExtension
    {
        private static readonly IConfiguration Config = new ConfigurationBuilder().AddJsonFile("validation-rules.json", true, true).Build();

        /// <summary>Creates default validator.</summary>
        /// <param name="builder">Builder.</param>
        /// <returns>Returns <see cref="IRecordValidator"/> object.</returns>
        /// <exception cref="ArgumentNullException">Thrown when builder is null.</exception>
        public static IRecordValidator CreateDefault(this ValidatorBuilder builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            ValidationRules rules = Config.GetSection("default").Get<ValidationRules>();
            return CreateBuilder(builder, rules);
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

            ValidationRules rules = Config.GetSection("custom").Get<ValidationRules>();
            return CreateBuilder(builder, rules);
        }

        private static IRecordValidator CreateBuilder(ValidatorBuilder builder, ValidationRules rules)
        {
            return builder
                .FirstNameValidator(rules.FirstName.Min, rules.FirstName.Max)
                .LastNameValidator(rules.LastName.Min, rules.LastName.Max)
                .DateOfBirthValidator(rules.DateOfBirth.From)
                .WorkPlaceNumberValidator(rules.WorkPlaceNumber.Min)
                .SalaryValidator(rules.Salary.Min)
                .Create();
        }
    }
}
