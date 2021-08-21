using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

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
        public int CreateRecord(FileCabinetRecord record)
        {
            var sw = Stopwatch.StartNew();
            int id = this.service.CreateRecord(record);
            sw.Stop();
            Console.WriteLine($"CreateRecord method execution duration is {sw.ElapsedTicks} ticks.");
            return id;
        }

        /// <inheritdoc cref="IFileCabinetService.EditRecords(ReadOnlyCollection{FileCabinetRecord})"/>
        public void EditRecords(ReadOnlyCollection<FileCabinetRecord> records)
        {
            var sw = Stopwatch.StartNew();
            this.service.EditRecords(records);
            sw.Stop();
            Console.WriteLine($"EditRecord method execution duration is {sw.ElapsedTicks} ticks.");
        }

        /// <inheritdoc cref="IFileCabinetService.GetRecord(int)"/>
        public FileCabinetRecord GetRecord(int id)
        {
            var sw = Stopwatch.StartNew();
            var result = this.service.GetRecord(id);
            sw.Stop();
            Console.WriteLine($"EditRecord method execution duration is {sw.ElapsedTicks} ticks.");
            return result;
        }

        /// <inheritdoc cref="IFileCabinetService.FindByFirstName(string)"/>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            var sw = Stopwatch.StartNew();
            var result = this.service.FindByFirstName(firstName);
            sw.Stop();
            Console.WriteLine($"FindByFirstName method execution duration is {sw.ElapsedTicks} ticks.");
            return result;
        }

        /// <inheritdoc cref="IFileCabinetService.FindByLastName(string)"/>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            var sw = Stopwatch.StartNew();
            var result = this.service.FindByLastName(lastName);
            sw.Stop();
            Console.WriteLine($"FindByLastName method execution duration is {sw.ElapsedTicks} ticks.");
            return result;
        }

        /// <inheritdoc cref="IFileCabinetService.FindByDateOfBirth(string)"/>
        public IEnumerable<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth)
        {
            var sw = Stopwatch.StartNew();
            var result = this.service.FindByDateOfBirth(dateOfBirth);
            sw.Stop();
            Console.WriteLine($"FindByDateOfBirth method execution duration is {sw.ElapsedTicks} ticks.");
            return result;
        }

        /// <inheritdoc cref="IFileCabinetService.GetRecords"/>
        public IEnumerable<FileCabinetRecord> GetRecords()
        {
            var sw = Stopwatch.StartNew();
            var result = this.service.GetRecords();
            sw.Stop();
            Console.WriteLine($"GetRecords method execution duration is {sw.ElapsedTicks} ticks.");
            return result;
        }

        /// <inheritdoc cref="IFileCabinetService.IdExists(int)"/>
        public bool IdExists(int id)
        {
            var sw = Stopwatch.StartNew();
            bool result = this.service.IdExists(id);
            sw.Stop();
            Console.WriteLine($"IdExists method execution duration is {sw.ElapsedTicks} ticks.");
            return result;
        }

        /// <inheritdoc cref="IFileCabinetService.GetStat"/>
        public ServiceStat GetStat()
        {
            var sw = Stopwatch.StartNew();
            ServiceStat result = this.service.GetStat();
            sw.Stop();
            Console.WriteLine($"GetStat method execution duration is {sw.ElapsedTicks} ticks.");
            return result;
        }

        /// <inheritdoc cref="IFileCabinetService.MakeSnapshot"/>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            var sw = Stopwatch.StartNew();
            var result = this.service.MakeSnapshot();
            sw.Stop();
            Console.WriteLine($"MakeSnapshot method execution duration is {sw.ElapsedTicks} ticks.");
            return result;
        }

        /// <inheritdoc cref="IFileCabinetService.Restore(FileCabinetServiceSnapshot)"/>
        public void Restore(FileCabinetServiceSnapshot snapshot)
        {
            var sw = Stopwatch.StartNew();
            this.service.Restore(snapshot);
            sw.Stop();
            Console.WriteLine($"Restore method execution duration is {sw.ElapsedTicks} ticks.");
        }

        /// <inheritdoc cref="IFileCabinetService.RemoveRecords(ReadOnlyCollection{int})"/>
        public void RemoveRecords(ReadOnlyCollection<int> ids)
        {
            var sw = Stopwatch.StartNew();
            this.service.RemoveRecords(ids);
            sw.Stop();
            Console.WriteLine($"RemoveRecord method execution duration is {sw.ElapsedTicks} ticks.");
        }

        /// <inheritdoc cref="IFileCabinetService.Purge"/>
        public void Purge()
        {
            var sw = Stopwatch.StartNew();
            this.service.Purge();
            sw.Stop();
            Console.WriteLine($"Purge method execution duration is {sw.ElapsedTicks} ticks.");
        }
    }
}