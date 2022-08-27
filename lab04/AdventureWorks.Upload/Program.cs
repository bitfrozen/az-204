using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Diagnostics;

namespace AdventureWorks.Upload
{
	public static class Program
	{
		private static string _endpointUrl = "";
		private static string _authorizationKey = "";
		private static string _databaseName = "";
		private static string _containerName = "";
		private static string _partitionKey = "";
		private static string _jsonFilePath = "";

		private static int _amountToInsert;
		private static List<Model>? _models;

		private static async Task Main()
		{
			GetConfiguration();

			try
			{
				// Create Client
				CosmosClient cosmosClient = new(_endpointUrl, _authorizationKey,
					new CosmosClientOptions { AllowBulkExecution = true });

				// Initialize
				await Console.Out.WriteLineAsync("Creating a database if not already exists...");
				Database database = await cosmosClient.CreateDatabaseIfNotExistsAsync(_databaseName);

				// Configure indexing policy to exclude all attributes to maximize RU/s usage
				await Console.Out.WriteLineAsync("Creating a container if not already exists...");
				await database.DefineContainer(_containerName, _partitionKey)
					.WithIndexingPolicy()
						.WithIndexingMode(IndexingMode.Consistent)
							.WithIncludedPaths()
							.Attach()
							.WithExcludedPaths()
								.Path("/*")
							.Attach()
						.Attach()
					.CreateAsync();

				using StreamReader reader = new(File.OpenRead(_jsonFilePath));
				var json = await reader.ReadToEndAsync();
				_models = JsonSerializer.Deserialize<List<Model>>(json);
				_models ??= new List<Model>();
				_amountToInsert = _models.Count;

				// Prepare items for insertion
				await Console.Out.WriteLineAsync($"Preparing {_amountToInsert} items to insert...");

				// Create the list of Tasks
				await Console.Out.WriteLineAsync("Starting...");
				var stopwatch = Stopwatch.StartNew();

				// ConcurrentTasks
				var container = database.GetContainer(_containerName);

				List<Task> tasks = new(_amountToInsert);
				foreach (var model in _models)
				{
					tasks.Add(
						container
							.CreateItemAsync(model, new PartitionKey(model.Category))
							.ContinueWith(itemResponse =>
								{
									if (itemResponse.IsCompletedSuccessfully) return;

									var innerExceptions = itemResponse.Exception?.Flatten() ?? new AggregateException();
									if (innerExceptions.InnerExceptions
										    .FirstOrDefault(innerEx => innerEx is CosmosException) is CosmosException cosmosException)
									{
										Console.Out.WriteLine($"Received {cosmosException.StatusCode} ({cosmosException.Message}).");
									}
									else
									{
										Console.Out.WriteLine($"Exception {innerExceptions.InnerExceptions.FirstOrDefault()}.");
									}
								}
							)
					);
				}

				// Wait until all are done
				await Task.WhenAll(tasks);
				stopwatch.Stop();

				await Console.Out.WriteLineAsync($"Finished writing {_amountToInsert} items in {stopwatch.Elapsed}.");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		private static void GetConfiguration()
		{
			var configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json")
				.AddEnvironmentVariables("Adv_");
			var config = configuration.Build();
			_endpointUrl = config.GetRequiredSection("EndpointUrl").Value;
			_databaseName = config.GetRequiredSection("DatabaseName").Value;
			_containerName = config.GetRequiredSection("ContainerName").Value;
			_partitionKey = config.GetRequiredSection("PartitionKey").Value;
			_jsonFilePath = config.GetRequiredSection("JSONFilePath").Value;
			// Value should be stored in environment variable Adv_AuthorizationKey
			_authorizationKey = config.GetRequiredSection("AuthorizationKey").Value;
		}
	}

	// ReSharper disable UnusedMember.Global
	// ReSharper disable ClassNeverInstantiated.Global
	// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
	public class Model
	{
		// ReSharper disable once InconsistentNaming
		public string id { get; set; } = null!;
		public string Name { get; set; } = null!;
		public string Category { get; set; } = null!;
		public string Description { get; set; } = null!;
		public string Photo { get; set; } = null!;
		public IList<Product> Products { get; set; } = null!;
	}

	public class Product
	{
		// ReSharper disable once InconsistentNaming
		public string id { get; set; } = null!;
		public string Name { get; set; } = null!;
		public string Number { get; set; } = null!;
		public string Category { get; set; } = null!;
		public string Color { get; set; } = null!;
		public string Size { get; set; } = null!;
		public decimal? Weight { get; set; }
		public decimal ListPrice { get; set; }
		public string Photo { get; set; } = null!;
	}
	// ReSharper restore ClassNeverInstantiated.Global
	// ReSharper restore UnusedMember.Global
	// ReSharper restore AutoPropertyCanBeMadeGetOnly.Global
}