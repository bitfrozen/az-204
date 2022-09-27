using Azure;
using Azure.Messaging.EventGrid;
using Microsoft.Extensions.Configuration;

namespace EventPublisher;

public class Program
{
	private static string _topicEndpoint = "";
	private static string _topicKey = "";

	public static async Task Main()
	{
		GetConfiguration();
		
		var endpoint = new Uri(_topicEndpoint);
		var credential = new AzureKeyCredential(_topicKey);
		var client = new EventGridPublisherClient(endpoint, credential);

		var firstEvent = new EventGridEvent(
			subject: $"New Employee: Alba Sutton",
			eventType: "Employees.Registration.New",
			dataVersion: "1.0",
			data: new
			{
				FullName = "Alba Sutton",
				Address = "4567 Pine Avenue, Edison, WA 97202"
			}
		);
		var secondEvent = new EventGridEvent(
			subject: $"New Employee: Alexandre Doyon",
			eventType: "Employees.Registration.New",
			dataVersion: "1.0",
			data: new
			{
				FullName = "Alexandre Doyon",
				Address = "456 College Street, Bow, WA 98107"
			}
		);
		await client.SendEventAsync(firstEvent);
		Console.WriteLine("First event published");
		await client.SendEventAsync(secondEvent);
		Console.WriteLine("Second event published");
	}
	
	private static void GetConfiguration()
	{
		IConfigurationBuilder? configuration = new ConfigurationBuilder()
			.SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile($"appsettings.json")
			.AddUserSecrets<Program>();
		IConfigurationRoot? config = configuration.Build();
		_topicEndpoint = config.GetRequiredSection("TopicEndpoint").Value;
		_topicKey = config.GetRequiredSection("TopicKey").Value;
	}
}