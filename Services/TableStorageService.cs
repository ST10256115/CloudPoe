namespace CloudPoe.Services
{
    using Azure.Data.Tables;

    public class TableStorageService
    {
        private readonly string _connectionString;

        public TableStorageService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public TableClient GetTableClient(string tableName)
        {
            var serviceClient = new TableServiceClient(_connectionString);
            return serviceClient.GetTableClient(tableName);
        }

        public async Task<IEnumerable<TableEntity>> GetAllProductsAsync()
        {
            var tableClient = GetTableClient("Products");
            var entities = new List<TableEntity>();

            await foreach (var entity in tableClient.QueryAsync<TableEntity>())
            {
                entities.Add(entity);
            }

            return entities;
        }




    }

}
