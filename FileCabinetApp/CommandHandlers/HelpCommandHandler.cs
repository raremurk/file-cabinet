﻿using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>Help command handler.</summary>
    /// <seealso cref="CommandHandlerBase" />
    public class HelpCommandHandler : CommandHandlerBase
    {
        private const string HelpCommand = "help";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;

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

        /// <summary>Handles the specified request.</summary>
        /// <param name="request">The request.</param>
        public override void Handle(AppCommandRequest request) => this.Handle(request, HelpCommand, PrintHelp);

        private static void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(HelpMessages, i => string.Equals(i[CommandHelpIndex], parameters, StringComparison.OrdinalIgnoreCase));
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
    }
}
