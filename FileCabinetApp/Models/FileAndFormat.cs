namespace FileCabinetApp.Models
{
    /// <summary>File name and format.</summary>
    public class FileAndFormat
    {
        /// <summary>Gets or sets file format.</summary>
        /// <value>File format.</value>
        public Formats Format { get; set; } = Formats.Unknown;

        /// <summary>Gets or sets file name.</summary>
        /// <value>File name.</value>
        public string FileName { get; set; } = string.Empty;
    }
}
