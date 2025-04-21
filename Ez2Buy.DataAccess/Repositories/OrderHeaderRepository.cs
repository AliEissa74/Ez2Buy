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
	public class OrderHeaderRepository : RepositoryBase<OrderHeader>,IOrderHeaderRepository
	{
		private ApplicationDbContext _db;
		public OrderHeaderRepository(ApplicationDbContext db) : base(db)
		{
			_db = db;
		}

		public void Update(OrderHeader obj)
		{
			_db.OrderHeaders.Update(obj);
		}

		public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
		{
			var orderFromDb = _db.OrderHeaders.FirstOrDefault(u => u.Id == id); //retrive order header(order) from db base on id
			if (orderFromDb != null)
			{
				orderFromDb.OrderStatus = orderStatus; //update the order status
				if (!string.IsNullOrEmpty(paymentStatus))
				{
					orderFromDb.PaymentStatus = paymentStatus; //update the payment status
				}
				
			}
		}

		public void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId)
		{
			var orderFromDb = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);
			if(!string.IsNullOrEmpty(sessionId))
			{
				orderFromDb.SessionId = sessionId; //update the session id
			}
			if (!string.IsNullOrEmpty(paymentIntentId))  //if the payment intent id is not null or empty
			{
				orderFromDb.PaymentIntentId = paymentIntentId; //update the payment intent id
				orderFromDb.PaymentDate = DateTime.Now; //update the payment date to the current date

			}
		}
	}
}
