using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>Application command request.</summary>
    public class AppCommandRequest
    {
        /// <summary>Initializes a new instance of the <see cref="AppCommandRequest"/> class.</summary>
        /// <param name="command">Command.</param>
        /// <param name="parameters">Parameters.</param>
        /// <exception cref="ArgumentNullException">Thrown when command or parameters is null.</exception>
        public AppCommandRequest(string command, string parameters)
        {
            this.Command = command ?? throw new ArgumentNullException(nameof(command));
            this.Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
        }

        /// <summary>Gets the command.</summary>
        /// <value>The command.</value>
        public string Command { get; }

        /// <summary>Gets the parameters.</summary>
        /// <value>The parameters.</value>
        public string Parameters { get; }
    }
}
