using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>Stat command handler.</summary>
    /// <seealso cref="CommandHandlerBase" />
    public class StatCommandHandler : CommandHandlerBase
    {
        private const string StatCommand = "stat";
        private readonly IFileCabinetService fileCabinetService;

        /// <summary>Initializes a new instance of the <see cref="StatCommandHandler"/> class.</summary>
        /// <param name="fileCabinetService">IFileCabinetService.</param>
        public StatCommandHandler(IFileCabinetService fileCabinetService)
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

            if (string.Equals(StatCommand, request.Command, StringComparison.OrdinalIgnoreCase))
            {
                this.Stat(request.Parameters);
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

        private void Stat(string parameters)
        {
            ServiceStat stat = this.fileCabinetService.GetStat();
            Console.WriteLine($"{stat.NumberOfRecords} record(s). {stat.DeletedRecordsIds.Count} deleted record(s).");
        }
    }
}
