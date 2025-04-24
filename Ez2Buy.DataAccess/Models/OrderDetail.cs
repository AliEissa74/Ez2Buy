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
    //contain details about items in the order like : product id, quantity, price, etc. 
    public class OrderDetail
    {
        public int Id { get; set; }
        [Required]
        public int OrderHeaderId { get; set; } //foreign key to the OrderHeader
        [ForeignKey("OrderHeaderId")]
        [ValidateNever]
        public OrderHeader OrderHeader { get; set; }

        [Required]
        public int ProductId { get; set; }  //product that is being ordered
        [ForeignKey("ProductId")]
        [ValidateNever]
        public Product Product { get; set; } 

        public int Quantity { get; set; }
        public decimal Price { get; set; } //price of the product at the time of order won't change later with the changes of the product prices



    }
}
