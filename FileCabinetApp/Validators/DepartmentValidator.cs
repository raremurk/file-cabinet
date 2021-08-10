﻿using System;

namespace FileCabinetApp.Validators
{
    /// <summary>Department validator.</summary>
    public static class DepartmentValidator
    {
        /// <summary>Department validation.</summary>
        /// <param name="department">Department.</param>
        /// <returns>Returns false and exception message if department is incorrect, else returns true.</returns>
        public static Tuple<bool, string> ValidateParameter(char department)
        {
            bool valid = char.IsLetter(department) && char.IsUpper(department);
            string message = !valid ? $"{nameof(department)} can only be uppercase letter." : string.Empty;
            return new (valid, message);
        }
    }
}