using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Xml.Serialization;
using FileCabinetApp.Models;

namespace FileCabinetApp.FileIO
{
    /// <summary>Provides functionality for writing record to xml file.</summary>
    public class FileCabinetRecordXmlSerializer
    {
        private readonly StreamWriter writer;

        /// <summary>Initializes a new instance of the <see cref="FileCabinetRecordXmlSerializer"/> class.</summary>
        /// <param name="writer">StreamWriter.</param>
        public FileCabinetRecordXmlSerializer(StreamWriter writer)
        {
            this.writer = writer ?? throw new ArgumentNullException(nameof(writer));
        }

        /// <summary>Writes the specified records to xml file.</summary>
        /// <param name="records">Records.</param>
        /// <exception cref="ArgumentNullException">Thrown when record is null.</exception>
        public void Write(IEnumerable<FileCabinetRecord> records)
        {
            _ = records ?? throw new ArgumentNullException(nameof(records));

            var recordsXml = new Collection<Record>();
            foreach (var record in records)
            {
                var rec = new Record
                {
                    Id = record.Id,
                    FullName = new FullName { FirstName = record.FirstName, LastName = record.LastName },
                    DateOfBirth = record.DateOfBirth.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                    WorkPlaceNumber = record.WorkPlaceNumber,
                    Salary = record.Salary.ToString("F2", CultureInfo.InvariantCulture),
                    Department = char.ToString(record.Department),
                };

                recordsXml.Add(rec);
            }

            var serializer = new XmlSerializer(typeof(CollectionOfRecords));
            serializer.Serialize(this.writer, new CollectionOfRecords { Records = recordsXml });
        }
    }
}
