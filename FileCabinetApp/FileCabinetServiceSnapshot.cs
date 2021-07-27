﻿using System;
using System.Collections.ObjectModel;
using System.IO;

namespace FileCabinetApp
{
    /// <summary>Provides functionality for creating snapshot.</summary>
    public class FileCabinetServiceSnapshot
    {
        private readonly FileCabinetRecord[] records;

        /// <summary>Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.</summary>
        /// <param name="records">ReadOnlyCollection of records.</param>
        public FileCabinetServiceSnapshot(ReadOnlyCollection<FileCabinetRecord> records)
        {
            if (records is null)
            {
                throw new ArgumentNullException(nameof(records));
            }

            this.records = new FileCabinetRecord[records.Count];
            records.CopyTo(this.records, 0);
        }

        /// <summary>Writes snapshot to csv file.</summary>
        /// <param name="writer">StreamWriter.</param>
        /// <exception cref="ArgumentNullException">Thrown when writer is null.</exception>
        public void SaveToCsv(StreamWriter writer)
        {
            if (writer is null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            writer.WriteLine("Id,First Name,Last Name,Date of Birth,Workplace Number,Salary,Department");

            var csvWriter = new FileCabinetRecordCsvWriter(writer);
            foreach (var record in this.records)
            {
                csvWriter.Write(record);
            }
        }
    }
}
