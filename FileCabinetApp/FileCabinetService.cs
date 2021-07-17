using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public class FileCabinetService
    {
        private static readonly DateTime MinDate = new (1950, 1, 1);
        private readonly List<FileCabinetRecord> list = new ();

        public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth, short workPlaceNumber, decimal salary, char department)
        {
            Guard.StringNullOrWhiteSpace(firstName, nameof(firstName));
            Guard.StringLength(firstName, nameof(firstName));
            Guard.StringNullOrWhiteSpace(lastName, nameof(lastName));
            Guard.StringLength(lastName, nameof(lastName));
            Guard.DateTimeRange(dateOfBirth, nameof(dateOfBirth));
            Guard.WorkPlaceNumberMinValue(workPlaceNumber, nameof(workPlaceNumber));
            Guard.SalaryMinValue(salary, nameof(salary));
            Guard.DepartmentRange(department, nameof(department));

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

        public static class Guard
        {
            public static void StringNullOrWhiteSpace(string argument, string argumentName)
            {
                if (string.IsNullOrWhiteSpace(argument))
                {
                    throw new ArgumentNullException($"{argumentName} cannot be null or whitespace only.");
                }
            }

            public static void StringLength(string argument, string argumentName)
            {
                if (argument.Length < 2 || argument.Length > 60)
                {
                    throw new ArgumentException($"{argumentName} length is less than 2 or more than 60.");
                }
            }

            public static void DateTimeRange(DateTime argument, string argumentName)
            {
                if (DateTime.Compare(DateTime.Now, argument) < 0 || DateTime.Compare(MinDate, argument) > 0)
                {
                    throw new ArgumentException($"{argumentName} is less than {MinDate.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture)} or more than current date.");
                }
            }

            public static void WorkPlaceNumberMinValue(short argument, string argumentName)
            {
                if (argument < 1)
                {
                    throw new ArgumentException($"{argumentName} is less than 1.");
                }
            }

            public static void SalaryMinValue(decimal argument, string argumentName)
            {
                if (argument < decimal.Zero)
                {
                    throw new ArgumentException($"{argumentName} cannot be less than 0.");
                }
            }

            public static void DepartmentRange(char argument, string argumentName)
            {
                if (!char.IsLetter(argument) || !char.IsUpper(argument))
                {
                    throw new ArgumentException($"{argumentName} can only be uppercase letter.");
                }
            }
        }
    }
}
