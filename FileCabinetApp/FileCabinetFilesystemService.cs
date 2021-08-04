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
        private const int FirstNameStartPos = 6;
        private const int LastNameStartPos = 127;
        private const int NameSize = 121;
        private const int DateOfBirthStartPos = 248;
        private const int DateOfBirthSize = 12;

        private readonly FileStream fileStream;
        private readonly BinaryWriter writer;
        private readonly BinaryReader reader;
        private readonly IRecordValidator validator;
        private readonly Func<BinaryReader, string> readNameProperty = reader => reader.ReadString().TrimEnd();
        private readonly Func<BinaryReader, string> readDateOfBirthProperty = reader => new DateTime(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);

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
            this.numberOfRecords = 0;
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

            this.validator.ValidateParameters(record);
            this.fileStream.Seek(Offset, SeekOrigin.End);
            record.Id = ++this.numberOfRecords;
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

            this.validator.ValidateParameters(record);
            this.fileStream.Seek((SizeOFRecord * (record.Id - 1)) + Offset, SeekOrigin.Begin);
            this.WriteRecordUsingBinaryWriter(record);
        }

        /// <summary>Finds records by first name.</summary>
        /// <param name="firstName">First name to find.</param>
        /// <returns>Returns readonly collection of found records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            return this.SearchByProperty(firstName, NameSize, FirstNameStartPos, this.readNameProperty);
        }

        /// <summary>Finds records by last name.</summary>
        /// <param name="lastName">Last name to find.</param>
        /// <returns>Returns readonly collection of found records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            return this.SearchByProperty(lastName, NameSize, LastNameStartPos, this.readNameProperty);
        }

        /// <summary>Finds records by date of birth.</summary>
        /// <param name="dateOfBirth">Date of birth to find.</param>
        /// <returns>Returns readonly collection of found records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth)
        {
            return this.SearchByProperty(dateOfBirth, DateOfBirthSize, DateOfBirthStartPos, this.readDateOfBirthProperty);
        }

        /// <summary>Returns all records.</summary>
        /// <returns>Returns readonly collection of all records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            List<FileCabinetRecord> records = new ();
            this.fileStream.Seek(Offset, SeekOrigin.Begin);

            while (this.reader.PeekChar() > -1)
            {
                FileCabinetRecord record = this.ReadRecordUsingBinaryReader();
                records.Add(record);
                this.fileStream.Seek(Offset, SeekOrigin.Current);
            }

            return new ReadOnlyCollection<FileCabinetRecord>(records);
        }

        /// <summary>Returns number of records.</summary>
        /// <returns>Returns number.</returns>
        public int GetStat() => this.numberOfRecords;

        /// <summary>Makes snapshot of current object state.</summary>
        /// <returns>Returns new <see cref="FileCabinetServiceSnapshot"/>.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            throw new NotImplementedException();
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
                Tuple<bool, string>[] validationResults =
                {
                this.validator.NameIsCorrect(record.FirstName),
                this.validator.NameIsCorrect(record.LastName),
                this.validator.DateOfBirthIsCorrect(record.DateOfBirth),
                this.validator.WorkPlaceNumberIsCorrect(record.WorkPlaceNumber),
                this.validator.SalaryIsCorrect(record.Salary),
                this.validator.DepartmentIsCorrect(record.Department),
                };

                bool recordIsValid = true;

                foreach (var result in validationResults)
                {
                    recordIsValid = result.Item1;
                    if (!recordIsValid)
                    {
                        Console.WriteLine($"Record #{record.Id} is invalid. {result.Item2}");
                        break;
                    }
                }

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

        private ReadOnlyCollection<FileCabinetRecord> SearchByProperty(string propertyValue, int propertySize, int propertyStartPos, Func<BinaryReader, string> readProperty)
        {
            List<FileCabinetRecord> records = new ();

            this.fileStream.Seek(propertyStartPos, SeekOrigin.Begin);

            for (int i = 0; i < this.numberOfRecords; i++)
            {
                string propertyValueFromDB = readProperty(this.reader);
                this.fileStream.Seek(-propertySize, SeekOrigin.Current);

                if (string.Equals(propertyValue, propertyValueFromDB, StringComparison.OrdinalIgnoreCase))
                {
                    this.fileStream.Seek(-(propertyStartPos - Offset), SeekOrigin.Current);
                    FileCabinetRecord record = this.ReadRecordUsingBinaryReader();
                    records.Add(record);
                    this.fileStream.Seek(propertyStartPos, SeekOrigin.Current);
                }
                else
                {
                    this.fileStream.Seek(SizeOFRecord, SeekOrigin.Current);
                }
            }

            return new ReadOnlyCollection<FileCabinetRecord>(records);
        }
    }
}
