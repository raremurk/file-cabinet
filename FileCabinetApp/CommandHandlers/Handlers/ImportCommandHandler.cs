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

            var file = Parser.GetFileAndFormatFromString(parameters);
            var snapshot = new FileCabinetServiceSnapshot();
            switch (file.Format)
            {
                case Formats.CSV:
                    {
                        using var reader = new StreamReader(file.FileName, System.Text.Encoding.UTF8);
                        snapshot.LoadFromCsv(reader);
                        break;
                    }

                case Formats.XML:
                    {
                        using var reader = XmlReader.Create(file.FileName);
                        snapshot.LoadFromXml(reader);
                        break;
                    }

                default:
                    Console.WriteLine("Unknown format.");
                    return;
            }

            int restoredRecordsCount = this.fileCabinetService.Restore(snapshot);
            Console.WriteLine($"{restoredRecordsCount} records are imported from file {file.FileName}.");
        }
    }
}
