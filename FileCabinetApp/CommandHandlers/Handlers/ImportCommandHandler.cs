using System;
using System.IO;
using System.Xml;
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
            if (string.IsNullOrEmpty(parameters))
            {
                Console.WriteLine("Input property name.");
                return;
            }

            var inputs = parameters.Split(' ', 2);

            if (inputs.Length != 2)
            {
                Console.WriteLine("Wrong number of parameters.");
                return;
            }

            string[] availableFormats = { "csv", "xml" };
            string[] fileExtensions = { ".csv", ".xml" };
            string format = inputs[0];
            string filename = inputs[1];

            int formatIndex = Array.FindIndex(availableFormats, x => x.Equals(format, StringComparison.OrdinalIgnoreCase));
            bool csvFormat = formatIndex == 0;
            bool xmlFormat = formatIndex == 1;

            if (filename.Length < 5)
            {
                Console.WriteLine("Invalid file name.");
                return;
            }

            string fileExtension = filename[^4..];
            int fileExtensionIndex = Array.FindIndex(fileExtensions, i => i.Equals(fileExtension, StringComparison.OrdinalIgnoreCase));

            var way = filename.Split('\\', StringSplitOptions.RemoveEmptyEntries);
            string directory = way.Length > 1 ? string.Join('\\', way[0..^1]) : Directory.GetCurrentDirectory();

            bool directoryAndFilenameExists = new DirectoryInfo(directory).Exists && new FileInfo(filename).Exists;
            filename = directoryAndFilenameExists && ((fileExtensionIndex == 0 && csvFormat) || (fileExtensionIndex == 1 && xmlFormat)) ? filename : string.Empty;

            if (!csvFormat && !xmlFormat)
            {
                Console.WriteLine("Invalid format.");
                return;
            }

            if (string.IsNullOrEmpty(filename))
            {
                Console.WriteLine("No such directory or invalid file name");
                return;
            }

            ServiceStat stat = this.fileCabinetService.GetStat();

            if (csvFormat)
            {
                using StreamReader reader = new (filename, System.Text.Encoding.UTF8);
                FileCabinetServiceSnapshot snapshot = FileCabinetServiceSnapshot.LoadFromCsv(reader);
                this.fileCabinetService.Restore(snapshot);
            }

            if (xmlFormat)
            {
                using var reader = XmlReader.Create(filename);
                FileCabinetServiceSnapshot snapshot = FileCabinetServiceSnapshot.LoadFromXml(reader);
                this.fileCabinetService.Restore(snapshot);
            }

            int numberOfRecords = this.fileCabinetService.GetStat().ExistingRecordsIds.Count - stat.ExistingRecordsIds.Count;

            Console.WriteLine($"{numberOfRecords} records are imported from file {filename}.");
        }
    }
}
