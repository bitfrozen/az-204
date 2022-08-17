using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

public class InspectionService : IHostedService, IDisposable
{
    private readonly ILogger<InspectionService> _logger;
    private readonly ApplicationPartManager _partManager;

    public InspectionService(ILogger<InspectionService> logger, ApplicationPartManager partManager)
    {
        _logger = logger;
        _partManager = partManager;
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting inspection service");

        // Get the names of all the application parts. This is the short assembly name for AssemblyParts
        var applicationParts = _partManager.ApplicationParts.Select(x => x.Name);

        // Create a controller feature, and populate it from the application parts
        var controllerFeature = new ControllerFeature();
        _partManager.PopulateFeature(controllerFeature);

        // Get the names of all of the controllers
        var controllers = controllerFeature.Controllers.Select(x => x.Name);

        // Log the application parts and controllers
        _logger.LogInformation("Found the following application parts: '{ApplicationParts}' with the following controllers: '{Controllers}'",
            string.Join(", ", applicationParts), string.Join(", ", controllers));

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}