using System;
using System.Collections.Generic;

namespace FileCabinetApp
{
    /// <summary>Provides functionality for interaction with records in the file cabinet.</summary>
    public interface IFileCabinetService
    {
        /// <summary>Creates new record and returns its id.</summary>
        /// <param name="record">File cabinet record.</param>
        /// <returns>Id of created record.</returns>
        /// <exception cref="ArgumentNullException">Thrown when record is null.</exception>
        public int CreateRecord(FileCabinetRecord record);

        /// <summary>Edits existing record with the specified id.</summary>
        /// <param name="record">File cabinet record.</param>
        /// <exception cref="ArgumentNullException">Thrown when record is null.</exception>
        public void EditRecord(FileCabinetRecord record);

        /// <summary>Finds records by first name.</summary>
        /// <param name="firstName">First name to find.</param>
        /// <returns>Returns IEnumerable of found records.</returns>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName);

        /// <summary>Finds records by last name.</summary>
        /// <param name="lastName">Last name to find.</param>
        /// <returns>Returns IEnumerable of found records.</returns>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName);

        /// <summary>Finds records by date of birth.</summary>
        /// <param name="dateOfBirth">Date of birth to find.</param>
        /// <returns>Returns IEnumerable of found records.</returns>
        public IEnumerable<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth);

        /// <summary>Returns all records.</summary>
        /// <returns>Returns IEnumerable of all records.</returns>
        public IEnumerable<FileCabinetRecord> GetRecords();

        /// <summary>Checks for a record with the specified id.</summary>
        /// <param name="id">Id.</param>
        /// <returns>Returns true if record exists, else false.</returns>
        public bool IdExists(int id);

        /// <summary>Returns service statistics.</summary>
        /// <returns>Returns ServiceStat.</returns>
        public ServiceStat GetStat();

        /// <summary>Makes snapshot of current object state.</summary>
        /// <returns>Returns new <see cref="FileCabinetServiceSnapshot"/>.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot();

        /// <summary>Restores the specified snapshot.</summary>
        /// <param name="snapshot">Snapshot.</param>
        /// <exception cref="ArgumentNullException">Thrown when snapshot is null.</exception>
        public void Restore(FileCabinetServiceSnapshot snapshot);

        /// <summary>Removes record with the specified id.</summary>
        /// <param name="id">Id.</param>
        public void RemoveRecord(int id);

        /// <summary>Defragments the data file.</summary>
        public void Purge();
    }
}
