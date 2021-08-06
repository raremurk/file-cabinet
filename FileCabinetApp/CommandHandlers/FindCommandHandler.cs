using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>Find command handler.</summary>
    /// <seealso cref="ServiceCommandHandlerBase" />
    public class FindCommandHandler : ServiceCommandHandlerBase
    {
        private const string FindCommand = "find";
        private readonly IRecordPrinter printer;

        /// <summary>Initializes a new instance of the <see cref="FindCommandHandler"/> class.</summary>
        /// <param name="fileCabinetService">IFileCabinetService.</param>
        /// <param name="printer">IRecordPrinter.</param>
        public FindCommandHandler(IFileCabinetService fileCabinetService, IRecordPrinter printer)
            : base(fileCabinetService)
        {
            this.printer = printer;
        }

        /// <summary>Handles the specified request.</summary>
        /// <param name="request">The request.</param>
        public override void Handle(AppCommandRequest request) => this.Handle(request, FindCommand, this.Find);

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

            this.printer.Print(records);
        }
    }
}