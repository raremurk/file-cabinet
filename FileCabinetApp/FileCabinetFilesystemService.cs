using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace FileCabinetApp
{
    /// <summary>Class for working with records.</summary>
    public class FileCabinetFilesystemService : IFileCabinetService
    {
        private const int Offset = 2;
        private readonly FileStream fileStream;
        private readonly IRecordValidator validator;
        private int numberOfRecords;

        /// <summary>Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class.</summary>
        /// <param name="fileStream">FileStream.</param>
        /// <param name="validator">Validator.</param>
        public FileCabinetFilesystemService(FileStream fileStream, IRecordValidator validator)
        {
            this.fileStream = fileStream;
            this.validator = validator;
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
            using BinaryWriter writer = new (this.fileStream, System.Text.Encoding.UTF8, true);

            writer.Seek(Offset, SeekOrigin.End);
            writer.Write(++this.numberOfRecords);
            writer.Write(record.FirstName.PadRight(60));
            writer.Write(record.LastName.PadRight(60));
            writer.Write(record.DateOfBirth.Year);
            writer.Write(record.DateOfBirth.Month);
            writer.Write(record.DateOfBirth.Day);
            writer.Write(record.WorkPlaceNumber);
            writer.Write(record.Salary);
            writer.Write(record.Department);

            return this.numberOfRecords;
        }

        /// <summary>Edits a record with the specified id.</summary>
        /// <param name="record">Object representing a record.</param>
        public void EditRecord(FileCabinetRecord record)
        {
            throw new NotImplementedException();
        }

        /// <summary>Finds records by first name.</summary>
        /// <param name="firstName">First name to find.</param>
        /// <returns>Returns readonly collection of found records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            throw new NotImplementedException();
        }

        /// <summary>Finds records by last name.</summary>
        /// <param name="lastName">Last name to find.</param>
        /// <returns>Returns readonly collection of found records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            throw new NotImplementedException();
        }

        /// <summary>Finds records by date of birth.</summary>
        /// <param name="dateOfBirth">Date of birth to find.</param>
        /// <returns>Returns readonly collection of found records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth)
        {
            throw new NotImplementedException();
        }

        /// <summary>Returns all records.</summary>
        /// <returns>Returns readonly collection of all records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            this.fileStream.Seek(Offset, SeekOrigin.Begin);
            using BinaryReader reader = new (this.fileStream, System.Text.Encoding.UTF8, true);
            List<FileCabinetRecord> records = new ();

            while (reader.PeekChar() > -1)
            {
                var record = new FileCabinetRecord
                {
                    Id = reader.ReadInt32(),
                    FirstName = reader.ReadString().TrimEnd(),
                    LastName = reader.ReadString().TrimEnd(),
                    DateOfBirth = new DateTime(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32()),
                    WorkPlaceNumber = reader.ReadInt16(),
                    Salary = reader.ReadDecimal(),
                    Department = reader.ReadChar(),
                };

                this.fileStream.Seek(Offset, SeekOrigin.Current);
                records.Add(record);
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
    }
}
