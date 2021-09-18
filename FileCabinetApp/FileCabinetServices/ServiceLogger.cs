using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
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
            string parameters = record is null ? "null" : record.ToString(", ");
            return this.GetLog(() => this.service.CreateRecord(record), parameters, this.ResultToString);
        }

        /// <inheritdoc cref="IFileCabinetService.EditRecord(FileCabinetRecord)"/>
        public void EditRecord(FileCabinetRecord record)
        {
            string parameters = record is null ? "null" : record.ToString(", ");
            this.GetLog(() => this.service.EditRecord(record), parameters);
        }

        /// <inheritdoc cref="IFileCabinetService.IdExists(int)"/>
        public bool IdExists(int id)
        {
            string parameters = $"Id = '{id}'";
            return this.GetLog(() => this.service.IdExists(id), parameters, this.ResultToString);
        }

        /// <inheritdoc cref="IFileCabinetService.GetRecords"/>
        public IEnumerable<FileCabinetRecord> GetRecords()
        {
            string parameters = string.Empty;
            return this.GetLog(() => this.service.GetRecords(), parameters, this.ResultToString);
        }

        /// <inheritdoc cref="IFileCabinetService.Search(RecordToSearch)"/>
        public IEnumerable<FileCabinetRecord> Search(RecordToSearch search)
        {
            string parameters = search is null ? "null" : search.ToString(", ");
            return this.GetLog(() => this.service.Search(search), parameters, this.ResultToString);
        }

        /// <inheritdoc cref="IFileCabinetService.GetStat"/>
        public ServiceStat GetStat()
        {
            string parameters = string.Empty;
            return this.GetLog(() => this.service.GetStat(), parameters, this.ResultToString);
        }

        /// <inheritdoc cref="IFileCabinetService.MakeSnapshot"/>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            string parameters = string.Empty;
            return this.GetLog(() => this.service.MakeSnapshot(), parameters, this.ResultToString);
        }

        /// <inheritdoc cref="IFileCabinetService.Restore(FileCabinetServiceSnapshot)"/>
        public int Restore(FileCabinetServiceSnapshot snapshot)
        {
            string parameters = snapshot is null ? "null" : snapshot.ToString();
            return this.GetLog(() => this.service.Restore(snapshot), parameters, this.ResultToString);
        }

        /// <inheritdoc cref="IFileCabinetService.RemoveRecord(int)"/>
        public void RemoveRecord(int id)
        {
            string parameters = $"Id = '{id}'";
            this.GetLog(() => this.service.RemoveRecord(id), parameters);
        }

        /// <inheritdoc cref="IFileCabinetService.Purge"/>
        public void Purge()
        {
            string parameters = string.Empty;
            this.GetLog(() => this.service.Purge(), parameters);
        }

        private T GetLog<T>(Expression<Func<T>> expression, string parameters, Func<T, string> resultToString)
        {
            string methodName = ((MethodCallExpression)expression.Body).Method.Name;
            parameters = string.IsNullOrWhiteSpace(parameters) ? string.Empty : $"with parameters {parameters}";
            this.WriteOperationLog($"Calling {methodName} method {parameters}");
            try
            {
                T result = expression.Compile()();
                string returned = resultToString(result);
                this.WriteOperationLog($"{methodName} method returned {returned}");
                return result;
            }
            catch (Exception ex)
            {
                this.WriteOperationLog($"{methodName} threw an exception: {ex.Message}");
                throw;
            }
        }

        private void GetLog(Expression<Action> expression, string parameters)
        {
            string methodName = ((MethodCallExpression)expression.Body).Method.Name;
            parameters = string.IsNullOrWhiteSpace(parameters) ? string.Empty : $"with parameters {parameters}";
            this.WriteOperationLog($"Calling {methodName} method {parameters}");
            try
            {
                expression.Compile()();
                this.WriteOperationLog($"{methodName} executed successfully");
            }
            catch (Exception ex)
            {
                this.WriteOperationLog($"{methodName} threw an exception: {ex.Message}");
                throw;
            }
        }

        private void WriteOperationLog(string message)
        {
            using var writer = new StreamWriter(this.logFileName, true, Encoding.UTF8);
            string now = DateTime.Now.ToString("MM/dd/yyyy HH:mm", new CultureInfo("en-US"));
            writer.WriteLine($"{now} - {message}");
        }

        private string ResultToString(int number) => $"{number}";

        private string ResultToString(bool result) => $"{result}";

        private string ResultToString(ServiceStat stat) => stat.ToString();

        private string ResultToString(FileCabinetServiceSnapshot snapshot) => snapshot.ToString();

        private string ResultToString(IEnumerable<FileCabinetRecord> records) => $"{records.Count()} record(s)";
    }
}