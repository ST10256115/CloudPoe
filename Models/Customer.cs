namespace CloudPoe.Models
{
    using Azure;
    using Azure.Data.Tables;

    public class Customer : ITableEntity
    {
        public string PartitionKey { get; set; }  // Email (Partition Key)
        public string RowKey { get; set; }  // Phone (Row Key)
        public string Email { get; set; }
        public string Phone { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

    

            public bool NotFound { get; set; }
        
        // Required for the Table Entity interface
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }

}
