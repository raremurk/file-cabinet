using System;
using System.Globalization;
using FileCabinetApp.Models;

namespace FileCabinetApp.Helpers
{
    /// <summary>Converter.</summary>
    public static class Converter
    {
        private static readonly string[] DateFormats = { "MM/dd/yyyy", "M/d/yyyy", "MM/d/yyyy", "M/dd/yyyy" };

        /// <summary>String to int converting.</summary>
        /// <param name="input">Input string.</param>
        /// <returns>Returns TryParse result, message and int value .</returns>
        public static Tuple<bool, string, int> IntConverter(string input)
        {
            bool tryParse = int.TryParse(input, out int id);
            string message = tryParse ? string.Empty : "Invalid Id.";
            return new (tryParse, message, id);
        }

        /// <summary>String to DateTime converting.</summary>
        /// <param name="input">Input string.</param>
        /// <returns>Returns TryParse result, message and DateTime value .</returns>
        public static Tuple<bool, string, DateTime> DateTimeConverter(string input)
        {
            bool tryParse = DateTime.TryParseExact(input, DateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateOfBirth);
            string message = tryParse ? string.Empty : "Invalid date.";
            return new (tryParse, message, dateOfBirth);
        }

        /// <summary>String to short converting.</summary>
        /// <param name="input">Input string.</param>
        /// <returns>Returns TryParse result, message and short value .</returns>
        public static Tuple<bool, string, short> ShortConverter(string input)
        {
            bool tryParse = short.TryParse(input, out short workPlaceNumber);
            string message = tryParse ? string.Empty : "Invalid workplace number.";
            return new (tryParse, message, workPlaceNumber);
        }

        /// <summary>String to decimal converting.</summary>
        /// <param name="input">Input string.</param>
        /// <returns>Returns TryParse result, message and decimal value .</returns>
        public static Tuple<bool, string, decimal> DecimalConverter(string input)
        {
            bool tryParse = decimal.TryParse(input, out decimal salary);
            string message = tryParse ? string.Empty : "Invalid salary.";
            return new (tryParse, message, salary);
        }

        /// <summary>String to char converting.</summary>
        /// <param name="input">Input string.</param>
        /// <returns>Returns TryParse result, message and char value .</returns>
        public static Tuple<bool, string, char> CharConverter(string input)
        {
            bool tryParse = char.TryParse(input, out char department);
            string message = tryParse ? string.Empty : "Invalid department.";
            return new (tryParse, message, department);
        }

        /// <summary>StringFileCabinetRecord to FileCabinetRecord converting.</summary>
        /// <param name="stringRecord">Input record.</param>
        /// <returns>Returns TryParse result, message and FileCabinetRecord value .</returns>
        public static Tuple<bool, string, FileCabinetRecord> RecordConverter(StringRecord stringRecord)
        {
            if (stringRecord is null)
            {
                throw new ArgumentNullException(nameof(stringRecord));
            }

            bool successfully = true;
            string message = string.Empty;

            var resultConversionId = IntConverter(stringRecord.Id);
            var resultConversionDateOfBirth = DateTimeConverter(stringRecord.DateOfBirth);
            var resultConversionWorkPlaceNumber = ShortConverter(stringRecord.WorkPlaceNumber);
            var resultConversionSalary = DecimalConverter(stringRecord.Salary);
            var resultConversionDepartment = CharConverter(stringRecord.Department);

            var record = new FileCabinetRecord
            {
                Id = resultConversionId.Item3,
                FirstName = stringRecord.FirstName,
                LastName = stringRecord.LastName,
                DateOfBirth = resultConversionDateOfBirth.Item3,
                WorkPlaceNumber = resultConversionWorkPlaceNumber.Item3,
                Salary = resultConversionSalary.Item3,
                Department = resultConversionDepartment.Item3,
            };

            Tuple<bool, string>[] conversionResults =
            {
                new (resultConversionId.Item1, resultConversionId.Item2),
                new (resultConversionDateOfBirth.Item1, resultConversionDateOfBirth.Item2),
                new (resultConversionWorkPlaceNumber.Item1, resultConversionWorkPlaceNumber.Item2),
                new (resultConversionSalary.Item1, resultConversionSalary.Item2),
                new (resultConversionDepartment.Item1, resultConversionDepartment.Item2),
            };

            foreach (var result in conversionResults)
            {
                successfully = result.Item1;
                if (!successfully)
                {
                    message = result.Item2;
                    break;
                }
            }

            return new (successfully, message, record);
        }
    }
}
