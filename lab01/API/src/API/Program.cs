using API;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllers();
            
// Add configuration
var _configuration = builder.Configuration.Get<Options>();
builder.Services.AddSingleton(_configuration);
// Add inspection service (used only for understanding asp.net core structure).
// Using an IHostedService because it runs after application part discovery, and only executes once on startup.
builder.Services.AddHostedService<InspectionService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.MapControllers();

await app.RunAsync();
