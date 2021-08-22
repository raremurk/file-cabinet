using System.Collections.ObjectModel;

namespace FileCabinetApp.Models
{
    /// <summary>Сlass representing service statistics.</summary>
    public class ServiceStat
    {
        /// <summary>Gets or sets list of ids of existing records.</summary>
        /// <value>List of ids of existing records.</value>
        public ReadOnlyCollection<int> ExistingRecordsIds { get; set; }

        /// <summary>Gets or sets list of ids of deleted records.</summary>
        /// <value>List of ids of deleted records.</value>
        public ReadOnlyCollection<int> DeletedRecordsIds { get; set; }
    }
}
