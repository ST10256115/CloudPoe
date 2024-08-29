namespace CloudPoe.Services
{
    using Azure.Storage.Queues;
    using Azure.Storage.Queues.Models;
    using System.Threading.Tasks;

    public class QueueService
    {
        private readonly QueueClient _queueClient;

        public QueueService(string connectionString, string queueName)
        {
            var queueServiceClient = new QueueServiceClient(connectionString);
            _queueClient = queueServiceClient.GetQueueClient(queueName);
            _queueClient.CreateIfNotExists();
        }

        public async Task SendMessageAsync(string message)
        {
            await _queueClient.SendMessageAsync(message);
        }

        public async Task<string> ReceiveMessageAsync()
        {
            var response = await _queueClient.ReceiveMessagesAsync(1);
            var message = response.Value.FirstOrDefault();

            if (message != null)
            {
                await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt);
                return message.MessageText;
            }

            return null;
        }
    }

}
