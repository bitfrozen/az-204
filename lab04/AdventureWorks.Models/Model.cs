using System.ComponentModel.DataAnnotations;
// ReSharper disable UnusedMember.Global

namespace AdventureWorks.Models
{
	public class Model
	{
		[Key]
#pragma warning disable IDE1006 // Naming Styles
		public Guid id { get; set; }
#pragma warning restore IDE1006 // Naming Styles
		public string Name { get; set; } = null!;
		public string Category { get; set; } = null!;
		public string Description { get; set; } = null!;
		public List<Product> Products { get; set; } = null!;
		public string Photo { get; set; } = null!;
	}
}