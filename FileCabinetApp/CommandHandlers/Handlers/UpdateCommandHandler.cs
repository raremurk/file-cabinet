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

        private static FileCabinetRecord JoinRecords(FileCabinetRecord originalRecord, RecordToEdit changes)
        {
            return new FileCabinetRecord
            {
                Id = originalRecord.Id,
                FirstName = !changes.FirstName.Item1 ? originalRecord.FirstName : changes.FirstName.Item2,
                LastName = !changes.LastName.Item1 ? originalRecord.LastName : changes.LastName.Item2,
                DateOfBirth = !changes.DateOfBirth.Item1 ? originalRecord.DateOfBirth : changes.DateOfBirth.Item2,
                WorkPlaceNumber = !changes.WorkPlaceNumber.Item1 ? originalRecord.WorkPlaceNumber : changes.WorkPlaceNumber.Item2,
                Salary = !changes.Salary.Item1 ? originalRecord.Salary : changes.Salary.Item2,
                Department = !changes.Department.Item1 ? originalRecord.Department : changes.Department.Item2,
            };
        }

        private void Update(string parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters))
            {
                Console.WriteLine("Input parameters. Example : update set DateOfBirth = '5/18/1986' where FirstName='Stan' and LastName='Smith'");
                return;
            }

            string[] inputs = parameters.Split(new char[] { '=', ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int indexOfWhere = Array.FindIndex(inputs, x => x.Equals("where", StringComparison.OrdinalIgnoreCase));

            if (inputs.Length < 6 || !string.Equals(inputs[0], "set", StringComparison.OrdinalIgnoreCase) || indexOfWhere == -1)
            {
                Console.WriteLine("Invalid input. Example : update set DateOfBirth = '5/18/1986' where FirstName='Stan' and LastName='Smith'");
                return;
            }

            RecordToSearch recordToSearch = new Parser(this.validator).GetRecordToSearchFromString(inputs[(indexOfWhere + 1) ..]);
            if (!recordToSearch.NeedToSearch())
            {
                Console.WriteLine("Search parameters are missing or incorrect.");
                return;
            }

            var foundRecords = this.fileCabinetService.Search(recordToSearch);
            if (!foundRecords.Any())
            {
                Console.WriteLine("No records with such parameters.");
                return;
            }

            RecordToEdit template = this.GetValues(inputs[0..indexOfWhere]);
            List<string> identifiers = new ();
            foreach (var rec in foundRecords)
            {
                var buff = JoinRecords(rec, template);
                this.fileCabinetService.EditRecord(buff);
                identifiers.Add($"#{buff.Id}");
            }

            string ids = string.Join(", ", identifiers);
            string message = identifiers.Count < 2 ? $"Record {ids} is updated." : $"Records {ids} are updated.";
            Console.WriteLine(message);
        }

        private RecordToEdit GetValues(string[] arrayToSearch)
        {
            string[] properties = { "firstName", "lastName", "dateOfBirth", "workPlaceNumber", "salary", "department" };
            string[] values = { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };
            for (int i = 0; i < properties.Length; i++)
            {
                int index = Array.FindIndex(arrayToSearch, x => x.Equals(properties[i], StringComparison.OrdinalIgnoreCase));
                string value = index != -1 && index + 1 < arrayToSearch.Length ? arrayToSearch[index + 1] : string.Empty;
                if (value.Length > 2 && value[0] == '\'' && value[^1] == '\'')
                {
                    values[i] = value.Trim('\'');
                }
            }

            string firstName = values[0];
            string lastName = values[1];
            DateTime dateOfBirth = Converter.DateTimeConverter(values[2]).Item3;
            short workPlaceNumber = Converter.ShortConverter(values[3]).Item3;
            decimal salary = Converter.DecimalConverter(values[4]).Item3;
            char department = Converter.CharConverter(values[5]).Item3;

            return new RecordToEdit
            {
                FirstName = new (this.validator.ValidateFirstName(firstName).Item1, firstName),
                LastName = new (this.validator.ValidateLastName(lastName).Item1, lastName),
                DateOfBirth = new (this.validator.ValidateDateOfBirth(dateOfBirth).Item1, dateOfBirth),
                WorkPlaceNumber = new (this.validator.ValidateWorkPlaceNumber(workPlaceNumber).Item1, workPlaceNumber),
                Salary = new (this.validator.ValidateSalary(salary).Item1, salary),
                Department = new (this.validator.ValidateDepartment(department).Item1, department),
            };
        }
    }
}
