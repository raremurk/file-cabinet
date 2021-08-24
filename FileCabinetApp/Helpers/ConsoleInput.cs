using System;
using FileCabinetApp.Models;

namespace FileCabinetApp.Helpers
{
    /// <summary>Get record from console.</summary>
    public class ConsoleInput
    {
        private readonly IRecordValidator validator;

        /// <summary>Initializes a new instance of the <see cref="ConsoleInput"/> class.</summary>
        /// <param name="validator">IRecordValidator.</param>
        public ConsoleInput(IRecordValidator validator)
        {
            this.validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        /// <summary>Get record from console.</summary>
        /// <returns>FileCabinetRecord.</returns>
        public FileCabinetRecord GetRecord()
        {
            Console.Write("First name: ");
            string firstName = ReadInput(input => new Tuple<bool, string, string>(true, string.Empty, input), this.validator.ValidateLastName);

            Console.Write("Last name: ");
            string lastName = ReadInput(input => new Tuple<bool, string, string>(true, string.Empty, input), this.validator.ValidateLastName);

            Console.Write("Date of birth (month/day/year): ");
            DateTime dateOfBirth = ReadInput(Converter.DateTimeConverter, this.validator.ValidateDateOfBirth);

            Console.Write("Workplace number: ");
            short workPlaceNumber = ReadInput(Converter.ShortConverter, this.validator.ValidateWorkPlaceNumber);

            Console.Write("Salary: ");
            decimal salary = ReadInput(Converter.DecimalConverter, this.validator.ValidateSalary);

            Console.Write("Department (uppercase letter): ");
            char department = ReadInput(Converter.CharConverter, this.validator.ValidateDepartment);

            return new FileCabinetRecord { FirstName = firstName, LastName = lastName, DateOfBirth = dateOfBirth, WorkPlaceNumber = workPlaceNumber, Salary = salary, Department = department };
        }

        private static T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
        {
            do
            {
                T value;

                var input = Console.ReadLine();
                var conversionResult = converter(input);

                if (!conversionResult.Item1)
                {
                    Console.WriteLine($"Conversion failed: {conversionResult.Item2}. Please, correct your input.");
                    continue;
                }

                value = conversionResult.Item3;

                var validationResult = validator(value);
                if (!validationResult.Item1)
                {
                    Console.WriteLine($"Validation failed: {validationResult.Item2}. Please, correct your input.");
                    continue;
                }

                return value;
            }
            while (true);
        }
    }
}
