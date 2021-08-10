using System;
using System.IO;
using System.Xml;

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

        /// <summary>Handles the specified request.</summary>
        /// <param name="request">The request.</param>
        public override void Handle(AppCommandRequest request) => this.Handle(request, ExportCommand, this.Export);

        private void Export(string parameters)
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

            bool directoryAndFilenameExists = new DirectoryInfo(directory).Exists;
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

            if (new FileInfo(filename).Exists)
            {
                Console.Write($"File is exist - rewrite {filename}?[Y / N] ");
                string answer = Console.ReadLine();

                if (!string.Equals(answer, "Y", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Operation canceled.");
                    return;
                }
            }

            FileCabinetServiceSnapshot snapshot = this.fileCabinetService.MakeSnapshot();

            if (csvFormat)
            {
                using StreamWriter writer = new (filename, false, System.Text.Encoding.UTF8);
                snapshot.SaveToCsv(writer);
            }
            else
            {
                XmlWriterSettings settings = new ();
                settings.Indent = true;
                using var writer = XmlWriter.Create(filename, settings);
                snapshot.SaveToXml(writer);
            }

            Console.WriteLine($"All records are exported to file {filename}.");
        }
    }
}