using Microsoft.AspNetCore.Mvc;

namespace CloudPoe.Controllers
{
    using CloudPoe.Services;
    using System.Threading.Tasks;

    public class AccountController : Controller
    {
        private readonly CustomerService _customerService;

        public AccountController(CustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError(string.Empty, "Email is required.");
                return View();
            }

            // Normalize the email to ensure case-insensitivity
            email = email.Trim().ToLower();

            // Attempt to retrieve the customer from the database using the email
            var customer = await _customerService.GetCustomerByEmailAsync(email);

            if (customer == null)
            {
                // 

                // If no matching customer was found, return an error
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View();
            }


            // If customer exists, save the email to the session
            HttpContext.Session.SetString("UserEmail", customer.Email);

            // Redirect to the home page or wherever appropriate after login
            return RedirectToAction("PlaceOrder", "Order");
        }




        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserEmail");
            HttpContext.Session.Remove("UserPhone");
            return RedirectToAction("Login");
        }
    }

}
