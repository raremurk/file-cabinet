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

        /// <summary>Creates a record and returns its id.</summary>
        /// <param name="record">Object representing a record.</param>
        /// <returns>Id of a new record.</returns>
        public int CreateRecord(FileCabinetRecord record)
        {
            var sw = Stopwatch.StartNew();
            int id = this.service.CreateRecord(record);
            sw.Stop();
            Console.WriteLine($"CreateRecord method execution duration is {sw.ElapsedTicks} ticks.");
            return id;
        }

        /// <summary>Edits a record with the specified id.</summary>
        /// <param name="record">Object representing a record.</param>
        public void EditRecord(FileCabinetRecord record)
        {
            var sw = Stopwatch.StartNew();
            this.service.EditRecord(record);
            sw.Stop();
            Console.WriteLine($"EditRecord method execution duration is {sw.ElapsedTicks} ticks.");
        }

        /// <summary>Finds records by first name.</summary>
        /// <param name="firstName">First name to find.</param>
        /// <returns>Returns readonly collection of found records.</returns>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            var sw = Stopwatch.StartNew();
            var result = this.service.FindByFirstName(firstName);
            sw.Stop();
            Console.WriteLine($"FindByFirstName method execution duration is {sw.ElapsedTicks} ticks.");
            return result;
        }

        /// <summary>Finds records by last name.</summary>
        /// <param name="lastName">Last name to find.</param>
        /// <returns>Returns readonly collection of found records.</returns>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            var sw = Stopwatch.StartNew();
            var result = this.service.FindByLastName(lastName);
            sw.Stop();
            Console.WriteLine($"FindByLastName method execution duration is {sw.ElapsedTicks} ticks.");
            return result;
        }

        /// <summary>Finds records by date of birth.</summary>
        /// <param name="dateOfBirth">Date of birth to find.</param>
        /// <returns>Returns readonly collection of found records.</returns>
        public IEnumerable<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth)
        {
            var sw = Stopwatch.StartNew();
            var result = this.service.FindByDateOfBirth(dateOfBirth);
            sw.Stop();
            Console.WriteLine($"FindByDateOfBirth method execution duration is {sw.ElapsedTicks} ticks.");
            return result;
        }

        /// <summary>Returns all records.</summary>
        /// <returns>Returns readonly collection of all records.</returns>
        public IEnumerable<FileCabinetRecord> GetRecords()
        {
            var sw = Stopwatch.StartNew();
            var result = this.service.GetRecords();
            sw.Stop();
            Console.WriteLine($"GetRecords method execution duration is {sw.ElapsedTicks} ticks.");
            return result;
        }

        /// <summary>Returns service statistics.</summary>
        /// <returns>Returns ServiceStat.</returns>
        public ServiceStat GetStat()
        {
            var sw = Stopwatch.StartNew();
            ServiceStat result = this.service.GetStat();
            sw.Stop();
            Console.WriteLine($"GetStat method execution duration is {sw.ElapsedTicks} ticks.");
            return result;
        }

        /// <summary>Makes snapshot of current object state.</summary>
        /// <returns>Returns new <see cref="FileCabinetServiceSnapshot"/>.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            var sw = Stopwatch.StartNew();
            var result = this.service.MakeSnapshot();
            sw.Stop();
            Console.WriteLine($"MakeSnapshot method execution duration is {sw.ElapsedTicks} ticks.");
            return result;
        }

        /// <summary>Restores the specified snapshot.</summary>
        /// <param name="snapshot">Snapshot.</param>
        public void Restore(FileCabinetServiceSnapshot snapshot)
        {
            var sw = Stopwatch.StartNew();
            this.service.Restore(snapshot);
            sw.Stop();
            Console.WriteLine($"Restore method execution duration is {sw.ElapsedTicks} ticks.");
        }

        /// <summary>Removes a record with the specified id.</summary>
        /// <param name="id">Id of the record to delete.</param>
        public void RemoveRecord(int id)
        {
            var sw = Stopwatch.StartNew();
            this.service.RemoveRecord(id);
            sw.Stop();
            Console.WriteLine($"RemoveRecord method execution duration is {sw.ElapsedTicks} ticks.");
        }

        /// <summary>Defragments the data file.</summary>
        public void Purge()
        {
            var sw = Stopwatch.StartNew();
            this.service.Purge();
            sw.Stop();
            Console.WriteLine($"Purge method execution duration is {sw.ElapsedTicks} ticks.");
        }
    }
}