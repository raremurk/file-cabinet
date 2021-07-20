using System;
using System.Collections.Generic;
using System.Globalization;

namespace FileCabinetApp
{
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new ();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new ();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new ();
        private readonly Dictionary<string, List<FileCabinetRecord>> dateOfBirthDictionary = new ();

        public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth, short workPlaceNumber, decimal salary, char department)
        {
            FileCabinetServiceGuard.CheckStrings(new string[] { firstName, lastName });
            FileCabinetServiceGuard.CheckDateTimeRange(dateOfBirth);
            FileCabinetServiceGuard.CheckWorkPlaceNumber(workPlaceNumber);
            FileCabinetServiceGuard.CheckSalary(salary);
            FileCabinetServiceGuard.CheckDepartment(department);

            var record = new FileCabinetRecord
            {
                Id = this.list.Count + 1,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                WorkPlaceNumber = workPlaceNumber,
                Salary = salary,
                Department = department,
            };

            this.list.Add(record);

            FileCabinetService.AddRecordToDictionary(firstName, record, this.firstNameDictionary);
            FileCabinetService.AddRecordToDictionary(lastName, record, this.lastNameDictionary);
            FileCabinetService.AddRecordToDictionary(dateOfBirth.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture), record, this.dateOfBirthDictionary);

            return record.Id;
        }

        public void EditRecord(int id, string firstName, string lastName, DateTime dateOfBirth, short workPlaceNumber, decimal salary, char department)
        {
            if (!this.list.Exists(x => x.Id == id))
            {
                throw new ArgumentException("No record with this id.");
            }

            FileCabinetRecord record = this.list.Find(x => x.Id == id);

            FileCabinetService.RemoveRecordFromDictionary(record.FirstName, record, this.firstNameDictionary);
            FileCabinetService.RemoveRecordFromDictionary(record.LastName, record, this.lastNameDictionary);
            FileCabinetService.RemoveRecordFromDictionary(record.DateOfBirth.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture), record, this.dateOfBirthDictionary);

            record.FirstName = firstName;
            record.LastName = lastName;
            record.DateOfBirth = dateOfBirth;
            record.WorkPlaceNumber = workPlaceNumber;
            record.Salary = salary;
            record.Department = department;

            FileCabinetService.AddRecordToDictionary(firstName, record, this.firstNameDictionary);
            FileCabinetService.AddRecordToDictionary(lastName, record, this.lastNameDictionary);
            FileCabinetService.AddRecordToDictionary(dateOfBirth.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture), record, this.dateOfBirthDictionary);
        }

        public FileCabinetRecord[] FindByFirstName(string firstName)
        {
            string firstNameKey = firstName is null ? string.Empty : firstName.ToUpperInvariant();
            return this.firstNameDictionary.ContainsKey(firstNameKey) ? this.firstNameDictionary[firstNameKey].ToArray() : Array.Empty<FileCabinetRecord>();
        }

        public FileCabinetRecord[] FindByLastName(string lastName)
        {
            string lastNameKey = lastName is null ? string.Empty : lastName.ToUpperInvariant();
            return this.lastNameDictionary.ContainsKey(lastNameKey) ? this.lastNameDictionary[lastNameKey].ToArray() : Array.Empty<FileCabinetRecord>();
        }

        public FileCabinetRecord[] FindByDateOfBirth(string dateOfBirth)
        {
            return this.dateOfBirthDictionary.ContainsKey(dateOfBirth) ? this.dateOfBirthDictionary[dateOfBirth].ToArray() : Array.Empty<FileCabinetRecord>();
        }

        public FileCabinetRecord[] GetRecords()
        {
            return this.list.ToArray();
        }

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

        private class FileCabinetServiceGuard
        {
            public static void CheckStrings(string[] arguments)
            {
                foreach (string argument in arguments)
                {
                    if (string.IsNullOrWhiteSpace(argument))
                    {
                        throw new ArgumentNullException(nameof(argument), " cannot be null or whitespace only.");
                    }

                    if (Guard.StringIsIncorrect(argument))
                    {
                        throw new ArgumentException($"{nameof(argument)} length is less than {Guard.MinStringLength} or more than {Guard.MaxStringLength}.");
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
