using System;
using System.Collections.Generic;
using System.Linq;
using FileCabinetApp.Models;

namespace FileCabinetApp
{
    /// <summary>Search cache.</summary>
    /// <seealso cref="IFileCabinetService" />
    public class SearchCache : IFileCabinetService
    {
        private readonly Dictionary<string, List<FileCabinetRecord>> searchHistory = new ();
        private readonly IFileCabinetService service;

        /// <summary>Initializes a new instance of the <see cref="SearchCache"/> class.</summary>
        /// <param name="service">IFileCabinetService.</param>
        /// <exception cref="ArgumentNullException">Thrown when service is null.</exception>
        public SearchCache(IFileCabinetService service)
        {
            this.service = service ?? throw new ArgumentNullException(nameof(service));
        }

        /// <inheritdoc cref="IFileCabinetService.CreateRecord(FileCabinetRecord)"/>
        public int CreateRecord(FileCabinetRecord record)
        {
            this.searchHistory.Clear();
            return this.service.CreateRecord(record);
        }

        /// <inheritdoc cref="IFileCabinetService.EditRecord(FileCabinetRecord)"/>
        public void EditRecord(FileCabinetRecord record)
        {
            this.searchHistory.Clear();
            this.service.EditRecord(record);
        }

        /// <inheritdoc cref="IFileCabinetService.IdExists(int)"/>
        public bool IdExists(int id) => this.service.IdExists(id);

        /// <inheritdoc cref="IFileCabinetService.GetRecords"/>
        public IEnumerable<FileCabinetRecord> GetRecords() => this.service.GetRecords();

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
                var records = this.service.Search(search).ToList();
                this.searchHistory.Add(hash, records);
            }

            return this.searchHistory[hash];
        }

        /// <inheritdoc cref="IFileCabinetService.GetStat"/>
        public ServiceStat GetStat() => this.service.GetStat();

        /// <inheritdoc cref="IFileCabinetService.MakeSnapshot"/>
        public FileCabinetServiceSnapshot MakeSnapshot() => this.service.MakeSnapshot();

        /// <inheritdoc cref="IFileCabinetService.Restore(FileCabinetServiceSnapshot)"/>
        public int Restore(FileCabinetServiceSnapshot snapshot)
        {
            this.searchHistory.Clear();
            return this.service.Restore(snapshot);
        }

        /// <inheritdoc cref="IFileCabinetService.RemoveRecord(int)"/>
        public void RemoveRecord(int id)
        {
            this.searchHistory.Clear();
            this.service.RemoveRecord(id);
        }

        /// <inheritdoc cref="IFileCabinetService.Purge"/>
        public void Purge() => this.service.Purge();
    }
}