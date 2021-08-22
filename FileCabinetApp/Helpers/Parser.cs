using System;
using FileCabinetApp.Models;

namespace FileCabinetApp.Helpers
{
    /// <summary>Parser.</summary>
    public class Parser
    {
        private readonly IRecordValidator validator;

        /// <summary>Initializes a new instance of the <see cref="Parser"/> class.</summary>
        /// <param name="validator">IRecordValidator.</param>
        public Parser(IRecordValidator validator)
        {
            this.validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        /// <summary>Reads search parameters from string array.</summary>
        /// <param name="arrayToSearch">Array to search.</param>
        /// <returns>Returns RecordToSearch.</returns>
        public RecordToSearch GetRecordToSearchFromString(string[] arrayToSearch)
        {
            _ = arrayToSearch ?? throw new ArgumentNullException(nameof(arrayToSearch));

            string[] recordProperties = { "id", "firstName", "lastName", "dateOfBirth", "workPlaceNumber", "salary", "department" };
            int orIndex = Array.FindIndex(arrayToSearch, x => x.Equals("OR", StringComparison.OrdinalIgnoreCase));
            int andIndex = Array.FindIndex(arrayToSearch, x => x.Equals("AND", StringComparison.OrdinalIgnoreCase));
            bool mode = orIndex == -1 || (andIndex != -1 && andIndex < orIndex);
            int propertiesCount = recordProperties.Length;
            string[] values = new string[propertiesCount];

            for (int i = 0; i < propertiesCount; i++)
            {
                int index = Array.FindIndex(arrayToSearch, x => x.Equals(recordProperties[i], StringComparison.OrdinalIgnoreCase));
                string value = index != -1 && index + 1 < arrayToSearch.Length ? arrayToSearch[index + 1] : string.Empty;
                value = value.Length > 2 && value[0] == '\'' && value[^1] == '\'' ? value.Trim('\'') : string.Empty;
                values[i] = value;
            }

            var stringRecord = new StringRecord
            {
                Id = values[0],
                FirstName = values[1],
                LastName = values[2],
                DateOfBirth = values[3],
                WorkPlaceNumber = values[4],
                Salary = values[5],
                Department = values[6],
            };

            FileCabinetRecord convertedRecord = Converter.RecordConverter(stringRecord).Item3;

            return new RecordToSearch
            {
                AndMode = mode,
                Id = convertedRecord.Id > 0 ? new (true, convertedRecord.Id) : new (false, 0),
                FirstName = this.validator.ValidateFirstName(convertedRecord.FirstName).Item1 ? new (true, convertedRecord.FirstName) : new (false, string.Empty),
                LastName = this.validator.ValidateLastName(convertedRecord.LastName).Item1 ? new (true, convertedRecord.LastName) : new (false, string.Empty),
                DateOfBirth = this.validator.ValidateDateOfBirth(convertedRecord.DateOfBirth).Item1 ? new (true, convertedRecord.DateOfBirth) : new (false, DateTime.MinValue),
                WorkPlaceNumber = this.validator.ValidateWorkPlaceNumber(convertedRecord.WorkPlaceNumber).Item1 ? new (true, convertedRecord.WorkPlaceNumber) : new (false, (short)0),
                Salary = this.validator.ValidateSalary(convertedRecord.Salary).Item1 ? new (true, convertedRecord.Salary) : new (false, decimal.Zero),
                Department = this.validator.ValidateDepartment(convertedRecord.Department).Item1 ? new (true, convertedRecord.Department) : new (false, char.MinValue),
            };
        }
    }
}
