using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using FileCabinetApp.Models;

namespace FileCabinetGenerator
{
    /// <summary>Generates records.</summary>
    public static class RecordGenerator
    {
        private static readonly string[] FisrtNames =
        {
            "Emma", "Olivia", "Sophia", "Ava", "Isabella", "Mia", "Abigail", "Emily", "Charlotte", "Harper", "Madison", "Amelia", "Elizabeth", "Sofia", "Evelyn", "Avery", "Chloe",
            "Ella", "Grace", "Victoria", "Michael", "Christopher", "Matthew", "Joshua", "David", "James", "Daniel", "Robert", "John", "Joseph", "Jason", "Justin", "Andrew", "Ryan",
            "William", "Brian", "Brandon", "Jonathan", "Nicholas", "Anthony", "Eric", "Adam", "Kevin", "Thomas", "Steven", "Timothy", "Richard", "Jeremy", "Jeffrey", "Kyle",
        };

        private static readonly string[] LastNames =
        {
            "Smith", "Johnson", "Williams", "Jones", "Brown", "Davis", "Miller", "Wilson", "Moore", "Taylor", "Anderson", "Thomas", "Jackson", "White", "Harris", "Martin", "Thompson",
            "Garcia", "Martinez", "Robinson", "Clark", "Rodriguez", "Lewis", "Lee", "Walker", "Hall", "Allen", "Young", "Hernandez", "King", "Wright", "Lopez", "Hill", "Scott",
            "Green", "Adams", "Baker", "Gonzalez", "Nelson", "Carter", "Mitchell", "Perez", "Roberts", "Turner", "Phillips", "Campbell", "Parker", "Evans", "Edwards", "Collins",
        };

        /// <summary>Generates records.</summary>
        /// <param name="count">Records count.</param>
        /// <param name="startId">Start record Id.</param>
        /// <returns>Returns IEnumerable of FileCabinetRecords.</returns>
        public static IEnumerable<FileCabinetRecord> GetRandomRecords(int count, int startId)
        {
            DateTime start = new (1950, 1, 1);
            int range = (DateTime.Today - start).Days;

            for (int i = 0; i < count; i++)
            {
                yield return new FileCabinetRecord
                {
                    Id = startId++,
                    FirstName = FisrtNames[RandomNumberGenerator.GetInt32(FisrtNames.Length)],
                    LastName = LastNames[RandomNumberGenerator.GetInt32(LastNames.Length)],
                    DateOfBirth = start.AddDays(RandomNumberGenerator.GetInt32(range)),
                    WorkPlaceNumber = (short)RandomNumberGenerator.GetInt32(1, 1000),
                    Salary = (decimal)RandomNumberGenerator.GetInt32(300, 5001),
                    Department = (char)RandomNumberGenerator.GetInt32(65, 91),
                };
            }
        }
    }
}
