using System;
using System.Collections.Generic;
using System.Globalization;

namespace FileCabinetApp.Models
{
    /// <summary>Сlass representing a record.</summary>
    public class FileCabinetRecord
    {
        /// <summary>Gets or sets the id.</summary>
        /// <value>Id of record.</value>
        public int Id { get; set; }

        /// <summary>Gets or sets the first name.</summary>
        /// <value>First name of record.</value>
        public string FirstName { get; set; }

        /// <summary>Gets or sets the last name.</summary>
        /// <value>Last name of record.</value>
        public string LastName { get; set; }

        /// <summary>Gets or sets the date of birth.</summary>
        /// <value>Date of birth of record.</value>
        public DateTime DateOfBirth { get; set; }

        /// <summary>Gets or sets the work place number.</summary>
        /// <value>Work place number of record.</value>
        public short WorkPlaceNumber { get; set; }

        /// <summary>Gets or sets the salary.</summary>
        /// <value>Salary of record.</value>
        public decimal Salary { get; set; }

        /// <summary>Gets or sets the department.</summary>
        /// <value>Department of record.</value>
        public char Department { get; set; }

        /// <summary>String representation of record.</summary>
        /// <param name="separator">Separator.</param>
        /// <returns>Returns string representation of record.</returns>
        public string ToString(string separator)
        {
            var properties = new List<string>
            {
                $"Id = '{this.Id}'",
                $"FirstName = '{this.FirstName}'",
                $"LastName = '{this.LastName}'",
                $"DateOfBirth = '{this.DateOfBirth.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)}'",
                $"WorkPlaceNumber = '{this.WorkPlaceNumber}'",
                $"Salary = '{this.Salary.ToString("F2", CultureInfo.InvariantCulture)}'",
                $"Department = '{this.Department}'",
            };

            return string.Join(separator, properties);
        }
    }
}
