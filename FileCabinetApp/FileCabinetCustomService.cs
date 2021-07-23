namespace FileCabinetApp
{
    /// <summary>Data validation by custom parameters.</summary>
    public class FileCabinetCustomService : FileCabinetService
    {
        /// <summary>Record validation.</summary>
        /// <returns>Validation interface.</returns>
        public override IRecordValidator CreateValidator()
        {
            return new CustomValidator();
        }
    }
}
