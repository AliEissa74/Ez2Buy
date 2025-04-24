using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ez2Buy.DataAccess.Models
{
    //contains the order information like : where the order is being shipped to, 
    //payment status, the order date, payment id, tracking info,etc.
    public class OrderHeader
    {
        public int Id { get; set; }
        public string AppUserId { get; set; }
        [ForeignKey("AppUserId")]
        [ValidateNever]
        public AppUser AppUser { get; set; }

        public DateTime OrderDate { get; set; }
        public DateTime ShippingDate { get; set; }
        public decimal OrderTotal { get; set; }

        public string? OrderStatus { get; set; } //pending, approved, shipped, delivered, cancelled
        public string? PaymentStatus { get; set; } //pending, approved, declined
        public string? TrackingNumber { get; set; }
        public string? Carrier { get; set; } //the shipping company that will ship the order

        public DateTime PaymentDate { get; set; }


        //data that we need to collect from the user when he is placing an order
        [Required]
        public string Name { get; set; } //name of the person who will receive the order
        [Required]
        public string PhoneNumber { get; set; } 
        [Required]
        public string StreetAddress { get; set; } 
        [Required]
        public string City { get; set; } 
        [Required]
        public string Governorate { get; set; } 


        //for stripe payment
        public string? SessionId { get; set; }        //using stripe checkout session to charge the customer
		public string? PaymentIntentId { get; set; } //means that Stripe creates generate id for the payment 
                                                     //when we about to charge a customer, we create a Payment Intent first by stripe.
                                                     //useful for:Confirm payment status(pending, succeeded, failed)
                                                     //Track the payment related to a specific order or user.

    }
}
