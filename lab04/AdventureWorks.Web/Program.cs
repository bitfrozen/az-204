using AdventureWorks.Context;

namespace AdventureWorks.Web
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);
			// Generate lowercase urls
			builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
			// Add services to the container.
			builder.Services.AddRazorPages();
			// Bind Settings
			builder.Services.Configure<SettingsOptions>(
				builder.Configuration.GetSection(SettingsOptions.Settings));
			builder.Services.Configure<CosmosOptions>(
				builder.Configuration.GetSection(CosmosOptions.CosmosSettings));
			builder.Services.AddScoped<IAdventureWorksProductContext, AdventureWorksCosmosContext>();

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			app.UseDeveloperExceptionPage();
			app.UseStaticFiles();
			app.UseRouting();
			app.MapRazorPages();

			app.Run();
		}
	}
}