using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using FileCabinetApp.Models;

namespace FileCabinetApp
{
    /// <summary>Service meter.</summary>
    /// <seealso cref="IFileCabinetService" />
    public class ServiceMeter : IFileCabinetService
    {
        private readonly IFileCabinetService service;

        /// <summary>Initializes a new instance of the <see cref="ServiceMeter"/> class.</summary>
        /// <param name="service">IFileCabinetService.</param>
        /// <exception cref="ArgumentNullException">Thrown when service is null.</exception>
        public ServiceMeter(IFileCabinetService service)
        {
            this.service = service ?? throw new ArgumentNullException(nameof(service));
        }

        /// <inheritdoc cref="IFileCabinetService.CreateRecord(FileCabinetRecord)"/>
        public int CreateRecord(FileCabinetRecord record) => GetExecutionDuration(() => this.service.CreateRecord(record));

        /// <inheritdoc cref="IFileCabinetService.EditRecord(FileCabinetRecord)"/>
        public void EditRecord(FileCabinetRecord record) => GetExecutionDuration(() => this.service.EditRecord(record));

        /// <inheritdoc cref="IFileCabinetService.IdExists(int)"/>
        public bool IdExists(int id) => GetExecutionDuration(() => this.service.IdExists(id));

        /// <inheritdoc cref="IFileCabinetService.GetRecords"/>
        public IEnumerable<FileCabinetRecord> GetRecords() => GetExecutionDuration(() => this.service.GetRecords());

        /// <inheritdoc cref="IFileCabinetService.Search(RecordToSearch)"/>
        public IEnumerable<FileCabinetRecord> Search(RecordToSearch search) => GetExecutionDuration(() => this.service.Search(search));

        /// <inheritdoc cref="IFileCabinetService.GetStat"/>
        public ServiceStat GetStat() => GetExecutionDuration(() => this.service.GetStat());

        /// <inheritdoc cref="IFileCabinetService.MakeSnapshot"/>
        public FileCabinetServiceSnapshot MakeSnapshot() => GetExecutionDuration(() => this.service.MakeSnapshot());

        /// <inheritdoc cref="IFileCabinetService.Restore(FileCabinetServiceSnapshot)"/>
        public int Restore(FileCabinetServiceSnapshot snapshot) => GetExecutionDuration(() => this.service.Restore(snapshot));

        /// <inheritdoc cref="IFileCabinetService.RemoveRecord(int)"/>
        public void RemoveRecord(int id) => GetExecutionDuration(() => this.service.RemoveRecord(id));

        /// <inheritdoc cref="IFileCabinetService.Purge"/>
        public void Purge() => GetExecutionDuration(() => this.service.Purge());

        private static void GetExecutionDuration(Action action)
        {
            var sw = Stopwatch.StartNew();
            action();
            sw.Stop();
            string methodName = GetMethodName(action.Method.Name);
            Console.WriteLine($"{methodName} method execution duration is {sw.ElapsedTicks} ticks.");
        }

        private static T GetExecutionDuration<T>(Func<T> action)
        {
            var sw = Stopwatch.StartNew();
            T result = action();
            sw.Stop();
            string methodName = GetMethodName(action.Method.Name);
            Console.WriteLine($"{methodName} method execution duration is {sw.ElapsedTicks} ticks.");
            return result;
        }

        private static string GetMethodName(string methodName)
        {
            string matchValue = Regex.Match(methodName, @"<\w*>").Value;
            if (matchValue.Length > 2)
            {
                methodName = matchValue[1..^1];
            }

            return methodName;
        }
    }
}