using System;

namespace FileCabinetApp
{
    /// <summary>Provides functionality to validate parameters.</summary>
    public interface IRecordValidator
    {
        /// <summary>Name validation.</summary>
        /// <param name="name">Input string representing the name.</param>
        /// <returns>Returns true and exception message if name is incorrect, else returns false.</returns>
        public Tuple<bool, string> NameIsCorrect(string name);

        /// <summary>Date of birth validation.</summary>
        /// <param name="dateOfBirth">Date of birth.</param>
        /// <returns>Returns true and exception message if date of birth is incorrect, else returns false.</returns>
        public Tuple<bool, string> DateOfBirthIsCorrect(DateTime dateOfBirth);

        /// <summary>Work place number validation.</summary>
        /// <param name="workPlaceNumber">Work place number.</param>
        /// <returns>Returns true and exception message if work place number is incorrect, else returns false.</returns>
        public Tuple<bool, string> WorkPlaceNumberIsCorrect(short workPlaceNumber);

        /// <summary>Salary validation.</summary>
        /// <param name="salary">Salary.</param>
        /// <returns>Returns true and exception message if salary is incorrect, else returns false.</returns>
        public Tuple<bool, string> SalaryIsCorrect(decimal salary);

        /// <summary>Department validation.</summary>
        /// <param name="department">Department.</param>
        /// <returns>Returns true and exception message if department is incorrect, else returns false.</returns>
        public Tuple<bool, string> DepartmentIsCorrect(char department);

        /// <summary>Record validation.</summary>
        /// <param name="record">Object representing a record.</param>
        public void ValidateParameters(FileCabinetRecord record);
    }
}
