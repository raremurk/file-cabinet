using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FileCabinetApp.Helpers;
using FileCabinetApp.Models;

namespace FileCabinetApp
{
    /// <summary>Class for working with records in filesystem.</summary>
    public class FileCabinetFilesystemService : FileCabinetService
    {
        private const short Deleted = 8192;
        private const short NotDeleted = 0;
        private const int RecordSize = 280;
        private const int StringLength = 60;
        private const int Offset = 2;

        private readonly List<RecordInfo> recordsInfo = new ();
        private readonly FileStream fileStream;
        private readonly BinaryWriter writer;
        private readonly BinaryReader reader;

        /// <summary>Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class.</summary>
        /// <param name="fileStream">FileStream.</param>
        /// <param name="validator">Validator.</param>
        public FileCabinetFilesystemService(IRecordValidator validator, FileStream fileStream)
            : base(validator)
        {
            this.fileStream = fileStream ?? throw new ArgumentNullException(nameof(fileStream));
            this.writer = new (this.fileStream, System.Text.Encoding.Unicode, true);
            this.reader = new (this.fileStream, System.Text.Encoding.Unicode, true);
            this.Initialize();
        }

        /// <inheritdoc cref="IFileCabinetService.GetRecords"/>
        public override IEnumerable<FileCabinetRecord> GetRecords() => this.GetAllRecords();

        /// <inheritdoc cref="IFileCabinetService.GetStat"/>
        public override ServiceStat GetStat()
        {
            int existingRecordsCount = this.recordsInfo.Count(rec => rec.Id != 0);
            int deletedRecordsCount = this.recordsInfo.Count - existingRecordsCount;
            return new ServiceStat { AllRecordsCount = existingRecordsCount, DeletedRecordsCount = deletedRecordsCount };
        }

        /// <inheritdoc cref="IFileCabinetService.MakeSnapshot"/>
        public override FileCabinetServiceSnapshot MakeSnapshot()
        {
            var list = this.GetAllRecords().ToArray();
            return new FileCabinetServiceSnapshot(list);
        }

        /// <inheritdoc cref="IFileCabinetService.Purge"/>
        public override void Purge()
        {
            List<FileCabinetRecord> records = this.GetAllRecords().ToList();
            this.recordsInfo.Clear();
            this.fileStream.SetLength(0);
            foreach (var record in records)
            {
                this.ThisCreateRecord(record);
            }
        }

        /// <inheritdoc cref="FileCabinetService.ThisCreateRecord(FileCabinetRecord)"/>
        private protected override void ThisCreateRecord(FileCabinetRecord record)
        {
            this.SetPosition(record.Id);
            this.recordsInfo.Add(new () { Id = record.Id, RecordPosition = this.fileStream.Position });
            this.WriteRecordUsingBinaryReader(record);
        }

        /// <inheritdoc cref="FileCabinetService.ThisEditRecord(FileCabinetRecord)"/>
        private protected override void ThisEditRecord(FileCabinetRecord record)
        {
            var recordPos = this.recordsInfo.Find(x => x.Id == record.Id) ?? throw new ArgumentException($"No record with Id = '{record.Id}'.");
            this.fileStream.Seek(recordPos.RecordPosition, SeekOrigin.Begin);
            this.WriteRecordUsingBinaryReader(record);
        }

        /// <inheritdoc cref="FileCabinetService.ThisRemoveRecord(int)"/>
        private protected override void ThisRemoveRecord(int id)
        {
            int index = this.recordsInfo.FindIndex(x => x.Id == id);
            if (index == -1)
            {
                throw new ArgumentException($"No record with Id = '{id}'.");
            }

            this.recordsInfo[index].Id = 0;
            this.fileStream.Seek(this.recordsInfo[index].RecordPosition, SeekOrigin.Begin);
            this.writer.Write(Deleted);
        }

        /// <inheritdoc cref="FileCabinetService.ThisIdExists(int)"/>
        private protected override bool ThisIdExists(int id) => this.recordsInfo.Exists(x => x.Id == id);

        /// <inheritdoc cref="FileCabinetService.ThisSearch(RecordToSearch)"/>
        private protected override IEnumerable<FileCabinetRecord> ThisSearch(RecordToSearch search) => this.GetAllRecords().Where(record => RecordsComparer.RecordsEquals(record, search));

        private void SetPosition(int id)
        {
            int index = this.recordsInfo.FindIndex(x => x.Id == 0);
            if (index != -1)
            {
                this.recordsInfo[index].Id = id;
                this.fileStream.Seek(this.recordsInfo[index].RecordPosition, SeekOrigin.Begin);
            }
            else
            {
                this.fileStream.Seek(0, SeekOrigin.End);
            }
        }

        private IEnumerable<FileCabinetRecord> GetAllRecords()
        {
            foreach (var pos in this.recordsInfo)
            {
                if (pos.Id != 0)
                {
                    this.fileStream.Seek(pos.RecordPosition + Offset, SeekOrigin.Begin);
                    yield return this.ReadRecordUsingBinaryReader();
                }
            }
        }

        private void WriteRecordUsingBinaryReader(FileCabinetRecord record)
        {
            this.writer.Write(NotDeleted);
            this.writer.Write(record.Id);
            this.writer.Write(record.FirstName.PadRight(StringLength));
            this.writer.Write(record.LastName.PadRight(StringLength));
            this.writer.Write(record.DateOfBirth.Year);
            this.writer.Write(record.DateOfBirth.Month);
            this.writer.Write(record.DateOfBirth.Day);
            this.writer.Write(record.WorkPlaceNumber);
            this.writer.Write(record.Salary);
            this.writer.Write(record.Department);
        }

        private FileCabinetRecord ReadRecordUsingBinaryReader()
        {
            return new FileCabinetRecord
            {
                Id = this.reader.ReadInt32(),
                FirstName = this.reader.ReadString().TrimEnd(),
                LastName = this.reader.ReadString().TrimEnd(),
                DateOfBirth = new DateTime(this.reader.ReadInt32(), this.reader.ReadInt32(), this.reader.ReadInt32()),
                WorkPlaceNumber = this.reader.ReadInt16(),
                Salary = this.reader.ReadDecimal(),
                Department = this.reader.ReadChar(),
            };
        }

        private void Initialize()
        {
            while (this.reader.PeekChar() > -1)
            {
                long pos = this.fileStream.Position;
                bool notDeleted = this.reader.ReadInt16() == NotDeleted;
                int id = notDeleted ? this.reader.ReadInt32() : 0;
                this.recordsInfo.Add(new RecordInfo { Id = id, RecordPosition = pos });
                this.fileStream.Seek(pos + RecordSize, SeekOrigin.Begin);
            }
        }
    }
}
