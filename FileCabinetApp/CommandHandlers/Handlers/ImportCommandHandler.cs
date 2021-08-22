using System;
using System.IO;
using System.Xml;
using FileCabinetApp.Helpers;
using FileCabinetApp.Models;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>Import command handler.</summary>
    /// <seealso cref="ServiceCommandHandlerBase" />
    public class ImportCommandHandler : ServiceCommandHandlerBase
    {
        private const string ImportCommand = "import";

        /// <summary>Initializes a new instance of the <see cref="ImportCommandHandler"/> class.</summary>
        /// <param name="fileCabinetService">IFileCabinetService.</param>
        public ImportCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
        }

        /// <inheritdoc cref="CommandHandlerBase.Handle(AppCommandRequest)"/>
        public override void Handle(AppCommandRequest request) => this.Handle(request, ImportCommand, this.Import);

        private void Import(string parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters))
            {
                Console.WriteLine("Invalid input. Example: import csv d:\\data\\records.csv");
                return;
            }

            var file = Parser.GetFilePathFromString(parameters);
            if (file is null)
            {
                return;
            }

            ServiceStat stat = this.fileCabinetService.GetStat();
            var snapshot = new FileCabinetServiceSnapshot();

            if (file.CSVFormat)
            {
                using StreamReader reader = new (file.FileName, System.Text.Encoding.UTF8);
                snapshot.LoadFromCsv(reader);
            }
            else
            {
                using var reader = XmlReader.Create(file.FileName);
                snapshot.LoadFromXml(reader);
            }

            this.fileCabinetService.Restore(snapshot);
            int numberOfRecords = this.fileCabinetService.GetStat().ExistingRecordsIds.Count - stat.ExistingRecordsIds.Count;
            Console.WriteLine($"{numberOfRecords} records are imported from file {file.FileName}.");
        }
    }
}
