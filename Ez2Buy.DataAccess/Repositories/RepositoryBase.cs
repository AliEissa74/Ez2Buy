using Ez2Buy.DataAccess.Contracts;
using Ez2Buy.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ez2Buy.DataAccess.Repositories
{
	public class RepositoryBase<T> : IRepositoryBase<T> where T : class
	{
		private readonly ApplicationDbContext _db;
		internal DbSet<T> dbSet;
		public RepositoryBase(ApplicationDbContext db)
		{
			_db = db;
			this.dbSet = _db.Set<T>();  // _db.Categories == dbSet it a reference to any table(dbset is generic)

		}

		public void Add(T model)
		{
			_db.Add(model);
		}

		public IEnumerable<T> GetAll()
		{
			IQueryable<T> query = dbSet;
			return query.ToList();
		}

		public T GetById(System.Linq.Expressions.Expression<Func<T, bool>> filter)
		{
			IQueryable<T> query = dbSet;
			query = query.Where(filter);
			return query.FirstOrDefault(); //this is same as Category controller firstorDefault
		}

		public void Delete(T model)
		{
			dbSet.Remove(model);
		}

		public void DeleteRange(IEnumerable<T> model)
		{
			dbSet.RemoveRange(model);
		}

	}
}
