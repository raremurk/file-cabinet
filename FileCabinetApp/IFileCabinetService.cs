using System;
using System.Collections.ObjectModel;

namespace FileCabinetApp
{
    /// <summary>Provides functionality for interaction with records in the file cabinet.</summary>
    public interface IFileCabinetService
    {
        /// <summary>Creates a record and returns its id.</summary>
        /// <param name="record">Object representing a record.</param>
        /// <returns>Id of a new record.</returns>
        public int CreateRecord(FileCabinetRecord record);

        /// <summary>Edits a record with the specified id.</summary>
        /// <param name="record">Object representing a record.</param>
        public void EditRecord(FileCabinetRecord record);

        /// <summary>Finds records by first name.</summary>
        /// <param name="firstName">First name to find.</param>
        /// <returns>Returns readonly collection of found records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName);

        /// <summary>Finds records by last name.</summary>
        /// <param name="lastName">Last name to find.</param>
        /// <returns>Returns readonly collection of found records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName);

        /// <summary>Finds records by date of birth.</summary>
        /// <param name="dateOfBirth">Date of birth to find.</param>
        /// <returns>Returns readonly collection of found records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth);

        /// <summary>Returns all records.</summary>
        /// <returns>Returns readonly collection of all records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords();

        /// <summary>Returns number of records.</summary>
        /// <returns>Returns number.</returns>
        public int GetStat();

        /// <summary>Makes snapshot of current object state.</summary>
        /// <returns>Returns new <see cref="FileCabinetServiceSnapshot"/>.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot();
    }
}
