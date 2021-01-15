using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using P0_CLibrary.Models;

namespace P0_RepositoryLayer.Models
{
    public class P0_RLayer 
    {
        //All the logic interaction between the objects, later we are going to populate it.
        public List<Customer> Customers = new List<Customer>();
        public List<Location> Locations = new List<Location>();
        public List<Product> Products = new List<Product>();
        public List<Inventory> Inventory = new List<Inventory>();
        public List<Order> Orders = new List<Order>();

        // -> Not sure if it needs the OrderDetail list, or with the order object it can retrieve it...

        private Order CurrentOrder = new Order();
        private Customer LoggedCustomer = new Customer();
        
        // ->
        private List<Inventory> LocalInventory = new List<Inventory>();

        StoreDbContext myDbContext = new StoreDbContext();
        
        // private Order ActualOrder;
        
        /// <summary>
        /// Constructor for our Repository Layer Object. It initializes the Lists and gets the context from the production database.
        /// </summary>
        public P0_RLayer()
        {
            
            // using(StoreDbContext Db = new StoreDbContext())
            // {   
                DbInitializer.Initialize(myDbContext);

                this.Customers =myDbContext.Customers.ToList();
                this.Locations = myDbContext.Locations.ToList();
                this.Products = myDbContext.Products.ToList();
                this.Inventory = myDbContext.Inventory.ToList();
                this.Orders = myDbContext.Orders.ToList();

            // }
        }

        /// <summary>
        /// Constructor for our Repository Layer Object. It initializes the Lists and gets the context from the In-Memory database.
        /// </summary>
        public P0_RLayer(StoreDbContext context)
        {
              
            DbInitializer.Initialize(context);

            this.Customers = context.Customers.ToList();
            this.Locations = context.Locations.ToList();
            this.Products = context.Products.ToList();
            this.Inventory = context.Inventory.ToList();
            this.Orders = context.Orders.ToList();
            
        }
        
        /// <summary>
        /// This method is used for creating a new Customer object if its not registered, if its registered, it will find the customer in the Customers list and saved it in a private
        /// LoggedCustomer object.  
        /// </summary>
        /// <param name="strFirstName">string parameter, it contains the fisrt name of the customer.</param>
        /// <param name="strLastName">string parameter, it contains the last name of the customer</param>
        /// <returns>returns a message if the customer was registered, or else it welcomes the user logged in.</returns>
        public string CreateCustomer(string strFirstName = "", string strLastName = "")
        {
            if ( !this.Customers.Exists( X => X.FirstName == strFirstName && X.LastName == strLastName ) )
            {

                //Every customer created it will have a default selection, it will be central.
                Customer customer = new Customer()
                {
                    FirstName = strFirstName,
                    LastName = strLastName,
                    Location = Locations.Find( x => x.LocationID == 1 ),
                };

                myDbContext.Customers.Add(customer);
                myDbContext.Locations.Update(customer.Location);
                myDbContext.Inventory.UpdateRange(customer.Location.Inventory);
                    
                foreach (Inventory inventory in customer.Location.Inventory)
                {
                    myDbContext.Products.Update(inventory.Product);
                }

                    // ->
                myDbContext.SaveChanges();

                this.Customers = myDbContext.Customers.ToList();
                this.LoggedCustomer = this.myDbContext.Customers.First(x => x.FirstName == strFirstName && x.LastName == strLastName);
                return $"The customer {customer.ToString()} was created successfully.\n";
            }

            this.LoggedCustomer = this.myDbContext.Customers.First(x => x.FirstName == strFirstName && x.LastName == strLastName);
            return $"Welcome {this.LoggedCustomer.ToString()}"; 

        }

        /// <summary>
        /// This method is used for searching a customer by name, it uses a Contains( in SQL will "LIKE('%string%')"), so it will find every elements in the list and 
        /// it will print the customers.
        /// </summary>
        /// <param name="strSearchName">string parameter, this is the name we want to search in the customer List.</param>
        /// <returns>It will return a string with all the results. If there is no results, it will return a empty customer format list. </returns>
        public string SearchCustomerByName(string strSearchName)
        {
            // IEnumerable<Customer> SearchCustomer = this.Customers.Where( x => x.ToString().Contains( strSearchName ) );

            string strSearchCustomer = "\nUserID\t\tFirst Name\t\tLast Name\n";

            foreach (Customer cust in ( this.Customers.Where( x => x.ToString().ToLower().Contains( strSearchName.ToLower() ) ) ) )
            {
                strSearchCustomer += $"{cust.CustomerID}\t\t{cust.FirstName}\t\t{cust.LastName}\n";
            }

            return strSearchCustomer;
        }
        /// <summary>
        /// This method is used for accessing the logged user and get the location of the customer logged.
        /// </summary>
        /// <returns>Returns a string with the locatrion name where the customer is registered.(The default location is central when a customer is created)</returns>
        public string GetActualLocation()
        {
            return this.LoggedCustomer.Location.Name;
        }

