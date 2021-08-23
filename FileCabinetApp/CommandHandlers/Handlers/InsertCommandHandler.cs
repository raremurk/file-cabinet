using System;
using System.Collections.Generic;
using FileCabinetApp.Helpers;
using FileCabinetApp.Models;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>Insert command handler.</summary>
    /// <seealso cref="ServiceCommandHandlerBase" />
    public class InsertCommandHandler : ServiceCommandHandlerBase
    {
        private const string InsertCommand = "insert";
        private readonly IRecordValidator validator;

        /// <summary>Initializes a new instance of the <see cref="InsertCommandHandler"/> class.</summary>
        /// <param name="fileCabinetService">IFileCabinetService.</param>
        /// <param name="validator">IRecordValidator.</param>
        public InsertCommandHandler(IFileCabinetService fileCabinetService, IRecordValidator validator)
            : base(fileCabinetService)
        {
            this.validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        /// <inheritdoc cref="CommandHandlerBase.Handle(AppCommandRequest)"/>
        public override void Handle(AppCommandRequest request) => this.Handle(request, InsertCommand, this.Insert);

        private static List<string> ParseProperties(string[] recordProperties, string[] properties, string[] values)
        {
            List<string> propertyValues = new ();
            for (int i = 0; i < recordProperties.Length; i++)
            {
                int index = Array.FindIndex(properties, x => x.Equals(recordProperties[i], StringComparison.OrdinalIgnoreCase));
                if (index == -1)
                {
                    Console.WriteLine($"This property is not set : {recordProperties[i]}.");
                    return new List<string>();
                }

                if (values[index].Length < 3 || values[index][0] != '\'' || values[index][^1] != '\'')
                {
                    Console.WriteLine("Values must be in single quotes. Example: ('value1', 'value2', ...)");
                    return new List<string>();
                }

                propertyValues.Add(values[index].Trim('\''));
            }

            return propertyValues;
        }

        private void Insert(string parameters)
        {
            if (string.IsNullOrEmpty(parameters))
            {
                Console.WriteLine("Input parameters. Example: insert (Property1, Property2, ...) values ('value1', 'value2', ...)");
                return;
            }

            string separator = "values";
            var inputs = parameters.Split(separator, 2);

            if (inputs.Length != 2)
            {
                Console.WriteLine("Invalid input. Example: insert (Property1, Property2, ...) values ('value1', 'value2', ...)");
                return;
            }

            string propertiesSection = inputs[0].Trim();
            string valuesSection = inputs[1].Trim();
            if (propertiesSection[0] != '(' || propertiesSection[^1] != ')' || valuesSection[0] != '(' || valuesSection[^1] != ')')
            {
                Console.WriteLine("You forgot to open or close a parenthesis. Example: insert (Property1, Property2, ...) values ('value1', 'value2', ...).");
                return;
            }

            propertiesSection = propertiesSection[1..^1];
            valuesSection = valuesSection[1..^1];
            string[] recordProperties = { "id", "firstName", "lastName", "dateOfBirth", "workPlaceNumber", "salary", "department" };
            int propertiesCount = recordProperties.Length;

            string[] properties = propertiesSection.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string[] values = valuesSection.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (properties.Length != propertiesCount || values.Length != propertiesCount)
            {
                Console.WriteLine("All properties and their values must be specified (id, firstname, lastname, dateofbirth, workplacenumber, salary, department).");
                return;
            }

            List<string> propertyValues = ParseProperties(recordProperties, properties, values);
            if (propertyValues.Count == 0)
            {
                return;
            }

            var resultConversionId = Converter.IntConverter(propertyValues[0]);
            if (resultConversionId.Item3 <= 0)
            {
                Console.WriteLine($"Invalid Id.");
                return;
            }

            if (this.fileCabinetService.GetRecord(resultConversionId.Item3) != null)
            {
                Console.WriteLine($"Record with ID = '{resultConversionId.Item3}' already exists.");
                return;
            }

            string firstName = propertyValues[1];
            string lastName = propertyValues[2];
            var resultConversionDateOfBirth = Converter.DateTimeConverter(propertyValues[3]);
            var resultConversionWorkPlaceNumber = Converter.ShortConverter(propertyValues[4]);
            var resultConversionSalary = Converter.DecimalConverter(propertyValues[5]);
            var resultConversionDepartment = Converter.CharConverter(propertyValues[6]);

            Tuple<bool, string>[] conversionResults =
            {
                new (resultConversionId.Item1, resultConversionId.Item2),
                new (resultConversionDateOfBirth.Item1, resultConversionDateOfBirth.Item2),
                new (resultConversionWorkPlaceNumber.Item1, resultConversionWorkPlaceNumber.Item2),
                new (resultConversionSalary.Item1, resultConversionSalary.Item2),
                new (resultConversionDepartment.Item1, resultConversionDepartment.Item2),
            };

            foreach (var result in conversionResults)
            {
                if (!result.Item1)
                {
                    Console.WriteLine(result.Item2);
                    return;
                }
            }

            var record = new FileCabinetRecord
            {
                Id = resultConversionId.Item3,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = resultConversionDateOfBirth.Item3,
                WorkPlaceNumber = resultConversionWorkPlaceNumber.Item3,
                Salary = resultConversionSalary.Item3,
                Department = resultConversionDepartment.Item3,
            };

            var validationResult = this.validator.ValidateRecord(record);

            if (!validationResult.Item1)
            {
                Console.WriteLine(validationResult.Item2);
                return;
            }

            this.fileCabinetService.CreateRecord(record);
            Console.WriteLine($"Record with ID = '{record.Id}' added.");
        }
    }
}