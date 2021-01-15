using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using P0_CLibrary.Models;

namespace P0_RepositoryLayer.Models
{
    public class StoreDbContext:DbContext
    {
        public DbSet<Customer> Customers {get;set;}
        public DbSet<Inventory> Inventory {get;set;}
        public DbSet<Location> Locations {get;set;}
        public DbSet<Order> Orders {get;set;}
        public DbSet<OrderDetail> OrderDetails {get;set;}
        public DbSet<Product> Products {get;set;}

        public StoreDbContext() {}

        public StoreDbContext (DbContextOptions options) : base (options) 
        {
            
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=DESKTOP-0IVLTFI;Database=My_P0_Test2;Trusted_Connection=True");

            }

        }   
    }
}