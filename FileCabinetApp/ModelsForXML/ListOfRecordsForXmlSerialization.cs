using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace FileCabinetApp
{
    /// <summary>Сlass representing list of FileCabinetRecordsForXmlSerialization.</summary>
    [XmlRoot(ElementName = "records")]
    public class ListOfRecordsForXmlSerialization
    {
        /// <summary>Initializes a new instance of the <see cref="ListOfRecordsForXmlSerialization"/> class.</summary>
        /// <param name="records">Collection of records.</param>
        public ListOfRecordsForXmlSerialization(Collection<FileCabinetRecordForXmlSerialization> records)
        {
            this.Records = records;
        }

        /// <summary>Initializes a new instance of the <see cref="ListOfRecordsForXmlSerialization"/> class.</summary>
        public ListOfRecordsForXmlSerialization()
        {
        }

        /// <summary>Gets the list of records.</summary>
        /// <value>List of records.</value>
        [XmlElement(ElementName = "record")]
        public Collection<FileCabinetRecordForXmlSerialization> Records { get; }
    }
}
