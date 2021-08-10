using System;
using System.Globalization;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>Get record from console.</summary>
    public class GetRecordFromConsole
    {
        private const string InputDateFormat = "MM/dd/yyyy";

        private static readonly Func<string, Tuple<bool, string, string>> StringConverter = input => new (true, string.Empty, input);

        private static readonly Func<string, Tuple<bool, string, DateTime>> DateConverter = input =>
        {
            bool tryParse = DateTime.TryParseExact(input, InputDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateOfBirth);
            return tryParse ? new (true, string.Empty, dateOfBirth) : new (false, "Invalid date.", DateTime.MinValue);
        };

        private static readonly Func<string, Tuple<bool, string, short>> ShortConverter = input =>
        {
            return short.TryParse(input, out short workPlaceNumber) ? new (true, string.Empty, workPlaceNumber) : new (false, "Invalid workplace number.", 0);
        };

        private static readonly Func<string, Tuple<bool, string, decimal>> DecimalConverter = input =>
        {
            return decimal.TryParse(input, out decimal salary) ? new (true, string.Empty, salary) : new (false, "Invalid salary.", 0);
        };

        private static readonly Func<string, Tuple<bool, string, char>> CharConverter = input =>
        {
            return char.TryParse(input, out char department) ? new (true, string.Empty, department) : new (false, "Invalid department.", char.MinValue);
        };

        private readonly IRecordValidator validator;

        /// <summary>Initializes a new instance of the <see cref="GetRecordFromConsole"/> class.</summary>
        /// <param name="validator">IRecordValidator.</param>
        public GetRecordFromConsole(IRecordValidator validator)
        {
            this.validator = validator;
        }

        /// <summary>Get record from console.</summary>
        /// <returns>returns object representing a record.</returns>
        public FileCabinetRecord СonsoleInput()
        {
            Console.Write("First name: ");
            string firstName = ReadInput(StringConverter, this.validator.ValidateLastName);

            Console.Write("Last name: ");
            string lastName = ReadInput(StringConverter, this.validator.ValidateLastName);

            Console.Write("Date of birth (month/day/year): ");
            DateTime dateOfBirth = ReadInput(DateConverter, this.validator.ValidateDateOfBirth);

            Console.Write("Workplace number: ");
            short workPlaceNumber = ReadInput(ShortConverter, this.validator.ValidateWorkPlaceNumber);

            Console.Write("Salary: ");
            decimal salary = ReadInput(DecimalConverter, this.validator.ValidateSalary);

            Console.Write("Department (uppercase letter): ");
            char department = ReadInput(CharConverter, this.validator.ValidateDepartment);

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
