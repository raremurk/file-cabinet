using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>Print records to the console.</summary>
    /// <seealso cref="IRecordPrinter"/>
    public class DefaultRecordPrinter : IRecordPrinter
    {
        /// <summary>Print records to the console.</summary>
        /// <param name="records">Records to print.</param>
        public void Print(IEnumerable<FileCabinetRecord> records)
        {
            if (records is null)
            {
                throw new ArgumentNullException(nameof(records));
            }

            foreach (var record in records)
            {
                StringBuilder builder = new ();
                builder.Append($"#{record.Id}, ");
                builder.Append($"{record.FirstName}, ");
                builder.Append($"{record.LastName}, ");
                builder.Append($"{record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)}, ");
                builder.Append($"{record.WorkPlaceNumber}, ");
                builder.Append($"{record.Salary.ToString("F2", CultureInfo.InvariantCulture)}, ");
                builder.Append($"{record.Department}");

                Console.WriteLine(builder.ToString());
            }
        }
    }
}
