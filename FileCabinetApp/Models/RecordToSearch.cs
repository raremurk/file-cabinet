using System;
using System.Collections.Generic;
using System.Globalization;
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

        /// <summary>String representation of record.</summary>
        /// <param name="separator">Separator.</param>
        /// <returns>Returns string representation of record.</returns>
        public string ToString(string separator)
        {
            var properties = new List<Tuple<bool, string>>
            {
                new (this.Id.Item1, $"Id = '{this.Id.Item2}'"),
                new (this.FirstName.Item1, $"FirstName = '{this.FirstName.Item2}'"),
                new (this.LastName.Item1, $"LastName = '{this.LastName.Item2}'"),
                new (this.DateOfBirth.Item1, $"DateOfBirth = '{this.DateOfBirth.Item2.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)}'"),
                new (this.WorkPlaceNumber.Item1, $"WorkPlaceNumber = '{this.WorkPlaceNumber.Item2}'"),
                new (this.Salary.Item1, $"Salary = '{this.Salary.Item2.ToString("F2", CultureInfo.InvariantCulture)}'"),
                new (this.Department.Item1, $"Department = '{this.Department.Item2}'"),
            };

            string searchMode = this.AndMode ? "AND" : "OR";
            var answer = new List<string> { $"Search Mode = '{searchMode}'" };
            foreach (var property in properties)
            {
                if (property.Item1)
                {
                    answer.Add(property.Item2);
                }
            }

            return string.Join(separator, answer);
        }
    }
}
