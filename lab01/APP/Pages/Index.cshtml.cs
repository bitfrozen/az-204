using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace APP.Pages
{
	public class IndexModel : PageModel
	{
		private readonly ILogger<IndexModel> _logger;
		private readonly HttpClient _httpClient;
		private readonly Options _options;

		public IndexModel(ILogger<IndexModel> logger, HttpClient httpClient, Options options)
		{
			_logger = logger;
			_httpClient = httpClient;
			_options = options;
		}

		[BindProperty] public List<string>? ImageList { get; private set; }

		[BindProperty] public IFormFile? Upload { get; set; }

		public async Task OnGetAsync()
		{
			var imagesUrl = _options.ApiUrl;
			_logger.LogInformation($"Getting images from {imagesUrl}");
			var imagesJson = await _httpClient.GetStringAsync(imagesUrl);
			var imagesList = JsonConvert.DeserializeObject<IEnumerable<string>>(imagesJson);
			ImageList = imagesList != null ? imagesList.ToList() : new List<string>();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (Upload is not { Length: > 0 }) return RedirectToPage("/Index");
			var imagesUrl = _options.ApiUrl;
			using var image = new StreamContent(Upload.OpenReadStream());
			image.Headers.ContentType = new MediaTypeHeaderValue(Upload.ContentType);
			await _httpClient.PostAsync(imagesUrl, image);
			return RedirectToPage("/Index");
		}
	}
}