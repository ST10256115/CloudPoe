using Microsoft.AspNetCore.Mvc;
using Azure.Data.Tables;
using System;
using System.Threading.Tasks;
using CloudPoe.Models;
using CloudPoe.Services;

namespace CloudPoe.Controllers
{
    public class CustomerController : Controller
    {
        private readonly TableStorageService _tableStorageService;

        public CustomerController(TableStorageService tableStorageService)
        {
            _tableStorageService = tableStorageService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(CustomerViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Generate a unique PartitionKey and RowKey
                string partitionKey = model.Email;  // Use the email for partitioning
                string rowKey = $"email_{model.Email}";  // Use a structured row key format

                // Prepare the entity
                var customerEntity = new TableEntity(partitionKey, rowKey)
                {
                    ["CustomerFirstName"] = model.CustomerFirstName,
                    ["CustomerSecondName"] = model.CustomerSecondName,
                    ["Email"] = model.Email,
                    ["Phone"] = model.Phone
                };

                // Save the entity to Table Storage
                var tableClient = _tableStorageService.GetTableClient("Customers");
                await tableClient.CreateIfNotExistsAsync();
                await tableClient.UpsertEntityAsync(customerEntity);

                // Redirect to a success page or another action
                return RedirectToAction("RegisterSuccess");
            }

            // If model state is invalid, return the same view with validation messages
            return View(model);
        }

        public IActionResult RegisterSuccess()
        {
            return View();
        }
    }
}
