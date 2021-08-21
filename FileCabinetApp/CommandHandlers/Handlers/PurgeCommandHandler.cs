using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>Purge command handler.</summary>
    /// <seealso cref="ServiceCommandHandlerBase" />
    public class PurgeCommandHandler : ServiceCommandHandlerBase
    {
        private const string PurgeCommand = "purge";

        /// <summary>Initializes a new instance of the <see cref="PurgeCommandHandler"/> class.</summary>
        /// <param name="fileCabinetService">IFileCabinetService.</param>
        public PurgeCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
        }

        /// <summary>Handles the specified request.</summary>
        /// <param name="request">The request.</param>
        public override void Handle(AppCommandRequest request) => this.Handle(request, PurgeCommand, this.Purge);

        private void Purge(string parameters)
        {
            ServiceStat stat = this.fileCabinetService.GetStat();
            this.fileCabinetService.Purge();
            Console.WriteLine($"Data file processing is completed: {stat.DeletedRecordsIds.Count} of {stat.ExistingRecordsIds.Count + stat.DeletedRecordsIds.Count} records were purged.");
        }
    }
}
