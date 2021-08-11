using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>Service logger.</summary>
    /// <seealso cref="IFileCabinetService" />
    public class ServiceLogger : IFileCabinetService
    {
        private readonly IFileCabinetService service;
        private readonly string logFileName = "log.txt";

        /// <summary>Initializes a new instance of the <see cref="ServiceLogger"/> class.</summary>
        /// <param name="service">IFileCabinetService.</param>
        /// <exception cref="ArgumentNullException">Thrown when service is null.</exception>
        public ServiceLogger(IFileCabinetService service)
        {
            this.service = service ?? throw new ArgumentNullException(nameof(service));
        }

        /// <summary>Creates a record and returns its id.</summary>
        /// <param name="record">Object representing a record.</param>
        /// <returns>Id of a new record.</returns>
        public int CreateRecord(FileCabinetRecord record)
        {
            using var writer = new StreamWriter(this.logFileName, true, Encoding.UTF8);

            try
            {
                WriteOperationLog(writer, $"Calling CreateRecord() with {RecordToString(record, false)}");
                int id = this.service.CreateRecord(record);
                WriteOperationLog(writer, $"CreateRecord() returned '{id}'");
                return id;
            }
            catch (Exception ex)
            {
                if (ex is ArgumentNullException || ex is ArgumentException)
                {
                    WriteOperationLog(writer, $"CreateRecord() threw an exception: {ex.Message}");
                }

                throw;
            }
        }

        /// <summary>Edits a record with the specified id.</summary>
        /// <param name="record">Object representing a record.</param>
        public void EditRecord(FileCabinetRecord record)
        {
            using var writer = new StreamWriter(this.logFileName, true, Encoding.UTF8);

            try
            {
                WriteOperationLog(writer, $"Calling EditRecord() with {RecordToString(record, true)}");
                this.service.EditRecord(record);
                WriteOperationLog(writer, "EditRecord() executed successfully");
            }
            catch (Exception ex)
            {
                if (ex is ArgumentNullException || ex is ArgumentException)
                {
                    WriteOperationLog(writer, $"EditRecord() threw an exception: {ex.Message}");
                }

                throw;
            }
        }

        /// <summary>Finds records by first name.</summary>
        /// <param name="firstName">First name to find.</param>
        /// <returns>Returns readonly collection of found records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            using var writer = new StreamWriter(this.logFileName, true, Encoding.UTF8);
            WriteOperationLog(writer, $"Calling FindByFirstName() with firstName = '{firstName}'");
            var result = this.service.FindByFirstName(firstName);
            WriteOperationLog(writer, $"FindByFirstName() returned {result.Count} record(s)");
            return result;
        }

        /// <summary>Finds records by last name.</summary>
        /// <param name="lastName">Last name to find.</param>
        /// <returns>Returns readonly collection of found records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            using var writer = new StreamWriter(this.logFileName, true, Encoding.UTF8);
            WriteOperationLog(writer, $"Calling FindByLastName() with lastName = '{lastName}'");
            var result = this.service.FindByLastName(lastName);
            WriteOperationLog(writer, $"FindByLastName() returned {result.Count} record(s)");
            return result;
        }

        /// <summary>Finds records by date of birth.</summary>
        /// <param name="dateOfBirth">Date of birth to find.</param>
        /// <returns>Returns readonly collection of found records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth)
        {
            using var writer = new StreamWriter(this.logFileName, true, Encoding.UTF8);
            WriteOperationLog(writer, $"Calling FindByDateOfBirth() with dateOfBirth = '{dateOfBirth}'");
            var result = this.service.FindByDateOfBirth(dateOfBirth);
            WriteOperationLog(writer, $"FindByDateOfBirth() returned {result.Count} record(s)");
            return result;
        }

        /// <summary>Returns all records.</summary>
        /// <returns>Returns readonly collection of all records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            using var writer = new StreamWriter(this.logFileName, true, Encoding.UTF8);
            WriteOperationLog(writer, "Calling GetRecords()");
            var result = this.service.GetRecords();
            WriteOperationLog(writer, $"GetRecords() returned {result.Count} record(s)");
            return result;
        }

        /// <summary>Returns service statistics.</summary>
        /// <returns>Returns ServiceStat.</returns>
        public ServiceStat GetStat()
        {
            using var writer = new StreamWriter(this.logFileName, true, Encoding.UTF8);
            WriteOperationLog(writer, "Calling GetStat()");
            var result = this.service.GetStat();
            WriteOperationLog(writer, $"GetStat() returned '{result.NumberOfRecords}' record(s). '{result.DeletedRecordsIds.Count}' deleted record(s)");
            return result;
        }

        /// <summary>Makes snapshot of current object state.</summary>
        /// <returns>Returns new <see cref="FileCabinetServiceSnapshot"/>.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            using var writer = new StreamWriter(this.logFileName, true, Encoding.UTF8);
            WriteOperationLog(writer, "Calling MakeSnapshot()");
            var result = this.service.MakeSnapshot();
            WriteOperationLog(writer, "MakeSnapshot() returned new Snapshot object");
            return result;
        }

        /// <summary>Restores the specified snapshot.</summary>
        /// <param name="snapshot">Snapshot.</param>
        public void Restore(FileCabinetServiceSnapshot snapshot)
        {
            using var writer = new StreamWriter(this.logFileName, true, Encoding.UTF8);

            try
            {
                WriteOperationLog(writer, "Calling Restore() snapshot");
                this.service.Restore(snapshot);
                WriteOperationLog(writer, "Restore() executed successfully");
            }
            catch (ArgumentNullException ex)
            {
                WriteOperationLog(writer, $" Restore() threw an exception: {ex.Message}");
                throw;
            }
        }

        /// <summary>Removes a record with the specified id.</summary>
        /// <param name="id">Id of the record to delete.</param>
        public void RemoveRecord(int id)
        {
            using var writer = new StreamWriter(this.logFileName, true, Encoding.UTF8);

            try
            {
                WriteOperationLog(writer, $"Calling RemoveRecord() with Id = '{id}'");
                this.service.RemoveRecord(id);
                WriteOperationLog(writer, "RemoveRecord() executed successfully");
            }
            catch (ArgumentException ex)
            {
                WriteOperationLog(writer, $" RemoveRecord() threw an exception: {ex.Message}");
                throw;
            }
        }

        /// <summary>Defragments the data file.</summary>
        public void Purge()
        {
            using var writer = new StreamWriter(this.logFileName, true, Encoding.UTF8);
            WriteOperationLog(writer, "Calling Purge()");
            this.service.Purge();
            WriteOperationLog(writer, "Purge() executed successfully");
        }

        private static void WriteOperationLog(StreamWriter writer, string message)
        {
            string now = DateTime.Now.ToString("MM/dd/yyyy HH:mm", new CultureInfo("en-US"));
            writer.WriteLine($"{now} - {message}");
        }

        private static string RecordToString(FileCabinetRecord record, bool withId)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            StringBuilder builder = new ();
            if (withId)
            {
                builder.Append($"Id = '{record.Id}', ");
            }

            builder.Append($"FirstName = '{record.FirstName}', ");
            builder.Append($"LastName = '{record.LastName}', ");
            builder.Append($"DateOfBirth = '{record.DateOfBirth.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)}', ");
            builder.Append($"WorkPlaceNumber = '{record.WorkPlaceNumber}', ");
            builder.Append($"Salary = '{record.Salary.ToString("F2", CultureInfo.InvariantCulture)}', ");
            builder.Append($"Department = '{record.Department}'");

            return builder.ToString();
        }
    }
}