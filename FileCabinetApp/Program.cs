using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using FileCabinetApp.CommandHandlers;

[assembly: CLSCompliant(false)]

namespace FileCabinetApp
{
    /// <summary>Main class of the project.</summary>
    public static class Program
    {
        private const string DeveloperName = "Evgeny Fursevich";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private static readonly Action<bool> SetProgramStatus = status => isRunning = status;

        private static IFileCabinetService fileCabinetService;
        private static IRecordValidator validator;
        private static bool isRunning = true;

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

                ICommandHandler commandHandler = CreateCommandHandlers();
                commandHandler.Handle(new AppCommandRequest(command, parameters));
            }
            while (isRunning);
        }

        private static void Print(IEnumerable<FileCabinetRecord> records)
        {
            if (records is null)
            {
                throw new ArgumentNullException(nameof(records));
            }

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

        private static ICommandHandler CreateCommandHandlers()
        {
            var helpHandler = new HelpCommandHandler();
            var createHandler = new CreateCommandHandler(fileCabinetService, validator);
            var editHandler = new EditCommandHandler(fileCabinetService, validator);
            var exitHandler = new ExitCommandHandler(SetProgramStatus);
            var exporthandler = new ExportCommandHandler(fileCabinetService);
            var findHandler = new FindCommandHandler(fileCabinetService, Print);
            var importHandler = new ImportCommandHandler(fileCabinetService);
            var listHandler = new ListCommandHandler(fileCabinetService, Print);
            var purgeHandler = new PurgeCommandHandler(fileCabinetService);
            var removeHandler = new RemoveCommandHandler(fileCabinetService);
            var statHandler = new StatCommandHandler(fileCabinetService);

            helpHandler.SetNext(createHandler);
            createHandler.SetNext(editHandler);
            editHandler.SetNext(exitHandler);
            exitHandler.SetNext(exporthandler);
            exporthandler.SetNext(findHandler);
            findHandler.SetNext(importHandler);
            importHandler.SetNext(listHandler);
            listHandler.SetNext(purgeHandler);
            purgeHandler.SetNext(removeHandler);
            removeHandler.SetNext(statHandler);

            return helpHandler;
        }
    }
}