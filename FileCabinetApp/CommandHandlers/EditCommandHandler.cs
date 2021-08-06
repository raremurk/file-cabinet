using System;
using System.Globalization;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>Edit command handler.</summary>
    /// <seealso cref="ServiceCommandHandlerBase" />
    public class EditCommandHandler : ServiceCommandHandlerBase
    {
        private const string EditCommand = "edit";
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

        private static IRecordValidator validator;

        /// <summary>Initializes a new instance of the <see cref="EditCommandHandler"/> class.</summary>
        /// <param name="fileCabinetService">IFileCabinetService.</param>
        /// <param name="val1dator">IRecordValidator.</param>
        public EditCommandHandler(IFileCabinetService fileCabinetService, IRecordValidator val1dator)
            : base(fileCabinetService)
        {
            validator = val1dator;
        }

        /// <summary>Handles the specified request.</summary>
        /// <param name="request">The request.</param>
        public override void Handle(AppCommandRequest request) => this.Handle(request, EditCommand, this.Edit);

        private static FileCabinetRecord СonsoleInput()
        {
            Console.Write("First name: ");
            string firstName = ReadInput(StringConverter, validator.NameIsCorrect);

            Console.Write("Last name: ");
            string lastName = ReadInput(StringConverter, validator.NameIsCorrect);

            Console.Write("Date of birth (month/day/year): ");
            DateTime dateOfBirth = ReadInput(DateConverter, validator.DateOfBirthIsCorrect);

            Console.Write("Workplace number: ");
            short workPlaceNumber = ReadInput(ShortConverter, validator.WorkPlaceNumberIsCorrect);

            Console.Write("Salary: ");
            decimal salary = ReadInput(DecimalConverter, validator.SalaryIsCorrect);

            Console.Write("Department (uppercase letter): ");
            char department = ReadInput(CharConverter, validator.DepartmentIsCorrect);

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

        private void Edit(string parameters)
        {
            if (!int.TryParse(parameters, out int id))
            {
                Console.WriteLine("Invalid id value.");
                return;
            }

            ServiceStat stat = this.fileCabinetService.GetStat();
            if (id < 1 || id > stat.NumberOfRecords || stat.DeletedRecordsIds.Contains(id))
            {
                Console.WriteLine($"Record #{id} doesn't exists or removed.");
                return;
            }

            FileCabinetRecord record = СonsoleInput();
            record.Id = id;
            this.fileCabinetService.EditRecord(record);
            Console.WriteLine($"Record #{id} is updated.");
        }
    }
}
