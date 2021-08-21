using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>Delete command handler.</summary>
    /// <seealso cref="ServiceCommandHandlerBase" />
    public class DeleteCommandHandler : ServiceCommandHandlerBase
    {
        private const string DeleteCommand = "delete";

        /// <summary>Initializes a new instance of the <see cref="DeleteCommandHandler"/> class.</summary>
        /// <param name="fileCabinetService">IFileCabinetService.</param>
        public DeleteCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
        }

        /// <summary>Handles the specified request.</summary>
        /// <param name="request">The request.</param>
        public override void Handle(AppCommandRequest request) => this.Handle(request, DeleteCommand, this.Delete);

        private void Delete(string parameters)
        {
            if (string.IsNullOrEmpty(parameters))
            {
                Console.WriteLine("Input parameters. Example : delete where id = '1'");
                return;
            }

            string[] inputs = parameters.Split(new char[] { '=', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string[] allowedProperties = { "id", "firstName", "lastName", "dateOfBirth" };

            if (inputs.Length != 3 || !string.Equals(inputs[0], "where", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Invalid input. Example : delete where id = '1'");
                return;
            }

            string property = inputs[1];
            string value = inputs[2];

            int index = Array.FindIndex(allowedProperties, x => x.Equals(property, StringComparison.OrdinalIgnoreCase));
            if (index == -1)
            {
                Console.WriteLine("Invalid property. Available properties : 'id', 'firstname', 'lastname', 'dateofbirth'.");
                return;
            }

            if (value.Length < 3 || value[0] != '\'' || value[^1] != '\'')
            {
                Console.WriteLine("Value must be in single quotes.");
                return;
            }

            List<int> recordsToDelete = new ();
            string ids = string.Empty;
            value = value.Trim('\'');

            if (index == 0)
            {
                if (!int.TryParse(value, out int id))
                {
                    Console.WriteLine("Invalid id value.");
                    return;
                }

                if (this.fileCabinetService.IdExists(id))
                {
                    recordsToDelete.Add(id);
                    ids = $"#{id}";
                }
            }
            else
            {
                var searchResult = index switch
                {
                    1 => this.fileCabinetService.FindByFirstName(value),
                    2 => this.fileCabinetService.FindByLastName(value),
                    3 => this.fileCabinetService.FindByDateOfBirth(value),
                    _ => throw new ArgumentException("Invalid index.")
                };

                List<string> stringIds = new ();
                foreach (var record in searchResult)
                {
                    recordsToDelete.Add(record.Id);
                    stringIds.Add($"#{record.Id}");
                }

                ids = string.Join(", ", stringIds);
            }

            if (recordsToDelete.Count != 0)
            {
                string message = recordsToDelete.Count == 1 ? $"Record {ids} is deleted." : $"Records {ids} are deleted.";
                this.fileCabinetService.RemoveRecords(new (recordsToDelete));
                Console.WriteLine(message);
            }
            else
            {
                Console.WriteLine("No records with such parameters.");
            }
        }
    }
}
