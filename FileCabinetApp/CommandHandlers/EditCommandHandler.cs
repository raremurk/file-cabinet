using System;
using System.Globalization;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>Edit command handler.</summary>
    /// <seealso cref="ServiceCommandHandlerBase" />
    public class EditCommandHandler : ServiceCommandHandlerBase
    {
        private const string EditCommand = "edit";
        private readonly IRecordValidator validator;

        /// <summary>Initializes a new instance of the <see cref="EditCommandHandler"/> class.</summary>
        /// <param name="fileCabinetService">IFileCabinetService.</param>
        /// <param name="validator">IRecordValidator.</param>
        public EditCommandHandler(IFileCabinetService fileCabinetService, IRecordValidator validator)
            : base(fileCabinetService)
        {
            this.validator = validator;
        }

        /// <summary>Handles the specified request.</summary>
        /// <param name="request">The request.</param>
        public override void Handle(AppCommandRequest request) => this.Handle(request, EditCommand, this.Edit);

        private void Edit(string parameters)
        {
            if (!int.TryParse(parameters, out int id))
            {
                Console.WriteLine("Invalid id value.");
                return;
            }

            ServiceStat stat = this.fileCabinetService.GetStat();
            if (id < 1 || id > stat.NumberOfRecords || stat.DeletedRecordsIds.Contains(id))
            {
                Console.WriteLine($"Record #{id} doesn't exists or removed.");
                return;
            }

            FileCabinetRecord record = new GetRecordFromConsole(this.validator).СonsoleInput();
            record.Id = id;
            this.fileCabinetService.EditRecord(record);
            Console.WriteLine($"Record #{id} is updated.");
        }
    }
}
