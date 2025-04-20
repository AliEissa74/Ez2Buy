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
	public class AppUserRepository : RepositoryBase<AppUser>,IAppUserRepository
	{
		private ApplicationDbContext _db;
		public AppUserRepository(ApplicationDbContext db) : base(db)
		{
			_db = db;
		}
	}
}
