using Microsoft.AspNetCore.Mvc;
using CloudPoe.Services; // Adjust to your namespace
using System.Threading.Tasks;
using CloudPoe.Services;

namespace YourNamespace.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ProductService _productService;

        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }

        // Action to list all products
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();
            return View(products);
        }

        // Action to show details of a single product
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _productService.GetAllProductsAsync();
            var product = products.FirstOrDefault(p => p.ProductID == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
    }
}
