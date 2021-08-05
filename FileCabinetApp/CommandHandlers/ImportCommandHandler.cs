using System;
using System.IO;
using System.Xml;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>Import command handler.</summary>
    /// <seealso cref="CommandHandlerBase" />
    public class ImportCommandHandler : CommandHandlerBase
    {
        private const string ImportCommand = "import";

        /// <summary>Handles the specified request.</summary>
        /// <param name="request">The request.</param>
        public override void Handle(AppCommandRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (string.Equals(ImportCommand, request.Command, StringComparison.OrdinalIgnoreCase))
            {
                Import(request.Parameters);
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

        private static void Import(string parameters)
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

            ServiceStat stat = Program.fileCabinetService.GetStat();

            if (csvFormat)
            {
                using StreamReader reader = new (filename, System.Text.Encoding.UTF8);
                FileCabinetServiceSnapshot snapshot = FileCabinetServiceSnapshot.LoadFromCsv(reader);
                Program.fileCabinetService.Restore(snapshot);
            }

            if (xmlFormat)
            {
                using var reader = XmlReader.Create(filename);
                FileCabinetServiceSnapshot snapshot = FileCabinetServiceSnapshot.LoadFromXml(reader);
                Program.fileCabinetService.Restore(snapshot);
            }

            int numberOfRecords = Program.fileCabinetService.GetStat().NumberOfRecords - stat.NumberOfRecords;

            Console.WriteLine($"{numberOfRecords} records are imported from file {filename}.");
        }
    }
}
