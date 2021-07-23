using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>Provides functionality to validate parameters.</summary>
    public interface IRecordValidator
    {
        /// <summary>String validation.</summary>
        /// <param name="stringLength">Length of input string.</param>
        /// <returns>Returns true if string is incorrect, else false.</returns>
        public bool CheckString(int stringLength);

        /// <summary>Date of birth validation.</summary>
        /// <param name="argument">Date of birth.</param>
        /// <returns>Returns true if date of birth is incorrect, else false.</returns>
        public bool CheckDateTimeRange(DateTime argument);

        /// <summary>Work place number validation.</summary>
        /// <param name="argument">Work place number.</param>
        /// <returns>Returns true if work place number is incorrect, else false.</returns>
        public bool CheckWorkPlaceNumber(short argument);

        /// <summary>Salary validation.</summary>
        /// <param name="argument">Salary.</param>
        /// <returns>Returns true if salary is incorrect, else false.</returns>
        public bool CheckSalary(decimal argument);

        /// <summary>Department validation.</summary>
        /// <param name="argument">Department.</param>
        /// <returns>Returns true if department is incorrect, else false.</returns>
        public bool CheckDepartment(char argument);

        /// <summary>Record validation.</summary>
        /// <param name="record">Object representing a record.</param>
        public void ValidateParameters(FileCabinetRecord record);
    }
}
