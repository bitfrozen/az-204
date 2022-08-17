using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly Options _options;
        private readonly ILogger<ImagesController> _logger;

        public ImagesController(ILogger<ImagesController> logger, Options options)
        {
	        _logger = logger;
          _options = options;
        }

        private async Task<BlobContainerClient> GetCloudBlobContainer(string containerName)
        {
            BlobServiceClient blobServiceClient = new(_options.StorageConnectionString);
            BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
            await blobContainerClient.CreateIfNotExistsAsync();

            return blobContainerClient;
        }

        // GET: api/Images
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            string containerName = _options.FullImageContainerName ?? "";
            BlobContainerClient containerClient = await GetCloudBlobContainer(containerName);
            List<string> results = new();
            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
            {
                results.Add(Flurl.Url.Combine(containerClient.Uri.AbsoluteUri, blobItem.Name));
            }

            _logger.LogInformation("Got Images");

            return Ok(results);
        }

        // POST api/Images
        [HttpPost]
        public async Task<ActionResult> Post()
        {
            var containerName = _options.FullImageContainerName ?? "";
            Stream image = Request.Body;
            BlobContainerClient blobContainerClient = await GetCloudBlobContainer(containerName);
            string blobName = Guid.NewGuid().ToString().ToLower().Replace("-", string.Empty);
            BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);
            await blobContainerClient.UploadBlobAsync(blobName, image);

            return Created(blobClient.Uri, null);
        }
    }
}
