using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml;
using FileCabinetApp.Models;

namespace FileCabinetApp
{
    /// <summary>Provides functionality for creating snapshot.</summary>
    public class FileCabinetServiceSnapshot
    {
        private FileCabinetRecord[] records = Array.Empty<FileCabinetRecord>();

        /// <summary>Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.</summary>
        /// <param name="records">ReadOnlyCollection of records.</param>
        public FileCabinetServiceSnapshot(FileCabinetRecord[] records)
        {
            this.records = records ?? throw new ArgumentNullException(nameof(records));
        }

        /// <summary>Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.</summary>
        public FileCabinetServiceSnapshot()
        {
        }

        /// <summary>Gets collection of records.</summary>
        /// <value>ReadOnlyCollection of FileCabinetRecord.</value>
        public ReadOnlyCollection<FileCabinetRecord> Records
        {
            get { return new (this.records); }
        }

        /// <summary>Loads records from CSV file.</summary>
        /// <param name="reader">Reader.</param>
        public void LoadFromCsv(StreamReader reader)
        {
            var csvReader = new FileCabinetRecordCsvReader(reader);
            this.records = csvReader.ReadAll().ToArray();
        }

        /// <summary>Loads records from XML file.</summary>
        /// <param name="reader">Reader.</param>
        public void LoadFromXml(XmlReader reader)
        {
            var xmlReader = new FileCabinetRecordXmlReader(reader);
            this.records = xmlReader.ReadAll().ToArray();
        }

        /// <summary>Writes snapshot to csv file.</summary>
        /// <param name="writer">StreamWriter.</param>
        public void SaveToCsv(StreamWriter writer)
        {
            var csvWriter = new FileCabinetRecordCsvWriter(writer);
            csvWriter.Write(this.records);
        }

        /// <summary>Writes snapshot to xml file.</summary>
        /// <param name="writer">StreamWriter.</param>
        public void SaveToXml(XmlWriter writer)
        {
            var xmlWriter = new FileCabinetRecordXmlWriter(writer);
            xmlWriter.Write(this.records);
        }

        /// <summary>String representation of snapshot.</summary>
        /// <returns>Returns string representation of snapshot.</returns>
        public override string ToString() => $"Snapshot object with {this.records.Length} record(s)";
    }
}
