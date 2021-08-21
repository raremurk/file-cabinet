using System;

namespace FileCabinetApp.Models
{
    /// <summary>Сlass representing a record.</summary>
    public class StringRecord
    {
        /// <summary>Gets or sets the id.</summary>
        /// <value>Id of record.</value>
        public string Id { get; set; }

        /// <summary>Gets or sets the first name.</summary>
        /// <value>First name of record.</value>
        public string FirstName { get; set; }

        /// <summary>Gets or sets the last name.</summary>
        /// <value>Last name of record.</value>
        public string LastName { get; set; }

        /// <summary>Gets or sets the date of birth.</summary>
        /// <value>Date of birth of record.</value>
        public string DateOfBirth { get; set; }

        /// <summary>Gets or sets the work place number.</summary>
        /// <value>Work place number of record.</value>
        public string WorkPlaceNumber { get; set; }

        /// <summary>Gets or sets the salary.</summary>
        /// <value>Salary of record.</value>
        public string Salary { get; set; }

        /// <summary>Gets or sets the department.</summary>
        /// <value>Department of record.</value>
        public string Department { get; set; }
    }
}
