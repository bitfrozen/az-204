using AdventureWorks.Context;
using AdventureWorks.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdventureWorks.Web.Pages
{


	public class Index : PageModel
	{
		private readonly IAdventureWorksProductContext _productContext;
		private readonly ILogger<Index> _logger;

		public Index(IAdventureWorksProductContext productContext, ILogger<Index> logger)
		{
			_productContext = productContext;
			_logger = logger;
		}

		[BindProperty(SupportsGet = true)]
		public List<Model> Models { get; set; } = new();

		public async Task OnGetAsync()
		{
			_logger.LogInformation("Getting models for {PageName} page", nameof(Index));
			this.Models = await _productContext.GetModelsAsync();
		}
	}
}