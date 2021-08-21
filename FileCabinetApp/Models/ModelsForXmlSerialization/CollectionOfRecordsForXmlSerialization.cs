using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace FileCabinetApp
{
    /// <summary>Сlass representing list of FileCabinetRecordsForXmlSerialization.</summary>
    [XmlRoot(ElementName = "records")]
    public class CollectionOfRecordsForXmlSerialization
    {
        #pragma warning disable CA2227
        /// <summary>Gets or sets the list of records.</summary>
        /// <value>List of records.</value>
        [XmlElement(ElementName = "record")]
        public Collection<FileCabinetRecordForXmlSerialization> Records { get; set; }
        #pragma warning restore CA2227
    }
}
