using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>Remove command handler.</summary>
    /// <seealso cref="ServiceCommandHandlerBase" />
    public class RemoveCommandHandler : ServiceCommandHandlerBase
    {
        private const string RemoveCommand = "remove";

        /// <summary>Initializes a new instance of the <see cref="RemoveCommandHandler"/> class.</summary>
        /// <param name="fileCabinetService">IFileCabinetService.</param>
        public RemoveCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
        }

        /// <summary>Handles the specified request.</summary>
        /// <param name="request">The request.</param>
        public override void Handle(AppCommandRequest request) => this.Handle(request, RemoveCommand, this.Remove);

        private void Remove(string parameters)
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

            this.fileCabinetService.RemoveRecord(id);
            Console.WriteLine($"Record #{id} is removed.");
        }
    }
}
