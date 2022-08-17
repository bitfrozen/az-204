using APP;

var builder = WebApplication.CreateBuilder(args);

// Add configuration
var configuration = builder.Configuration.Get<Options>();
builder.Services.AddSingleton(configuration);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSingleton<HttpClient>(new HttpClient());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();
//app.UseRouting();
app.MapRazorPages();
app.Run();
