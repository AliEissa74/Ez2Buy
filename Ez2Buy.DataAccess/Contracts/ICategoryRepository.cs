using Ez2Buy.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ez2Buy.DataAccess.Contracts
{
	public interface ICategoryRepository : IRepositoryBase<Category>
	{
		void Update(Category obj);
		void Save();
	}
}
