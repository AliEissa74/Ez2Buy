using Ez2Buy.DataAccess.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ez2BuyWeb.ViewModels
{
	public class ProductVM
	{
        public Product Product { get; set; }
		[ValidateNever]
		public IEnumerable<SelectListItem> CategoryList { get; set; }
	}
}
