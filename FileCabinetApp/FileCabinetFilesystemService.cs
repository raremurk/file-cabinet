using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;

namespace FileCabinetApp
{
    /// <summary>Class for working with records in filesystem.</summary>
    public class FileCabinetFilesystemService : IFileCabinetService
    {
        private const int SizeOFRecord = 280;
        private const int Offset = 2;
        private const short NotDeleted = 0;
        private const short IsDeleted = 8192;

        private readonly Dictionary<string, List<int>> firstNameDictionary = new ();
        private readonly Dictionary<string, List<int>> lastNameDictionary = new ();
        private readonly Dictionary<string, List<int>> dateOfBirthDictionary = new ();
        private readonly IRecordValidator validator;
        private List<Tuple<int, long>> recordPositions = new ();
        private FileStream fileStream;
        private BinaryWriter writer;
        private BinaryReader reader;

        /// <summary>Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class.</summary>
        /// <param name="fileStream">FileStream.</param>
        /// <param name="validator">Validator.</param>
        public FileCabinetFilesystemService(FileStream fileStream, IRecordValidator validator)
        {
            this.fileStream = fileStream;
            this.validator = validator;
            this.writer = new (this.fileStream, System.Text.Encoding.Unicode, true);
            this.reader = new (this.fileStream, System.Text.Encoding.Unicode, true);
        }

        /// <inheritdoc cref="IFileCabinetService.CreateRecord(FileCabinetRecord)"/>
        public int CreateRecord(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            this.validator.ValidateRecordWithExceptions(record);

            var deletedRecord = this.recordPositions.Find(x => x.Item1 == 0);
            record.Id = record.Id == 0 ? this.NextAvailableId() : record.Id;

            if (!(deletedRecord is null))
            {
                this.fileStream.Seek(deletedRecord.Item2, SeekOrigin.Begin);
                this.writer.Write(NotDeleted);
                this.recordPositions.Remove(deletedRecord);
            }
            else
            {
                this.fileStream.Seek(Offset, SeekOrigin.End);
            }

            this.recordPositions.Add(new (record.Id, this.fileStream.Position - Offset));
            this.WriteRecordUsingBinaryWriter(record);
            this.AddRecordToDictionaries(record);
            return record.Id;
        }

        /// <inheritdoc cref="IFileCabinetService.EditRecords(ReadOnlyCollection{FileCabinetRecord})"/>
        public void EditRecords(ReadOnlyCollection<FileCabinetRecord> records)
        {
            if (records is null)
            {
                throw new ArgumentNullException(nameof(records));
            }

            foreach (var record in records)
            {
                var recordPos = this.recordPositions.Find(x => x.Item1 == record.Id);
                if (recordPos != null)
                {
                    this.validator.ValidateRecordWithExceptions(record);
                    this.fileStream.Seek(recordPos.Item2 + Offset, SeekOrigin.Begin);
                    FileCabinetRecord originalRecord = this.ReadRecordUsingBinaryReader();
                    this.RemoveRecordFromDictionaries(originalRecord);
                    this.WriteRecordUsingBinaryWriter(record);
                    this.AddRecordToDictionaries(record);
                }
            }
        }

        /// <inheritdoc cref="IFileCabinetService.GetRecord(int)"/>
        public FileCabinetRecord GetRecord(int id)
        {
            var recordPos = this.recordPositions.Find(x => x.Item1 == id);
            if (recordPos != null)
            {
                this.fileStream.Seek(recordPos.Item2 + Offset, SeekOrigin.Begin);
                return this.ReadRecordUsingBinaryReader();
            }

            return null;
        }

        /// <inheritdoc cref="IFileCabinetService.FindByFirstName(string)"/>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName) => this.SearchByProperty(firstName, this.firstNameDictionary);

        /// <inheritdoc cref="IFileCabinetService.FindByLastName(string)"/>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName) => this.SearchByProperty(lastName, this.lastNameDictionary);

        /// <inheritdoc cref="IFileCabinetService.FindByDateOfBirth(string)"/>
        public IEnumerable<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth) => this.SearchByProperty(dateOfBirth, this.dateOfBirthDictionary);

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

        /// <inheritdoc cref="IFileCabinetService.IdExists(int)"/>
        public bool IdExists(int id)
        {
            return !(this.recordPositions.Find(x => x.Item1 == id) is null);
        }

        /// <inheritdoc cref="IFileCabinetService.GetStat"/>
        public ServiceStat GetStat()
        {
            List<int> existingRecordsIds = new ();
            List<int> deletedRecordsIds = new ();
            foreach (var item in this.recordPositions)
            {
                if (item.Item1 != 0)
                {
                    existingRecordsIds.Add(item.Item1);
                }
                else
                {
                    deletedRecordsIds.Add(item.Item1);
                }
            }

            return new ServiceStat { ExistingRecordsIds = new (existingRecordsIds), DeletedRecordsIds = new (deletedRecordsIds) };
        }

