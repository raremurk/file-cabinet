namespace FileCabinetApp.Models
{
    /// <summary>Id and position of record in fileStream.</summary>
    public class RecordInfo
    {
        /// <summary>Gets or sets the id.</summary>
        /// <value>Id of record.</value>
        public int Id { get; set; }

        /// <summary>Gets or sets the id.</summary>
        /// <value>Deleted records count.</value>
        public long RecordPosition { get; set; }
    }
}
