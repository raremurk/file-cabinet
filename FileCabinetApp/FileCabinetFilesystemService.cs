using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;

namespace FileCabinetApp
{
    /// <summary>Class for working with records.</summary>
    public class FileCabinetFilesystemService : IFileCabinetService
    {
        private const int SizeOFRecord = 280;
        private const int Offset = 2;
        private const short NotDeleted = 0;
        private const short IsDeleted = 8192;
        private const int FirstNameStartPos = 6;
        private const int LastNameStartPos = 127;
        private const int DateOfBirthStartPos = 248;

        private readonly IRecordValidator validator;
        private readonly Func<BinaryReader, string> readNameProperty = reader => reader.ReadString().TrimEnd();
        private readonly Func<BinaryReader, string> readDateOfBirthProperty = reader => new DateTime(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
        private List<int> deletedIds = new ();
        private FileStream fileStream;
        private BinaryWriter writer;
        private BinaryReader reader;

        private int numberOfRecords;

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

        /// <summary>Creates a record and returns its id.</summary>
        /// <param name="record">Object representing a record.</param>
        /// <returns>Id of a new record.</returns>
        public int CreateRecord(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            this.validator.ValidateRecordWithExceptions(record);

            if (this.deletedIds.Count == 0)
            {
                this.fileStream.Seek(Offset, SeekOrigin.End);
                record.Id = ++this.numberOfRecords;
            }
            else
            {
                record.Id = this.deletedIds[0];
                this.deletedIds.RemoveAt(0);
                this.fileStream.Seek(SizeOFRecord * (record.Id - 1), SeekOrigin.Begin);
                this.writer.Write(NotDeleted);
            }

            this.WriteRecordUsingBinaryWriter(record);

            return this.numberOfRecords;
        }

        /// <summary>Edits a record with the specified id.</summary>
        /// <param name="record">Object representing a record.</param>
        public void EditRecord(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            this.validator.ValidateRecordWithExceptions(record);
            this.fileStream.Seek((SizeOFRecord * (record.Id - 1)) + Offset, SeekOrigin.Begin);
            this.WriteRecordUsingBinaryWriter(record);
        }

        /// <summary>Returns all records.</summary>
        /// <returns>Returns readonly collection of all records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            List<FileCabinetRecord> records = new ();

            for (int i = 0; i < this.numberOfRecords; i++)
            {
                this.fileStream.Seek(SizeOFRecord * i, SeekOrigin.Begin);
                short offset = this.reader.ReadInt16();

                if (offset != IsDeleted)
                {
                    FileCabinetRecord record = this.ReadRecordUsingBinaryReader();
                    records.Add(record);
                }
            }

            return new ReadOnlyCollection<FileCabinetRecord>(records);
        }

        /// <summary>Restores the specified snapshot.</summary>
        /// <param name="snapshot">Snapshot.</param>
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
                        this.EditRecord(record);
                    }
                }
                else
                {
                    Console.WriteLine(message);
                }
            }
        }

        /// <summary>Removes a record with the specified id.</summary>
        /// <param name="id">Id of the record to delete.</param>
        public void RemoveRecord(int id)
        {
            this.fileStream.Seek(SizeOFRecord * (id - 1), SeekOrigin.Begin);
            this.writer.Write(IsDeleted);
            this.deletedIds.Add(id);
        }

        /// <summary>Finds records by first name.</summary>
        /// <param name="firstName">First name to find.</param>
        /// <returns>Returns readonly collection of found records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            return this.SearchByProperty(firstName, FirstNameStartPos, this.readNameProperty);
        }

        /// <summary>Finds records by last name.</summary>
        /// <param name="lastName">Last name to find.</param>
        /// <returns>Returns readonly collection of found records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            return this.SearchByProperty(lastName, LastNameStartPos, this.readNameProperty);
        }

        /// <summary>Finds records by date of birth.</summary>
        /// <param name="dateOfBirth">Date of birth to find.</param>
        /// <returns>Returns readonly collection of found records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth)
        {
            return this.SearchByProperty(dateOfBirth, DateOfBirthStartPos, this.readDateOfBirthProperty);
        }

        /// <summary>Returns service statistics.</summary>
        /// <returns>Returns ServiceStat.</returns>
        public ServiceStat GetStat()
        {
            return new ServiceStat { NumberOfRecords = this.numberOfRecords, DeletedRecordsIds = new (this.deletedIds) };
        }

        /// <summary>Makes snapshot of current object state.</summary>
        /// <returns>Returns new <see cref="FileCabinetServiceSnapshot"/>.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            throw new NotImplementedException();
        }

        /// <summary>Defragments the data file.</summary>
        public void Purge()
        {
            ReadOnlyCollection<FileCabinetRecord> records = this.GetRecords();
            this.deletedIds = new ();
            this.numberOfRecords = 0;
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

        private ReadOnlyCollection<FileCabinetRecord> SearchByProperty(string propertyValue, int propertyStartPos, Func<BinaryReader, string> readProperty)
        {
            List<FileCabinetRecord> records = new ();

            for (int i = 0; i < this.numberOfRecords; i++)
            {
                int start = SizeOFRecord * i;

                this.fileStream.Seek(start, SeekOrigin.Begin);
                short offset = this.reader.ReadInt16();

                this.fileStream.Seek(propertyStartPos, SeekOrigin.Current);
                string propertyValueFromDB = readProperty(this.reader);

                if (offset != IsDeleted && string.Equals(propertyValue, propertyValueFromDB, StringComparison.OrdinalIgnoreCase))
                {
                    this.fileStream.Seek(start + Offset, SeekOrigin.Begin);
                    FileCabinetRecord record = this.ReadRecordUsingBinaryReader();
                    records.Add(record);
                }
            }

            return new ReadOnlyCollection<FileCabinetRecord>(records);
        }
    }
}
