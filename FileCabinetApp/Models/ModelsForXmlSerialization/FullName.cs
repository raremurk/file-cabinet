using System.Xml.Serialization;

namespace FileCabinetApp.Models
{
    /// <summary>Сlass representing full name.</summary>
    public class FullName
    {
        /// <summary>Gets or sets the first name.</summary>
        /// <value>First name.</value>
        [XmlAttribute(AttributeName = "first")]
        public string FirstName { get; set; }

        /// <summary>Gets or sets the last name.</summary>
        /// <value>Last name.</value>
        [XmlAttribute(AttributeName = "last")]
        public string LastName { get; set; }
    }
}
