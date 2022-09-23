using System;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace func
{
	public static class FileParser
	{
		[FunctionName("FileParser")]
		public static async Task<IActionResult> Run(
				[HttpTrigger("GET")] HttpRequest request, ILogger logger)
		{
			string connectionString = Environment.GetEnvironmentVariable("StorageConnectionString");

			var blob = new BlobClient(connectionString, "drop", "records.json");
			var response = await blob.DownloadAsync();
			
			return new FileStreamResult(response?.Value?.Content, response?.Value?.ContentType);
		}
	}
}
