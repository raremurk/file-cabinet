using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>Command handler base.</summary>
    /// <seealso cref="ICommandHandler" />
    public abstract class CommandHandlerBase : ICommandHandler
    {
        /// <summary>Gets the next handler.</summary>
        /// <value>The next handler.</value>
        public ICommandHandler NextHandler { get; private set; }

        /// <summary>Handles the specified request.</summary>
        /// <param name="request">The request.</param>
        public abstract void Handle(AppCommandRequest request);

        /// <summary>Sets the next handler.</summary>
        /// <param name="handler">The handler.</param>
        /// <exception cref="ArgumentNullException">Thrown when handler is null.</exception>
        public void SetNext(ICommandHandler handler)
        {
            this.NextHandler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        /// <summary>Print missed command info.</summary>
        /// <param name="command">The command.</param>
        protected static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }
    }
}