        /// <inheritdoc cref="IFileCabinetService.MakeSnapshot"/>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc cref="IFileCabinetService.Restore(FileCabinetServiceSnapshot)"/>
        public void Restore(FileCabinetServiceSnapshot snapshot)
        {
            if (snapshot == null)
            {
                throw new ArgumentNullException(nameof(snapshot));
            }

            List<FileCabinetRecord> records = new (this.GetRecords());

            ReadOnlyCollection<FileCabinetRecord> unverifiedRecords = snapshot.Records;
            foreach (var record in unverifiedRecords)
            {
                Tuple<bool, string> validationResult = this.validator.ValidateRecord(record);
                bool recordIsValid = validationResult.Item1;
                string message = validationResult.Item2;

                if (recordIsValid)
                {
                    if (records.FindIndex(x => x.Id == record.Id) == -1)
                    {
                        this.CreateRecord(record);
                    }
                    else
                    {
                        List<FileCabinetRecord> list = new ();
                        list.Add(record);
                        this.EditRecords(new (list));
                    }
                }
                else
                {
                    Console.WriteLine(message);
                }
            }
        }

        /// <inheritdoc cref="IFileCabinetService.RemoveRecords(ReadOnlyCollection{int})"/>
        public void RemoveRecords(ReadOnlyCollection<int> ids)
        {
            if (ids is null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            foreach (int id in ids)
            {
                var recordPos = this.recordPositions.Find(x => x.Item1 == id);
                this.recordPositions.Add(new (0, recordPos.Item2));
                this.recordPositions.Remove(recordPos);

                this.fileStream.Seek(recordPos.Item2, SeekOrigin.Begin);
                this.writer.Write(IsDeleted);
                FileCabinetRecord record = this.ReadRecordUsingBinaryReader();
                this.RemoveRecordFromDictionaries(record);
            }
        }

        /// <inheritdoc cref="IFileCabinetService.Purge"/>
        public void Purge()
        {
            List<FileCabinetRecord> records = new (this.GetRecords());
            this.recordPositions = new ();
            string fileName = this.fileStream.Name;
            this.fileStream.Dispose();
            this.fileStream = new (fileName, FileMode.Create);
            this.writer = new (this.fileStream, System.Text.Encoding.Unicode, true);
            this.reader = new (this.fileStream, System.Text.Encoding.Unicode, true);

            foreach (var record in records)
            {
                this.CreateRecord(record);
            }
        }

        private static void AddRecordToDictionary(string propertyValue, int recordPosition, Dictionary<string, List<int>> dictionary)
        {
            string key = propertyValue.ToUpperInvariant();
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, new List<int>());
            }

            dictionary[key].Add(recordPosition);
        }

        private static void RemoveRecordFromDictionary(string propertyValue, int recordPosition, Dictionary<string, List<int>> dictionary)
        {
            string key = propertyValue.ToUpperInvariant();
            dictionary[key].Remove(recordPosition);
            if (dictionary[key].Count == 0)
            {
                dictionary.Remove(key);
            }
        }

        private void RemoveRecordFromDictionaries(FileCabinetRecord record)
        {
            RemoveRecordFromDictionary(record.FirstName, record.Id, this.firstNameDictionary);
            RemoveRecordFromDictionary(record.LastName, record.Id, this.lastNameDictionary);
            RemoveRecordFromDictionary(record.DateOfBirth.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture), record.Id, this.dateOfBirthDictionary);
        }

        private void AddRecordToDictionaries(FileCabinetRecord record)
        {
            AddRecordToDictionary(record.FirstName, record.Id, this.firstNameDictionary);
            AddRecordToDictionary(record.LastName, record.Id, this.lastNameDictionary);
            AddRecordToDictionary(record.DateOfBirth.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture), record.Id, this.dateOfBirthDictionary);
        }

        private IEnumerable<FileCabinetRecord> SearchByProperty(string propertyValue, Dictionary<string, List<int>> dictionary)
        {
            string key = propertyValue is null ? string.Empty : propertyValue.ToUpperInvariant();
            var recordsPositions = dictionary.ContainsKey(key) ? dictionary[key] : new List<int>();
            foreach (int pos in recordsPositions)
            {
                this.fileStream.Seek((SizeOFRecord * (pos - 1)) + Offset, SeekOrigin.Begin);
                yield return this.ReadRecordUsingBinaryReader();
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
            var existingRecords = this.GetStat().ExistingRecordsIds;
            int id = 0;
            while (true)
            {
                if (!existingRecords.Contains(++id))
                {
                    return id;
                }
            }
        }
    }
}
