using System;
using System.Collections.Generic;

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
            this.validator = validator;
        }

        /// <summary>Handles the specified request.</summary>
        /// <param name="request">The request.</param>
        public override void Handle(AppCommandRequest request) => this.Handle(request, InsertCommand, this.Insert);

        private void Insert(string parameters)
        {
            if (string.IsNullOrEmpty(parameters))
            {
                Console.WriteLine("Invalid input. Example: insert (Property1, Property2, ...) values ('value1', 'value2', ...).");
                return;
            }

            string separator = "values";
            var inputs = parameters.Split(separator, 2);

            if (inputs.Length != 2)
            {
                Console.WriteLine("Invalid input. Example: insert (Property1, Property2, ...) values ('value1', 'value2', ...).");
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
            var recordProperties = typeof(FileCabinetRecord).GetProperties();
            int propertiesCount = recordProperties.Length;

            string[] properties = propertiesSection.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string[] values = valuesSection.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (properties.Length != propertiesCount || values.Length != propertiesCount)
            {
                Console.WriteLine("All properties and their values must be specified (id, firstname, lastname, dateofbirth, workplacenumber, salary, department).");
                return;
            }

            List<string> propertyValues = new ();
            for (int i = 0; i < propertiesCount; i++)
            {
                int index = Array.FindIndex(properties, x => x.Equals(recordProperties[i].Name, StringComparison.OrdinalIgnoreCase));
                if (index == -1)
                {
                    Console.WriteLine($"This property is not set : {recordProperties[i].Name}.");
                    return;
                }

                if (values[index].Length < 3 || values[index][0] != '\'' || values[index][^1] != '\'')
                {
                    Console.WriteLine("Values must be in single quotes. Example: ('value1', 'value2', ...)");
                    return;
                }

                propertyValues.Add(values[index].Trim('\''));
            }

            var stringRecord = new StringFileCabinetRecord
            {
                Id = propertyValues[0],
                FirstName = propertyValues[1],
                LastName = propertyValues[2],
                DateOfBirth = propertyValues[3],
                WorkPlaceNumber = propertyValues[4],
                Salary = propertyValues[5],
                Department = propertyValues[6],
            };

            var conversionResult = Converter.RecordConverter(stringRecord);
            if (!conversionResult.Item1)
            {
                Console.WriteLine(conversionResult.Item2);
                return;
            }

            FileCabinetRecord record = conversionResult.Item3;
            if (this.fileCabinetService.IdExists(record.Id))
            {
                Console.WriteLine($"Record with ID = '{record.Id}' already exists.");
                return;
            }

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