        /// <summary>
        /// This methods is used for handling the menu user interface. It uses a Enun MenuOptions with the available options in the menu. It will validate the user selection.
        /// If its not valid, it will be marked as NotValid
        /// </summary>
        /// <param name="strUISelection">string parameter, Is the selection of the user, it can be 1-5, or if its spelled the Enum option correctly</param>
        /// <returns>Returns a Enum MenuOptions with the requested option</returns>
        public MenuOptions GetUIMenuOptions(string strUISelection)
        {
            MenuOptions myMenuOptions = new MenuOptions();
            if ( !MenuOptions.TryParse(strUISelection, out myMenuOptions) )
            {
                myMenuOptions = MenuOptions.NotValid;
            }
            return myMenuOptions;
        }

    /// <summary>
    /// This method is used for verifying if the user want to stay in their registered location or if it wants to change to another one.(Only accepts Y/N)
    /// </summary>
    /// <param name="strChangeLocation">string parameter, is the user selection</param>
    /// <returns>Returns a boolean value indicating a false for change into another location and true to stay in the location.</returns>    
        public bool ChangeLocationBranch(string strChangeLocation)
        {
            if ( strChangeLocation.ToLower().Trim() != "y" )
            {
                //User input N/n or else.
                return false;
            }
            else
            {
                //Continue in the same branch
                return true;
            }
        }

        /// <summary>
        /// This method return a table-like string with all the locations available to the customer to choose excluding the actual store
        /// </summary>
        /// <returns>Returns a string with a table-like format for selecting a location</returns>
        public string ListEveryLocationExceptCustomerLocation()
        {
            // IEnumerable<Customer> SearchCustomer = this.Customers.Where( x => x.ToString().Contains( strSearchName ) );

            string strLocations = "\nLocationID\t\tName\n";

            foreach (Location local in myDbContext.Locations.Where( x => x.LocationID != this.LoggedCustomer.Location.LocationID ).OrderBy( x => x.LocationID ) ) //Except( this.LoggedCustomer.Location ) )
            {
                strLocations += $"{local.LocationID}\t\t{local.Name}\n";
            }

            return strLocations;
        }

        /// <summary>
        /// This method return a table-like string with all the locations available to the customer to choose
        /// </summary>
        /// <returns>Returns a string with a table-like format for selecting a location</returns>
        public string ListEveryLocation()
        {
            string strLocations = "\nLocationID\t\tName\n";

            foreach (Location local in myDbContext.Locations.OrderBy( x => x.LocationID ) ) //Except( this.LoggedCustomer.Location ) )
            {
                strLocations += $"{local.LocationID}\t\t{local.Name}\n";
            }

            return strLocations;
        }

        /// <summary>
        /// This method execute a int.TryParse and verifyies if the locationID exists in the location table. If exists in the table, it will perform a change to the selected
        /// location and update the database for the default location
        /// </summary>
        /// <param name="strLocationID">string param, is the user input to validate if it is a valid ID.</param>
        /// <returns>Is false when it doesn't find any element with the ID or when the string
        /// passed is not a valid number. It returns true when is a number and exists in the table</returns>
        public bool LocationChangeVerification(string strLocationID)
        {
            int result = 0;

            if( !int.TryParse( strLocationID, out result ) || !Locations.Exists( x => x.LocationID == result ) )
            {
                // Not valid...
                return false;
            }
            else
            {
                SetLocationForLoggedCustomer(result);
                return true;
            }

        }

