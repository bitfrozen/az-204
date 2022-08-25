using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;

namespace BlobManager
{
    public class Program
    {
        private static string blobServiceEndpoint = "";
        private static string storageAccountName = "";
        private static string storageAccountKey = "";

        public static async Task Main()
        {
            GetConfiguration();

            StorageSharedKeyCredential accountCredentials = new(storageAccountName, storageAccountKey);
            BlobServiceClient serviceClient = new(new Uri(blobServiceEndpoint), accountCredentials);
            AccountInfo info = await serviceClient.GetAccountInfoAsync();
            await Console.Out.WriteLineAsync($"Connected to {info.AccountKind} Azure Storage Account");
            await Console.Out.WriteLineAsync($"Account name:\t{storageAccountName}");
            await Console.Out.WriteLineAsync($"Account sku:\t{info?.SkuName}");

            string newContainerName = "vector-graphics";
            BlobContainerClient containerClient = await GetContainerAsync(serviceClient, newContainerName);

            await EnumerateContainersAsync(serviceClient);

            string uploadedBlobName = "graph.svg";
            BlobClient blobClient = await GetBlobAsync(containerClient, uploadedBlobName);
            await Console.Out.WriteLineAsync($"Blob url:\t{blobClient.Uri}");
        }

        private static async Task EnumerateContainersAsync(BlobServiceClient client)
        {
            await foreach (BlobContainerItem container in client.GetBlobContainersAsync())
            {
                await Console.Out.WriteLineAsync($"Container:\t{container.Name}");
                await EnumerateBlobsAsync(client, container.Name);
            }
        }

        private static async Task EnumerateBlobsAsync(BlobServiceClient client, string containerName)
        {
            BlobContainerClient container = client.GetBlobContainerClient(containerName);
            await Console.Out.WriteLineAsync($"Searching:\t{container.Name}");
            await foreach(BlobItem blob in container.GetBlobsAsync())
            {
                await Console.Out.WriteLineAsync($"Existing blob:\t{blob.Name}");
            }
        }

        private static async Task<BlobContainerClient> GetContainerAsync(BlobServiceClient client, string containerName)
        {
            BlobContainerClient container = client.GetBlobContainerClient(containerName);
            await container.CreateIfNotExistsAsync(PublicAccessType.Blob);
            await Console.Out.WriteLineAsync($"New container:\t{container.Name}");

            return container;
        }

        private static async Task<BlobClient> GetBlobAsync(BlobContainerClient client, string blobName)
        {
            BlobClient blob = client.GetBlobClient(blobName);
            await Console.Out.WriteLineAsync($"Blob found:\t{blob.Name}");
            return blob;
        }

        private static void GetConfiguration()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.json")
                .AddEnvironmentVariables("BlobMngr_");
            var config = configuration.Build();
            blobServiceEndpoint = config.GetRequiredSection("StorageBlobEndpoint").Value;
            storageAccountName = config.GetRequiredSection("StorageAccountName").Value;
            // Value should be stored in environtment variable BLOBMNGR_STORAGEACCOUNTKEY
            storageAccountKey = config.GetRequiredSection("StorageAccountKey").Value;
        }
    }
}
