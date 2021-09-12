using System;
using System.Collections.Generic;
using System.Linq;
using FileCabinetApp.Models;

namespace FileCabinetApp
{
    /// <summary>Class for working with records.</summary>
    public abstract class FileCabinetService : IFileCabinetService
    {
        private readonly Dictionary<string, List<FileCabinetRecord>> searchHistory = new ();
        private readonly IRecordValidator validator;

        /// <summary>Initializes a new instance of the <see cref="FileCabinetService"/> class.</summary>
        /// <param name="validator">Validator.</param>
        protected FileCabinetService(IRecordValidator validator)
        {
            this.validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        /// <inheritdoc cref="IFileCabinetService.CreateRecord(FileCabinetRecord)"/>
        public int CreateRecord(FileCabinetRecord record)
        {
            this.searchHistory.Clear();

            _ = record ?? throw new ArgumentNullException(nameof(record));
            this.validator.ValidateRecordWithExceptions(record);
            this.SetId(record);
            this.ThisCreateRecord(record);
            return record.Id;
        }

        /// <inheritdoc cref="IFileCabinetService.EditRecord(FileCabinetRecord)"/>
        public void EditRecord(FileCabinetRecord record)
        {
            this.searchHistory.Clear();

            _ = record ?? throw new ArgumentNullException(nameof(record));
            this.validator.ValidateRecordWithExceptions(record);
            this.ThisEditRecord(record);
        }

        /// <inheritdoc cref="IFileCabinetService.RemoveRecord(int)"/>
        public void RemoveRecord(int id)
        {
            this.searchHistory.Clear();
            this.ThisRemoveRecord(id);
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
            if (!this.searchHistory.ContainsKey(hash))
            {
                var records = this.ThisSearch(search).ToList();
                this.searchHistory.Add(hash, records);
            }

            return this.searchHistory[hash];
        }

        /// <inheritdoc cref="IFileCabinetService.Restore(FileCabinetServiceSnapshot)"/>
        public int Restore(FileCabinetServiceSnapshot snapshot)
        {
            this.searchHistory.Clear();

            _ = snapshot ?? throw new ArgumentNullException(nameof(snapshot));
            int recordsCount = 0;
            foreach (var record in snapshot.Records)
            {
                Tuple<bool, string> validationResult = this.validator.ValidateRecord(record);
                if (validationResult.Item1)
                {
                    recordsCount++;
                    if (this.ThisIdExists(record.Id))
                    {
                        this.ThisEditRecord(record);
                    }
                    else
                    {
                        this.ThisCreateRecord(record);
                    }
                }
                else
                {
                    Console.WriteLine(validationResult.Item2);
                }
            }

            return recordsCount;
        }

        /// <inheritdoc cref="IFileCabinetService.IdExists(int)"/>
        public bool IdExists(int id) => this.ThisIdExists(id);

        /// <inheritdoc cref="IFileCabinetService.GetRecords"/>
        public abstract IEnumerable<FileCabinetRecord> GetRecords();

        /// <inheritdoc cref="IFileCabinetService.GetStat"/>
        public abstract ServiceStat GetStat();

        /// <inheritdoc cref="IFileCabinetService.MakeSnapshot"/>
        public abstract FileCabinetServiceSnapshot MakeSnapshot();

        /// <inheritdoc cref="IFileCabinetService.Purge"/>
        public abstract void Purge();

        /// <summary>Creates new record.</summary>
        /// <param name="record">File cabinet record.</param>
        private protected abstract void ThisCreateRecord(FileCabinetRecord record);

        /// <summary>Edits existing record.</summary>
        /// <param name="record">File cabinet record.</param>
        private protected abstract void ThisEditRecord(FileCabinetRecord record);

        /// <inheritdoc cref="IFileCabinetService.RemoveRecord(int)"/>
        private protected abstract void ThisRemoveRecord(int id);

        /// <inheritdoc cref="IFileCabinetService.IdExists(int)"/>
        private protected abstract bool ThisIdExists(int id);

        /// <summary>Finds records by parameters.</summary>
        /// <param name="search">RecordToSearch item.</param>
        /// <returns>Returns IEnumerable of found records.</returns>
        private protected abstract IEnumerable<FileCabinetRecord> ThisSearch(RecordToSearch search);

        private void SetId(FileCabinetRecord record)
        {
            if (record.Id <= 0)
            {
                int id = 1;
                while (this.ThisIdExists(id))
                {
                    id++;
                }

                record.Id = id;
            }
        }
    }
}