        /// <summary>
        /// This method execute a int.TryParse and verifyies if the locationID exists in the location table. If exists in the table, it will return the LocationID, if else, it will return a 0.
        /// </summary>
        /// <param name="strLocationID">string param, is the user input to validate if it is a valid ID.</param>
        /// <returns>Is 0 when it doesn't find any element with the ID or when the string
        /// passed is not a valid number. It returns the InventoryID when is a number and exists in the table</returns>
        public int LocationHistoryVerification(string strLocationID)
        {
            int result = 0;

            if (!int.TryParse(strLocationID, out result) || !Locations.Exists(x => x.LocationID == result))
            {
                // Not valid...
                return 0;
            }
            else
            {
                return result;
            }
        }
        /// <summary>
        /// This method is executed for changing the logged customer the default store to perform the operations.
        /// </summary>
        /// <param name="LocationID">integer parameter, it is the new LocationID the logged customer is going to be linked to.</param>
        private void SetLocationForLoggedCustomer(int LocationID)
        {
            Location myLocation = Locations.First( x => x.LocationID == LocationID );

            this.LoggedCustomer.Location = myLocation;
            // using(StoreDbContext Db = new StoreDbContext())
            // {
                // Verify if the changelocation Works in DB.
                myDbContext.Customers.Update(this.LoggedCustomer);
                myDbContext.SaveChanges();

            // }
            this.Customers.First(x => x.CustomerID == this.LoggedCustomer.CustomerID).Location = this.LoggedCustomer.Location;
        }

        /// <summary>
        /// This method is used to list all the inventory in a table-style string
        /// </summary>
        /// <returns>Returns a string-like-table for printing the results.</returns>
        public string ListAllInventoryInLocation()
        {

            LocalInventory.Clear();

            string strSearchInventory = "\nProduct ID\t\tProduct Name\t\tQuantity\t\tPrice\n";

            List<Inventory> myInventory = this.LoggedCustomer.Location.Inventory.OrderByDescending(x => x.InventoryID).ToList();

            foreach (Inventory inv in myInventory)
            {
                strSearchInventory += $"{inv.Product.ProductID}\t\t{inv.Product.Name}\t\t{inv.Quantity}\t\t{inv.Product.Price}\n";
                //
                LocalInventory.Add(inv);
            }

            return strSearchInventory;
        }

        /// <summary>
        /// This method is used to verify if the requested InventoryID is a valid input or if its exist in the Inventory List
        /// </summary>
        /// <param name="strInventoryID">string parameter, is the InventoryID to verify if its a int and if it exists in the Inventory list.</param>
        /// <returns>return a true when is a int and a valid element in the inventory, if else, returns false</returns>
        public bool SearchProductWithID(string strProductID)
        {
            int result = 0;

            if( !int.TryParse( strProductID, out result ) || !this.LocalInventory.Exists( x => x.Product.ProductID == result ) )
            {
                return false;
            }
            else
            {
                return true;
            }

        }


        /// <summary>
        /// This method is used for set the private CurrentOrder object and assign the product details and quantity into the customer order.
        /// </summary>
        /// <param name="InventoryID">int parameter, is the InventoryID</param>
        /// <param name="Quantity">int parameter, is the Quantity of products to add in the order</param>
        public void SetOrderForCustomer(int ProductID, int Quantity)
        {

            Inventory myInventory = LocalInventory.First( x=> x.Product.ProductID == ProductID );            
            
            if ( Quantity <= myInventory.Quantity)
            {
                //Product available
                OrderDetail myOrderDetail = new OrderDetail();

            
                myOrderDetail.Product = myInventory.Product;
                myOrderDetail.Quantity = Quantity;

                myInventory.Quantity -= Quantity;

                //Update to DB...
                // using(StoreDbContext Db = new StoreDbContext())
                // {
                    // Verify if the changelocation Works in DB.
                    myDbContext.Inventory.Update(myInventory);
                    myDbContext.SaveChanges();
                // }

                this.CurrentOrder.OrderDetails.Add(myOrderDetail);

            }
            else
            {
                //product not available
                throw new Exception("The quantity received is greater than the available inventory.");
            }
        }

        /// <summary>
        /// This private method is used to set the values of the CurrentOrder.
        /// </summary>
        private void SetCurrentOrderInformation()
        {
            this.CurrentOrder.Customer = this.LoggedCustomer;
            this.CurrentOrder.Date = DateTime.Now;
            this.CurrentOrder.Location = LoggedCustomer.Location;
        }

        /// <summary>
        /// Private method that perform the Insert of the Order the customer have generated with the list of orderDetails withhin.
        /// </summary>
        private void SaveOrderChanges()
        {
            myDbContext.Orders.Add(this.CurrentOrder);
            this.Orders.Add(this.CurrentOrder);
            myDbContext.SaveChanges();
        }

