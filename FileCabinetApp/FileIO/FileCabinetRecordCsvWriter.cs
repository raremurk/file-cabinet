using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using FileCabinetApp.Models;

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
            this.writer = writer ?? throw new ArgumentNullException(nameof(writer));
        }

        /// <summary>Writes the specified records to csv file.</summary>
        /// <param name="records">Records.</param>
        /// <exception cref="ArgumentNullException">Thrown when records is null.</exception>
        public void Write(IEnumerable<FileCabinetRecord> records)
        {
            _ = records ?? throw new ArgumentNullException(nameof(records));

            this.writer.WriteLine("Id,First Name,Last Name,Date of Birth,Workplace Number,Salary,Department");
            foreach (var record in records)
            {
                StringBuilder builder = new ();
                builder.Append($"{record.Id},");
                builder.Append($"{record.FirstName},");
                builder.Append($"{record.LastName},");
                builder.Append($"{record.DateOfBirth.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)},");
                builder.Append($"{record.WorkPlaceNumber},");
                builder.Append($"{record.Salary.ToString("F2", CultureInfo.InvariantCulture)},");
                builder.Append($"{record.Department}");
                this.writer.WriteLine(builder.ToString());
            }
        }
    }
}
