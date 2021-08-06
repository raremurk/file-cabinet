using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>Service command handler base.</summary>
    /// <seealso cref="CommandHandlerBase" />
    public abstract class ServiceCommandHandlerBase : CommandHandlerBase
    {
        /// <summary>File cabinet service.</summary>
        #pragma warning disable SA1401
        private protected readonly IFileCabinetService fileCabinetService;
        #pragma warning restore SA1401

        /// <summary>Initializes a new instance of the <see cref="ServiceCommandHandlerBase"/> class.</summary>
        /// <param name="fileCabinetService">The file cabinet service.</param>
        /// <exception cref="ArgumentNullException">Thrown when fileCabinetService is null.</exception>
        protected ServiceCommandHandlerBase(IFileCabinetService fileCabinetService)
        {
            this.fileCabinetService = fileCabinetService ?? throw new ArgumentNullException(nameof(fileCabinetService));
        }
    }
}
