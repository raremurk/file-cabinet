namespace FileCabinetApp
{
    /// <summary>Data validation by custom parameters.</summary>
    public class FileCabinetCustomService : FileCabinetService
    {
        /// <summary>Initializes a new instance of the <see cref="FileCabinetCustomService"/> class.</summary>
        public FileCabinetCustomService()
            : base(new CustomValidator())
        {
        }
    }
}
