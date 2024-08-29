using Azure.Data.Tables;
using Azure.Identity;
using Azure.Storage.Blobs;
using CloudPoe.Services;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Register TableStorageService
builder.Services.AddSingleton<TableStorageService>(sp =>
    new TableStorageService(builder.Configuration.GetConnectionString("AzureStorageConnectionString")));

builder.Services.AddSingleton<ProductService>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    return new ProductService(configuration.GetConnectionString("AzureStorageConnectionString"));
});
// Add session services
builder.Services.AddDistributedMemoryCache(); // Adds a default in-memory implementation of IDistributedCache
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // Make the session cookie essential for the application
});


// Add services to the container.
builder.Services.AddControllersWithViews();

// Add FileService
builder.Services.AddSingleton(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetValue<string>("ConnectionStrings:AzureStorageConnectionString");
    var shareName = configuration.GetValue<string>("ConnectionStrings:FileShareName");

    if (string.IsNullOrWhiteSpace(connectionString) || string.IsNullOrWhiteSpace(shareName))
    {
        throw new InvalidOperationException("Azure Storage connection string or file share name is missing.");
    }

    return new FileService(connectionString, shareName);
});

// Add QueueService
builder.Services.AddSingleton(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("AzureStorageConnectionString");
    var queueName = "poe-queue"; 
    return new QueueService(connectionString, queueName);
});

// Add background queue processor
builder.Services.AddHostedService<QueueProcessorService>();


// Retrieve the connection string from configuration
string azureStorageConnectionString = builder.Configuration.GetConnectionString("AzureStorageConnectionString");

// Instantiate the TableServiceClient with the connection string
var tableServiceClient = new TableServiceClient(azureStorageConnectionString);

// Register your services using the instantiated TableServiceClient
builder.Services.AddSingleton<OrderService>(provider =>
{
    return new OrderService(tableServiceClient);
});


builder.Services.AddSingleton<CustomerService>(provider =>
{
    return new CustomerService(tableServiceClient);
});

var app = builder.Build();

app.UseSession();

// Ensure Azure Tables are created at startup
EnsureTablesCreated(builder.Configuration);



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}



app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

void EnsureTablesCreated(IConfiguration configuration)
{
    string connectionString = configuration.GetConnectionString("AzureStorageConnectionString");

    // Create the TableServiceClient
    TableServiceClient serviceClient = new TableServiceClient(connectionString);

    // Create Products Table
    TableClient productsTable = serviceClient.GetTableClient("Products");
    productsTable.CreateIfNotExists();

    // Create Customers Table
    TableClient customersTable = serviceClient.GetTableClient("Customers");
    customersTable.CreateIfNotExists();

    // Create Orders Table
    TableClient ordersTable = serviceClient.GetTableClient("Orders");
    ordersTable.CreateIfNotExists();
}
