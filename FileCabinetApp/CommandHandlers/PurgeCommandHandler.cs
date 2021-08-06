using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>Purge command handler.</summary>
    /// <seealso cref="CommandHandlerBase" />
    public class PurgeCommandHandler : CommandHandlerBase
    {
        private const string PurgeCommand = "purge";
        private readonly IFileCabinetService fileCabinetService;

        /// <summary>Initializes a new instance of the <see cref="PurgeCommandHandler"/> class.</summary>
        /// <param name="fileCabinetService">IFileCabinetService.</param>
        public PurgeCommandHandler(IFileCabinetService fileCabinetService)
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

            if (string.Equals(PurgeCommand, request.Command, StringComparison.OrdinalIgnoreCase))
            {
                this.Purge(request.Parameters);
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

        private void Purge(string arameters)
        {
            ServiceStat stat = this.fileCabinetService.GetStat();
            this.fileCabinetService.Purge();
            Console.WriteLine($"Data file processing is completed: {stat.DeletedRecordsIds.Count} of {stat.NumberOfRecords} records were purged.");
        }
    }
}
