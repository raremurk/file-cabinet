using System.Xml.Serialization;

namespace FileCabinetApp
{
    /// <summary>Сlass representing a record for for XmlSerialization.</summary>
    public class FileCabinetRecordForXmlSerialization
    {
        /// <summary>Gets or sets the id.</summary>
        /// <value>Id of record.</value>
        [XmlAttribute(AttributeName = "id")]
        public int Id { get; set; }

        /// <summary>Gets or sets the full name.</summary>
        /// <value>Full name of record.</value>
        [XmlElement(ElementName = "name")]
        public FullName FullName { get; set; }

        /// <summary>Gets or sets the date of birth.</summary>
        /// <value>Date of birth of record.</value>
        [XmlElement(ElementName = "dateOfBirth")]
        public string DateOfBirth { get; set; }

        /// <summary>Gets or sets the work place number.</summary>
        /// <value>Work place number of record.</value>
        [XmlElement(ElementName = "workPlaceNumber")]
        public short WorkPlaceNumber { get; set; }

        /// <summary>Gets or sets the salary.</summary>
        /// <value>Salary of record.</value>
        [XmlElement(ElementName = "salary")]
        public string Salary { get; set; }

        /// <summary>Gets or sets the department.</summary>
        /// <value>Department of record.</value>
        [XmlElement(ElementName = "department")]
        public string Department { get; set; }
    }
}
