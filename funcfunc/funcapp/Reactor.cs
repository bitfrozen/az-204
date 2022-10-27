// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Azure.Messaging.EventGrid;

namespace func2func;

public static class Reactor
{
	[FunctionName("Reactor")]
	public static void Run([EventGridTrigger] EventGridEvent eventGridEvent, ILogger log)
	{
		log.LogInformation("Reactor received event grid event");
		BinaryData eventData = eventGridEvent.Data;
		log.LogInformation("Event data: {EventData}", eventData.ToString());
	}
}