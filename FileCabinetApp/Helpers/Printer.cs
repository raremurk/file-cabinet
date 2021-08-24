using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FileCabinetApp.Models;

namespace FileCabinetApp.Helpers
{
    /// <summary>Class for console output.</summary>
    public static class Printer
    {
        /// <summary>Method for printing records to the console.</summary>
        /// <param name="records">Records for printing.</param>
        /// <param name="format">Required columns format.</param>
        public static void Print(IEnumerable<FileCabinetRecord> records, BoolRecord format)
        {
            _ = records ?? throw new ArgumentNullException(nameof(records));
            _ = format ?? throw new ArgumentNullException(nameof(format));

            if (!records.Any())
            {
                Console.WriteLine("No records found.");
                return;
            }

            string[] columnHeaders = { "Id", "FirstName", "LastName", "DateOfBirth", "Place №", "Salary", "Department" };

            int lengthOfId = columnHeaders[0].Length;
            int firstNameLength = columnHeaders[1].Length;
            int lastNameLength = columnHeaders[2].Length;
            int dateOfBirthLength = columnHeaders[3].Length;
            int workPlaceNumberLength = columnHeaders[4].Length;
            int salaryLength = columnHeaders[5].Length;
            int departmentLength = columnHeaders[6].Length;

            List<StringRecord> recordsToPrint = new ();
            foreach (var record in records)
            {
                var stringRecord = new StringRecord()
                {
                    Id = $"{record.Id}",
                    FirstName = record.FirstName,
                    LastName = record.LastName,
                    DateOfBirth = record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture),
                    WorkPlaceNumber = $"{record.WorkPlaceNumber}",
                    Salary = $"{record.Salary.ToString("F2", CultureInfo.InvariantCulture)}",
                    Department = $"{record.Department}",
                };

                recordsToPrint.Add(stringRecord);

                lengthOfId = stringRecord.Id.Length > lengthOfId ? stringRecord.Id.Length : lengthOfId;
                firstNameLength = stringRecord.FirstName.Length > firstNameLength ? stringRecord.FirstName.Length : firstNameLength;
                lastNameLength = stringRecord.LastName.Length > lastNameLength ? stringRecord.LastName.Length : lastNameLength;
                salaryLength = stringRecord.Salary.Length > salaryLength ? stringRecord.Salary.Length : salaryLength;
            }

            Tuple<bool, string, int, bool>[] columns =
            {
                new (format.Id, columnHeaders[0], lengthOfId, true),
                new (format.FirstName, columnHeaders[1], firstNameLength, false),
                new (format.LastName, columnHeaders[2], lastNameLength, false),
                new (format.DateOfBirth, columnHeaders[3], dateOfBirthLength, true),
                new (format.WorkPlaceNumber, columnHeaders[4], workPlaceNumberLength, true),
                new (format.Salary, columnHeaders[5], salaryLength, true),
                new (format.Department, columnHeaders[6], departmentLength, true),
            };

            var headerAndSeparator = MakeHeaderAndSeparator(columns);
            string header = headerAndSeparator.Item1;
            string separator = headerAndSeparator.Item2;

            Console.WriteLine(separator);
            Console.WriteLine(header);
            Console.WriteLine(separator);

            foreach (var record in recordsToPrint)
            {
                string line = MakeLine(record, columns);
                Console.WriteLine(line);
            }

            Console.WriteLine(separator);
        }

        private static string MakeLine(StringRecord record, Tuple<bool, string, int, bool>[] columns)
        {
            string[] properties = { record.Id, record.FirstName, record.LastName, record.DateOfBirth, record.WorkPlaceNumber, record.Salary, record.Department };
            var line = new List<string>();
            for (int i = 0; i < properties.Length; i++)
            {
                if (columns[i].Item1)
                {
                    string property = columns[i].Item4 ? properties[i].PadLeft(columns[i].Item3) : properties[i].PadRight(columns[i].Item3);
                    line.Add(property);
                }
            }

            return $"| {string.Join(" | ", line)} |";
        }

        private static Tuple<string, string> MakeHeaderAndSeparator(Tuple<bool, string, int, bool>[] columns)
        {
            var header = new List<string>();
            var separator = new List<string>();
            foreach (var column in columns)
            {
                if (column.Item1)
                {
                    header.Add(column.Item2.PadRight(column.Item3));
                    separator.Add(new string('-', column.Item3));
                }
            }

            return new ($"| {string.Join(" | ", header)} |", $"+-{string.Join("-+-", separator)}-+");
        }
    }
}
