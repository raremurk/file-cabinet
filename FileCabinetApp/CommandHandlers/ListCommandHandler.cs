using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>List command handler.</summary>
    /// <seealso cref="ServiceCommandHandlerBase" />
    public class ListCommandHandler : ServiceCommandHandlerBase
    {
        private const string ListCommand = "list";

        /// <summary>Initializes a new instance of the <see cref="ListCommandHandler"/> class.</summary>
        /// <param name="fileCabinetService">IFileCabinetService.</param>
        public ListCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
        }

        /// <summary>Handles the specified request.</summary>
        /// <param name="request">The request.</param>
        public override void Handle(AppCommandRequest request) => this.Handle(request, ListCommand, this.List);

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

        private void List(string parameters)
        {
            var records = this.fileCabinetService.GetRecords();
            PrintRecords(records);
        }
    }
}
