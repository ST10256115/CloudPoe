using Microsoft.AspNetCore.Mvc;
using CloudPoe.Models;
using CloudPoe.Services;
using System.Threading.Tasks;

namespace CloudPoe.Controllers
{
    public class OrderController : Controller
    {
        private readonly OrderService _orderService;
        private readonly ProductService _productService;
        private readonly CustomerService _customerService;
        private readonly QueueService _queueService;

        public OrderController(OrderService orderService, ProductService productService, CustomerService customerService, QueueService queueService)
        {
            _orderService = orderService;
            _productService = productService;
            _customerService = customerService;
            _queueService = queueService;
        }

        // GET: /Order/PlaceOrder
        [HttpGet]
        public IActionResult PlaceOrder()
        {
            // Return the view for placing an order
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(PlaceOrderViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var email = HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account");
            }

            var customer = await _customerService.GetCustomerByEmailAsync(email);
            if (customer == null)
            {
                return Unauthorized();
            }

            var product = await _productService.GetProductByIdAsync(model.ProductId);
            if (product == null)
            {
                ModelState.AddModelError(string.Empty, "Product not found.");
                return View(model);
            }

            var order = new Order
            {
                PartitionKey = email,
                RowKey = Guid.NewGuid().ToString(),
                ProductId = model.ProductId,
                Quantity = model.Quantity,
                OrderDate = DateTime.UtcNow,
                OrderStatus = "Pending"
            };

            await _orderService.AddOrderAsync(order);

            // Send a message to the queue with the order details
            await _queueService.SendMessageAsync($"Order placed: {order.RowKey}");

            return RedirectToAction("OrderConfirmation", new { orderId = order.RowKey });
        }

        // GET: /Order/OrderConfirmation
        [HttpGet]
        public async Task<IActionResult> OrderConfirmation(string orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
    }
}
