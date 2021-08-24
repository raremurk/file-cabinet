using System;
using System.Collections.Generic;
using System.Linq;
using FileCabinetApp.Helpers;
using FileCabinetApp.Models;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>Select command handler.</summary>
    /// <seealso cref="ServiceCommandHandlerBase" />
    public class SelectCommandHandler : ServiceCommandHandlerBase
    {
        private const string SelectCommand = "select";
        private readonly Action<IEnumerable<FileCabinetRecord>, BoolRecord> printer;
        private readonly IRecordValidator validator;

        /// <summary>Initializes a new instance of the <see cref="SelectCommandHandler"/> class.</summary>
        /// <param name="fileCabinetService">IFileCabinetService.</param>
        /// <param name="printer">IRecordPrinter.</param>
        /// <param name="validator">IRecordValidator.</param>
        public SelectCommandHandler(IFileCabinetService fileCabinetService, Action<IEnumerable<FileCabinetRecord>, BoolRecord> printer, IRecordValidator validator)
            : base(fileCabinetService)
        {
            this.printer = printer ?? throw new ArgumentNullException(nameof(printer));
            this.validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        /// <inheritdoc cref="CommandHandlerBase.Handle(AppCommandRequest)"/>
        public override void Handle(AppCommandRequest request) => this.Handle(request, SelectCommand, this.Select);

        private void Select(string parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters))
            {
                var records = this.fileCabinetService.GetRecords();
                this.printer(records, new BoolRecord());
                return;
            }

            string[] recordProperties = { "id", "firstName", "lastName", "dateOfBirth", "workPlaceNumber", "salary", "department" };

            string[] inputs = parameters.Split(new char[] { '=', ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int indexOfWhere = Array.FindIndex(inputs, x => x.Equals("where", StringComparison.OrdinalIgnoreCase));
            string[] columns = indexOfWhere == -1 ? inputs : inputs[0..indexOfWhere];
            var propertiesToPrint = recordProperties.Intersect(columns, StringComparer.OrdinalIgnoreCase);

            var format = new BoolRecord();
            if (propertiesToPrint.Any())
            {
                format.Id = propertiesToPrint.Contains(recordProperties[0]);
                format.FirstName = propertiesToPrint.Contains(recordProperties[1]);
                format.LastName = propertiesToPrint.Contains(recordProperties[2]);
                format.DateOfBirth = propertiesToPrint.Contains(recordProperties[3]);
                format.WorkPlaceNumber = propertiesToPrint.Contains(recordProperties[4]);
                format.Salary = propertiesToPrint.Contains(recordProperties[5]);
                format.Department = propertiesToPrint.Contains(recordProperties[6]);
            }

            if (indexOfWhere != -1)
            {
                var parser = new Parser(this.validator);
                RecordToSearch recordToSearch = parser.GetRecordToSearchFromString(inputs[(indexOfWhere + 1) ..]);
                var records = this.fileCabinetService.Search(recordToSearch);
                this.printer(records, format);
            }
            else
            {
                var records = this.fileCabinetService.GetRecords();
                this.printer(records, format);
            }
        }
    }
}