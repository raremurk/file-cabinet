using System;

namespace FileCabinetApp.Models
{
    /// <summary>Сlass representing a record to edit.</summary>
    public class RecordToEdit
    {
        /// <summary>Gets or sets the first name.</summary>
        /// <value>First name of record.</value>
        public Tuple<bool, string> FirstName { get; set; }

        /// <summary>Gets or sets the last name.</summary>
        /// <value>Last name of record.</value>
        public Tuple<bool, string> LastName { get; set; }

        /// <summary>Gets or sets the date of birth.</summary>
        /// <value>Date of birth of record.</value>
        public Tuple<bool, DateTime> DateOfBirth { get; set; }

        /// <summary>Gets or sets the work place number.</summary>
        /// <value>Work place number of record.</value>
        public Tuple<bool, short> WorkPlaceNumber { get; set; }

        /// <summary>Gets or sets the salary.</summary>
        /// <value>Salary of record.</value>
        public Tuple<bool, decimal> Salary { get; set; }

        /// <summary>Gets or sets the department.</summary>
        /// <value>Department of record.</value>
        public Tuple<bool, char> Department { get; set; }
    }
}
