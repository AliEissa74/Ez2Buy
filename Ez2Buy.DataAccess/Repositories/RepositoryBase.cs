using Ez2Buy.DataAccess.Contracts;
using Ez2Buy.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

			_db.Products.Include(u => u.Category); //this is used to include the category in the product table

		}

		public void Add(T model)
		{
			_db.Add(model);
		}

        //this is used to add the model to the database
        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter, string? includeProperties = null)
		{
            IQueryable<T> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return query.ToList();
        }

        //this is used to get the model by id
        public T GetById(System.Linq.Expressions.Expression<Func<T, bool>> filter, string? includePorperties = null)
		{
			IQueryable<T> query = dbSet;
			query = query.Where(filter);
			if (!string.IsNullOrEmpty(includePorperties))
			{
				foreach (var includeProp in includePorperties
					.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
				{
					query = query.Include(includeProp);
				}
			}
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
        //this is used to get the first or default value of the model
        public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false)
        {
            IQueryable<T> query;
            if (tracked)
            {
                query = dbSet;

            }
            else
            {
                query = dbSet.AsNoTracking();
            }

            query = query.Where(filter);
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return query.FirstOrDefault();
        }
    }
}
