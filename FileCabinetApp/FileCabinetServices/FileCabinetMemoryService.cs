using System;
using System.Collections.Generic;
using FileCabinetApp.Helpers;
using FileCabinetApp.Models;

namespace FileCabinetApp
{
    /// <summary>Class for working with records in memory.</summary>
    public class FileCabinetMemoryService : IFileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new ();
        private readonly IRecordValidator validator;

        /// <summary>Initializes a new instance of the <see cref="FileCabinetMemoryService"/> class.</summary>
        /// <param name="validator">Validator.</param>
        public FileCabinetMemoryService(IRecordValidator validator)
        {
            this.validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        /// <inheritdoc cref="IFileCabinetService.CreateRecord(FileCabinetRecord)"/>
        public int CreateRecord(FileCabinetRecord record)
        {
            _ = record ?? throw new ArgumentNullException(nameof(record));
            this.validator.ValidateRecordWithExceptions(record);

            if (record.Id == 0)
            {
                record.Id = this.NextAvailableId();
            }

            this.list.Add(record);
            return record.Id;
        }

        /// <inheritdoc cref="IFileCabinetService.EditRecord(FileCabinetRecord)"/>
        public void EditRecord(FileCabinetRecord record)
        {
            _ = record ?? throw new ArgumentNullException(nameof(record));
            int index = this.list.FindIndex(x => x.Id == record.Id);
            if (index == -1)
            {
                throw new ArgumentException($"No record with Id = '{record.Id}'.");
            }

            this.validator.ValidateRecordWithExceptions(record);
            this.list[index] = record;
        }

        /// <inheritdoc cref="IFileCabinetService.IdExists(int)"/>
        public bool IdExists(int id) => this.list.Exists(x => x.Id == id);

        /// <inheritdoc cref="IFileCabinetService.GetRecords"/>
        public IEnumerable<FileCabinetRecord> GetRecords() => this.list;

        /// <inheritdoc cref="IFileCabinetService.Search(RecordToSearch)"/>
        public IEnumerable<FileCabinetRecord> Search(RecordToSearch search) => this.list.FindAll(record => RecordsComparer.RecordsEquals(record, search));

        /// <inheritdoc cref="IFileCabinetService.GetStat"/>
        public ServiceStat GetStat() => new () { AllRecordsCount = this.list.Count, DeletedRecordsCount = 0 };

        /// <inheritdoc cref="IFileCabinetService.MakeSnapshot"/>
        public FileCabinetServiceSnapshot MakeSnapshot() => new (this.list.ToArray());

        /// <inheritdoc cref="IFileCabinetService.Restore(FileCabinetServiceSnapshot)"/>
        public int Restore(FileCabinetServiceSnapshot snapshot)
        {
            _ = snapshot ?? throw new ArgumentNullException(nameof(snapshot));

            int recordsCount = 0;
            foreach (var record in snapshot.Records)
            {
                Tuple<bool, string> validationResult = this.validator.ValidateRecord(record);
                if (validationResult.Item1)
                {
                    recordsCount++;
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

            return recordsCount;
        }

        /// <inheritdoc cref="IFileCabinetService.RemoveRecord(int)"/>
        public void RemoveRecord(int id)
        {
            FileCabinetRecord record = this.list.Find(x => x.Id == id) ?? throw new ArgumentException("No record with this id.");
            this.list.Remove(record);
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
