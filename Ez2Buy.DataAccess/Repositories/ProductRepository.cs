using Ez2Buy.DataAccess.Contracts;
using Ez2Buy.DataAccess.Data;
using Ez2Buy.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ez2Buy.DataAccess.Repositories
{
	public class ProductRepository : RepositoryBase<Product>,IProductRepository
	{
		private ApplicationDbContext _db;
		public ProductRepository(ApplicationDbContext db) : base(db)
		{
			_db = db;
		}

		// so the reason we make update not in genereic repository(RepositoryBase) bec we do custom update here
		public void Update(Product obj)
		{
			var objFromDb = _db.Products.FirstOrDefault(u => u.Id == obj.Id);
			if (objFromDb != null) {
				objFromDb.Name = obj.Name;
				objFromDb.Description = obj.Description;
				objFromDb.ListPrice = obj.ListPrice;
				objFromDb.Price = obj.Price;
				objFromDb.CategoryId = obj.CategoryId;
				if (obj.ImageUrl != null)
				{
					objFromDb.ImageUrl = obj.ImageUrl;
				}
			}
			//_db.Products.Update(obj); //this is not good because it will update all fields

		}
	}
}
