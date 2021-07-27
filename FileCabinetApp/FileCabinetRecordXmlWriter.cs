using System;
using System.Globalization;
using System.Xml;

namespace FileCabinetApp
{
    /// <summary>Provides functionality for writing record to xml file.</summary>
    public class FileCabinetRecordXmlWriter
    {
        private readonly XmlWriter writer;

        /// <summary>Initializes a new instance of the <see cref="FileCabinetRecordXmlWriter"/> class.</summary>
        /// <param name="writer">StreamWriter.</param>
        public FileCabinetRecordXmlWriter(XmlWriter writer)
        {
            this.writer = writer;
        }

        /// <summary>Writes the specified record to xml file.</summary>
        /// <param name="record">Record.</param>
        /// <exception cref="ArgumentNullException">Thrown when record is null.</exception>
        public void Write(FileCabinetRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            this.writer.WriteStartElement("record");
            this.writer.WriteAttributeString("id", $"{record.Id}");

            this.writer.WriteStartElement("name");
            this.writer.WriteAttributeString("first", $"{record.FirstName}");
            this.writer.WriteAttributeString("last", $"{record.LastName}");
            this.writer.WriteEndElement();

            this.writer.WriteElementString("dateOfBirth", $"{record.DateOfBirth.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)}");
            this.writer.WriteElementString("workPlaceNumber", $"{record.WorkPlaceNumber}");
            this.writer.WriteElementString("salary", $"{record.Salary}");
            this.writer.WriteElementString("department", $"{record.Department}");

            this.writer.WriteEndElement();
        }
    }
}
