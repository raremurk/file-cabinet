using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>Provides functionality for writing record to csv file.</summary>
    public class FileCabinetRecordCsvWriter
    {
        private readonly StreamWriter writer;

        /// <summary>Initializes a new instance of the <see cref="FileCabinetRecordCsvWriter"/> class.</summary>
        /// <param name="writer">StreamWriter.</param>
        public FileCabinetRecordCsvWriter(StreamWriter writer)
        {
            this.writer = writer;
        }

        /// <summary>Writes the specified record to csv file.</summary>
        /// <param name="record">Record.</param>
        /// <exception cref="ArgumentNullException">Thrown when record is null.</exception>
        public void Write(FileCabinetRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            StringBuilder builder = new ();
            builder.Append($"{record.Id},");
            builder.Append($"{record.FirstName},");
            builder.Append($"{record.LastName},");
            builder.Append($"{record.DateOfBirth.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)},");
            builder.Append($"{record.WorkPlaceNumber},");
            builder.Append($"{record.Salary},");
            builder.Append($"{record.Department}");

            this.writer.WriteLine(builder.ToString());
        }
    }
}
