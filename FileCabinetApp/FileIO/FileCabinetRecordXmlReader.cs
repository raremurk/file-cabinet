using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;
using FileCabinetApp.Models;

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
            this.reader = reader ?? throw new ArgumentNullException(nameof(reader));
        }

        /// <summary>Reads all records from xml file.</summary>
        /// <returns>Returns IEnumerable of records.</returns>
        public IEnumerable<FileCabinetRecord> ReadAll()
        {
            XmlSerializer serializer = new (typeof(CollectionOfRecords));
            CollectionOfRecords collection = (CollectionOfRecords)serializer.Deserialize(this.reader);
            foreach (var record in collection.Records)
            {
                yield return new FileCabinetRecord
                {
                    Id = record.Id,
                    FirstName = record.FullName.FirstName,
                    LastName = record.FullName.LastName,
                    DateOfBirth = DateTime.Parse(record.DateOfBirth, CultureInfo.InvariantCulture),
                    WorkPlaceNumber = record.WorkPlaceNumber,
                    Salary = decimal.Parse(record.Salary, CultureInfo.InvariantCulture),
                    Department = char.Parse(record.Department),
                };
            }
        }
    }
}