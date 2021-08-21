using System;
using System.Collections.Generic;
using System.Linq;

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
            this.validator = validator;
        }

        /// <summary>Handles the specified request.</summary>
        /// <param name="request">The request.</param>
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

            string[] inputs = parameters.Split(new char[] { '=', ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int indexOfWhere = Array.FindIndex(inputs, x => x.Equals("where", StringComparison.OrdinalIgnoreCase));

            if (inputs.Length < 6 || !string.Equals(inputs[0], "set", StringComparison.OrdinalIgnoreCase) || indexOfWhere == -1)
            {
                Console.WriteLine("Invalid input. Example : update set DateOfBirth = '5/18/1986' where FirstName='Stan' and LastName='Smith'");
                return;
            }

            string[] propertiesToUpdate = { "firstName", "lastName", "dateOfBirth", "workPlaceNumber", "salary", "department" };
            string[] propertiesToSearch = { "id", "firstName", "lastName", "dateOfBirth" };
            string[] valuesToUpdate = GetValues(propertiesToUpdate, inputs[0..indexOfWhere]);
            string[] valuesToSearch = GetValues(propertiesToSearch, inputs[(indexOfWhere + 1) ..]);

            string stringId = valuesToSearch[0];
            string firstName = valuesToSearch[1];
            string lastName = valuesToSearch[2];
            string dateOfBirthToFind = valuesToSearch[3];

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

            List<FileCabinetRecord> recordsToUpdate = new ();
            List<FileCabinetRecord> foundRecords = new ();
            List<string> identifiers = new ();

            if (!string.IsNullOrEmpty(stringId))
            {
                int id = Converter.IntConverter(stringId).Item3;
                if (id != 0 && this.fileCabinetService.IdExists(id))
                {
                    foundRecords.Add(this.fileCabinetService.GetRecord(id));
                }
            }
            else
            {
                List<IEnumerable<FileCabinetRecord>> searchResults = new ();

                if (!string.IsNullOrEmpty(firstName))
                {
                    searchResults.Add(this.fileCabinetService.FindByFirstName(firstName));
                }

                if (!string.IsNullOrEmpty(lastName))
                {
                    searchResults.Add(this.fileCabinetService.FindByLastName(lastName));
                }

                if (!string.IsNullOrEmpty(dateOfBirthToFind))
                {
                    searchResults.Add(this.fileCabinetService.FindByDateOfBirth(dateOfBirthToFind));
                }

                foundRecords = searchResults.Count switch
                {
                    1 => searchResults[0].ToList(),
                    2 => searchResults[0].Intersect(searchResults[1]).ToList(),
                    3 => searchResults[0].Intersect(searchResults[1]).Intersect(searchResults[2]).ToList(),
                    _ => foundRecords
                };
            }

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

                recordsToUpdate.Add(buff);
                identifiers.Add($"#{rec.Id}");
            }

            string message = "No records with such parameters.";
            if (recordsToUpdate.Count != 0)
            {
                string ids = string.Join(", ", identifiers);
                message = recordsToUpdate.Count < 2 ? $"Record {ids} is updated." : $"Records {ids} are updated.";
                this.fileCabinetService.EditRecords(new (recordsToUpdate));
            }

            Console.WriteLine(message);
        }
    }
}
