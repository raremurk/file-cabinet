using System.Collections.ObjectModel;

namespace FileCabinetApp
{
    /// <summary>Сlass representing service statistics.</summary>
    public class ServiceStat
    {
        /// <summary>Gets or sets number of records.</summary>
        /// <value>Number of records.</value>
        public int NumberOfRecords { get; set; }

        /// <summary>Gets or sets list of ids of deleted records.</summary>
        /// <value>List of ids of deleted records.</value>
        public ReadOnlyCollection<int> DeletedRecordsIds { get; set; }
    }
}
