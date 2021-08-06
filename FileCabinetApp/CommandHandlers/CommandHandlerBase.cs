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

        /// <summary>Handles the specified request.</summary>
        /// <param name="request">The request.</param>
        /// <param name="commandName">The command name.</param>
        /// <param name="command">The command.</param>
        public void Handle(AppCommandRequest request, string commandName, Action<string> command)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (command is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (string.Equals(commandName, request.Command, StringComparison.OrdinalIgnoreCase))
            {
                command(request.Parameters);
            }
            else
            {
                if (this.NextHandler != null)
                {
                    this.NextHandler.Handle(request);
                }
                else
                {
                    PrintMissedCommandInfo(request.Command);
                }
            }
        }

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