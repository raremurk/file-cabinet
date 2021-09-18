namespace FileCabinetApp.Models
{
    /// <summary>Сlass representing service statistics.</summary>
    public class ServiceStat
    {
        /// <summary>Gets or sets all records count.</summary>
        /// <value>All records count.</value>
        public int AllRecordsCount { get; set; }

        /// <summary>Gets or sets deleted records count.</summary>
        /// <value>Deleted records count.</value>
        public int DeletedRecordsCount { get; set; }

        /// <summary>String representation of ServiceStat.</summary>
        /// <returns>Returns string representation of ServiceStat.</returns>
        public override string ToString() => $"'{this.AllRecordsCount}' record(s). '{this.DeletedRecordsCount}' of them are deleted.";
    }
}
