﻿using System;
using System.Collections.Generic;
using System.Globalization;

namespace FileCabinetApp
{
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new ();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new ();

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

            string firstNameKey = firstName is null ? string.Empty : firstName.ToUpperInvariant();
            if (!this.firstNameDictionary.ContainsKey(firstNameKey))
            {
                this.firstNameDictionary.Add(firstNameKey, new List<FileCabinetRecord>());
            }

            this.firstNameDictionary[firstNameKey].Add(record);

            return record.Id;
        }

        public void EditRecord(int id, string firstName, string lastName, DateTime dateOfBirth, short workPlaceNumber, decimal salary, char department)
        {
            if (!this.list.Exists(x => x.Id == id))
            {
                throw new ArgumentException("No record with this id.");
            }

            FileCabinetRecord record = this.list.Find(x => x.Id == id);

            string firstNameKey = record.FirstName is null ? string.Empty : record.FirstName.ToUpperInvariant();
            this.firstNameDictionary[firstNameKey].Remove(record);
            if (this.firstNameDictionary[firstNameKey].Count == 0)
            {
                this.firstNameDictionary.Remove(firstNameKey);
            }

            record.FirstName = firstName;
            record.LastName = lastName;
            record.DateOfBirth = dateOfBirth;
            record.WorkPlaceNumber = workPlaceNumber;
            record.Salary = salary;
            record.Department = department;

            firstNameKey = firstName is null ? string.Empty : firstName.ToUpperInvariant();
            if (!this.firstNameDictionary.ContainsKey(firstNameKey))
            {
                this.firstNameDictionary.Add(firstNameKey, new List<FileCabinetRecord>());
            }

            this.firstNameDictionary[firstNameKey].Add(record);
        }

        public FileCabinetRecord[] FindByFirstName(string firstName)
        {
            string firstNameKey = firstName is null ? string.Empty : firstName.ToUpperInvariant();
            return this.firstNameDictionary.ContainsKey(firstNameKey) ? this.firstNameDictionary[firstNameKey].ToArray() : Array.Empty<FileCabinetRecord>();
        }

        public FileCabinetRecord[] FindByLastName(string lastName)
        {
            return this.list.FindAll(x => x.LastName.Equals(lastName, StringComparison.OrdinalIgnoreCase)).ToArray();
        }

        public FileCabinetRecord[] FindByDateOfBirth(DateTime dateOfBirth)
        {
            return this.list.FindAll(x => x.DateOfBirth.CompareTo(dateOfBirth) == 0).ToArray();
        }

        public FileCabinetRecord[] GetRecords()
        {
            return this.list.ToArray();
        }

        #pragma warning disable CA1024
        public int GetStat() => this.list.Count;
        #pragma warning restore CA1024

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
