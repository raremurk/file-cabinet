using System.Xml.Serialization;

namespace FileCabinetApp
{
    /// <summary>Сlass representing Fullname for FileCabinetRecordForXmlSerialization.</summary>
    public class FullName
    {
        /// <summary>Gets or sets the first name.</summary>
        /// <value>First name of record.</value>
        [XmlAttribute(AttributeName = "first")]
        public string FirstName { get; set; }

        /// <summary>Gets or sets the last name.</summary>
        /// <value>Last name of record.</value>
        [XmlAttribute(AttributeName = "last")]
        public string LastName { get; set; }
    }
}
