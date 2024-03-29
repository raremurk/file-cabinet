﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using FileCabinetApp.Models;

namespace FileCabinetApp
{
    /// <summary>Provides method to import <see cref="FileCabinetRecord"/> records from csv file.</summary>
    public class FileCabinetRecordCsvReader
    {
        private readonly StreamReader reader;

        /// <summary>Initializes a new instance of the <see cref="FileCabinetRecordCsvReader"/> class.</summary>
        /// <param name="reader">StreamReader.</param>
        public FileCabinetRecordCsvReader(StreamReader reader)
        {
            this.reader = reader ?? throw new ArgumentNullException(nameof(reader));
        }

        /// <summary>Reads all records from csv file.</summary>
        /// <returns>Returns IEnumerable of records.</returns>
        public IEnumerable<FileCabinetRecord> ReadAll()
        {
            this.reader.ReadLine();
            while (!this.reader.EndOfStream)
            {
                var recordString = this.reader.ReadLine().Split(',');
                yield return new FileCabinetRecord
                {
                    Id = int.Parse(recordString[0], CultureInfo.InvariantCulture),
                    FirstName = recordString[1],
                    LastName = recordString[2],
                    DateOfBirth = DateTime.Parse(recordString[3], CultureInfo.InvariantCulture),
                    WorkPlaceNumber = short.Parse(recordString[4], CultureInfo.InvariantCulture),
                    Salary = decimal.Parse(recordString[5], CultureInfo.InvariantCulture),
                    Department = char.Parse(recordString[6]),
                };
            }
        }
    }
}