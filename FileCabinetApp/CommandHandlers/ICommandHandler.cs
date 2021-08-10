namespace FileCabinetApp.CommandHandlers
{
    /// <summary>Command handler interface.</summary>
    public interface ICommandHandler
    {
        /// <summary>Sets the next handler.</summary>
        /// <param name="handler">The handler.</param>
        public void SetNext(ICommandHandler handler);

        /// <summary>Handles the specified request.</summary>
        /// <param name="request">The request.</param>
        public void Handle(AppCommandRequest request);
    }
}