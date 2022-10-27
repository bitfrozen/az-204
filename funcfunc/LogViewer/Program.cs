using System.Text.Json;
using Azure.Messaging.EventHubs.Consumer;

namespace LogViewer
{
  internal static class Program
  {
    private const string EventHubNamespaceConnectionString = "Endpoint=sb://";
    private const string EventHubName = "initiator";
    private static EventHubConsumerClient _consumerClient = null!;

    private static async Task Main()
    {
	    const string consumerGroup = EventHubConsumerClient.DefaultConsumerGroupName;
	    await using (_consumerClient =
		                 new EventHubConsumerClient(consumerGroup, EventHubNamespaceConnectionString, EventHubName))
      {
	      using var cancellationSource = new CancellationTokenSource();

	      await foreach (PartitionEvent receivedEvent in _consumerClient.ReadEventsAsync(cancellationSource.Token))
	      {
		      JsonDocument data = JsonDocument.Parse(receivedEvent.Data.EventBody.ToString());
		      JsonElement root = data.RootElement;
		      JsonElement logs = root.GetProperty("records");
		      foreach (JsonElement log in logs.EnumerateArray())
		      {
			      Console.Write($"{log.GetProperty("time")}:\t");
			      JsonElement properties = log.GetProperty("properties");
			      Console.WriteLine(properties.TryGetProperty("message", out JsonElement message)
				      ? message.ToString()
				      : "--");
		      }
	      }
      }
    }
  }
}