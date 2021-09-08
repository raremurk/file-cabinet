using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FileCabinetApp.Helpers;
using FileCabinetApp.Models;

namespace FileCabinetApp
{
    /// <summary>Class for working with records in filesystem.</summary>
    public class FileCabinetFilesystemService : IFileCabinetService
    {
        private const short IsDeleted = 8192;
        private const short NotDeleted = 0;
        private const int Offset = 2;

        private readonly List<RecordInfo> recordsInfo = new ();
        private readonly IRecordValidator validator;
        private readonly FileStream fileStream;
        private readonly BinaryWriter writer;
        private readonly BinaryReader reader;

        /// <summary>Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class.</summary>
        /// <param name="fileStream">FileStream.</param>
        /// <param name="validator">Validator.</param>
        public FileCabinetFilesystemService(FileStream fileStream, IRecordValidator validator)
        {
            this.fileStream = fileStream ?? throw new ArgumentNullException(nameof(fileStream));
            this.validator = validator ?? throw new ArgumentNullException(nameof(validator));
            this.writer = new (this.fileStream, System.Text.Encoding.Unicode, true);
            this.reader = new (this.fileStream, System.Text.Encoding.Unicode, true);

            while (this.reader.PeekChar() > -1)
            {
                long pos = this.fileStream.Position;
                this.fileStream.Seek(Offset, SeekOrigin.Current);
                FileCabinetRecord record = this.ReadRecordUsingBinaryReader();
                this.recordsInfo.Add(new RecordInfo { Id = record.Id, RecordPosition = pos });
            }
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

            int index = this.recordsInfo.FindIndex(x => x.Id == 0);
            if (index != -1)
            {
                this.recordsInfo[index].Id = record.Id;
                this.fileStream.Seek(this.recordsInfo[index].RecordPosition, SeekOrigin.Begin);
            }
            else
            {
                this.fileStream.Seek(0, SeekOrigin.End);
            }

            this.WriteRecordUsingBinaryWriter(record);
            return record.Id;
        }

        /// <inheritdoc cref="IFileCabinetService.EditRecord(FileCabinetRecord)"/>
        public void EditRecord(FileCabinetRecord record)
        {
            _ = record ?? throw new ArgumentNullException(nameof(record));
            var recordPos = this.recordsInfo.Find(x => x.Id == record.Id) ?? throw new ArgumentException($"No record with Id = '{record.Id}'.");
            this.validator.ValidateRecordWithExceptions(record);

            this.fileStream.Seek(recordPos.RecordPosition, SeekOrigin.Begin);
            this.WriteRecordUsingBinaryWriter(record);
        }

        /// <inheritdoc cref="IFileCabinetService.IdExists(int)"/>
        public bool IdExists(int id) => this.recordsInfo.Exists(x => x.Id == id);

        /// <inheritdoc cref="IFileCabinetService.GetRecords"/>
        public IEnumerable<FileCabinetRecord> GetRecords() => this.GetAllRecords();

        /// <inheritdoc cref="IFileCabinetService.Search(RecordToSearch)"/>
        public IEnumerable<FileCabinetRecord> Search(RecordToSearch search)
        {
            _ = search ?? throw new ArgumentNullException(nameof(search));
            return this.GetAllRecords().Where(record => RecordsComparer.RecordsEquals(record, search));
        }

        /// <inheritdoc cref="IFileCabinetService.GetStat"/>
        public ServiceStat GetStat()
        {
            int existingRecordsCount = this.recordsInfo.Where(rec => rec.Id != 0).Count();
            int deletedRecordsCount = this.recordsInfo.Count - existingRecordsCount;
            return new ServiceStat { AllRecordsCount = existingRecordsCount, DeletedRecordsCount = deletedRecordsCount };
        }

        /// <inheritdoc cref="IFileCabinetService.MakeSnapshot"/>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            var list = this.GetAllRecords().ToArray();
            return new FileCabinetServiceSnapshot(list);
        }

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
                    if (!this.recordsInfo.Exists(x => x.Id == record.Id))
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
            int index = this.recordsInfo.FindIndex(x => x.Id == 0);
            if (index == -1)
            {
                throw new ArgumentException($"No record with Id = '{id}'.");
            }

            this.recordsInfo[index].Id = 0;
            this.fileStream.Seek(this.recordsInfo[index].RecordPosition, SeekOrigin.Begin);
            this.writer.Write(IsDeleted);
        }

        /// <inheritdoc cref="IFileCabinetService.Purge"/>
        public void Purge()
        {
            List<FileCabinetRecord> records = this.GetAllRecords().ToList();
            this.recordsInfo.Clear();
            this.fileStream.SetLength(0);
            foreach (var record in records)
            {
                this.CreateRecord(record);
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

        private void WriteRecordUsingBinaryWriter(FileCabinetRecord record)
        {
            this.writer.Write(NotDeleted);
            this.writer.Write(record.Id);
            this.writer.Write(record.FirstName.PadRight(60));
            this.writer.Write(record.LastName.PadRight(60));
            this.writer.Write(record.DateOfBirth.Year);
            this.writer.Write(record.DateOfBirth.Month);
            this.writer.Write(record.DateOfBirth.Day);
            this.writer.Write(record.WorkPlaceNumber);
            this.writer.Write(record.Salary);
            this.writer.Write(record.Department);
        }

        private int NextAvailableId()
        {
            int id = 1;
            while (this.recordsInfo.Exists(x => x.Id == id))
            {
                id++;
            }

            return id;
        }
    }
}
