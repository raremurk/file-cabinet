using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

[assembly: CLSCompliant(false)]

namespace FileCabinetApp
{
    /// <summary>Main class of the project.</summary>
    public static class Program
    {
        private const string DeveloperName = "Evgeny Fursevich";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;
        private const string InputDateFormat = "MM/dd/yyyy";

        private static readonly Tuple<string, Action<string>>[] Commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("export", Export),
            new Tuple<string, Action<string>>("find", Find),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
        };

        private static readonly string[][] HelpMessages = new string[][]
        {
            new string[] { "export", "exports records to a file", "The 'export' command exports records to a file." },
            new string[] { "find", "searches records by property name value", "The 'find' command searches records by property name value." },
            new string[] { "edit", "edits a record with the specified id", "The 'edit' command edits a record with the specified id." },
            new string[] { "list", "returns a list of all records", "The 'list' command returns a list of all records." },
            new string[] { "create", "сreates a record and returns its id", "The 'create' command сreates a record and returns its id." },
            new string[] { "stat", "prints statistics on records", "The 'stat' command prints statistics on records." },
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

        private static IFileCabinetService fileCabinetService;
        private static IRecordValidator validator;
        private static bool isRunning = true;

        /// <summary>Defines the entry point of the application.</summary>
        /// <param name="args">Command line arguments.</param>
        public static void Main(string[] args)
        {
            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");

            string[] customArgs = { "--validation-rules=custom", "-v custom" };
            bool customMode = false;

            if (!(args is null) && args.Length == 1)
            {
                customMode = Array.FindIndex(customArgs, i => i.Equals(args[0], StringComparison.OrdinalIgnoreCase)) >= 0;
            }
            else if (!(args is null) && args.Length == 2)
            {
                string shortArg = string.Join(' ', args[0], args[1]);
                customMode = Array.FindIndex(customArgs, i => i.Equals(shortArg, StringComparison.OrdinalIgnoreCase)) >= 0;
            }

            if (customMode)
            {
                Console.WriteLine("Using custom validation rules.");
                fileCabinetService = new FileCabinetMemoryService(new CustomValidator());
                validator = new CustomValidator();
            }
            else
            {
                Console.WriteLine("Using default validation rules.");
                fileCabinetService = new FileCabinetMemoryService(new DefaultValidator());
                validator = new DefaultValidator();
            }

            Console.WriteLine(Program.HintMessage);
            Console.WriteLine();

            do
            {
                Console.Write("> ");
                var inputs = Console.ReadLine().Split(' ', 2);
                const int commandIndex = 0;
                var command = inputs[commandIndex];

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine(Program.HintMessage);
                    continue;
                }

                var index = Array.FindIndex(Commands, 0, Commands.Length, i => i.Item1.Equals(command, StringComparison.OrdinalIgnoreCase));
                if (index >= 0)
                {
                    const int parametersIndex = 1;
                    var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                    Commands[index].Item2(parameters);
                }
                else
                {
                    PrintMissedCommandInfo(command);
                }
            }
            while (isRunning);
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

            string format = inputs[0];
            string file = inputs[1];
            string fileExtension = file[^4..];
            string[] formats = { "csv", "xml" };
            string[] fileExtensions = { ".csv", ".xml" };

            if (Array.FindIndex(formats, i => i.Equals(format, StringComparison.OrdinalIgnoreCase)) == -1)
            {
                Console.WriteLine("Invalid format.");
                return;
            }

            if (file.Length < 5 || Array.FindIndex(fileExtensions, i => i.Equals(fileExtension, StringComparison.Ordinal)) == -1)
            {
                Console.WriteLine("Invalid file name.");
                return;
            }

            var way = file.Split('\\');
            string directory = way.Length > 1 ? string.Join('\\', way[0..^1]) : Directory.GetCurrentDirectory();

            DirectoryInfo dirInfo = new (directory);
            if (dirInfo.Exists)
            {
                FileInfo fileInfo = new (file);
                if (fileInfo.Exists)
                {
                    Console.Write($"File is exist - rewrite {file}?[Y / N] ");
                    string answer = Console.ReadLine();

                    if (!string.Equals(answer, "Y", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Operation canceled.");
                        return;
                    }
                }

                FileCabinetServiceSnapshot snapshot = fileCabinetService.MakeSnapshot();

                if (string.Equals(format, "csv", StringComparison.OrdinalIgnoreCase))
                {
                    using StreamWriter writer = new (file, false, System.Text.Encoding.Default);
                    snapshot.SaveToCsv(writer);
                }
                else
                {
                    XmlWriterSettings settings = new ();
                    settings.Indent = true;
                    using var writer = XmlWriter.Create(file, settings);
                    snapshot.SaveToXml(writer);
                }

                Console.WriteLine($"All records are exported to file {file}.");
            }
            else
            {
                Console.WriteLine($"Export failed: can't open file {file}");
            }
        }

        private static void List(string parameters)
        {
            var records = fileCabinetService.GetRecords();
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
                records = fileCabinetService.FindByFirstName(value);
            }
            else if (index == 1)
            {
                records = fileCabinetService.FindByLastName(value);
            }
            else if (index == 2)
            {
                const string Format = "MM/dd/yyyy";
                if (!DateTime.TryParseExact(value, Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateOfBirth))
                {
                    Console.WriteLine("Incorrect property value.");
                    return;
                }

                records = fileCabinetService.FindByDateOfBirth(value);
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
            var recordsCount = fileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount} record(s).");
        }

        private static void Create(string parameters)
        {
            FileCabinetRecord record = СonsoleInput();

            int id = fileCabinetService.CreateRecord(record);

            Console.WriteLine($"Record #{id} is created.");
        }

        private static void Edit(string parameters)
        {
            if (!int.TryParse(parameters, out int id))
            {
                Console.WriteLine("Invalid id value.");
                return;
            }

            int recordsCount = fileCabinetService.GetStat();
            if (id < 1 || id > recordsCount)
            {
                Console.WriteLine($"#{id} record is not found.");
                return;
            }

            FileCabinetRecord record = СonsoleInput();
            record.Id = id;

            fileCabinetService.EditRecord(record);

            Console.WriteLine($"Record #{id} is updated.");
        }

        private static FileCabinetRecord СonsoleInput()
        {
            Console.Write("First name: ");
            string firstName = ReadInput(StringConverter, validator.NameIsCorrect);

            Console.Write("Last name: ");
            string lastName = ReadInput(StringConverter, validator.NameIsCorrect);

            Console.Write("Date of birth (month/day/year): ");
            DateTime dateOfBirth = ReadInput(DateConverter, validator.DateOfBirthIsCorrect);

            Console.Write("Workplace number: ");
            short workPlaceNumber = ReadInput(ShortConverter, validator.WorkPlaceNumberIsCorrect);

            Console.Write("Salary: ");
            decimal salary = ReadInput(DecimalConverter, validator.SalaryIsCorrect);

            Console.Write("Department (uppercase letter): ");
            char department = ReadInput(CharConverter, validator.DepartmentIsCorrect);

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
                builder.Append($"{record.Salary}, ");
                builder.Append($"{record.Department}");

                Console.WriteLine(builder.ToString());
            }
        }

        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }

        private static void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(HelpMessages, 0, HelpMessages.Length, i => string.Equals(i[Program.CommandHelpIndex], parameters, StringComparison.OrdinalIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(HelpMessages[index][Program.ExplanationHelpIndex]);
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
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[Program.CommandHelpIndex], helpMessage[Program.DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }

        private static void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            isRunning = false;
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