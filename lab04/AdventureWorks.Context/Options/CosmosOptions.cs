namespace AdventureWorks.Context
{
	public class CosmosOptions
	{
		public const string CosmosSettings = "CosmosSettings";

		public string ConnectionString { get; set; } = String.Empty;
		public string DatabaseName { get; set; } = String.Empty;
		public string ContainerName { get; set; } = String.Empty;
	}
}