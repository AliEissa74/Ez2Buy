using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ez2Buy.DataAccess.Contracts
{
    public interface IRepositoryBase<T> where T : class
    {
        //T - Category, Product, Order,etc
        //we remove update method bec Different Update Strategies
        IEnumerable<T> GetAll(string? includeProperties = null);
        T GetById(Expression<Func<T, bool>> filter, string? includeProperties = null);
        void Add(T model);

        void Delete(T model);
        void DeleteRange(IEnumerable<T> model);
    }
}
