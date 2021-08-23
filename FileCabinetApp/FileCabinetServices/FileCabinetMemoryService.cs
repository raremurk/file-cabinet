using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FileCabinetApp.Helpers;
using FileCabinetApp.Models;

namespace FileCabinetApp
{
    /// <summary>Class for working with records in memory.</summary>
    public class FileCabinetMemoryService : IFileCabinetService
    {
        private readonly Dictionary<string, List<FileCabinetRecord>> searchHistory = new ();
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
            _ = record ?? throw new ArgumentNullException(nameof(record));

            this.validator.ValidateRecordWithExceptions(record);
            record.Id = record.Id == 0 ? this.NextAvailableId() : record.Id;
            this.list.Add(record);
            this.searchHistory.Clear();
            return record.Id;
        }

        /// <inheritdoc cref="IFileCabinetService.EditRecord(FileCabinetRecord)"/>
        /// <exception cref="ArgumentException">Thrown when no record with the specified id.</exception>
        public void EditRecord(FileCabinetRecord record)
        {
            _ = record ?? throw new ArgumentNullException(nameof(record));
            FileCabinetRecord originalRecord = this.list.Find(x => x.Id == record.Id) ?? throw new ArgumentException("No record with this id.");

            this.validator.ValidateRecordWithExceptions(record);
            originalRecord.FirstName = record.FirstName;
            originalRecord.LastName = record.LastName;
            originalRecord.DateOfBirth = record.DateOfBirth;
            originalRecord.WorkPlaceNumber = record.WorkPlaceNumber;
            originalRecord.Salary = record.Salary;
            originalRecord.Department = record.Department;
            this.searchHistory.Clear();
        }

        /// <inheritdoc cref="IFileCabinetService.GetRecord(int)"/>
        public FileCabinetRecord GetRecord(int id)
        {
            return this.list.Find(x => x.Id == id);
        }

        /// <inheritdoc cref="IFileCabinetService.GetRecords"/>
        public IEnumerable<FileCabinetRecord> GetRecords()
        {
            foreach (var record in this.list)
            {
                yield return record;
            }
        }

        /// <inheritdoc cref="IFileCabinetService.Search(RecordToSearch)"/>
        public IEnumerable<FileCabinetRecord> Search(RecordToSearch search)
        {
            _ = search ?? throw new ArgumentNullException(nameof(search));

            if (!search.NeedToSearch())
            {
                return new List<FileCabinetRecord>();
            }

            var hash = search.GetHash();
            if (this.searchHistory.ContainsKey(hash))
            {
                return this.searchHistory[hash];
            }

            var records = this.GetRecords();
            var answer = new List<FileCabinetRecord>();
            foreach (var record in records)
            {
                if (RecordsComparer.RecordsEquals(record, search))
                {
                    answer.Add(record);
                }
            }

            this.searchHistory.Add(hash, answer);
            return answer;
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
        public FileCabinetServiceSnapshot MakeSnapshot() => new (this.list.ToArray());

        /// <inheritdoc cref="IFileCabinetService.Restore(FileCabinetServiceSnapshot)"/>
        public void Restore(FileCabinetServiceSnapshot snapshot)
        {
            _ = snapshot ?? throw new ArgumentNullException(nameof(snapshot));

            foreach (var record in snapshot.Records)
            {
                Tuple<bool, string> validationResult = this.validator.ValidateRecord(record);
                if (validationResult.Item1)
                {
                    if (!this.list.Exists(x => x.Id == record.Id))
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
                    Console.WriteLine(validationResult.Item2);
                }
            }

            this.searchHistory.Clear();
        }

        /// <inheritdoc cref="IFileCabinetService.RemoveRecord(int)"/>
        public void RemoveRecord(int id)
        {
            FileCabinetRecord record = this.list.Find(x => x.Id == id) ?? throw new ArgumentException("No record with this id.");
            this.list.Remove(record);
            this.searchHistory.Clear();
        }

        /// <inheritdoc cref="IFileCabinetService.Purge"/>
        public void Purge()
        {
            // this method shouldn't do anything
        }

        private int NextAvailableId()
        {
            int id = 1;
            while (this.list.Exists(x => x.Id == id))
            {
                id++;
            }

            return id;
        }
    }
}
