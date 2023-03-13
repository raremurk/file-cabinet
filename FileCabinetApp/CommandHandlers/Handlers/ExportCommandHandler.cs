using System;
using System.IO;
using System.Xml;
using FileCabinetApp.Helpers;
using FileCabinetApp.Models;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>Export command handler.</summary>
    /// <seealso cref="ServiceCommandHandlerBase" />
    public class ExportCommandHandler : ServiceCommandHandlerBase
    {
        private const string ExportCommand = "export";

        /// <summary>Initializes a new instance of the <see cref="ExportCommandHandler"/> class.</summary>
        /// <param name="fileCabinetService">IFileCabinetService.</param>
        public ExportCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
        }

        /// <inheritdoc cref="CommandHandlerBase.Handle(AppCommandRequest)"/>
        public override void Handle(AppCommandRequest request) => this.Handle(request, ExportCommand, this.Export);

        private void Export(string parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters))
            {
                Console.WriteLine("Invalid input. Example: export csv d:\\data\\records.csv");
                return;
            }

            var file = Parser.GetFileAndFormatFromString(parameters);
            if (new FileInfo(file.FileName).Exists)
            {
                Console.Write($"File is exist - rewrite {file.FileName}?[Y / N] ");
                string answer = Console.ReadLine();
                if (!string.Equals(answer, "Y", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Operation canceled.");
                    return;
                }
            }

            FileCabinetServiceSnapshot snapshot = this.fileCabinetService.MakeSnapshot();
            switch (file.Format)
            {
                case Formats.CSV:
                    {
                        using var writer = new StreamWriter(file.FileName, false, System.Text.Encoding.UTF8);
                        snapshot.SaveToCsv(writer);
                        break;
                    }

                case Formats.XML:
                    {
                        using var writer = XmlWriter.Create(file.FileName, new XmlWriterSettings { Indent = true });
                        snapshot.SaveToXml(writer);
                        break;
                    }

                default:
                    Console.WriteLine("Unknown format.");
                    return;
            }

            Console.WriteLine($"All records are exported to file {file.FileName}.");
        }
    }
}