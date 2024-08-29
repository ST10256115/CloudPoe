namespace CloudPoe.Models
{
    using Azure;
    using Azure.Data.Tables;
    using System;

    public class Order : ITableEntity
    {
        public ETag ETag { get; set; }
        public DateTimeOffset? Timestamp { get; set; }

        public string PartitionKey { get; set; }  
        public string RowKey { get; set; }  
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderStatus { get; set; }
    }

}
