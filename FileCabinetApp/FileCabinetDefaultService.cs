namespace FileCabinetApp
{
    /// <summary>Data validation by default parameters.</summary>
    public class FileCabinetDefaultService : FileCabinetService
    {
        /// <summary>Record validation.</summary>
        /// <returns>Validation interface.</returns>
        public override IRecordValidator CreateValidator()
        {
            return new DefaultValidator();
        }
    }
}
