using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>Exit command handler.</summary>
    /// <seealso cref="CommandHandlerBase" />
    public class ExitCommandHandler : CommandHandlerBase
    {
        private const string ExitCommand = "exit";

        /// <summary>Handles the specified request.</summary>
        /// <param name="request">The request.</param>
        public override void Handle(AppCommandRequest request) => this.Handle(request, ExitCommand, Exit);

        private static void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            Program.isRunning = false;
        }
    }
}
