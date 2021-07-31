using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using FileCabinetApp;

[assembly: CLSCompliant(false)]

namespace FileCabinetGenerator
{
    /// <summary>Main class of the FileCabinetGenerator project.</summary>
    public static class Program
    {
        /// <summary>Defines the entry point of the FileCabinetGenerator.</summary>
        /// <param name="args">Command line arguments.</param>
        public static void Main(string[] args)
        {
            string[] availableFormats = { "csv", "xml" };
            string[] fileExtensions = { ".csv", ".xml" };

            string[] outputFormatArgs = { "--output-type=", "-t" };
            string[] outputFileNameArgs = { "--output=", "-o" };
            string[] amountOfRecordsArgs = { "--records-amount=", "-a" };
            string[] startIdValueArgs = { "--start-id=", "-i" };

            bool csvFormat = false;
            bool xmlFormat = false;
            string filename = null;
            int amountOfRecords = 0;
            int startIdValue = 0;

            List<string> arguments = new (args);
            arguments.Add(string.Empty);

            for (int i = 0; i < arguments.Count - 1; i++)
            {
                if (!csvFormat && !xmlFormat)
                {
                    string format = ParseArgs(arguments[i], arguments[i + 1], outputFormatArgs);

                    int index = Array.FindIndex(availableFormats, x => x.Equals(format, StringComparison.OrdinalIgnoreCase));
                    csvFormat = index == 0;
                    xmlFormat = index == 1;
                }

                if (string.IsNullOrEmpty(filename))
                {
                    string fileNameArg = ParseArgs(arguments[i], arguments[i + 1], outputFileNameArgs);

                    if (fileNameArg.Length >= 5)
                    {
                        string fileExtension = fileNameArg[^4..];
                        int index = Array.FindIndex(fileExtensions, i => i.Equals(fileExtension, StringComparison.OrdinalIgnoreCase));

                        var way = fileNameArg.Split('\\', StringSplitOptions.RemoveEmptyEntries);
                        string directory = way.Length > 1 ? string.Join('\\', way[0..^1]) : Directory.GetCurrentDirectory();

                        if (new DirectoryInfo(directory).Exists)
                        {
                            filename = (index == 0 && csvFormat) || (index == 1 && xmlFormat) ? fileNameArg : string.Empty;
                        }
                    }
                }

                if (amountOfRecords == 0)
                {
                    string amountOfRecordsArg = ParseArgs(arguments[i], arguments[i + 1], amountOfRecordsArgs);
                    amountOfRecords = int.TryParse(amountOfRecordsArg, out int result) ? result : 0;
                }

                if (startIdValue == 0)
                {
                    string startIdValueArg = ParseArgs(arguments[i], arguments[i + 1], startIdValueArgs);
                    startIdValue = int.TryParse(startIdValueArg, out int result) ? result : 0;
                }
            }

            if (!csvFormat && !xmlFormat)
            {
                Console.WriteLine("Invalid format.");
                return;
            }

            if (string.IsNullOrEmpty(filename))
            {
                Console.WriteLine("No such directory or invalid file name");
                return;
            }

            if (amountOfRecords == 0)
            {
                Console.WriteLine("Invalid amount of records.");
                return;
            }

            if (startIdValue == 0)
            {
                Console.WriteLine("Invalid start Id.");
                return;
            }

            Console.WriteLine($"{amountOfRecords} records were written to {filename}.");
        }

        private static string ParseArgs(string arg1, string arg2, string[] allowedArgs)
        {
            if (arg1.StartsWith(allowedArgs[0], StringComparison.OrdinalIgnoreCase))
            {
                return arg1[allowedArgs[0].Length..];
            }

            if (arg1.Equals(allowedArgs[1], StringComparison.OrdinalIgnoreCase))
            {
                return arg2;
            }

            return string.Empty;
        }

        #pragma warning disable CA5394
        private static List<FileCabinetRecord> GetRandomRecords(int count, int startId)
        {
            List<FileCabinetRecord> randomRecords = new ();
            Random random = new ();
            DateTime start = new (1950, 1, 1);
            int range = (DateTime.Today - start).Days;

            for (int i = 0; i < count; i++)
            {
                var record = new FileCabinetRecord
                {
                    Id = startId++,
                    FirstName = GetRandomString(15),
                    LastName = GetRandomString(15),
                    DateOfBirth = start.AddDays(random.Next(range)),
                    WorkPlaceNumber = (short)random.Next(1, 1000),
                    Salary = (decimal)random.Next(0, 10001) + (decimal)Math.Round(random.NextDouble(), 2),
                    Department = (char)random.Next(65, 91),
                };

                randomRecords.Add(record);
            }

            string GetRandomString(int length)
            {
                StringBuilder builder = new ();
                for (int j = 0; j < length; j++)
                {
                    builder.Append((char)random.Next(97, 123));
                }

                return builder.ToString();
            }

            return randomRecords;
        }
        #pragma warning restore CA5394
    }
}
