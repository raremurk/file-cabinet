using System;
using System.Collections.Generic;
using System.IO;
using FileCabinetApp;
using FileCabinetApp.FileIO;

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
            var parseResult = args != null ? ParseArgs(args) : new (string.Empty, false, false, 0, 0);

            string filename = parseResult.Item1;
            bool csvFormat = parseResult.Item2;
            bool xmlFormat = parseResult.Item3;
            int amountOfRecords = parseResult.Item4;
            int startIdValue = parseResult.Item5;

            if (!csvFormat && !xmlFormat)
            {
                Console.WriteLine("Invalid format.");
                return;
            }

            if (string.IsNullOrWhiteSpace(filename))
            {
                Console.WriteLine("No such directory or invalid file name");
                return;
            }

            if (amountOfRecords <= 0)
            {
                Console.WriteLine("Invalid amount of records.");
                return;
            }

            if (startIdValue <= 0)
            {
                Console.WriteLine("Invalid start Id.");
                return;
            }

            var records = RecordGenerator.GetRandomRecords(amountOfRecords, startIdValue);
            using StreamWriter writer = new (filename, false, System.Text.Encoding.UTF8);

            if (csvFormat)
            {
                new FileCabinetRecordCsvWriter(writer).Write(records);
            }

            if (xmlFormat)
            {
                new FileCabinetRecordXmlSerializer(writer).Write(records);
            }

            Console.WriteLine($"{amountOfRecords} records were written to {filename}.");
        }

        private static string ExtractArgValue(string arg1, string arg2, string[] allowedArgs)
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

        private static Tuple<string, bool, bool, int, int> ParseArgs(string[] args)
        {
            string[] outputFormatArgs = { "--output-type=", "-t" };
            string[] outputFileNameArgs = { "--output=", "-o" };
            string[] amountOfRecordsArgs = { "--records-amount=", "-a" };
            string[] startIdValueArgs = { "--start-id=", "-i" };

            string[] availableFormats = { "csv", "xml" };

            string filename = null;
            bool csvFormat = false;
            bool xmlFormat = false;
            int amountOfRecords = 0;
            int startIdValue = 0;

            List<string> arguments = new (args) { string.Empty };
            for (int i = 0; i < arguments.Count - 1; i++)
            {
                if (!csvFormat && !xmlFormat)
                {
                    string format = ExtractArgValue(arguments[i], arguments[i + 1], outputFormatArgs);
                    int index = Array.FindIndex(availableFormats, x => x.Equals(format, StringComparison.OrdinalIgnoreCase));
                    csvFormat = index == 0;
                    xmlFormat = index == 1;
                }

                if (string.IsNullOrWhiteSpace(filename))
                {
                    string fileNameArg = ExtractArgValue(arguments[i], arguments[i + 1], outputFileNameArgs);
                    filename = ValidateFileName(fileNameArg, csvFormat, xmlFormat);
                }

                if (amountOfRecords <= 0)
                {
                    string amountOfRecordsArg = ExtractArgValue(arguments[i], arguments[i + 1], amountOfRecordsArgs);
                    _ = int.TryParse(amountOfRecordsArg, out amountOfRecords);
                }

                if (startIdValue <= 0)
                {
                    string startIdValueArg = ExtractArgValue(arguments[i], arguments[i + 1], startIdValueArgs);
                    _ = int.TryParse(startIdValueArg, out startIdValue);
                }
            }

            return new (filename, csvFormat, xmlFormat, amountOfRecords, startIdValue);
        }

        private static string ValidateFileName(string filename, bool csvFormat, bool xmlFormat)
        {
            string[] fileExtensions = { ".csv", ".xml" };
            string answer = string.Empty;

            if (filename.Length >= 5)
            {
                string fileExtension = filename[^4..];
                int index = Array.FindIndex(fileExtensions, i => i.Equals(fileExtension, StringComparison.OrdinalIgnoreCase));

                var way = filename.Split('\\', StringSplitOptions.RemoveEmptyEntries);
                string directory = way.Length > 1 ? string.Join('\\', way[0..^1]) : Directory.GetCurrentDirectory();

                if (new DirectoryInfo(directory).Exists)
                {
                    answer = (index == 0 && csvFormat) || (index == 1 && xmlFormat) ? filename : string.Empty;
                }
            }

            return answer;
        }
    }
}
