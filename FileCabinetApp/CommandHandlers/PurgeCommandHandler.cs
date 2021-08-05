using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>Purge command handler.</summary>
    /// <seealso cref="CommandHandlerBase" />
    public class PurgeCommandHandler : CommandHandlerBase
    {
        private const string PurgeCommand = "purge";

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
                Purge(request.Parameters);
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

        private static void Purge(string parameters)
        {
            ServiceStat stat = Program.fileCabinetService.GetStat();
            Program.fileCabinetService.Purge();
            Console.WriteLine($"Data file processing is completed: {stat.DeletedRecordsIds.Count} of {stat.NumberOfRecords} records were purged.");
        }
    }
}
