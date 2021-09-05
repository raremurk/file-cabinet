using System;
using FileCabinetApp.Models;

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

        /// <inheritdoc cref="CommandHandlerBase.Handle(AppCommandRequest)"/>
        public override void Handle(AppCommandRequest request) => this.Handle(request, StatCommand, this.Stat);

        private void Stat(string parameters)
        {
            ServiceStat stat = this.fileCabinetService.GetStat();
            Console.WriteLine($"{stat.AllRecordsCount} record(s). {stat.DeletedRecordsCount} of them are deleted.");
        }
    }
}
