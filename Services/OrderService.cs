namespace CloudPoe.Services
{
    using Azure;
    using Azure.Data.Tables;
    using CloudPoe.Models;
    using Microsoft.Azure.Cosmos.Table;
    using System.Threading.Tasks;

    public class OrderService
    {
        private readonly TableClient _tableClient;

        public OrderService(TableServiceClient tableServiceClient)
        {
            _tableClient = tableServiceClient.GetTableClient("Orders");
            _tableClient.CreateIfNotExists();
        }

        public async Task AddOrderAsync(Order order)
        {
            await _tableClient.AddEntityAsync(order);
        }

        // Example method to get orders by email (optional)
        public async Task<IEnumerable<Order>> GetOrdersByEmailAsync(string email)
        {
            var query = _tableClient.QueryAsync<Order>(e => e.PartitionKey == email);
            var results = new List<Order>();

            await foreach (var order in query)
            {
                results.Add(order);
            }

            return results;
        }


        public async Task<Order> GetOrderByIdAsync(string orderId)
        {
            try
            {
                // Construct the filter to match the RowKey
                var filter = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, orderId);

                // Execute the query
                var response = _tableClient.QueryAsync<Order>(filter);

                await foreach (var order in response)
                {
                    // Return the first matching order
                    return order;
                }

                // If no matching order is found, return null
                return null;
            }
            catch (RequestFailedException ex)
            {
             
                return null;
            }
        }
    }

}
