using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;

namespace MessageReader;

public class Program
{
	private static string _storageConnectionString = "";
	private static string _queueName = "";
	private static ServiceBusClient _client = default!;
	private static ServiceBusProcessor _processor = default!;

	public static async Task Main()
	{
		GetConfiguration();
		_client = new ServiceBusClient(_storageConnectionString);
		_processor = _client.CreateProcessor(_queueName, new ServiceBusProcessorOptions());

		try
		{
			_processor.ProcessMessageAsync += MessageHandler;
			_processor.ProcessErrorAsync += ErrorHandler;
			await _processor.StartProcessingAsync();
			Console.WriteLine("Wait for a minute and then press any key to end the processing");
			Console.ReadKey();
			Console.WriteLine("\nStopping the receiver...");
			await _processor.StopProcessingAsync();
			Console.WriteLine("Stopped receiving messages");
		}
		finally
		{
			await _processor.DisposeAsync();
			await _client.DisposeAsync();
		}
	}

	private static async Task MessageHandler(ProcessMessageEventArgs args)
	{
		var body = args.Message.Body.ToString();
		Console.WriteLine($"Received: {body}");
		await args.CompleteMessageAsync(args.Message);
	}

	private static Task ErrorHandler(ProcessErrorEventArgs args)
	{
		Console.WriteLine(args.Exception.ToString());
		return Task.CompletedTask;
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
	}
}