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

        private readonly Dictionary<string, List<FileCabinetRecord>> searchHistory = new ();
        private readonly List<Tuple<int, long>> recordPositions = new ();
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
                this.recordPositions.Add(new (record.Id, pos));
            }
        }

        /// <inheritdoc cref="IFileCabinetService.CreateRecord(FileCabinetRecord)"/>
        public int CreateRecord(FileCabinetRecord record)
        {
            _ = record ?? throw new ArgumentNullException(nameof(record));

            this.validator.ValidateRecordWithExceptions(record);
            var deletedRecord = this.recordPositions.Find(x => x.Item1 == 0);
            if (deletedRecord != null)
            {
                this.fileStream.Seek(deletedRecord.Item2, SeekOrigin.Begin);
                this.writer.Write(NotDeleted);
                this.recordPositions.Remove(deletedRecord);
            }
            else
            {
                this.fileStream.Seek(Offset, SeekOrigin.End);
            }

            record.Id = record.Id == 0 ? this.NextAvailableId() : record.Id;
            this.recordPositions.Add(new (record.Id, this.fileStream.Position - Offset));
            this.WriteRecordUsingBinaryWriter(record);
            this.searchHistory.Clear();
            return record.Id;
        }

        /// <inheritdoc cref="IFileCabinetService.EditRecord(FileCabinetRecord)"/>
        public void EditRecord(FileCabinetRecord record)
        {
            _ = record ?? throw new ArgumentNullException(nameof(record));
            var recordPos = this.recordPositions.Find(x => x.Item1 == record.Id) ?? throw new ArgumentException("No record with this id.");

            this.validator.ValidateRecordWithExceptions(record);
            this.fileStream.Seek(recordPos.Item2 + Offset, SeekOrigin.Begin);
            this.WriteRecordUsingBinaryWriter(record);
            this.searchHistory.Clear();
        }

        /// <inheritdoc cref="IFileCabinetService.IdExists(int)"/>
        public bool IdExists(int id) => this.recordPositions.Exists(x => x.Item1 == id);

        /// <inheritdoc cref="IFileCabinetService.GetRecords"/>
        public IEnumerable<FileCabinetRecord> GetRecords()
        {
            foreach (var pos in this.recordPositions)
            {
                if (pos.Item1 != 0)
                {
                    this.fileStream.Seek(pos.Item2 + Offset, SeekOrigin.Begin);
                    yield return this.ReadRecordUsingBinaryReader();
                }
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

            var answer = new List<FileCabinetRecord>();
            foreach (var pos in this.recordPositions)
            {
                this.fileStream.Seek(pos.Item2 + Offset, SeekOrigin.Begin);
                FileCabinetRecord record = this.ReadRecordUsingBinaryReader();
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
            int existingRecordsCount = 0;
            int deletedRecordsCount = 0;
            foreach (var pos in this.recordPositions)
            {
                _ = pos.Item1 != 0 ? existingRecordsCount++ : deletedRecordsCount++;
            }

            return new ServiceStat { AllRecordsCount = existingRecordsCount, DeletedRecordsCount = deletedRecordsCount };
        }

        /// <inheritdoc cref="IFileCabinetService.MakeSnapshot"/>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            var list = this.GetRecords().ToArray();
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
                    if (!this.recordPositions.Exists(x => x.Item1 == record.Id))
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
            return recordsCount;
        }

        /// <inheritdoc cref="IFileCabinetService.RemoveRecord(int)"/>
        public void RemoveRecord(int id)
        {
            var recordPos = this.recordPositions.Find(x => x.Item1 == id) ?? throw new ArgumentException("No record with this id.");
            this.fileStream.Seek(recordPos.Item2, SeekOrigin.Begin);
            this.writer.Write(IsDeleted);
            this.recordPositions.Add(new (0, recordPos.Item2));
            this.recordPositions.Remove(recordPos);
            this.searchHistory.Clear();
        }

        /// <inheritdoc cref="IFileCabinetService.Purge"/>
        public void Purge()
        {
            List<FileCabinetRecord> records = new (this.GetRecords());
            this.recordPositions.Clear();
            this.fileStream.SetLength(0);
            foreach (var record in records)
            {
                this.CreateRecord(record);
            }
        }

        private FileCabinetRecord ReadRecordUsingBinaryReader()
        {
            var record = new FileCabinetRecord
            {
                Id = this.reader.ReadInt32(),
                FirstName = this.reader.ReadString().TrimEnd(),
                LastName = this.reader.ReadString().TrimEnd(),
                DateOfBirth = new DateTime(this.reader.ReadInt32(), this.reader.ReadInt32(), this.reader.ReadInt32()),
                WorkPlaceNumber = this.reader.ReadInt16(),
                Salary = this.reader.ReadDecimal(),
                Department = this.reader.ReadChar(),
            };

            return record;
        }

        private void WriteRecordUsingBinaryWriter(FileCabinetRecord record)
        {
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
            while (this.recordPositions.Exists(x => x.Item1 == id))
            {
                id++;
            }

            return id;
        }
    }
}
