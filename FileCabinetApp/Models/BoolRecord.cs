namespace FileCabinetApp.Models
{
    /// <summary>Сlass representing a record.</summary>
    public class BoolRecord
    {
        /// <summary>Gets or sets a value indicating whether to display id.</summary>
        /// <value>Id of record.</value>
        public bool Id { get; set; } = true;

        /// <summary>Gets or sets a value indicating whether to display the first name.</summary>
        /// <value>First name of record.</value>
        public bool FirstName { get; set; } = true;

        /// <summary>Gets or sets a value indicating whether to display the last name.</summary>
        /// <value>Last name of record.</value>
        public bool LastName { get; set; } = true;

        /// <summary>Gets or sets a value indicating whether to display the date of birth.</summary>
        /// <value>Date of birth of record.</value>
        public bool DateOfBirth { get; set; } = true;

        /// <summary>Gets or sets a value indicating whether to display the work place number.</summary>
        /// <value>Work place number of record.</value>
        public bool WorkPlaceNumber { get; set; } = true;

        /// <summary>Gets or sets a value indicating whether to display the salary.</summary>
        /// <value>Salary of record.</value>
        public bool Salary { get; set; } = true;

        /// <summary>Gets or sets a value indicating whether to display the department.</summary>
        /// <value>Department of record.</value>
        public bool Department { get; set; } = true;
    }
}
