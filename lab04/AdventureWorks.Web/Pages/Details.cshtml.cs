using AdventureWorks.Context;
using AdventureWorks.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AdventureWorks.Web.Pages
{
  public class Details : PageModel
  {
    private readonly IAdventureWorksProductContext _productContext;

    public Details(IAdventureWorksProductContext productContext)
    {
      _productContext = productContext;
    }

    [BindProperty(SupportsGet = true)]
    public Model Model { get; set; } = null!;

    [BindProperty, Required(ErrorMessage = "Please select a produc.")]
    public string SelectedProductId { get; set; } = string.Empty;
    
    public async Task OnGetAsync(Guid id)
    {
      var result = await _productContext.FindModelAsync(id);
			this.Model = result ?? new Model();
    }
  }
}