        public string PrintOrder()
        {
            SetCurrentOrderInformation();
            SaveOrderChanges();
            string strOrderResult = $"Order by customer {LoggedCustomer.ToString()}\t\tLocation: {LoggedCustomer.Location.Name}\nDate: {this.CurrentOrder.Date}\n";

            double dblTotalAmount = Math.Round( this.CurrentOrder.GetTotalAmountFromOrderDetail(), 2);


            foreach (OrderDetail odetail in this.CurrentOrder.OrderDetails)
            {
                strOrderResult += $"Product Name: {odetail.Product.Name}\nQuantity: {odetail.Quantity}\nPrice: {odetail.Product.Price}\n";
            }
            strOrderResult += $"Total for order: {dblTotalAmount}";


            FinishOrders();
            return strOrderResult;
        }

        /// <summary>
        /// Private method that clears the localInventory List to be empty and recreate the CurrentOrder asa new Order to initialize its nulls values.
        /// </summary>
        private void FinishOrders()
        {
            this.LocalInventory.Clear();
            this.CurrentOrder = new Order();
        }

        /// <summary>
        /// This method is called for generate a table-like format with the total orders the customer have 
        /// </summary>
        /// <returns>Returns a string with a table-like format.</returns>
        public string GetAllTheHistoryFromCustomer()
        {
            //if ()
            //{

            //}
            string strCustomerHistory = $"For the logged customer {this.LoggedCustomer.ToString()}:\n";

            List<Order> CustomerOrders = this.Orders.Where( x=> x.Customer.CustomerID == this.LoggedCustomer.CustomerID ).ToList();


            foreach ( Order order in CustomerOrders )
            {
                List<OrderDetail> OrderDetails = myDbContext.Orders.Where(x => x.OrderID == order.OrderID ).SelectMany(x => x.OrderDetails).ToList();
                order.OrderDetails = OrderDetails;

                strCustomerHistory += $"\tCustomer: {order.Customer.ToString()}, OrderID: {order.OrderID}, Store: {order.Location.Name}, Date: {order.Date.ToString("MM/dd/yyyy hh:mm")}, Total: {order.GetTotalAmountFromOrderDetail()}\nDetails:\n";



                foreach (OrderDetail orderDetail in OrderDetails)
                {
                    strCustomerHistory += $"\t\tProduct: {orderDetail.Product.Name}, Description: {orderDetail.Product.Description}, Quantity: {orderDetail.Quantity}\n";
                }
            }

            return strCustomerHistory;
        }

        /// <summary>
        /// This method is called for generate a table-like format with the total orders the selected store have generated so long. 
        /// </summary>
        /// <returns>Returns a string with a table-like format.</returns>
        public string GetAllTheHistoryFromStore(int intLocationID)
        {
            Location myLocation = this.Locations.First( x => x.LocationID == intLocationID );

            List<Order> StoreOrders = this.Orders
                .Where(x => x.Location.LocationID == intLocationID).ToList();

            string strLocationHistory = $"For the selected store: {myLocation.Name}:\n";


            foreach (Order order in StoreOrders)
            {
                List<OrderDetail> OrderDetails = myDbContext.Orders.Where(x => x.OrderID == order.OrderID).SelectMany(x => x.OrderDetails).ToList();
                order.OrderDetails = OrderDetails;

                strLocationHistory += $"\tCustomer: {order.Customer.ToString()}, OrderID: {order.OrderID}, Store: {order.Location.Name}, Date: {order.Date.ToString("MM/dd/yyyy hh:mm")}, Total: {order.GetTotalAmountFromOrderDetail()}\nDetails:\n";



                foreach (OrderDetail orderDetail in OrderDetails)
                {
                    strLocationHistory += $"\t\tProduct: {orderDetail.Product.Name}, Description: {orderDetail.Product.Description}, Quantity: {orderDetail.Quantity}\n";
                }
                strLocationHistory += "\n";
            }

            return strLocationHistory;
        }

        /// <summary>
        /// This method is used for get all the products from the passed locationID parameter. It generate a table-like results with the products and save into a localInventory
        /// for later access, then it returns the table-like string to print into the console.
        /// </summary>
        /// <param name="intLocationID">int parameter, is the LocationID</param>
        /// <returns>returns a table-like string with the products available</returns>
        public string GetAllProductsFromInventory( int intLocationID)
        {
            this.LocalInventory.Clear();
            Location myLocation = this.Locations.First( x => x.LocationID == intLocationID );

            string strAllProductsFromInv = "\nProduct ID\t\tProduct Name\t\tQuantity\t\tPrice\n";
            foreach (Inventory inv in myLocation.Inventory)
            {
                strAllProductsFromInv += $"{inv.Product.ProductID}\t\t{inv.Product.Name}\t\t{inv.Quantity}\t\t{inv.Product.Price}\n";
                LocalInventory.Add(inv);
            }

            return strAllProductsFromInv;
        }

