using System;
using System.IO;
using System.Xml;
using FileCabinetApp.Helpers;

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

            var file = Parser.GetFilePathFromString(parameters);
            if (file is null)
            {
                return;
            }

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

            if (file.CSVFormat)
            {
                using StreamWriter writer = new (file.FileName, false, System.Text.Encoding.UTF8);
                snapshot.SaveToCsv(writer);
            }
            else
            {
                XmlWriterSettings settings = new ();
                settings.Indent = true;
                using var writer = XmlWriter.Create(file.FileName, settings);
                snapshot.SaveToXml(writer);
            }

            Console.WriteLine($"All records are exported to file {file.FileName}.");
        }
    }
}