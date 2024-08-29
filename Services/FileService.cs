namespace CloudPoe.Services
{
    using Azure.Storage.Files.Shares;
    using Azure.Storage.Files.Shares.Models;
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public class FileService
    {
        private readonly ShareClient _shareClient;

        public FileService(string connectionString, string shareName)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString), "Connection string cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(shareName))
                throw new ArgumentNullException(nameof(shareName), "Share name cannot be null or empty.");

            _shareClient = new ShareClient(connectionString, shareName);
            _shareClient.CreateIfNotExists();
        }

        public async Task UploadFileAsync(string filePath, Stream fileStream)
        {
            var fileClient = GetFileClient(filePath);
            await fileClient.CreateAsync(fileStream.Length);
            await fileClient.UploadAsync(fileStream);
        }

        public async Task<Stream> DownloadFileAsync(string filePath)
        {
            var fileClient = GetFileClient(filePath);
            var downloadResponse = await fileClient.DownloadAsync();
            return downloadResponse.Value.Content;
        }

        private ShareFileClient GetFileClient(string filePath)
        {
            // Create a directory client if your files are organized in directories.
            var directoryClient = _shareClient.GetDirectoryClient("");
            return directoryClient.GetFileClient(filePath);
        }
    }


}
