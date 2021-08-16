using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>Class for working with records in memory.</summary>
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

        /// <inheritdoc cref="IFileCabinetService.CreateRecord(FileCabinetRecord)"/>
        public int CreateRecord(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            this.validator.ValidateRecordWithExceptions(record);
            record.Id = record.Id == 0 ? this.NextAvailableId() : record.Id;
            this.list.Add(record);
            this.AddRecordToDictionaries(record);
            return record.Id;
        }

        /// <inheritdoc cref="IFileCabinetService.EditRecord(FileCabinetRecord)"/>
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

            this.validator.ValidateRecordWithExceptions(record);
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

        /// <inheritdoc cref="IFileCabinetService.FindByFirstName(string)"/>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            string firstNameKey = firstName is null ? string.Empty : firstName.ToUpperInvariant();
            var records = this.firstNameDictionary.ContainsKey(firstNameKey) ? this.firstNameDictionary[firstNameKey] : new ();
            foreach (var record in records)
            {
                yield return record;
            }
        }

        /// <inheritdoc cref="IFileCabinetService.FindByLastName(string)"/>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            string lastNameKey = lastName is null ? string.Empty : lastName.ToUpperInvariant();
            var records = this.lastNameDictionary.ContainsKey(lastNameKey) ? this.lastNameDictionary[lastNameKey] : new ();
            foreach (var record in records)
            {
                yield return record;
            }
        }

        /// <inheritdoc cref="IFileCabinetService.FindByDateOfBirth(string)"/>
        public IEnumerable<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth)
        {
            var records = this.dateOfBirthDictionary.ContainsKey(dateOfBirth) ? this.dateOfBirthDictionary[dateOfBirth] : new ();
            foreach (var record in records)
            {
                yield return record;
            }
        }

        /// <inheritdoc cref="IFileCabinetService.GetRecords"/>
        public IEnumerable<FileCabinetRecord> GetRecords()
        {
            foreach (var record in this.list)
            {
                yield return record;
            }
        }

        /// <inheritdoc cref="IFileCabinetService.IdExists(int)"/>
        public bool IdExists(int id)
        {
            return this.list.Exists(x => x.Id == id);
        }

        /// <inheritdoc cref="IFileCabinetService.GetStat"/>
        public ServiceStat GetStat()
        {
            List<int> existingRecordsIds = new ();
            foreach (var record in this.list)
            {
                existingRecordsIds.Add(record.Id);
            }

            return new ServiceStat { ExistingRecordsIds = new (existingRecordsIds), DeletedRecordsIds = new ReadOnlyCollection<int>(Array.Empty<int>()) };
        }

        /// <inheritdoc cref="IFileCabinetService.MakeSnapshot"/>
        public FileCabinetServiceSnapshot MakeSnapshot() => new (new ReadOnlyCollection<FileCabinetRecord>(this.list));

        /// <inheritdoc cref="IFileCabinetService.Restore(FileCabinetServiceSnapshot)"/>
        public void Restore(FileCabinetServiceSnapshot snapshot)
        {
            if (snapshot == null)
            {
                throw new ArgumentNullException(nameof(snapshot));
            }

            ReadOnlyCollection<FileCabinetRecord> unverifiedRecords = snapshot.Records;
            foreach (var record in unverifiedRecords)
            {
                Tuple<bool, string> validationResult = this.validator.ValidateRecord(record);
                bool recordIsValid = validationResult.Item1;
                string message = validationResult.Item2;

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
                else
                {
                    Console.WriteLine(message);
                }
            }
        }

        /// <inheritdoc cref="IFileCabinetService.RemoveRecord(int)"/>
        /// <exception cref="ArgumentException">Thrown when no record with the specified id.</exception>
        public void RemoveRecord(int id)
        {
            if (!this.list.Exists(x => x.Id == id))
            {
                throw new ArgumentException("No record with this id.");
            }

            FileCabinetRecord record = this.list.Find(x => x.Id == id);
            this.list.Remove(record);
            this.RemoveRecordFromDictionaries(record);
        }

        /// <inheritdoc cref="IFileCabinetService.Purge"/>
        public void Purge()
        {
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

        private int NextAvailableId()
        {
            int id = 1;
            while (true)
            {
                if (!this.list.Exists(x => x.Id == ++id))
                {
                    return id;
                }
            }
        }
    }
}
