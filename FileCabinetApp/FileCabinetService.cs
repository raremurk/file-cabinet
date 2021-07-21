using System;
using System.Collections.Generic;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>Class for working with records.</summary>
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new ();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new ();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new ();
        private readonly Dictionary<string, List<FileCabinetRecord>> dateOfBirthDictionary = new ();

        /// <summary>Creates a record and returns its id.</summary>
        /// <param name="record">Object representing a record.</param>
        /// <returns>Id of a new record.</returns>
        /// <exception cref="ArgumentNullException">Thrown when record is null.</exception>
        public int CreateRecord(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            FileCabinetServiceGuard.CheckStrings(new string[] { record.FirstName, record.LastName });
            FileCabinetServiceGuard.CheckDateTimeRange(record.DateOfBirth);
            FileCabinetServiceGuard.CheckWorkPlaceNumber(record.WorkPlaceNumber);
            FileCabinetServiceGuard.CheckSalary(record.Salary);
            FileCabinetServiceGuard.CheckDepartment(record.Department);

            record.Id = this.list.Count + 1;

            this.list.Add(record);

            FileCabinetService.AddRecordToDictionary(record.FirstName, record, this.firstNameDictionary);
            FileCabinetService.AddRecordToDictionary(record.LastName, record, this.lastNameDictionary);
            FileCabinetService.AddRecordToDictionary(record.DateOfBirth.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture), record, this.dateOfBirthDictionary);

            return record.Id;
        }

        /// <summary>Edits a record with the specified id.</summary>
        /// <param name="record">Object representing a record.</param>
        /// <exception cref="ArgumentNullException">Thrown when record is null.</exception>
        /// <exception cref="ArgumentException">Thrown when no record with the specified id.</exception>
        public void EditRecord(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            if (!this.list.Exists(x => x.Id == record.Id))
            {
                throw new ArgumentException("No record with this id.");
            }

            FileCabinetServiceGuard.CheckStrings(new string[] { record.FirstName, record.LastName });
            FileCabinetServiceGuard.CheckDateTimeRange(record.DateOfBirth);
            FileCabinetServiceGuard.CheckWorkPlaceNumber(record.WorkPlaceNumber);
            FileCabinetServiceGuard.CheckSalary(record.Salary);
            FileCabinetServiceGuard.CheckDepartment(record.Department);

            FileCabinetRecord originalRecord = this.list.Find(x => x.Id == record.Id);

            FileCabinetService.RemoveRecordFromDictionary(originalRecord.FirstName, originalRecord, this.firstNameDictionary);
            FileCabinetService.RemoveRecordFromDictionary(originalRecord.LastName, originalRecord, this.lastNameDictionary);
            FileCabinetService.RemoveRecordFromDictionary(originalRecord.DateOfBirth.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture), originalRecord, this.dateOfBirthDictionary);

            originalRecord = record;

            FileCabinetService.AddRecordToDictionary(originalRecord.FirstName, originalRecord, this.firstNameDictionary);
            FileCabinetService.AddRecordToDictionary(originalRecord.LastName, originalRecord, this.lastNameDictionary);
            FileCabinetService.AddRecordToDictionary(originalRecord.DateOfBirth.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture), originalRecord, this.dateOfBirthDictionary);
        }

        /// <summary>Finds records by first name.</summary>
        /// <param name="firstName">First name to find.</param>
        /// <returns>Returns array of found records.</returns>
        public FileCabinetRecord[] FindByFirstName(string firstName)
        {
            string firstNameKey = firstName is null ? string.Empty : firstName.ToUpperInvariant();
            return this.firstNameDictionary.ContainsKey(firstNameKey) ? this.firstNameDictionary[firstNameKey].ToArray() : Array.Empty<FileCabinetRecord>();
        }

        /// <summary>Finds records by last name.</summary>
        /// <param name="lastName">Last name to find.</param>
        /// <returns>Returns array of found records.</returns>
        public FileCabinetRecord[] FindByLastName(string lastName)
        {
            string lastNameKey = lastName is null ? string.Empty : lastName.ToUpperInvariant();
            return this.lastNameDictionary.ContainsKey(lastNameKey) ? this.lastNameDictionary[lastNameKey].ToArray() : Array.Empty<FileCabinetRecord>();
        }

        /// <summary>Finds records by date of birth.</summary>
        /// <param name="dateOfBirth">Date of birth to find.</param>
        /// <returns>Returns array of found records.</returns>
        public FileCabinetRecord[] FindByDateOfBirth(string dateOfBirth)
        {
            return this.dateOfBirthDictionary.ContainsKey(dateOfBirth) ? this.dateOfBirthDictionary[dateOfBirth].ToArray() : Array.Empty<FileCabinetRecord>();
        }

        /// <summary>Returns all records.</summary>
        /// <returns>Returns array of all records.</returns>
        public FileCabinetRecord[] GetRecords()
        {
            return this.list.ToArray();
        }

        /// <summary>Returns count of records.</summary>
        /// <returns>Returns count.</returns>
        #pragma warning disable CA1024
        public int GetStat() => this.list.Count;
        #pragma warning restore CA1024

        private static void AddRecordToDictionary(string propertyValue, FileCabinetRecord record, Dictionary<string, List<FileCabinetRecord>> dictionary)
        {
            string key = propertyValue is null ? string.Empty : propertyValue.ToUpperInvariant();
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, new List<FileCabinetRecord>());
            }

            dictionary[key].Add(record);
        }

        private static void RemoveRecordFromDictionary(string propertyValue, FileCabinetRecord record, Dictionary<string, List<FileCabinetRecord>> dictionary)
        {
            string key = propertyValue is null ? string.Empty : propertyValue.ToUpperInvariant();
            dictionary[key].Remove(record);
            if (dictionary[key].Count == 0)
            {
                dictionary.Remove(key);
            }
        }

        private static class FileCabinetServiceGuard
        {
            public static void CheckStrings(string[] arguments)
            {
                foreach (string argument in arguments)
                {
                    string nameOfArgument = nameof(argument);

                    if (string.IsNullOrWhiteSpace(argument))
                    {
                        throw new ArgumentNullException(nameOfArgument, " cannot be null or whitespace only.");
                    }

                    if (Guard.StringIsIncorrect(argument))
                    {
                        throw new ArgumentException($"{nameOfArgument} length is less than {Guard.MinStringLength} or more than {Guard.MaxStringLength}.");
                    }
                }
            }

            public static void CheckDateTimeRange(DateTime argument)
            {
                if (Guard.DateTimeRangeIsIncorrect(argument))
                {
                    throw new ArgumentException($"{nameof(argument)} is less than {Guard.MinDate.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture)} or more than current date.");
                }
            }

            public static void CheckWorkPlaceNumber(short argument)
            {
                if (Guard.WorkPlaceNumberIsLessThanMinValue(argument))
                {
                    throw new ArgumentException($"{nameof(argument)} is less than {Guard.WorkPlaceNumberMinValue}.");
                }
            }

            public static void CheckSalary(decimal argument)
            {
                if (Guard.SalaryIsLessThanThanMinValue(argument))
                {
                    throw new ArgumentException($"{nameof(argument)} cannot be less than {Guard.SalaryMinValue}.");
                }
            }

            public static void CheckDepartment(char argument)
            {
                if (Guard.DepartmentValueIsIncorrect(argument))
                {
                    throw new ArgumentException($"{nameof(argument)} can only be uppercase letter.");
                }
            }
        }
    }
}
