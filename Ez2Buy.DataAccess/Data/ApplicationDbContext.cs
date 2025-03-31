using Ez2Buy.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Ez2Buy.DataAccess.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Category>  Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name= "Electronics" , DisplayOrder= 1},
                new Category { Id = 2, Name= "Fashion", DisplayOrder= 2},
                new Category { Id = 3, Name = "Home & Kitchen", DisplayOrder = 3 }

                );
        }
    }
}
