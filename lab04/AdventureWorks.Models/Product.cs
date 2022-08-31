using System.ComponentModel.DataAnnotations;
// ReSharper disable UnusedMember.Global

namespace AdventureWorks.Models
{
	public class Product
	{
		[Key]
#pragma warning disable IDE1006 // Naming Styles
		public Guid id { get; set; }
#pragma warning restore IDE1006 // Naming Styles
		public string Name { get; set; } = null!;
		public string Number { get; set; } = null!;
		public string Category { get; set; } = null!;
		public string Color { get; set; } = null!;
		public string Size { get; set; } = null!;
		public decimal? Weight { get; set; }
		public decimal ListPrice { get; set; }
		public string Photo { get; set; } = null!;
	}
}