using System;
using System.Globalization;
using System.Text;

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
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
        };

        private static readonly string[][] HelpMessages = new string[][]
        {
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

                var index = Array.FindIndex(Commands, 0, Commands.Length, i => i.Item1.Equals(command, StringComparison.InvariantCultureIgnoreCase));
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

        private static void Stat(string parameters)
        {
            var recordsCount = Program.FileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount} record(s).");
        }

        private static void Create(string parameters)
        {
            Console.Write("First name: ");
            string firstName = Console.ReadLine();
            Console.Write("Last name: ");
            string lastName = Console.ReadLine();
            Console.Write("Date of birth (month/day/year): ");
            string birthday = Console.ReadLine();
            DateTime dateTimeBirthday;
            string format = "MM/dd/yyyy";

            while (!DateTime.TryParseExact(birthday, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeBirthday))
            {
                Console.WriteLine("Invalid date format. Try again.");
                Console.Write("Date of birth: ");
                birthday = Console.ReadLine();
            }

            Console.Write("Some short: ");
            string shortInput = Console.ReadLine();
            short workPlaceNumber;

            while (!short.TryParse(shortInput, out workPlaceNumber))
            {
                Console.WriteLine("This is not a short value. Try again.");
                Console.Write("Some short: ");
                shortInput = Console.ReadLine();
            }

            Console.Write("Some decimal: ");
            string decimalInput = Console.ReadLine();
            decimal salary;

            while (!decimal.TryParse(decimalInput, out salary))
            {
                Console.WriteLine("This is not a decimal value. Try again.");
                Console.Write("Some decimal: ");
                decimalInput = Console.ReadLine();
            }

            Console.Write("Some char: ");
            string charInput = Console.ReadLine();
            char department;

            while (!char.TryParse(charInput, out department))
            {
                Console.WriteLine("This is not a char value. Try again.");
                Console.Write("Some char: ");
                charInput = Console.ReadLine();
            }

            int id = Program.FileCabinetService.CreateRecord(firstName, lastName, dateTimeBirthday, workPlaceNumber, salary, department);

            Console.WriteLine($"Record #{id} is created.");
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
                var index = Array.FindIndex(HelpMessages, 0, HelpMessages.Length, i => string.Equals(i[Program.CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
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