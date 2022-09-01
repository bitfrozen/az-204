using AdventureWorks.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AdventureWorks.Context
{
	public class AdventureWorksCosmosContext : IAdventureWorksProductContext
	{
		private readonly Container _container;
		private readonly ILogger<AdventureWorksCosmosContext> _logger;
		private readonly CosmosOptions _cosmosOptions;

		public AdventureWorksCosmosContext(IOptions<CosmosOptions> options, ILogger<AdventureWorksCosmosContext> logger)
		{
			_logger = logger;
			_cosmosOptions = options.Value;
			_logger.LogWarning("Creating Cosmos client");
			_container = new CosmosClient(_cosmosOptions.ConnectionString)
				.GetDatabase(_cosmosOptions.DatabaseName)
				.GetContainer(_cosmosOptions.ContainerName);
		}

		public async Task<Model?> FindModelAsync(Guid id)
		{
			var iterator = _container.GetItemLinqQueryable<Model>()
				.Where(m => m.id == id)
				.ToFeedIterator();

			List<Model> matches = new();
			while (iterator.HasMoreResults)
			{
				var next = await iterator.ReadNextAsync();
				matches.AddRange(next);
			}

			return matches.SingleOrDefault();
		}

		public async Task<Product?> FindProductAsync(Guid id)
		{
			var query = $@"SELECT VALUE products
											FROM models
											JOIN products in models.Products
											WHERE products.id = '{id}'";

			var iterator = _container.GetItemQueryIterator<Product>(query);

			List<Product> matches = new();
			while (iterator.HasMoreResults)
			{
				var next = await iterator.ReadNextAsync();
				matches.AddRange(next);
			}

			return matches.SingleOrDefault();
		}

		public async Task<List<Model>> GetModelsAsync()
		{
			const string query = $@"SELECT * FROM items";
			var iterator = _container.GetItemQueryIterator<Model>(query);

			List<Model> matches = new();
			while (iterator.HasMoreResults)
			{
				var next = await iterator.ReadNextAsync();
				matches.AddRange(next);
			}

			return matches;
		}
	}
}