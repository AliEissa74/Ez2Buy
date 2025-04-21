using Ez2Buy.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ez2Buy.DataAccess.Contracts
{
	public interface IOrderHeaderRepository : IRepositoryBase<OrderHeader>
	{
		void Update(OrderHeader obj);
		void UpdateStatus(int id, string orderStatus, string? paymentStatus = null); //payment status is optional because we can update the order status 
																					 //without updating the payment status(payment status stay approved after first stage)

		void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId);//why we need this method?
																					 //because when we create a session id and payment intent id, we need to update the order header with these ids
	}
}
