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
    public class ShoppingCart
    {
        public int Id { get; set; }

        [Range(1,1000, ErrorMessage ="Please enter a Value between 1 and 1000")]
        public int Quantity { get; set; }

        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        [ValidateNever]
        public Product Product { get; set; }

        public string AppUserId { get; set; }
        [ForeignKey("AppUserId")]
        [ValidateNever]
        public AppUser AppUser { get; set; }

        [NotMapped]                     //this property is NotMapped to the database( temporary data not stored in the database)
        public decimal Price { get; set; } //price of the product in the cart

    }
}
