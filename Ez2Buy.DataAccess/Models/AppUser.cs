using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ez2Buy.DataAccess.Models
{
	public class AppUser : IdentityUser
	{
		[Required]
		public string Name { get; set; }

		public string? StreetAddress { get; set; }
		public string? City { get; set; }
		public string? Governorate { get; set; }
		[NotMapped]
		public string Role { get; set; } // this is used to get the role of the user(temporary data)
	}
}
