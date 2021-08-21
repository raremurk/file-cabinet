using System;
using FileCabinetApp.Models;

namespace FileCabinetApp
{
    /// <summary>Provides functionality to validate parameters.</summary>
    public interface IRecordValidator
    {
        /// <summary>First name validation.</summary>
        /// <param name="firstName">First name.</param>
        /// <returns>Returns false and exception message if parameter is incorrect, else returns true and string.Empty.</returns>
        public Tuple<bool, string> ValidateFirstName(string firstName);

        /// <summary>Last name validation.</summary>
        /// <param name="lastName">Last name.</param>
        /// <returns>Returns false and exception message if parameter is incorrect, else returns true and string.Empty.</returns>
        public Tuple<bool, string> ValidateLastName(string lastName);

        /// <summary>Date of birth validation.</summary>
        /// <param name="dateOfBirth">Date of birth.</param>
        /// <returns>Returns false and exception message if parameter is incorrect, else returns true and string.Empty.</returns>
        public Tuple<bool, string> ValidateDateOfBirth(DateTime dateOfBirth);

        /// <summary>Workplace number validation.</summary>
        /// <param name="workPlaceNumber">Workplace number.</param>
        /// <returns>Returns false and exception message if parameter is incorrect, else returns true and string.Empty.</returns>
        public Tuple<bool, string> ValidateWorkPlaceNumber(short workPlaceNumber);

        /// <summary>Salary validation.</summary>
        /// <param name="salary">Salary.</param>
        /// <returns>Returns false and exception message if parameter is incorrect, else returns true and string.Empty.</returns>
        public Tuple<bool, string> ValidateSalary(decimal salary);

        /// <summary>Department validation.</summary>
        /// <param name="department">Department.</param>
        /// <returns>Returns false and exception message if parameter is incorrect, else returns true and string.Empty.</returns>
        public Tuple<bool, string> ValidateDepartment(char department);

        /// <summary>Record validation.</summary>
        /// <param name="record">Object representing a record.</param>
        /// <returns>Returns false and exception message if record is incorrect, else returns true and string.Empty.</returns>
        public Tuple<bool, string> ValidateRecord(FileCabinetRecord record);

        /// <summary>Record validation with throwing exceptions.</summary>
        /// <param name="record">Object representing a record.</param>
        public void ValidateRecordWithExceptions(FileCabinetRecord record);
    }
}