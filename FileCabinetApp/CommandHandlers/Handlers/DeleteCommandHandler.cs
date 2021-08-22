using System;
using System.Collections.Generic;
using System.Linq;
using FileCabinetApp.Helpers;
using FileCabinetApp.Models;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>Delete command handler.</summary>
    /// <seealso cref="ServiceCommandHandlerBase" />
    public class DeleteCommandHandler : ServiceCommandHandlerBase
    {
        private const string DeleteCommand = "delete";
        private readonly IRecordValidator validator;

        /// <summary>Initializes a new instance of the <see cref="DeleteCommandHandler"/> class.</summary>
        /// <param name="fileCabinetService">IFileCabinetService.</param>
        /// /// <param name="validator">IRecordValidator.</param>
        public DeleteCommandHandler(IFileCabinetService fileCabinetService, IRecordValidator validator)
            : base(fileCabinetService)
        {
            this.validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        /// <inheritdoc cref="CommandHandlerBase.Handle(AppCommandRequest)"/>
        public override void Handle(AppCommandRequest request) => this.Handle(request, DeleteCommand, this.Delete);

        private void Delete(string parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters))
            {
                Console.WriteLine("Input parameters. Example: delete where id = '1'");
                return;
            }

            string[] inputs = parameters.Split(new char[] { '=', ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int indexOfWhere = Array.FindIndex(inputs, x => x.Equals("where", StringComparison.OrdinalIgnoreCase));

            if (indexOfWhere == -1)
            {
                Console.WriteLine("The keyword \"where\" was not found.");
                return;
            }

            RecordToSearch recordToSearch = new Parser(this.validator).GetRecordToSearchFromString(inputs[(indexOfWhere + 1) ..]);
            if (!recordToSearch.NeedToSearch())
            {
                Console.WriteLine("Search parameters are missing or incorrect.");
                return;
            }

            var recordsToDelete = this.fileCabinetService.Search(recordToSearch);
            if (!recordsToDelete.Any())
            {
                Console.WriteLine("No records with such parameters.");
                return;
            }

            List<string> identifiers = new ();
            foreach (var record in recordsToDelete)
            {
                this.fileCabinetService.RemoveRecord(record.Id);
                identifiers.Add($"#{record.Id}");
            }

            string ids = string.Join(", ", identifiers);
            string message = recordsToDelete.Count() < 2 ? $"Record {ids} is deleted." : $"Records {ids} are deleted.";
            Console.WriteLine(message);
        }
    }
}