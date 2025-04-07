using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ez2Buy.DataAccess.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [DisplayName("Product Name")]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        [DisplayName("List Price")]
        [Range(1, 200000, ErrorMessage = "Price must be between 1 and 200000.")]
        public Double ListPrice { get; set; }  //original price

        [Required]
        [Range(1, 200000, ErrorMessage = "Price must be between 1 and 200000.")]
        public Double Price { get; set; } //selling price
		[ValidateNever]
		public string ImageUrl { get; set; }

        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        [ValidateNever]
        public Category Category { get; set; }


    }
}
