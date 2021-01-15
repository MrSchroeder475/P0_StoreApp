using System.Collections.Generic;
using System.Linq;
using P0_CLibrary.Models;

namespace P0_RepositoryLayer.Models
{
    public static class DbInitializer
    {
        public static void Initialize(StoreDbContext context)
        {
            context.Database.EnsureCreated();

            // Look for any customer.
            if (context.Customers.Any())
            {
                return;   // DB has been seeded
            }

            Location centralLocation = new Location
            {
                Name="Central Branch"
            };
            Location location1 = new Location
            {
                Name="Store 1"
            };
            Location location2 = new Location
            {
                Name="Store 2"
            };

            // var location = new Location[]
            // {
            //     new Location{ Name="Central Branch"},
            //     new Location{ Name="Store 1"},
            //     new Location{ Name="Store 2"}
            // };

            // foreach (Location l in location)
            // {
            //     context.Locations.Add(l);
            // }
            // context.SaveChanges();

            Customer gsCust = new Customer{FirstName="Gabriel",LastName="Schroeder",Location=centralLocation };
            Customer jdCust = new Customer{FirstName="John",LastName="Doe",Location=centralLocation };

            // var customer = new Customer[]
            // {
            // new Customer{FirstName="Gabriel",LastName="Schroeder",Location=centralLocation }
            // new Customer{FirstName="John",LastName="Doe",Location=centralLocation }
            // };
            // foreach (Customer c in customer)
            // {
            //     context.Customers.Add(c);
            // }
            // context.SaveChanges();

            Product appleProd = new Product{Name="Apples",Price=0.6,Description="One apple, Its distinctively sweet flavor makes a perfect snack"};
            Product toiletProd = new Product{Name="Toilet Paper Generic 6 rolls",Price=3,Description="Our Finest Ultra Soft Toilet Paper"};
            Product bottledProd = new Product{Name="Bottled Water 16 Fl. oz.",Price=0.6,Description="Purified drinking water"};
            Product cocaProd = new Product{Name="Coca-Cola 12 Fl. oz.",Price=0.6,Description="The delicious soda you know and love."};
            Product chocolProd = new Product{Name="Hershey's Milk Chocolate Bar",Price=0.92,Description="Great For Making Personalized Party FavorsGluten-Free And Kosher Chocolate Candy"};
            Product angusProd = new Product{Name="Black Angus Top Round Steak",Price=0.6,Description="Often referred to as London broil, this lean, flavorful steak is guaranteed to make your mouth water."};
            Product milkProd = new Product{Name="Whole Milk 1 Gal.",Price=0.6,Description="Vitamin D. Grade A, pasteurized, homogenized."};
            Product eggsProd = new Product{Name="Large Eggs Dozen",Price=0.6,Description="Grade A large brown eggs"};
            // var products = new Product[]
            // {
            // new Product{Name="Apples",Price=0.6,Description="One apple, Its distinctively sweet flavor makes a perfect snack"},
            // new Product{Name="Toilet Paper Generic 6 rolls",Price=3,Description="Our Finest Ultra Soft Toilet Paper"},
            // new Product{Name="Bottled Water 16 Fl. oz.",Price=0.6,Description="Purified drinking water"},
            // new Product{Name="Coca-Cola 12 Fl. oz.",Price=0.6,Description="The delicious soda you know and love."},
            // new Product{Name="Hershey's Milk Chocolate Bar",Price=0.92,Description="Great For Making Personalized Party FavorsGluten-Free And Kosher Chocolate Candy"},
            // new Product{Name="Black Angus Top Round Steak",Price=0.6,Description="Often referred to as London broil, this lean, flavorful steak is guaranteed to make your mouth water."},
            // new Product{Name="Whole Milk 1 Gal.",Price=0.6,Description="Vitamin D. Grade A, pasteurized, homogenized."},
            // new Product{Name="Large Eggs Dozen",Price=0.6,Description="Grade A large brown eggs"},
            // };
            // foreach (Product p in products)
            // {
            //     context.Products.Add(p);
            // }
            // context.SaveChanges();
            centralLocation.Inventory = new List<Inventory>();
            location1.Inventory = new List<Inventory>();
            location2.Inventory = new List<Inventory>();
            //Location have a list of Inventory
            List<Inventory> centralInventory = new List<Inventory>
            {
                new Inventory{Product=appleProd,Quantity = 20},
                new Inventory{Product= toiletProd, Quantity=10},
                new Inventory{Product= bottledProd, Quantity=8},
                new Inventory{Product= cocaProd, Quantity=15},
                new Inventory{Product= chocolProd, Quantity=8},
                new Inventory{Product= angusProd, Quantity=5},
                new Inventory{Product= milkProd, Quantity=14},
                new Inventory{Product=eggsProd,Quantity=7}
            };
            List<Inventory> loc1Inventory = new List<Inventory>{
                new Inventory{Product= toiletProd, Quantity=4},
                new Inventory{Product= cocaProd, Quantity=8},
                new Inventory{Product= chocolProd, Quantity=14},
                new Inventory{Product= angusProd, Quantity=2},
                new Inventory{Product= milkProd, Quantity=6},
            };

            List<Inventory> loc2Inventory = new List<Inventory>{
                new Inventory{Product=appleProd,Quantity = 15},
                new Inventory{Product= bottledProd, Quantity=3},
                new Inventory{Product=eggsProd,Quantity=12},
                new Inventory{Product= chocolProd, Quantity=4},
                new Inventory{Product= angusProd, Quantity=6},
                new Inventory{Product= milkProd, Quantity=9},
            };

            centralLocation.Inventory.AddRange(centralInventory);
            location1.Inventory.AddRange(loc1Inventory);
            location2.Inventory.AddRange(loc2Inventory);

            //Save into SQL
            context.Locations.AddRange(centralLocation,location1,location2);
            context.Customers.AddRange(gsCust,jdCust);
            context.Products.AddRange(appleProd,toiletProd,bottledProd,cocaProd,chocolProd,angusProd,milkProd,eggsProd);
            
            //context.Inventory.AddRange(centralInventory,loc1Inventory,loc2Inventory);


            //Orders and OrderDetails will populate with the code.
            
            context.SaveChanges();
        }
    }
}