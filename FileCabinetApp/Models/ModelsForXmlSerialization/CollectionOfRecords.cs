using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace FileCabinetApp.Models
{
    /// <summary>Сlass representing list of records.</summary>
    [XmlRoot(ElementName = "records")]
    public class CollectionOfRecords
    {
        #pragma warning disable CA2227
        /// <summary>Gets or sets the list of records.</summary>
        /// <value>List of records.</value>
        [XmlElement(ElementName = "record")]
        public Collection<Record> Records { get; set; }
        #pragma warning restore CA2227
    }
}
