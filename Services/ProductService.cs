using Azure.Data.Tables;
using System.Collections.Generic;
using System.Threading.Tasks;
using CloudPoe.Models;
using Azure;

namespace CloudPoe.Services
{
    public class ProductService
    {
        private readonly TableClient _productsTableClient;

        public ProductService(string connectionString)
        {
            _productsTableClient = new TableClient(connectionString, "Products");
        }

        public async Task<List<ProductViewModel>> GetAllProductsAsync()
        {
            var products = new List<ProductViewModel>();
            await foreach (var entity in _productsTableClient.QueryAsync<TableEntity>())
            {
                var product = new ProductViewModel
                {
                    ProductID = entity.RowKey,
                    ProductName = entity.GetString("ProductName"),
                    ProductDesc = entity.GetString("ProductDesc"),
                    ProductPrice = entity.GetDouble("Price") ?? 0.0, // Default to 0.0 if null
                    ImageURL = entity.GetString("ImageURL")
                };
                products.Add(product);
            }
            return products;
        }

        public async Task<ProductViewModel> GetProductByIdAsync(string productId)
        {
            try
            {
                var entity = await _productsTableClient.GetEntityAsync<TableEntity>(partitionKey: productId, rowKey: productId);
                if (entity.Value != null)
                {
                    var product = new ProductViewModel
                    {
                        ProductID = entity.Value.RowKey,
                        ProductName = entity.Value.GetString("ProductName"),
                        ProductDesc = entity.Value.GetString("ProductDesc"),
                        ImageURL = entity.Value.GetString("ImageURL"),
                        ProductPrice = entity.Value.GetDouble("Price") ?? 0.0 // Default to 0.0 if null
                    };
                    return product;
                }
                return null;
            }
            catch (RequestFailedException)
            {
                return null;
            }
        }
    }
}
