using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>Exit command handler.</summary>
    /// <seealso cref="CommandHandlerBase" />
    public class ExitCommandHandler : CommandHandlerBase
    {
        private const string ExitCommand = "exit";
        private readonly Action<bool> setProgramStatus;

        /// <summary>Initializes a new instance of the <see cref="ExitCommandHandler"/> class.</summary>
        /// <param name="setProgramStatus">Sets program status.</param>
        /// <exception cref="ArgumentNullException">Thrown when setProgramStatus is null.</exception>
        public ExitCommandHandler(Action<bool> setProgramStatus)
        {
            this.setProgramStatus = setProgramStatus ?? throw new ArgumentNullException(nameof(setProgramStatus));
        }

        /// <inheritdoc cref="CommandHandlerBase.Handle(AppCommandRequest)"/>
        public override void Handle(AppCommandRequest request) => this.Handle(request, ExitCommand, this.Exit);

        private void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            this.setProgramStatus(false);
        }
    }
}
