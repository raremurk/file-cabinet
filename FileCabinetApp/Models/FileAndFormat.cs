namespace FileCabinetApp.Models
{
    /// <summary>File name and format.</summary>
    public class FileAndFormat
    {
        /// <summary>Gets or sets file name.</summary>
        /// <value>File name.</value>
        public string FileName { get; set; } = string.Empty;

        /// <summary>Gets or sets a value indicating whether to use XML format.</summary>
        /// <value>XML format.</value>
        public bool XMLFormat { get; set; }

        /// <summary>Gets or sets a value indicating whether to use CSV format.</summary>
        /// <value>CSV format.</value>
        public bool CSVFormat { get; set; }
    }
}
