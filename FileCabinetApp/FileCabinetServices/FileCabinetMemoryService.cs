using System;
using System.Collections.Generic;
using FileCabinetApp.Helpers;
using FileCabinetApp.Models;

namespace FileCabinetApp
{
    /// <summary>Class for working with records in memory.</summary>
    public class FileCabinetMemoryService : FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new ();

        /// <summary>Initializes a new instance of the <see cref="FileCabinetMemoryService"/> class.</summary>
        /// <param name="validator">Validator.</param>
        public FileCabinetMemoryService(IRecordValidator validator)
            : base(validator)
        {
        }

        /// <inheritdoc cref="IFileCabinetService.GetRecords"/>
        public override IEnumerable<FileCabinetRecord> GetRecords() => this.list;

        /// <inheritdoc cref="IFileCabinetService.GetStat"/>
        public override ServiceStat GetStat() => new () { AllRecordsCount = this.list.Count, DeletedRecordsCount = 0 };

        /// <inheritdoc cref="IFileCabinetService.MakeSnapshot"/>
        public override FileCabinetServiceSnapshot MakeSnapshot() => new (this.list.ToArray());

        /// <inheritdoc cref="IFileCabinetService.Purge"/>
        public override void Purge()
        {
            // this method shouldn't do anything in memory service
        }

        /// <inheritdoc cref="FileCabinetService.ThisCreateRecord(FileCabinetRecord)"/>
        private protected override void ThisCreateRecord(FileCabinetRecord record) => this.list.Add(record);

        /// <inheritdoc cref="FileCabinetService.ThisEditRecord(FileCabinetRecord)"/>
        private protected override void ThisEditRecord(FileCabinetRecord record)
        {
            int index = this.list.FindIndex(x => x.Id == record.Id);
            if (index == -1)
            {
                throw new ArgumentException($"No record with Id = '{record.Id}'.");
            }

            this.list[index] = record;
        }

        /// <inheritdoc cref="FileCabinetService.ThisRemoveRecord(int)"/>
        private protected override void ThisRemoveRecord(int id)
        {
            FileCabinetRecord record = this.list.Find(x => x.Id == id) ?? throw new ArgumentException($"No record with Id = '{id}'.");
            this.list.Remove(record);
        }

        /// <inheritdoc cref="FileCabinetService.ThisIdExists(int)"/>
        private protected override bool ThisIdExists(int id) => this.list.Exists(x => x.Id == id);

        /// <inheritdoc cref="FileCabinetService.ThisSearch(RecordToSearch)"/>
        private protected override IEnumerable<FileCabinetRecord> ThisSearch(RecordToSearch search) => this.list.FindAll(record => RecordsComparer.RecordsEquals(record, search));
    }
}
