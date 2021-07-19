using System;
using System.Globalization;
using System.Text;

[assembly: CLSCompliant(false)]

namespace FileCabinetApp
{
    public static class Program
    {
        private const string DeveloperName = "Evgeny Fursevich";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;

        private static readonly FileCabinetService FileCabinetService = new ();

        private static readonly Tuple<string, Action<string>>[] Commands = new Tuple<string, Action<string>>[]
        {
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
            new string[] { "find", "searches records by property name value", "The 'find' command searches records by property name value." },
            new string[] { "edit", "edits a record with the specified id", "The 'edit' command edits a record with the specified id." },
            new string[] { "list", "returns a list of all records", "The 'list' command returns a list of all records." },
            new string[] { "create", "сreates a record and returns its id", "The 'create' command сreates a record and returns its id." },
            new string[] { "stat", "prints statistics on records", "The 'stat' command prints statistics on records." },
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
        };

        private static bool isRunning = true;

        public static void Main()
        {
            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
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

        private static void List(string parameters)
        {
            var records = Program.FileCabinetService.GetRecords();
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
            FileCabinetRecord[] records = Array.Empty<FileCabinetRecord>();

            if (index == 0)
            {
                records = FileCabinetService.FindByFirstName(value);
            }
            else if (index == 1)
            {
                records = FileCabinetService.FindByLastName(value);
            }

            if (records.Length == 0)
            {
                Console.WriteLine("No such records.");
                return;
            }

            PrintRecords(records);
        }

        private static void Stat(string parameters)
        {
            var recordsCount = Program.FileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount} record(s).");
        }

        private static void Create(string parameters)
        {
            FileCabinetRecord record = СonsoleInput();

            int id = Program.FileCabinetService.CreateRecord(record.FirstName, record.LastName, record.DateOfBirth, record.WorkPlaceNumber, record.Salary, record.Department);

            Console.WriteLine($"Record #{id} is created.");
        }

        private static void Edit(string parameters)
        {
            if (!int.TryParse(parameters, out int id))
            {
                Console.WriteLine("Invalid id value.");
                return;
            }

            var records = FileCabinetService.GetRecords();
            if (!Array.Exists(records, x => x.Id == id))
            {
                Console.WriteLine($"#{id} record is not found.");
                return;
            }

            FileCabinetRecord record = СonsoleInput();

            Program.FileCabinetService.EditRecord(id, record.FirstName, record.LastName, record.DateOfBirth, record.WorkPlaceNumber, record.Salary, record.Department);

            Console.WriteLine($"Record #{id} is updated.");
        }

        private static FileCabinetRecord СonsoleInput()
        {
            string firstName;
            string lastName;
            DateTime dateOfBirth;
            short workPlaceNumber;
            decimal salary;
            char department;

            while (true)
            {
                Console.Write("First name: ");
                firstName = Console.ReadLine();
                bool incorrect = Guard.StringIsIncorrect(firstName);
                if (!incorrect)
                {
                    break;
                }

                Console.WriteLine("Invalid first name. Try again.");
            }

            while (true)
            {
                Console.Write("Last name: ");
                lastName = Console.ReadLine();
                bool incorrect = Guard.StringIsIncorrect(lastName);
                if (!incorrect)
                {
                    break;
                }

                Console.WriteLine("Invalid last name. Try again.");
            }

            const string Format = "MM/dd/yyyy";
            while (true)
            {
                Console.Write("Date of birth (month/day/year): ");
                string dateOfBirthInput = Console.ReadLine();
                bool incorrect = !DateTime.TryParseExact(dateOfBirthInput, Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateOfBirth) || Guard.DateTimeRangeIsIncorrect(dateOfBirth);
                if (!incorrect)
                {
                    break;
                }

                Console.WriteLine("Invalid date. Try again.");
            }

            while (true)
            {
                Console.Write("Workplace number: ");
                string workPlaceNumberInput = Console.ReadLine();
                bool incorrect = !short.TryParse(workPlaceNumberInput, out workPlaceNumber) || Guard.WorkPlaceNumberIsLessThanMinValue(workPlaceNumber);
                if (!incorrect)
                {
                    break;
                }

                Console.WriteLine("Invalid workplace number. Try again.");
            }

            while (true)
            {
                Console.Write("Salary: ");
                string salaryInput = Console.ReadLine();
                bool incorrect = !decimal.TryParse(salaryInput, out salary) || Guard.SalaryIsLessThanThanMinValue(salary);
                if (!incorrect)
                {
                    break;
                }

                Console.WriteLine("Invalid salary. Try again.");
            }

            while (true)
            {
                Console.Write("Department (uppercase letter): ");
                string departmentInput = Console.ReadLine();
                bool incorrect = !char.TryParse(departmentInput, out department) || Guard.DepartmentValueIsIncorrect(department);
                if (!incorrect)
                {
                    break;
                }

                Console.WriteLine("Invalid department. Try again.");
            }

            return new FileCabinetRecord { FirstName = firstName, LastName = lastName, DateOfBirth = dateOfBirth, WorkPlaceNumber = workPlaceNumber, Salary = salary, Department = department };
        }

        private static void PrintRecords(FileCabinetRecord[] records)
        {
            foreach (var record in records)
            {
                StringBuilder builder = new ();
                builder.Append($"{record.Id}, ");
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
    }
}