        /// <summary>
        /// This method is for updating the quantity of a already created product in the store inventory.
        /// </summary>
        /// <param name="intProductID">int parameter, is the Product ID we want to search into the local Inventory</param>
        /// <param name="intQuantity">int parameter, is the new quantity the product have in stock</param>
        public void SetProductFromInventory(int intProductID, int intQuantity)
        {


            Inventory myInventory = this.LocalInventory.First( x => x.Product.ProductID == intProductID );

            myInventory.Quantity = intQuantity;

            myDbContext.Inventory.Update(myInventory);
            myDbContext.SaveChanges();

            this.Inventory = myDbContext.Inventory.ToList();

        }
        /// <summary>
        /// This method is used for adding a new product into the store, or if the product was already created, links the new Inventory into the location to be available in the
        /// store. If the Product is new, it will create a new product, else it will add only a new Inventory with the product within.
        /// </summary>
        /// <param name="intLocationID">int parameter, is the LocationID for searching into the Local</param>
        /// <param name="strName">string parameter, is the Name of the product</param>
        /// <param name="strDescription">string parameter, is the Description of the product</param>
        /// <param name="dblPrice">double parameter, is the total price for the product</param>
        /// <param name="intQuantity">int parameter, is the quantity the product will have in the inventory</param>
        public void AddProductIntoStoreInventory(int intLocationID , string strName, string strDescription, double dblPrice, int intQuantity)
        {

            if ( this.myDbContext.Products.ToList().Exists(x => x.Name == strName && x.Description == strDescription) )
            {

                //It exist, only add to this store inventory with 
                Product VerifyExistingProduct = this.myDbContext.Products.FirstOrDefault(x => x.Name == strName && x.Description == strDescription);

                if ( this.LocalInventory.Exists( x => x.Product.ProductID == VerifyExistingProduct.ProductID ) )
                {
                    throw new Exception( "This product already exists in this store." );
                }
                Inventory myInventory = new Inventory();

                myInventory.Quantity = intQuantity;
                myInventory.Product = VerifyExistingProduct;

                Location StoreInventory = this.Locations.First(x => x.LocationID == intLocationID);


                StoreInventory.Inventory.Add(myInventory);

                this.myDbContext.Locations.Update(StoreInventory);
                this.myDbContext.Inventory.Add( myInventory );

                this.myDbContext.SaveChanges();
            }
            else
            {
                Product myProduct = new Product();

                myProduct.Name = strName;
                myProduct.Price = dblPrice;
                myProduct.Description = strDescription;

                Inventory myInventory = new Inventory();

                myInventory.Quantity = intQuantity;
                myInventory.Product = myProduct;

                Location StoreInventory = this.Locations.First( x => x.LocationID == intLocationID );

                StoreInventory.Inventory.Add( myInventory );
            
                this.myDbContext.Locations.Update( StoreInventory );

                this.myDbContext.SaveChanges();

            }

            


        }

        /// <summary>
        /// This method is for verify if the created store is already created, if is created, it returns a true, else it will create the new location and return false
        /// </summary>
        /// <param name="strName">string parameter, is the name of the new store</param>
        /// <returns>Returns a true when the store already exists, else it will create the store and will return false</returns>
        public bool IfStoreAlreadyExists(string strName)
        {
            if (this.Locations.Exists(x => x.Name == strName))
                return true;
            else
            {
                CreateNewStoreLocation(strName);
                return false;
            }
        }

        /// <summary>
        /// This method is called when creating a new store, it will save the new store into the Database.
        /// </summary>
        /// <param name="strName">string parameter, is the name of the new store</param>
        public void CreateNewStoreLocation(string strName)
        {
            Location myLocation = new Location();

            myLocation.Name = strName;
            myLocation.Inventory = new List<Inventory>();

            this.Locations.Add(myLocation);// local
            this.myDbContext.Locations.Add( myLocation ); //DB

            this.myDbContext.SaveChanges();
        }
    }
}
