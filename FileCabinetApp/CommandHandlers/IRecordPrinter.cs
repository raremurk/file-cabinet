using System.Collections.Generic;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>Record printer interface.</summary>
    public interface IRecordPrinter
    {
        /// <summary>Print records.</summary>
        /// <param name="records">FileCabinetRecords.</param>
        public void Print(IEnumerable<FileCabinetRecord> records);
    }
}