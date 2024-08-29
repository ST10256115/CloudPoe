namespace CloudPoe.Services
{
    using Azure;
    using Azure.Data.Tables;
    using System.Threading.Tasks;
    using CloudPoe.Models;
    using Microsoft.Azure.Cosmos.Table;

    public class CustomerService
    {
        private readonly TableClient _tableClient;

        public CustomerService(TableServiceClient tableServiceClient)
        {
            _tableClient = tableServiceClient.GetTableClient("Customers");
            _tableClient.CreateIfNotExists();
        }

        public async Task<Customer> GetCustomerByEmailAsync(string email)
        {
            try
            {
                // Normalize the email to lowercase and trim any spaces
                email = email.Trim().ToLower();

                // Construct the RowKey with the proper format
                var rowKey = $"email_{email}";

                // Construct the filter for PartitionKey and RowKey
                string filter = $"PartitionKey eq '{email}' and RowKey eq '{rowKey}'";

                // Execute the query
                var customers = _tableClient.QueryAsync<Customer>(filter);

                // Return the first customer found, or null if no match
                await foreach (var customer in customers)
                {
                    return customer;
                }

                // If no customer is found, return null
                return null;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return null;
            }
        }


    }

}
