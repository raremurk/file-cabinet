using System;
using System.Globalization;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>Create command handler.</summary>
    /// <seealso cref="ServiceCommandHandlerBase" />
    public class CreateCommandHandler : ServiceCommandHandlerBase
    {
        private const string CreateCommand = "create";
        private readonly IRecordValidator validator;

        /// <summary>Initializes a new instance of the <see cref="CreateCommandHandler"/> class.</summary>
        /// <param name="fileCabinetService">IFileCabinetService.</param>
        /// <param name="validator">IRecordValidator.</param>
        public CreateCommandHandler(IFileCabinetService fileCabinetService, IRecordValidator validator)
            : base(fileCabinetService)
        {
            this.validator = validator;
        }

        /// <summary>Handles the specified request.</summary>
        /// <param name="request">The request.</param>
        public override void Handle(AppCommandRequest request) => this.Handle(request, CreateCommand, this.Create);

        private void Create(string parameters)
        {
            FileCabinetRecord record = new GetRecordFromConsole(this.validator).СonsoleInput();
            int id = this.fileCabinetService.CreateRecord(record);
            Console.WriteLine($"Record #{id} is created.");
        }
    }
}
