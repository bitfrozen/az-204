using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;

namespace MessagePublisher;

public class Program
{
	private static string _storageConnectionString = "";
	private static string _queueName = "";
	private static int _numberOfMessages = 0;
	private static ServiceBusClient _client = default!;
	private static ServiceBusSender _sender = default!;

	public static async Task Main()
	{
		GetConfiguration();
		_client = new ServiceBusClient(_storageConnectionString);
		_sender = _client.CreateSender(_queueName);
		using ServiceBusMessageBatch messageBatch = await _sender.CreateMessageBatchAsync();
		for (var i = 1; i <= _numberOfMessages; i++)
		{
			if (!messageBatch.TryAddMessage(new ServiceBusMessage($"Message {i}")))
			{
				throw new Exception($"The message {i} is too large to fit in the batch.");
			}
		}

		try
		{
			await _sender.SendMessagesAsync(messageBatch);
			Console.WriteLine($"A batch of {_numberOfMessages} messages has been published to the queue.");
		}
		finally
		{
			await _sender.DisposeAsync();
			await _client.DisposeAsync();
		}
	}
	
	private static void GetConfiguration()
	{
		IConfigurationBuilder? configuration = new ConfigurationBuilder()
			.SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile($"appsettings.json")
			.AddUserSecrets<Program>();
		IConfigurationRoot? config = configuration.Build();
		_storageConnectionString = config.GetRequiredSection("StorageConnectionString").Value;
		_queueName = config.GetRequiredSection("QueueName").Value;
		_numberOfMessages = Convert.ToInt32(config.GetRequiredSection("NumberOfMessages").Value);
	}
}