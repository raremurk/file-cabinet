using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FileCabinetApp.Program;

namespace FileCabinetApp
{
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new ();

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

            return record.Id;
        }

        public FileCabinetRecord[] GetRecords()
        {
            return this.list.ToArray();
        }

        public int GetStat()
        {
            return this.list.Count;
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

                    if (Guard.NameLengthIsIncorrect(argument))
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
