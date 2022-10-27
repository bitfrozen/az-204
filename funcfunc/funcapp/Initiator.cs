using System;
using System.Threading.Tasks;
using Azure;
using Azure.Messaging.EventGrid;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace func2func;

public static class Initiator
{
	private static readonly EventGridPublisherClient PublisherClient;
	  
	static Initiator()
	{
		string eventGridTopicUrl = Environment.GetEnvironmentVariable("EventGridTopicUrl");
		string eventGridAccessKey = Environment.GetEnvironmentVariable("EventGridAccessKey1");
		if (eventGridTopicUrl != null && eventGridAccessKey != null)
		{
			PublisherClient =
				new EventGridPublisherClient(new Uri(eventGridTopicUrl), new AzureKeyCredential(eventGridAccessKey));
		}
	}
	  
	[FunctionName("Initiator")]
	public static async Task<IActionResult> Run(
		[HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
		ILogger log)
	{
		log.LogInformation("Started Initiator function");
	    
		string postId = req.Query["postid"];
		if (string.IsNullOrWhiteSpace(postId))
		{
			return new BadRequestObjectResult("Specify postid when calling initiator");
		}
		log.LogInformation("Initiator function triggered with postid {PostId}", postId);

		await PublisherClient.SendEventAsync(CreateNewPostEvent(postId));
      
		return new OkObjectResult($"Called root with postid {postId}. Passing information along");
	}

	private static EventGridEvent CreateNewPostEvent(string postId)
	{
		var result = new EventGridEvent(
			"PostId",
			"Func.Post.Init",
			"1.0",
			postId
		);

		return result;
	}
}