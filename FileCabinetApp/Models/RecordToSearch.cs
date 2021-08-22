using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace FileCabinetApp.Models
{
    /// <summary>Сlass representing a record to search.</summary>
    public class RecordToSearch
    {
        /// <summary>Gets or sets a value indicating whether need to use AND mode.</summary>
        /// <value>True - AND mode, false - OR mode.</value>
        public bool AndMode { get; set; }

        /// <summary>Gets or sets the id.</summary>
        /// <value>Id of record.</value>
        public Tuple<bool, int> Id { get; set; }

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

        /// <summary>Determines whether search needed.</summary>
        /// <returns>True if need to search.</returns>
        public bool NeedToSearch()
        {
            return this.Id.Item1
                || this.FirstName.Item1
                || this.LastName.Item1
                || this.DateOfBirth.Item1
                || this.WorkPlaceNumber.Item1
                || this.Salary.Item1
                || this.Department.Item1;
        }

        /// <summary>Calculates hash of the RecordToSearch.</summary>
        /// <returns>Returns hash.</returns>
        public string GetHash()
        {
            string json = JsonSerializer.Serialize(this);
            using var sha256 = SHA256.Create();
            byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(json));
            return BitConverter.ToString(hash).Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase);
        }
    }
}
