using AdventureWorks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureWorks.Context
{
	public interface IAdventureWorksProductContext
	{
		Task<Model?> FindModelAsync(Guid id);

		Task<List<Model>> GetModelsAsync();

		Task<Product?> FindProductAsync(Guid id);
	}
}