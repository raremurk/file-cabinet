using System;
using System.IO;
using FileCabinetApp.CommandHandlers;

[assembly: CLSCompliant(false)]

namespace FileCabinetApp
{
    /// <summary>Main class of the project.</summary>
    public static class Program
    {
        public static IFileCabinetService fileCabinetService;
        public static IRecordValidator validator;
        public static bool isRunning = true;

        private const string DeveloperName = "Evgeny Fursevich";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";

        /// <summary>Defines the entry point of the application.</summary>
        /// <param name="args">Command line arguments.</param>
        public static void Main(string[] args)
        {
            string[] customValidationArgs = { "--validation-rules=custom", "-v custom" };
            string[] fileMemoryArgs = { "--storage=file", "-s file" };
            string[] shortForms = { "-s", "-v" };
            bool customValidation = false;
            bool fileMemoryMode = false;

            if (!(args is null))
            {
                int argsLength = args.Length;
                for (int i = 0; i < argsLength; i++)
                {
                    string parameter = args[i];
                    if (Array.FindIndex(shortForms, x => x.Equals(args[i], StringComparison.OrdinalIgnoreCase)) >= 0 && argsLength - i >= 2)
                    {
                        parameter = string.Join(' ', args[i], args[++i]);
                    }

                    customValidation = Array.FindIndex(customValidationArgs, x => x.Equals(parameter, StringComparison.OrdinalIgnoreCase)) >= 0;
                    fileMemoryMode = Array.FindIndex(fileMemoryArgs, x => x.Equals(parameter, StringComparison.OrdinalIgnoreCase)) >= 0;
                }
            }

            string validationModeMessage = customValidation ? "Using custom validation rules." : "Using default validation rules.";
            string fileModeMessage = fileMemoryMode ? "Using file mode." : "Using memory mode.";
            string programMode = string.Join(' ', validationModeMessage, fileModeMessage);

            validator = customValidation ? new CustomValidator() : new DefaultValidator();

            if (fileMemoryMode)
            {
                string nameOfDb = "cabinet-records.db";
                FileStream fileStream = new (nameOfDb, FileMode.Create);
                fileCabinetService = new FileCabinetFilesystemService(fileStream, validator);
            }
            else
            {
                fileCabinetService = new FileCabinetMemoryService(validator);
            }

            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
            Console.WriteLine(programMode);
            Console.WriteLine(Program.HintMessage);
            Console.WriteLine();

            do
            {
                Console.Write("> ");
                var inputs = Console.ReadLine().Split(' ', 2);
                const int commandIndex = 0;
                const int parametersIndex = 1;
                string command = inputs[commandIndex];
                string parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine(Program.HintMessage);
                    continue;
                }

                CommandHandler commandHandler = CreateCommandHandlers();
                commandHandler.Handle(new AppCommandRequest(command, parameters));
            }
            while (isRunning);
        }

        private static CommandHandler CreateCommandHandlers()
        {
            return new CommandHandler();
        }
    }
}