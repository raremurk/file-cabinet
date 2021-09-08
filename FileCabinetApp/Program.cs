using System;
using System.IO;
using FileCabinetApp.CommandHandlers;
using FileCabinetApp.Helpers;
using FileCabinetApp.Validators;

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
            var parseResult = args != null ? ParseArgs(args) : new (false, false, false, false);
            bool customValidation = parseResult.Item1;
            bool fileMemoryMode = parseResult.Item2;
            bool serviceMeter = parseResult.Item3;
            bool serviceLogger = parseResult.Item4;

            string validationModeMessage = customValidation ? "Using custom validation rules." : "Using default validation rules.";
            string fileModeMessage = fileMemoryMode ? "Using file mode." : "Using memory mode.";
            string programMode = string.Join(' ', validationModeMessage, fileModeMessage);

            validator = customValidation ? new ValidatorBuilder().CreateCustom() : new ValidatorBuilder().CreateDefault();

            if (fileMemoryMode)
            {
                string nameOfDb = "cabinet-records.db";
                FileStream fileStream = new (nameOfDb, FileMode.OpenOrCreate);
                fileCabinetService = new FileCabinetFilesystemService(fileStream, validator);
            }
            else
            {
                fileCabinetService = new FileCabinetMemoryService(validator);
            }

            fileCabinetService = new SearchCache(fileCabinetService);

            if (serviceMeter && serviceLogger)
            {
                fileCabinetService = new ServiceLogger(new ServiceMeter(fileCabinetService));
            }
            else if (serviceMeter)
            {
                fileCabinetService = new ServiceMeter(fileCabinetService);
            }
            else if (serviceLogger)
            {
                fileCabinetService = new ServiceLogger(fileCabinetService);
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

                if (string.IsNullOrWhiteSpace(command))
                {
                    Console.WriteLine(Program.HintMessage);
                    continue;
                }

                ICommandHandler commandHandler = CreateCommandHandlers();
                commandHandler.Handle(new AppCommandRequest(command, parameters));
            }
            while (isRunning);
        }

        private static ICommandHandler CreateCommandHandlers()
        {
            var helpHandler = new HelpCommandHandler();
            var createHandler = new CreateCommandHandler(fileCabinetService, validator);
            var insertHandler = new InsertCommandHandler(fileCabinetService, validator);
            var updateHandler = new UpdateCommandHandler(fileCabinetService, validator);
            var exitHandler = new ExitCommandHandler(SetProgramStatus);
            var exporthandler = new ExportCommandHandler(fileCabinetService);
            var selectHandler = new SelectCommandHandler(fileCabinetService, Printer.Print, validator);
            var importHandler = new ImportCommandHandler(fileCabinetService);
            var purgeHandler = new PurgeCommandHandler(fileCabinetService);
            var deleteHandler = new DeleteCommandHandler(fileCabinetService, validator);
            var statHandler = new StatCommandHandler(fileCabinetService);

            helpHandler.SetNext(createHandler);
            createHandler.SetNext(insertHandler);
            insertHandler.SetNext(updateHandler);
            updateHandler.SetNext(exitHandler);
            exitHandler.SetNext(exporthandler);
            exporthandler.SetNext(selectHandler);
            selectHandler.SetNext(importHandler);
            importHandler.SetNext(purgeHandler);
            purgeHandler.SetNext(deleteHandler);
            deleteHandler.SetNext(statHandler);

            return helpHandler;
        }

        private static Tuple<bool, bool, bool, bool> ParseArgs(string[] args)
        {
            string[] customValidationArgs = { "--validation-rules=custom", "-v custom" };
            string[] fileMemoryArgs = { "--storage=file", "-s file" };
            string[] shortForms = { "-s", "-v" };
            string serviceMeterArg = "-use-stopwatch";
            string serviceLoggerArg = "-use-logger";
            bool customValidation = false;
            bool fileMemoryMode = false;
            bool serviceMeter = false;
            bool serviceLogger = false;

            int argsLength = args.Length;
            for (int i = 0; i < argsLength; i++)
            {
                string parameter = args[i];
                if (Array.FindIndex(shortForms, x => x.Equals(args[i], StringComparison.OrdinalIgnoreCase)) >= 0 && argsLength - i >= 2)
                {
                    parameter = string.Join(' ', args[i], args[++i]);
                }

                customValidation = customValidation || Array.FindIndex(customValidationArgs, x => x.Equals(parameter, StringComparison.OrdinalIgnoreCase)) >= 0;
                fileMemoryMode = fileMemoryMode || Array.FindIndex(fileMemoryArgs, x => x.Equals(parameter, StringComparison.OrdinalIgnoreCase)) >= 0;
                serviceMeter = serviceMeter || string.Equals(serviceMeterArg, parameter, StringComparison.OrdinalIgnoreCase);
                serviceLogger = serviceLogger || string.Equals(serviceLoggerArg, parameter, StringComparison.OrdinalIgnoreCase);
            }

            return new (customValidation, fileMemoryMode, serviceMeter, serviceLogger);
        }
    }
}