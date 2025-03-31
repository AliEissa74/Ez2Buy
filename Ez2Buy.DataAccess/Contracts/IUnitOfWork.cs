using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ez2Buy.DataAccess.Contracts
{
	public interface IUnitOfWork
	{
		ICategoryRepository Category { get; }
		void Save();
	}
}
