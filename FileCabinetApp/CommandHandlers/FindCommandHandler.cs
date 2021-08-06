﻿using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>Find command handler.</summary>
    /// <seealso cref="CommandHandlerBase" />
    public class FindCommandHandler : CommandHandlerBase
    {
        private const string FindCommand = "find";
        private readonly IFileCabinetService fileCabinetService;

        /// <summary>Initializes a new instance of the <see cref="FindCommandHandler"/> class.</summary>
        /// <param name="fileCabinetService">IFileCabinetService.</param>
        public FindCommandHandler(IFileCabinetService fileCabinetService)
        {
            this.fileCabinetService = fileCabinetService;
        }

        /// <summary>Handles the specified request.</summary>
        /// <param name="request">The request.</param>
        public override void Handle(AppCommandRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (string.Equals(FindCommand, request.Command, StringComparison.OrdinalIgnoreCase))
            {
                this.Find(request.Parameters);
            }
            else
            {
                if (this.NextHandler != null)
                {
                    this.NextHandler.Handle(request);
                }
                else
                {
                    PrintMissedCommandInfo(request.Command);
                }
            }
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

        private void Find(string parameters)
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
                records = this.fileCabinetService.FindByFirstName(value);
            }
            else if (index == 1)
            {
                records = this.fileCabinetService.FindByLastName(value);
            }
            else if (index == 2)
            {
                const string Format = "MM/dd/yyyy";
                if (!DateTime.TryParseExact(value, Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateOfBirth))
                {
                    Console.WriteLine("Incorrect property value.");
                    return;
                }

                records = this.fileCabinetService.FindByDateOfBirth(value);
            }

            if (records.Count == 0)
            {
                Console.WriteLine("No such records.");
                return;
            }

            PrintRecords(records);
        }
    }
}