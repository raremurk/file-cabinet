using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>Stat command handler.</summary>
    /// <seealso cref="ServiceCommandHandlerBase" />
    public class StatCommandHandler : ServiceCommandHandlerBase
    {
        private const string StatCommand = "stat";

        /// <summary>Initializes a new instance of the <see cref="StatCommandHandler"/> class.</summary>
        /// <param name="fileCabinetService">IFileCabinetService.</param>
        public StatCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
        }

        /// <summary>Handles the specified request.</summary>
        /// <param name="request">The request.</param>
        public override void Handle(AppCommandRequest request) => this.Handle(request, StatCommand, this.Stat);

        private void Stat(string parameters)
        {
            ServiceStat stat = this.fileCabinetService.GetStat();
            Console.WriteLine($"{stat.NumberOfRecords} record(s). {stat.DeletedRecordsIds.Count} deleted record(s).");
        }
    }
}
