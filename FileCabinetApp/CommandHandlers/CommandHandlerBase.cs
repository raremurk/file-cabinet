using System;
using System.Collections.Generic;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>Command handler base.</summary>
    /// <seealso cref="ICommandHandler" />
    public abstract class CommandHandlerBase : ICommandHandler
    {
        private static readonly string[] Commands = { "create", "update", "delete", "insert", "select", "stat", "import", "export", "purge", "help", "exit" };

        /// <summary>Gets the next handler.</summary>
        /// <value>The next handler.</value>
        public ICommandHandler NextHandler { get; private set; }

        /// <inheritdoc cref="ICommandHandler.Handle(AppCommandRequest)"/>
        public abstract void Handle(AppCommandRequest request);

        /// <summary>Handles the specified request.</summary>
        /// <param name="request">The request.</param>
        /// <param name="commandName">The command name.</param>
        /// <param name="command">The command.</param>
        public void Handle(AppCommandRequest request, string commandName, Action<string> command)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));
            _ = command ?? throw new ArgumentNullException(nameof(request));

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

        /// <inheritdoc cref="ICommandHandler.SetNext(ICommandHandler)"/>
        /// <exception cref="ArgumentNullException">Thrown when handler is null.</exception>
        public void SetNext(ICommandHandler handler)
        {
            this.NextHandler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        /// <summary>Print missed command info.</summary>
        /// <param name="command">The command.</param>
        protected static void PrintMissedCommandInfo(string command)
        {
            List<string> similarCommands = new ();
            if (!string.IsNullOrWhiteSpace(command))
            {
                foreach (var item in Commands)
                {
                    if (item.StartsWith(command[0]) || item.Contains(command, StringComparison.InvariantCulture) || command.Contains(item, StringComparison.InvariantCulture))
                    {
                        similarCommands.Add(item);
                    }
                }
            }

            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
            if (similarCommands.Count != 0)
            {
                string message = similarCommands.Count < 2 ? "The most similar command is" : "The most similar commands are";
                Console.WriteLine(message);
                foreach (var item in similarCommands)
                {
                    Console.WriteLine($"\t{item}");
                }
            }
            else
            {
                Console.WriteLine("Could not find a similar command.");
            }
        }
    }
}