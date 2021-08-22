using System;
using System.Collections.Generic;
using System.Linq;
using FileCabinetApp.Helpers;
using FileCabinetApp.Models;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>Update command handler.</summary>
    /// <seealso cref="ServiceCommandHandlerBase" />
    public class UpdateCommandHandler : ServiceCommandHandlerBase
    {
        private const string UpdateCommand = "update";
        private readonly IRecordValidator validator;

        /// <summary>Initializes a new instance of the <see cref="UpdateCommandHandler"/> class.</summary>
        /// <param name="fileCabinetService">IFileCabinetService.</param>
        /// <param name="validator">IRecordValidator.</param>
        public UpdateCommandHandler(IFileCabinetService fileCabinetService, IRecordValidator validator)
            : base(fileCabinetService)
        {
            this.validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        /// <inheritdoc cref="CommandHandlerBase.Handle(AppCommandRequest)"/>
        public override void Handle(AppCommandRequest request) => this.Handle(request, UpdateCommand, this.Update);

        private static string[] GetValues(string[] propertiesToSearch, string[] arrayToSearch)
        {
            string[] values = new string[propertiesToSearch.Length];
            for (int i = 0; i < propertiesToSearch.Length; i++)
            {
                int index = Array.FindIndex(arrayToSearch, x => x.Equals(propertiesToSearch[i], StringComparison.OrdinalIgnoreCase));
                string value = index != -1 && index + 1 < arrayToSearch.Length ? arrayToSearch[index + 1] : string.Empty;
                if (value.Length > 2 && value[0] == '\'' && value[^1] == '\'')
                {
                    value = value.Trim('\'');
                }

                values[i] = value;
            }

            return values;
        }

        private void Update(string parameters)
        {
            if (string.IsNullOrEmpty(parameters))
            {
                Console.WriteLine("Input parameters. Example : update set DateOfBirth = '5/18/1986' where FirstName='Stan' and LastName='Smith'");
                return;
            }

            string[] recordProperties = { "id", "firstName", "lastName", "dateOfBirth", "workPlaceNumber", "salary", "department" };

            string[] inputs = parameters.Split(new char[] { '=', ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int indexOfWhere = Array.FindIndex(inputs, x => x.Equals("where", StringComparison.OrdinalIgnoreCase));

            if (inputs.Length < 6 || !string.Equals(inputs[0], "set", StringComparison.OrdinalIgnoreCase) || indexOfWhere == -1)
            {
                Console.WriteLine("Invalid input. Example : update set DateOfBirth = '5/18/1986' where FirstName='Stan' and LastName='Smith'");
                return;
            }

            var parser = new Parser(this.validator);
            RecordToSearch recordToSearch = parser.GetRecordToSearchFromString(inputs[(indexOfWhere + 1) ..]);
            var foundRecords = this.fileCabinetService.Search(recordToSearch);

            string[] propertiesToUpdate = { "firstName", "lastName", "dateOfBirth", "workPlaceNumber", "salary", "department" };
            string[] valuesToUpdate = GetValues(propertiesToUpdate, inputs[0..indexOfWhere]);

            DateTime dateOfBirth = Converter.DateTimeConverter(valuesToUpdate[2]).Item3;
            short workPlaceNumber = Converter.ShortConverter(valuesToUpdate[3]).Item3;
            decimal salary = Converter.DecimalConverter(valuesToUpdate[4]).Item3;
            char department = Converter.CharConverter(valuesToUpdate[5]).Item3;

            var record = new FileCabinetRecord
            {
                FirstName = this.validator.ValidateFirstName(valuesToUpdate[0]).Item1 ? valuesToUpdate[0] : string.Empty,
                LastName = this.validator.ValidateLastName(valuesToUpdate[1]).Item1 ? valuesToUpdate[1] : string.Empty,
                DateOfBirth = this.validator.ValidateDateOfBirth(dateOfBirth).Item1 ? dateOfBirth : DateTime.MinValue,
                WorkPlaceNumber = this.validator.ValidateWorkPlaceNumber(workPlaceNumber).Item1 ? workPlaceNumber : (short)0,
                Salary = this.validator.ValidateSalary(salary).Item1 ? salary : decimal.Zero,
                Department = this.validator.ValidateDepartment(department).Item1 ? department : char.MinValue,
            };

            if (!foundRecords.Any())
            {
                Console.WriteLine("No records with such parameters.");
                return;
            }

            List<string> identifiers = new ();
            int count = 0;
            foreach (var rec in foundRecords)
            {
                var buff = new FileCabinetRecord
                {
                    Id = rec.Id,
                    FirstName = string.IsNullOrEmpty(record.FirstName) ? rec.FirstName : record.FirstName,
                    LastName = string.IsNullOrEmpty(record.LastName) ? rec.LastName : record.LastName,
                    DateOfBirth = record.DateOfBirth == DateTime.MinValue ? rec.DateOfBirth : record.DateOfBirth,
                    WorkPlaceNumber = record.WorkPlaceNumber == 0 ? rec.WorkPlaceNumber : record.WorkPlaceNumber,
                    Salary = record.Salary == decimal.Zero ? rec.Salary : record.Salary,
                    Department = record.Department == char.MinValue ? rec.Department : record.Department,
                };

                count++;
                identifiers.Add($"#{buff.Id}");
                this.fileCabinetService.EditRecord(buff);
            }

            string ids = string.Join(", ", identifiers);
            string message = count < 2 ? $"Record {ids} is updated." : $"Records {ids} are updated.";
            Console.WriteLine(message);
        }
    }
}
