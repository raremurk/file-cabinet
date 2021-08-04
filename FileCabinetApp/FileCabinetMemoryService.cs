using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>Class for working with records.</summary>
    public class FileCabinetMemoryService : IFileCabinetService
    {
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new ();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new ();
        private readonly Dictionary<string, List<FileCabinetRecord>> dateOfBirthDictionary = new ();
        private readonly List<FileCabinetRecord> list = new ();
        private readonly IRecordValidator validator;

        /// <summary>Initializes a new instance of the <see cref="FileCabinetMemoryService"/> class.</summary>
        /// <param name="validator">Validator.</param>
        public FileCabinetMemoryService(IRecordValidator validator)
        {
            this.validator = validator;
        }

        /// <summary>Creates a record and returns its id.</summary>
        /// <param name="record">Object representing a record.</param>
        /// <returns>Id of a new record.</returns>
        /// <exception cref="ArgumentNullException">Thrown when record is null.</exception>
        public int CreateRecord(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            this.validator.ValidateParameters(record);
            record.Id = this.list.Count + 1;
            this.list.Add(record);
            this.AddRecordToDictionaries(record);
            return record.Id;
        }

        /// <summary>Edits a record with the specified id.</summary>
        /// <param name="record">Object representing a record.</param>
        /// <exception cref="ArgumentNullException">Thrown when record is null.</exception>
        /// <exception cref="ArgumentException">Thrown when no record with the specified id.</exception>
        public void EditRecord(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            if (!this.list.Exists(x => x.Id == record.Id))
            {
                throw new ArgumentException("No record with this id.");
            }

            this.validator.ValidateParameters(record);
            FileCabinetRecord originalRecord = this.list.Find(x => x.Id == record.Id);
            this.RemoveRecordFromDictionaries(originalRecord);

            originalRecord.FirstName = record.FirstName;
            originalRecord.LastName = record.LastName;
            originalRecord.DateOfBirth = record.DateOfBirth;
            originalRecord.WorkPlaceNumber = record.WorkPlaceNumber;
            originalRecord.Salary = record.Salary;
            originalRecord.Department = record.Department;

            this.AddRecordToDictionaries(originalRecord);
        }

        /// <summary>Finds records by first name.</summary>
        /// <param name="firstName">First name to find.</param>
        /// <returns>Returns readonly collection of found records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            string firstNameKey = firstName is null ? string.Empty : firstName.ToUpperInvariant();
            return this.firstNameDictionary.ContainsKey(firstNameKey) ? new ReadOnlyCollection<FileCabinetRecord>(this.firstNameDictionary[firstNameKey]) : new ReadOnlyCollection<FileCabinetRecord>(Array.Empty<FileCabinetRecord>());
        }

        /// <summary>Finds records by last name.</summary>
        /// <param name="lastName">Last name to find.</param>
        /// <returns>Returns readonly collection of found records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            string lastNameKey = lastName is null ? string.Empty : lastName.ToUpperInvariant();
            return this.lastNameDictionary.ContainsKey(lastNameKey) ? new ReadOnlyCollection<FileCabinetRecord>(this.lastNameDictionary[lastNameKey]) : new ReadOnlyCollection<FileCabinetRecord>(Array.Empty<FileCabinetRecord>());
        }

        /// <summary>Finds records by date of birth.</summary>
        /// <param name="dateOfBirth">Date of birth to find.</param>
        /// <returns>Returns readonly collection of found records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth)
        {
            return this.dateOfBirthDictionary.ContainsKey(dateOfBirth) ? new ReadOnlyCollection<FileCabinetRecord>(this.dateOfBirthDictionary[dateOfBirth]) : new ReadOnlyCollection<FileCabinetRecord>(Array.Empty<FileCabinetRecord>());
        }

        /// <summary>Returns all records.</summary>
        /// <returns>Returns readonly collection of all records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords() => new (this.list);

        /// <summary>Returns number of records.</summary>
        /// <returns>Returns number.</returns>
        public int GetStat() => this.list.Count;

        /// <summary>Makes snapshot of current object state.</summary>
        /// <returns>Returns new <see cref="FileCabinetServiceSnapshot"/>.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot() => new (this.GetRecords());

        /// <summary>Restores the specified snapshot.</summary>
        /// <param name="snapshot">Snapshot.</param>
        /// <exception cref="ArgumentNullException">Thrown when snapshot is null.</exception>
        public void Restore(FileCabinetServiceSnapshot snapshot)
        {
            if (snapshot == null)
            {
                throw new ArgumentNullException(nameof(snapshot));
            }

            ReadOnlyCollection<FileCabinetRecord> unverifiedRecords = snapshot.Records;
            foreach (var record in unverifiedRecords)
            {
                Tuple<bool, string>[] validationResults =
                {
                this.validator.NameIsCorrect(record.FirstName),
                this.validator.NameIsCorrect(record.LastName),
                this.validator.DateOfBirthIsCorrect(record.DateOfBirth),
                this.validator.WorkPlaceNumberIsCorrect(record.WorkPlaceNumber),
                this.validator.SalaryIsCorrect(record.Salary),
                this.validator.DepartmentIsCorrect(record.Department),
                };

                bool recordIsValid = true;

                foreach (var result in validationResults)
                {
                    recordIsValid = result.Item1;
                    if (!recordIsValid)
                    {
                        Console.WriteLine($"Record #{record.Id} is invalid. {result.Item2}");
                        break;
                    }
                }

                if (recordIsValid)
                {
                    if (this.list.FindIndex(x => x.Id == record.Id) == -1)
                    {
                        this.CreateRecord(record);
                    }
                    else
                    {
                        this.EditRecord(record);
                    }
                }
            }
        }

        private static void AddRecordToDictionary(string propertyValue, FileCabinetRecord record, Dictionary<string, List<FileCabinetRecord>> dictionary)
        {
            string key = propertyValue is null ? string.Empty : propertyValue.ToUpperInvariant();
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, new List<FileCabinetRecord>());
            }

            dictionary[key].Add(record);
        }

        private static void RemoveRecordFromDictionary(string propertyValue, FileCabinetRecord record, Dictionary<string, List<FileCabinetRecord>> dictionary)
        {
            string key = propertyValue is null ? string.Empty : propertyValue.ToUpperInvariant();
            dictionary[key].Remove(record);
            if (dictionary[key].Count == 0)
            {
                dictionary.Remove(key);
            }
        }

        private void RemoveRecordFromDictionaries(FileCabinetRecord record)
        {
            RemoveRecordFromDictionary(record.FirstName, record, this.firstNameDictionary);
            RemoveRecordFromDictionary(record.LastName, record, this.lastNameDictionary);
            RemoveRecordFromDictionary(record.DateOfBirth.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture), record, this.dateOfBirthDictionary);
        }

        private void AddRecordToDictionaries(FileCabinetRecord record)
        {
            AddRecordToDictionary(record.FirstName, record, this.firstNameDictionary);
            AddRecordToDictionary(record.LastName, record, this.lastNameDictionary);
            AddRecordToDictionary(record.DateOfBirth.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture), record, this.dateOfBirthDictionary);
        }
    }
}
