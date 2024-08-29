namespace CloudPoe.Services
{
    using Microsoft.Extensions.Hosting;
    using System.Threading;
    using System.Threading.Tasks;

    public class QueueProcessorService : BackgroundService
    {
        private readonly QueueService _queueService;

        public QueueProcessorService(QueueService queueService)
        {
            _queueService = queueService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var message = await _queueService.ReceiveMessageAsync();
                if (message != null)
                {
                   

                    Console.WriteLine($"Processed message: {message}");
                }

                await Task.Delay(50000000, stoppingToken); 
            }
        }
    }

}
