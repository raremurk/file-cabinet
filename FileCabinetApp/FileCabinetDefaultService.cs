namespace FileCabinetApp
{
    /// <summary>Data validation by default parameters.</summary>
    public class FileCabinetDefaultService : FileCabinetService
    {
        /// <summary>Initializes a new instance of the <see cref="FileCabinetDefaultService"/> class.</summary>
        public FileCabinetDefaultService()
            : base(new DefaultValidator())
        {
        }
    }
}
