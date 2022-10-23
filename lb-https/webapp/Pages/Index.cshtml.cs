using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace webapp.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    public string? Hostname { get; set; }

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
        Hostname = Dns.GetHostName();
    }

    public void OnGet()
    {

    }
}
