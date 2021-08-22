using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;
using FileCabinetApp.Models;

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

        /// <inheritdoc cref="IFileCabinetService.CreateRecord(FileCabinetRecord)"/>
        public int CreateRecord(FileCabinetRecord record)
        {
            using var writer = new StreamWriter(this.logFileName, true, Encoding.UTF8);
            WriteOperationLog(writer, $"Calling CreateRecord() with {RecordToString(record, false)}");
            try
            {
                int id = this.service.CreateRecord(record);
                WriteOperationLog(writer, $"CreateRecord() returned '{id}'");
                return id;
            }
            catch (Exception ex)
            {
                WriteOperationLog(writer, $"CreateRecord() threw an exception: {ex.Message}");
                throw;
            }
        }

        /// <inheritdoc cref="IFileCabinetService.EditRecord(FileCabinetRecord)"/>
        public void EditRecord(FileCabinetRecord record)
        {
            using var writer = new StreamWriter(this.logFileName, true, Encoding.UTF8);
            WriteOperationLog(writer, $"Calling EditRecord() with parameters");
            try
            {
                this.service.EditRecord(record);
                WriteOperationLog(writer, "EditRecord() executed successfully");
            }
            catch (Exception ex)
            {
                WriteOperationLog(writer, $"EditRecord() threw an exception: {ex.Message}");
                throw;
            }
        }

        /// <inheritdoc cref="IFileCabinetService.GetRecord(int)"/>
        public FileCabinetRecord GetRecord(int id)
        {
            using var writer = new StreamWriter(this.logFileName, true, Encoding.UTF8);
            WriteOperationLog(writer, $"Calling GetRecord() with Id = '{id}'");
            try
            {
                var result = this.service.GetRecord(id);
                WriteOperationLog(writer, $"GetRecord() returned result");
                return result;
            }
            catch (Exception ex)
            {
                WriteOperationLog(writer, $"GetRecord() threw an exception: {ex.Message}");
                throw;
            }
        }

        /// <inheritdoc cref="IFileCabinetService.GetRecords"/>
        public IEnumerable<FileCabinetRecord> GetRecords()
        {
            using var writer = new StreamWriter(this.logFileName, true, Encoding.UTF8);
            WriteOperationLog(writer, "Calling GetRecords()");
            try
            {
                var result = this.service.GetRecords();
                WriteOperationLog(writer, $"GetRecords() returned result");
                return result;
            }
            catch (Exception ex)
            {
                WriteOperationLog(writer, $"GetRecords() threw an exception: {ex.Message}");
                throw;
            }
        }

        /// <inheritdoc cref="IFileCabinetService.Search(RecordToSearch)"/>
        public IEnumerable<FileCabinetRecord> Search(RecordToSearch search)
        {
            using var writer = new StreamWriter(this.logFileName, true, Encoding.UTF8);
            WriteOperationLog(writer, "Calling Search() with parameters");
            try
            {
                var result = this.service.Search(search);
                WriteOperationLog(writer, $"Search() returned result");
                return result;
            }
            catch (Exception ex)
            {
                WriteOperationLog(writer, $"Search() threw an exception: {ex.Message}");
                throw;
            }
        }

        /// <inheritdoc cref="IFileCabinetService.GetStat"/>
        public ServiceStat GetStat()
        {
            using var writer = new StreamWriter(this.logFileName, true, Encoding.UTF8);
            WriteOperationLog(writer, "Calling GetStat()");
            try
            {
                var result = this.service.GetStat();
                WriteOperationLog(writer, $"GetStat() returned '{result.ExistingRecordsIds.Count}' record(s). '{result.DeletedRecordsIds.Count}' deleted record(s)");
                return result;
            }
            catch (Exception ex)
            {
                WriteOperationLog(writer, $"GetStat() threw an exception: {ex.Message}");
                throw;
            }
        }

        /// <inheritdoc cref="IFileCabinetService.MakeSnapshot"/>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            using var writer = new StreamWriter(this.logFileName, true, Encoding.UTF8);
            WriteOperationLog(writer, "Calling MakeSnapshot()");
            try
            {
                var result = this.service.MakeSnapshot();
                WriteOperationLog(writer, "MakeSnapshot() returned new Snapshot object");
                return result;
            }
            catch (Exception ex)
            {
                WriteOperationLog(writer, $"MakeSnapshot() threw an exception: {ex.Message}");
                throw;
            }
        }

        /// <inheritdoc cref="IFileCabinetService.Restore(FileCabinetServiceSnapshot)"/>
        public void Restore(FileCabinetServiceSnapshot snapshot)
        {
            using var writer = new StreamWriter(this.logFileName, true, Encoding.UTF8);
            WriteOperationLog(writer, "Calling Restore() snapshot");
            try
            {
                this.service.Restore(snapshot);
                WriteOperationLog(writer, "Restore() executed successfully");
            }
            catch (Exception ex)
            {
                WriteOperationLog(writer, $"Restore() threw an exception: {ex.Message}");
                throw;
            }
        }

        /// <inheritdoc cref="IFileCabinetService.RemoveRecord(int)"/>
        public void RemoveRecord(int id)
        {
            using var writer = new StreamWriter(this.logFileName, true, Encoding.UTF8);
            WriteOperationLog(writer, $"Calling RemoveRecord() with parameters");
            try
            {
                this.service.RemoveRecord(id);
                WriteOperationLog(writer, "RemoveRecord() executed successfully");
            }
            catch (Exception ex)
            {
                WriteOperationLog(writer, $" RemoveRecord() threw an exception: {ex.Message}");
                throw;
            }
        }

        /// <inheritdoc cref="IFileCabinetService.Purge"/>
        public void Purge()
        {
            using var writer = new StreamWriter(this.logFileName, true, Encoding.UTF8);
            WriteOperationLog(writer, "Calling Purge()");
            try
            {
                this.service.Purge();
                WriteOperationLog(writer, "Purge() executed successfully");
            }
            catch (Exception ex)
            {
                WriteOperationLog(writer, $" Purge() threw an exception: {ex.Message}");
                throw;
            }
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