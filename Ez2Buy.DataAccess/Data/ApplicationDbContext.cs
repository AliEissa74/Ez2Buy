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
        public DbSet<Product> Products { get; set; }

        //this method is used to seed the database with initial data
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name= "Electronics" , DisplayOrder= 1},
                new Category { Id = 2, Name= "Fashion", DisplayOrder= 2},
                new Category { Id = 3, Name = "Home & Kitchen", DisplayOrder = 3 }

                );
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "iPhone 14 Pro",
                    Description = "The latest iPhone model with cutting-edge technology, featuring a powerful A15 Bionic chip, a stunning 6.1-inch Super Retina XDR display, and a pro camera system.",
                    ListPrice = 1199.99,
                    Price = 1099.99
                },
                new Product
                {
                    Id = 2,
                    Name = "Samsung Galaxy S23",
                    Description = "The Samsung Galaxy S23 offers a sleek design with powerful performance, featuring a 6.1-inch AMOLED display, and the latest Snapdragon chipset for speed and efficiency.",
                    ListPrice = 999.99,
                    Price = 899.99
                },
                new Product
                {
                    Id = 3,
                    Name = "Adidas Ultraboost 22",
                    Description = "Adidas Ultraboost 22 running shoes combine exceptional comfort with innovative design, featuring responsive Boost cushioning and a supportive Primeknit upper for a snug fit.",
                    ListPrice = 180.00,
                    Price = 159.99
                },
                new Product
                {
                    Id = 4,
                    Name = "Basic Cotton T-shirt",
                    Description = "A soft and breathable cotton t-shirt that is perfect for everyday casual wear. Available in a variety of colors and fits for all sizes.",
                    ListPrice = 20.00,
                    Price = 15.00
                },
                new Product
                {
                    Id = 5,
                    Name = "Electric Kettle",
                    Description = "A fast-heating electric kettle with an automatic shut-off feature for safety, ideal for boiling water quickly for tea, coffee, and other beverages.",
                    ListPrice = 24.99,
                    Price = 19.99
                }
                );
        }
    }
}
