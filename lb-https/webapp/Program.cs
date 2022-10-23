namespace webapp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddHealthChecks();
        // builder.WebHost.ConfigureKestrel(serverOptions =>
        // {
	       //  serverOptions.ConfigureEndpointDefaults(listenOptions =>
	       //  {
        //
	       //  });
        // });
        // builder.Services.AddAntiforgery(options =>
        // {
	       //  options.Cookie.Name = "AntiforgeryCookie";
	       //  options.Cookie.Domain = "192.168.77.109";
	       //  options.Cookie.Path = "/";
	       //  options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        // });
        // builder.Services.AddSession(options =>
        // {
	       //  options.Cookie.Name = "SessionCookie";
	       //  options.Cookie.Domain = "192.168.77.109";
	       //  options.Cookie.Path = "/";
	       //  options.Cookie.HttpOnly = true;
	       //  options.Cookie.SecurePolicy = CookieSecurePolicy.Always
        // });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // Don't use HSTS. Default stance is to ONLY use HTTPS in both API and web projects.
        }
        
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthorization();
        app.MapHealthChecks("/health").RequireHost("*:80");
        app.MapRazorPages();
        app.Run();
    }
}