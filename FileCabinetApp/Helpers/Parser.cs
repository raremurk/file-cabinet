using System;
using System.Linq;
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

        /// <summary>Reads file path and format from string.</summary>
        /// <param name="parameters">Input string.</param>
        /// <returns>Returns FileAndFormat.</returns>
        public static FileAndFormat GetFileAndFormatFromString(string parameters)
        {
            _ = parameters ?? throw new ArgumentNullException(nameof(parameters));

            var inputs = parameters.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            var availableFormats = (Formats[])Enum.GetValues(typeof(Formats));
            return new FileAndFormat
            {
                Format = availableFormats.FirstOrDefault(x => x.ToString().Equals(inputs[0], StringComparison.OrdinalIgnoreCase)),
                FileName = inputs.Length == 2 ? inputs[1] : string.Empty,
            };
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

            int id = Converter.IntConverter(values[0]).Item3;
            string firstName = values[1];
            string lastName = values[2];
            DateTime dateOfBirth = Converter.DateTimeConverter(values[3]).Item3;
            short workPlaceNumber = Converter.ShortConverter(values[4]).Item3;
            decimal salary = Converter.DecimalConverter(values[5]).Item3;
            char department = Converter.CharConverter(values[6]).Item3;

            return new RecordToSearch
            {
                AndMode = mode,
                Id = new (id > 0, id),
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
