using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;

namespace FileCabinetApp
{
    /// <summary>Provides method to import <see cref="FileCabinetRecord"/> records from xml file.</summary>
    public class FileCabinetRecordXmlReader
    {
        private readonly XmlReader reader;

        /// <summary>Initializes a new instance of the <see cref="FileCabinetRecordXmlReader"/> class.</summary>
        /// <param name="reader">StreamReader.</param>
        public FileCabinetRecordXmlReader(XmlReader reader)
        {
            this.reader = reader;
        }

        /// <summary>Reads all records from xml file.</summary>
        /// <returns>Returns List of records.</returns>
        public IList<FileCabinetRecord> ReadAll()
        {
            XmlSerializer serializer = new (typeof(CollectionOfRecordsForXmlSerialization));
            CollectionOfRecordsForXmlSerialization collection = (CollectionOfRecordsForXmlSerialization)serializer.Deserialize(this.reader);
            List<FileCabinetRecord> records = new ();
            foreach (var record in collection.Records)
            {
                var rec = new FileCabinetRecord
                {
                    Id = record.Id,
                    FirstName = record.FullName.FirstName,
                    LastName = record.FullName.LastName,
                    DateOfBirth = DateTime.Parse(record.DateOfBirth, CultureInfo.InvariantCulture),
                    WorkPlaceNumber = record.WorkPlaceNumber,
                    Salary = decimal.Parse(record.Salary, CultureInfo.InvariantCulture),
                    Department = char.Parse(record.Department),
                };

                records.Add(rec);
            }

            return records;
        }
    }
}