using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>Create command handler.</summary>
    /// <seealso cref="CommandHandlerBase" />
    public class CommandHandler : CommandHandlerBase
    {
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;
        private const string InputDateFormat = "MM/dd/yyyy";

        private static readonly Tuple<string, Action<string>>[] Commands = new Tuple<string, Action<string>>[]
        {
                new Tuple<string, Action<string>>("create", Create),
                new Tuple<string, Action<string>>("edit", Edit),
                new Tuple<string, Action<string>>("remove", Remove),
                new Tuple<string, Action<string>>("find", Find),
                new Tuple<string, Action<string>>("list", List),
                new Tuple<string, Action<string>>("stat", Stat),
                new Tuple<string, Action<string>>("import", Import),
                new Tuple<string, Action<string>>("export", Export),
                new Tuple<string, Action<string>>("purge", Purge),
                new Tuple<string, Action<string>>("help", PrintHelp),
                new Tuple<string, Action<string>>("exit", Exit),
        };

        private static readonly string[][] HelpMessages = new string[][]
        {
            new string[] { "create", "сreates a record and returns its id", "The 'create' command сreates a record and returns its id." },
            new string[] { "edit", "edits a record with the specified id", "The 'edit' command edits a record with the specified id." },
            new string[] { "remove", "removes a record with the specified id", "The 'remove' command removes a record with the specified id." },
            new string[] { "find", "searches records by property name value", "The 'find' command searches records by property name value." },
            new string[] { "list", "returns a list of all records", "The 'list' command returns a list of all records." },
            new string[] { "stat", "prints statistics on records", "The 'stat' command prints statistics on records." },
            new string[] { "import", "imports records from file", "The 'import' command imports records from file." },
            new string[] { "export", "exports records to a file", "The 'export' command exports records to a file." },
            new string[] { "purge", "defragments the data file", "The 'purge' command defragments the data file." },
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
        };

        private static readonly Func<string, Tuple<bool, string, string>> StringConverter = input => new (true, string.Empty, input);

        private static readonly Func<string, Tuple<bool, string, DateTime>> DateConverter = input =>
        {
            bool tryParse = DateTime.TryParseExact(input, InputDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateOfBirth);
            return tryParse ? new (true, string.Empty, dateOfBirth) : new (false, "Invalid date.", DateTime.MinValue);
        };

        private static readonly Func<string, Tuple<bool, string, short>> ShortConverter = input =>
        {
            return short.TryParse(input, out short workPlaceNumber) ? new (true, string.Empty, workPlaceNumber) : new (false, "Invalid workplace number.", 0);
        };

        private static readonly Func<string, Tuple<bool, string, decimal>> DecimalConverter = input =>
        {
            return decimal.TryParse(input, out decimal salary) ? new (true, string.Empty, salary) : new (false, "Invalid salary.", 0);
        };

        private static readonly Func<string, Tuple<bool, string, char>> CharConverter = input =>
        {
            return char.TryParse(input, out char department) ? new (true, string.Empty, department) : new (false, "Invalid department.", char.MinValue);
        };

        /// <summary>Handles the specified request.</summary>
        /// <param name="request">The request.</param>
        public override void Handle(AppCommandRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var index = Array.FindIndex(Commands, i => i.Item1.Equals(request.Command, StringComparison.OrdinalIgnoreCase));
            if (index >= 0)
            {
                Commands[index].Item2(request.Parameters);
            }
            else
            {
                PrintMissedCommandInfo(request.Command);
            }
        }

        private static void Create(string parameters)
        {
            FileCabinetRecord record = СonsoleInput();
            int id = Program.fileCabinetService.CreateRecord(record);
            Console.WriteLine($"Record #{id} is created.");
        }

        private static void Remove(string parameters)
        {
            if (!int.TryParse(parameters, out int id))
            {
                Console.WriteLine("Invalid id value.");
                return;
            }

            ServiceStat stat = Program.fileCabinetService.GetStat();
            if (id < 1 || id > stat.NumberOfRecords || stat.DeletedRecordsIds.Contains(id))
            {
                Console.WriteLine($"Record #{id} doesn't exists or removed.");
                return;
            }

            Program.fileCabinetService.RemoveRecord(id);
            Console.WriteLine($"Record #{id} is removed.");
        }

        private static void Edit(string parameters)
        {
            if (!int.TryParse(parameters, out int id))
            {
                Console.WriteLine("Invalid id value.");
                return;
            }

            ServiceStat stat = Program.fileCabinetService.GetStat();
            if (id < 1 || id > stat.NumberOfRecords || stat.DeletedRecordsIds.Contains(id))
            {
                Console.WriteLine($"Record #{id} doesn't exists or removed.");
                return;
            }

            FileCabinetRecord record = СonsoleInput();
            record.Id = id;
            Program.fileCabinetService.EditRecord(record);
            Console.WriteLine($"Record #{id} is updated.");
        }

        private static void Export(string parameters)
        {
            if (string.IsNullOrEmpty(parameters))
            {
                Console.WriteLine("Input property name.");
                return;
            }

            var inputs = parameters.Split(' ', 2);

            if (inputs.Length != 2)
            {
                Console.WriteLine("Wrong number of parameters.");
                return;
            }

            string[] availableFormats = { "csv", "xml" };
            string[] fileExtensions = { ".csv", ".xml" };
            string format = inputs[0];
            string filename = inputs[1];

            int formatIndex = Array.FindIndex(availableFormats, x => x.Equals(format, StringComparison.OrdinalIgnoreCase));
            bool csvFormat = formatIndex == 0;
            bool xmlFormat = formatIndex == 1;

            if (filename.Length < 5)
            {
                Console.WriteLine("Invalid file name.");
                return;
            }

            string fileExtension = filename[^4..];
            int fileExtensionIndex = Array.FindIndex(fileExtensions, i => i.Equals(fileExtension, StringComparison.OrdinalIgnoreCase));

            var way = filename.Split('\\', StringSplitOptions.RemoveEmptyEntries);
            string directory = way.Length > 1 ? string.Join('\\', way[0..^1]) : Directory.GetCurrentDirectory();

            bool directoryAndFilenameExists = new DirectoryInfo(directory).Exists;
            filename = directoryAndFilenameExists && ((fileExtensionIndex == 0 && csvFormat) || (fileExtensionIndex == 1 && xmlFormat)) ? filename : string.Empty;

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

            if (new FileInfo(filename).Exists)
            {
                Console.Write($"File is exist - rewrite {filename}?[Y / N] ");
                string answer = Console.ReadLine();

                if (!string.Equals(answer, "Y", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Operation canceled.");
                    return;
                }
            }

            FileCabinetServiceSnapshot snapshot = Program.fileCabinetService.MakeSnapshot();

            if (csvFormat)
            {
                using StreamWriter writer = new (filename, false, System.Text.Encoding.UTF8);
                snapshot.SaveToCsv(writer);
            }
            else
            {
                XmlWriterSettings settings = new ();
                settings.Indent = true;
                using var writer = XmlWriter.Create(filename, settings);
                snapshot.SaveToXml(writer);
            }

            Console.WriteLine($"All records are exported to file {filename}.");
        }

        private static void Import(string parameters)
        {
            if (string.IsNullOrEmpty(parameters))
            {
                Console.WriteLine("Input property name.");
                return;
            }

            var inputs = parameters.Split(' ', 2);

            if (inputs.Length != 2)
            {
                Console.WriteLine("Wrong number of parameters.");
                return;
            }

            string[] availableFormats = { "csv", "xml" };
            string[] fileExtensions = { ".csv", ".xml" };
            string format = inputs[0];
            string filename = inputs[1];

            int formatIndex = Array.FindIndex(availableFormats, x => x.Equals(format, StringComparison.OrdinalIgnoreCase));
            bool csvFormat = formatIndex == 0;
            bool xmlFormat = formatIndex == 1;

            if (filename.Length < 5)
            {
                Console.WriteLine("Invalid file name.");
                return;
            }

            string fileExtension = filename[^4..];
            int fileExtensionIndex = Array.FindIndex(fileExtensions, i => i.Equals(fileExtension, StringComparison.OrdinalIgnoreCase));

            var way = filename.Split('\\', StringSplitOptions.RemoveEmptyEntries);
            string directory = way.Length > 1 ? string.Join('\\', way[0..^1]) : Directory.GetCurrentDirectory();

            bool directoryAndFilenameExists = new DirectoryInfo(directory).Exists && new FileInfo(filename).Exists;
            filename = directoryAndFilenameExists && ((fileExtensionIndex == 0 && csvFormat) || (fileExtensionIndex == 1 && xmlFormat)) ? filename : string.Empty;

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

            ServiceStat stat = Program.fileCabinetService.GetStat();

            if (csvFormat)
            {
                using StreamReader reader = new (filename, System.Text.Encoding.UTF8);
                FileCabinetServiceSnapshot snapshot = FileCabinetServiceSnapshot.LoadFromCsv(reader);
                Program.fileCabinetService.Restore(snapshot);
            }

            if (xmlFormat)
            {
                using var reader = XmlReader.Create(filename);
                FileCabinetServiceSnapshot snapshot = FileCabinetServiceSnapshot.LoadFromXml(reader);
                Program.fileCabinetService.Restore(snapshot);
            }

            int numberOfRecords = Program.fileCabinetService.GetStat().NumberOfRecords - stat.NumberOfRecords;

            Console.WriteLine($"{numberOfRecords} records are imported from file {filename}.");
        }

        private static void Purge(string parameters)
        {
            ServiceStat stat = Program.fileCabinetService.GetStat();
            Program.fileCabinetService.Purge();
            Console.WriteLine($"Data file processing is completed: {stat.DeletedRecordsIds.Count} of {stat.NumberOfRecords} records were purged.");
        }

        private static void List(string parameters)
        {
            var records = Program.fileCabinetService.GetRecords();
            PrintRecords(records);
        }

        private static void Find(string parameters)
        {
            if (string.IsNullOrEmpty(parameters))
            {
                Console.WriteLine("Input property name.");
                return;
            }

            var inputs = parameters.Split(' ', 2);

            string[] properties = new string[] { "firstName", "lastName", "dateOfBirth" };
            string property = inputs[0];
            int index = Array.FindIndex(properties, x => x.Equals(property, StringComparison.OrdinalIgnoreCase));
            if (index == -1)
            {
                Console.WriteLine("No such property.");
                return;
            }

            if (inputs.Length == 1)
            {
                Console.WriteLine("Input property value.");
                return;
            }

            string value = inputs[1];

            if (value.Length < 3 || value[0] != '"' || value[^1] != '"')
            {
                Console.WriteLine("Incorrect property value.");
                return;
            }

            value = value[1..^1];
            ReadOnlyCollection<FileCabinetRecord> records = new (Array.Empty<FileCabinetRecord>());

            if (index == 0)
            {
                records = Program.fileCabinetService.FindByFirstName(value);
            }
            else if (index == 1)
            {
                records = Program.fileCabinetService.FindByLastName(value);
            }
            else if (index == 2)
            {
                const string Format = "MM/dd/yyyy";
                if (!DateTime.TryParseExact(value, Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateOfBirth))
                {
                    Console.WriteLine("Incorrect property value.");
                    return;
                }

                records = Program.fileCabinetService.FindByDateOfBirth(value);
            }

            if (records.Count == 0)
            {
                Console.WriteLine("No such records.");
                return;
            }

            PrintRecords(records);
        }

        private static void Stat(string parameters)
        {
            ServiceStat stat = Program.fileCabinetService.GetStat();
            Console.WriteLine($"{stat.NumberOfRecords} record(s). {stat.DeletedRecordsIds.Count} deleted record(s).");
        }

        private static FileCabinetRecord СonsoleInput()
        {
            Console.Write("First name: ");
            string firstName = ReadInput(StringConverter, Program.validator.NameIsCorrect);

            Console.Write("Last name: ");
            string lastName = ReadInput(StringConverter, Program.validator.NameIsCorrect);

            Console.Write("Date of birth (month/day/year): ");
            DateTime dateOfBirth = ReadInput(DateConverter, Program.validator.DateOfBirthIsCorrect);

            Console.Write("Workplace number: ");
            short workPlaceNumber = ReadInput(ShortConverter, Program.validator.WorkPlaceNumberIsCorrect);

            Console.Write("Salary: ");
            decimal salary = ReadInput(DecimalConverter, Program.validator.SalaryIsCorrect);

            Console.Write("Department (uppercase letter): ");
            char department = ReadInput(CharConverter, Program.validator.DepartmentIsCorrect);

            return new FileCabinetRecord { FirstName = firstName, LastName = lastName, DateOfBirth = dateOfBirth, WorkPlaceNumber = workPlaceNumber, Salary = salary, Department = department };
        }

        private static void PrintRecords(ReadOnlyCollection<FileCabinetRecord> records)
        {
            foreach (var record in records)
            {
                StringBuilder builder = new ();
                builder.Append($"#{record.Id}, ");
                builder.Append($"{record.FirstName}, ");
                builder.Append($"{record.LastName}, ");
                builder.Append($"{record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)}, ");
                builder.Append($"{record.WorkPlaceNumber}, ");
                builder.Append($"{record.Salary.ToString("F2", CultureInfo.InvariantCulture)}, ");
                builder.Append($"{record.Department}");

                Console.WriteLine(builder.ToString());
            }
        }

        private static void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(HelpMessages, 0, HelpMessages.Length, i => string.Equals(i[CommandHelpIndex], parameters, StringComparison.OrdinalIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(HelpMessages[index][ExplanationHelpIndex]);
                }
                else
                {
                    Console.WriteLine($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                Console.WriteLine("Available commands:");

                foreach (var helpMessage in HelpMessages)
                {
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[CommandHelpIndex], helpMessage[DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }

        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }

        private static void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            Program.isRunning = false;
        }

        private static T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
        {
            do
            {
                T value;

                var input = Console.ReadLine();
                var conversionResult = converter(input);

                if (!conversionResult.Item1)
                {
                    Console.WriteLine($"Conversion failed: {conversionResult.Item2}. Please, correct your input.");
                    continue;
                }

                value = conversionResult.Item3;

                var validationResult = validator(value);
                if (!validationResult.Item1)
                {
                    Console.WriteLine($"Validation failed: {validationResult.Item2}. Please, correct your input.");
                    continue;
                }

                return value;
            }
            while (true);
        }
    }
}
