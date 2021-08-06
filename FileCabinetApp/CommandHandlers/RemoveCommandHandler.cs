using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>Remove command handler.</summary>
    /// <seealso cref="CommandHandlerBase" />
    public class RemoveCommandHandler : CommandHandlerBase
    {
        private const string RemoveCommand = "remove";
        private readonly IFileCabinetService fileCabinetService;

        /// <summary>Initializes a new instance of the <see cref="RemoveCommandHandler"/> class.</summary>
        /// <param name="fileCabinetService">IFileCabinetService.</param>
        public RemoveCommandHandler(IFileCabinetService fileCabinetService)
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

            if (string.Equals(RemoveCommand, request.Command, StringComparison.OrdinalIgnoreCase))
            {
                this.Remove(request.Parameters);
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
