using System;
using System.IO;
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
        public static FileAndFormat GetFilePathFromString(string parameters)
        {
            _ = parameters ?? throw new ArgumentNullException(nameof(parameters));

            var inputs = parameters.Split(' ', 2);
            if (inputs.Length != 2)
            {
                Console.WriteLine("Wrong number of parameters.");
                return null;
            }

            string[] availableFormats = { "csv", "xml" };
            string[] fileExtensions = { ".csv", ".xml" };
            string format = inputs[0];
            string file = inputs[1];

            if (file.Length < 5)
            {
                Console.WriteLine("Invalid file name.");
                return null;
            }

            string fileExtension = file[^4..];
            int formatIndex = Array.FindIndex(availableFormats, x => x.Equals(format, StringComparison.OrdinalIgnoreCase));
            int fileExtensionIndex = Array.FindIndex(fileExtensions, i => i.Equals(fileExtension, StringComparison.OrdinalIgnoreCase));
            bool csvFormat = formatIndex == 0 && fileExtensionIndex == 0;
            bool xmlFormat = formatIndex == 1 && fileExtensionIndex == 1;

            var way = file.Split('\\', StringSplitOptions.RemoveEmptyEntries);
            string directory = way.Length > 1 ? string.Join('\\', way[0..^1]) : Directory.GetCurrentDirectory();

            if (!csvFormat && !xmlFormat)
            {
                Console.WriteLine("Invalid format.");
                return null;
            }

            if (!new DirectoryInfo(directory).Exists)
            {
                Console.WriteLine("No such directory or invalid file name");
                return null;
            }

            return new FileAndFormat { FileName = file, CSVFormat = csvFormat, XMLFormat = xmlFormat };
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
