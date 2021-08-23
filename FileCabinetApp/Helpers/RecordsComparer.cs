using System;
using FileCabinetApp.Models;

namespace FileCabinetApp.Helpers
{
    /// <summary>RecordsComparer class.</summary>
    public static class RecordsComparer
    {
        /// <summary>Compares records.</summary>
        /// <param name="record">FileCabinetRecord.</param>
        /// <param name="search">RecordToSearch.</param>
        /// <returns>Returns true if records are equal.</returns>
        public static bool RecordsEquals(FileCabinetRecord record, RecordToSearch search)
        {
            _ = record ?? throw new ArgumentNullException(nameof(record));
            _ = search ?? throw new ArgumentNullException(nameof(search));
            return search.AndMode ? RecordsEqualsAndMode(record, search) : RecordsEqualsOrMode(record, search);
        }

        private static bool RecordsEqualsAndMode(FileCabinetRecord record, RecordToSearch search)
        {
            return record.Id.Equals(search.Id.Item1 ? search.Id.Item2 : record.Id)
                && record.FirstName.Equals(search.FirstName.Item1 ? search.FirstName.Item2 : record.FirstName, StringComparison.OrdinalIgnoreCase)
                && record.LastName.Equals(search.LastName.Item1 ? search.LastName.Item2 : record.LastName, StringComparison.OrdinalIgnoreCase)
                && record.DateOfBirth.CompareTo(search.DateOfBirth.Item1 ? search.DateOfBirth.Item2 : record.DateOfBirth) == 0
                && record.WorkPlaceNumber.Equals(search.WorkPlaceNumber.Item1 ? search.WorkPlaceNumber.Item2 : record.WorkPlaceNumber)
                && record.Salary.Equals(search.Salary.Item1 ? search.Salary.Item2 : record.Salary)
                && record.Department.Equals(search.Department.Item1 ? search.Department.Item2 : record.Department);
        }

        private static bool RecordsEqualsOrMode(FileCabinetRecord record, RecordToSearch search)
        {
            return record.Id.Equals(search.Id.Item2)
                || record.FirstName.Equals(search.FirstName.Item2, StringComparison.OrdinalIgnoreCase)
                || record.LastName.Equals(search.LastName.Item2, StringComparison.OrdinalIgnoreCase)
                || record.DateOfBirth.CompareTo(search.DateOfBirth.Item2) == 0
                || record.WorkPlaceNumber.Equals(search.WorkPlaceNumber.Item2)
                || record.Salary.Equals(search.Salary.Item2)
                || record.Department.Equals(search.Department.Item2);
        }
    }
}
