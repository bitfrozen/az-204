using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BasicSite.Pages;

public class QuestionsModel : PageModel
{
    private readonly ILogger<PageModel> _logger;

    public QuestionsModel(ILogger<PageModel> logger)
    {
        _logger = logger;
    }
    
    public void OnGet()
    {
        
    }
}