using System;
using P0_RepositoryLayer.Models;

namespace P0_SuperStore
{
    class Program
    {
        static void Main(string[] args)
        {
            P0_RLayer myRepoLayer = new P0_RLayer();
            Console.WriteLine("Welcome to the Super");
            
            bool ExitMainMenu = false;

            do
            {
                Console.Write("Log in  (Press x to quit the app)\nFirst Name: ");      
                string strUserFirstName = Console.ReadLine().Trim().Split(' ')[0];// Trim any white spaces that the user input in the beggining and split to get only the first...
                if (strUserFirstName.ToLower() == "x")
                {
                    Console.WriteLine("Exiting the console...\n");
                    ExitMainMenu = true;
                    break;
                }
                Console.Write("Last Name: ");
                string strUserLastName = Console.ReadLine().Trim().Split(' ')[0];
                
                // ->
                Console.WriteLine( myRepoLayer.CreateCustomer(strUserFirstName, strUserLastName) );

                // Showing the Main menu
                
                //Call the switch method
                ExitMainMenu = MainMenuOptionsOperations(myRepoLayer);
                   
            } while ( !ExitMainMenu );
        }

        
        /// <summary>
        /// This static method is used for display all the available options in the Store menu. Only when the user logs out, it return a false to logOut into the menu. When the 
        /// option is a Exit, this method will return true
        /// </summary>
        /// <param name="myRepoLayer">Our Repository Layer to access into the Database.</param>
        /// <returns>Returns a true when it is ready to exit the application, returns false when is a log out, so it can prompt a login.</returns>
        private static bool MainMenuOptionsOperations( P0_RLayer myRepoLayer )
        {
            bool bolLogOut = false;
            do
            {
                Console.WriteLine("\nSelect one of the following options:\n1.-Generate a order\t\t2.-Search customer by Name\n3.-Customer History\t\t4.-Store History\n5.-Add a new store\t\t6.-Add a product to a Store\n7.-Log out");

                string strUIMainMenu = Console.ReadLine().Trim();

                MenuOptions menuOptions = new MenuOptions();
                if (!MenuOptions.TryParse(strUIMainMenu, out menuOptions))
                {
                    menuOptions = MenuOptions.NotValid;
                }

                switch (menuOptions)
                {
                    case MenuOptions.GenerateOrder:

                        GenerateOrder(myRepoLayer);

                        break;
                    case MenuOptions.SearchCustomer:

                        SearchCustomerByName(myRepoLayer);

                        break;
                    case MenuOptions.CustomerHistory:

                        Console.WriteLine(myRepoLayer.GetAllTheHistoryFromCustomer());

                        break;
                    case MenuOptions.StoreHistory:

                        StoreOrdersHistory(myRepoLayer);

                        break;
                    case MenuOptions.AddStore:

                        AddNewStore(myRepoLayer);

                        break;
                    case MenuOptions.ProductInventory:

                        AddOrUpdateProductInventoryInLocation(myRepoLayer);

                        break;
                    case MenuOptions.LogOut:
                        /*  */
                        bolLogOut = true;
                        break;
                    case MenuOptions.NotValid:
                        Console.WriteLine("\t\tInvalid input.");

                        break;
                    case MenuOptions.Exit:

                        return true;

                    default:
                        Console.WriteLine("\t\tInvalid input.");
                        break;

                }


            } while (!bolLogOut);


            return false;

        }

        /// <summary>
        /// This method writes into the console the requested query, similar to the SQL 'LIKE', it will bring everithing that enter in the format '%some%.
        /// </summary>
        /// <param name="myRepoLayer">Our Repository Layer to access into the Database.</param>
        private static void SearchCustomerByName( P0_RLayer myRepoLayer )
        {
            Console.Write("\n\tInput the Name of the customer: ");
            string strSearchResult = Console.ReadLine();

            Console.WriteLine( myRepoLayer.SearchCustomerByName(strSearchResult) + "\n");
        }
        /// <summary>
        /// This method generate in the console a table-like format with all the order history of the store.
        /// </summary>
        /// <param name="myRepoLayer">Our Repository Layer to access into the Database.</param>
        private static void StoreOrdersHistory( P0_RLayer myRepoLayer )
        {
            bool bolUILocationSelected = true;

            int intLocationID = 0;
            do
            {
                Console.Write($"{myRepoLayer.ListEveryLocation()}\nSelect the LocationID to view the orders history: ");

                string strLocationID = Console.ReadLine();

                intLocationID = myRepoLayer.LocationHistoryVerification(strLocationID);
                if (intLocationID != 0)
                    bolUILocationSelected = false;
                else
                    Console.WriteLine("\t\tInvalid option\n");

            } while (bolUILocationSelected);

            Console.WriteLine(myRepoLayer.GetAllTheHistoryFromStore(intLocationID));
        }

        /// <summary>
        /// This method implements the main functionality of generating order for the logged customer depending of the registered location.
        /// </summary>
        /// <param name="myRepoLayer">Our Repository Layer to access into the Database.</param>
        private static void GenerateOrder( P0_RLayer myRepoLayer )
        {
            #region GenerateOrder

            #region ChangeLocation
            Console.WriteLine($"Your default location is {myRepoLayer.GetActualLocation()}, stay in the location?\nY/N");
            string strUIChanceLocation = Console.ReadLine();

            bool bolUILocation = true;
            bool bolUIInventory = true;
            do
            {
                // Check if the selection is correct
                if (strUIChanceLocation.Trim().Length != 1)
                {
                    Console.WriteLine("Invalid input\nY/N\n");
                    strUIChanceLocation = Console.ReadLine();
                    continue;
                }
                if (myRepoLayer.ChangeLocationBranch(strUIChanceLocation))
                    // Stay in the location
                    bolUILocation = false;
                else
                {
                    //Change into another location
                    bool bolUILocationSelected = true;
                    do
                    {
                        Console.Write($"{myRepoLayer.ListEveryLocationExceptCustomerLocation()}\nSelect the LocationID to change into the location: ");

                        string strUILocation = Console.ReadLine();

                        if (myRepoLayer.LocationChangeVerification(strUILocation))
                            bolUILocationSelected = false;
                        else
                            Console.WriteLine("\t\tInvalid option\n");

                    } while (bolUILocationSelected);
                    bolUILocation = false;

                }

            } while (bolUILocation);
            #endregion


            do
            {

                Console.Write($"\t\t\tInventory: \n{myRepoLayer.ListAllInventoryInLocation()}\n\nSelect the product to add into the order:");
                string strUIProductID = Console.ReadLine();

                if (myRepoLayer.SearchProductWithID(strUIProductID))
                {
                    bool bolUIQuantity = true;
                    do
                    {
                        Console.Write("Quantity: ");
                        string strInventoryQuantity = Console.ReadLine();

                        int intQuantity = 0;
                        ///
                        if (!int.TryParse(strInventoryQuantity, out intQuantity))
                        {
                            Console.WriteLine("\t\tInvalid input for quantity.");
                            continue;
                        }


                        try
                        {
                            myRepoLayer.SetOrderForCustomer(int.Parse(strUIProductID), intQuantity);
                            bolUIQuantity = false;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }

                    } while (bolUIQuantity);

                    if (ValidateIfUserWantToRedoAction("Add another product?: \nY/N", myRepoLayer))
                        continue;
                    else //N
                        bolUIInventory = false;
                }
                else
                {
                    Console.WriteLine("Invalid input\n");
                    continue;//
                }
            } while (bolUIInventory);

            // Mostrar el total de la orden y guardar en DB
            Console.WriteLine(myRepoLayer.PrintOrder());


            #endregion
        }

        /// <summary>
        /// This method is for adding manually some products into the Store Inventory. If a product is already registered, it will link the already created product and add a 
        /// new store inventory.
        /// </summary>
        /// <param name="myRepoLayer">Our Repository Layer to access into the Database.</param>
        private static void AddOrUpdateProductInventoryInLocation(P0_RLayer myRepoLayer)
        {
            bool bolUILocationSelected = true;

            int intLocationID = 0;
            do
            {
                Console.Write($"{myRepoLayer.ListEveryLocation()}\nSelect the LocationID to view the Inventory of the store : ");

                string strLocationID = Console.ReadLine();

                intLocationID = myRepoLayer.LocationHistoryVerification(strLocationID);
                if (intLocationID != 0)
                    bolUILocationSelected = false;
                else
                    Console.WriteLine("\t\tInvalid option\n");

            } while (bolUILocationSelected);

            bool bolUIInventory = true;
            do
            {
                //GetAllProductsFromInventory

                Console.Write($"\t\t\tInventory: \n{myRepoLayer.GetAllProductsFromInventory(intLocationID)}\n\nSelect the product to modify (Input A to add a new product):");
                string strUIProductID = Console.ReadLine();

                if (strUIProductID.ToLower() == "a")
                {
                    // Add a new product 
                    string strName = "", strDescription = "";
                    double dblPrice = 0;
                    int intQuantity = 0;

                    Console.WriteLine("Name of the product: ");
                    strName = Console.ReadLine();

                    Console.WriteLine("Description fo the product: ");
                    strDescription = Console.ReadLine();

                    bool bolUIPriceAndQty = true;
                    do
                    {
                        Console.WriteLine("The price for the product: ");

                        if (!double.TryParse(Console.ReadLine(), out dblPrice))
                        {
                            Console.WriteLine("Invalid Input\n");
                            continue;
                        }
                        else
                            bolUIPriceAndQty = false;
                    }
                    while (bolUIPriceAndQty);
                    bolUIPriceAndQty = true;
                    do
                    {
                        Console.WriteLine("The quantity for the product: ");

                        if (!int.TryParse(Console.ReadLine(), out intQuantity))
                        {
                            Console.WriteLine("Invalid Input\n");
                            continue;
                        }
                        else
                            bolUIPriceAndQty = false;
                    }
                    while (bolUIPriceAndQty);


                    //Add the product into the inventory in the store  intLocationID


                    try
                    {
                        myRepoLayer.AddProductIntoStoreInventory(intLocationID, strName, strDescription, dblPrice, intQuantity);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }


                }
                else if (myRepoLayer.SearchProductWithID(strUIProductID))
                {
                    bool bolUIQuantity = true;
                    do
                    {
                        Console.Write("Quantity: ");
                        string strInventoryQuantity = Console.ReadLine();

                        int intQuantity = 0;
                        ///
                        if (!int.TryParse(strInventoryQuantity, out intQuantity))
                        {
                            Console.WriteLine("\t\tInvalid input for quantity.");
                            continue;
                        }


                        try
                        {
                            myRepoLayer.SetProductFromInventory(int.Parse(strUIProductID), intQuantity);
                            bolUIQuantity = false;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }


                    } while (bolUIQuantity);


                }
                else
                {
                    Console.WriteLine("Invalid input\n");
                    continue;//
                }

                if (ValidateIfUserWantToRedoAction("Add or update another product?: \nY/N", myRepoLayer))
                    continue;
                else //N
                    bolUIInventory = false;

            } while (bolUIInventory);

        }
        /// <summary>
        /// This method is to generate a new store by input the name of the new store. It verify if already exists and only add when doesn't exists.
        /// </summary>
        /// <param name="myRepoLayer">Our Repository Layer to access into the Database.</param>
        private static void AddNewStore( P0_RLayer myRepoLayer )
        {

            string strName = "";
            Console.WriteLine("Input the name of the store: ");
            strName = Console.ReadLine();

            if ( myRepoLayer.IfStoreAlreadyExists(strName ) )
            {
                //The name already exists.
                Console.WriteLine("The store name already exist.");
            }
            else
            {
                // Store created successfully
                if (ValidateIfUserWantToRedoAction("The store was added sucessfully!\n\nAdd some products?: \nY/N", myRepoLayer))
                    AddOrUpdateProductInventoryInLocation(myRepoLayer);
            }
        }

        /// <summary>
        /// This method is to return a boolean value every time there is a 'Y/N' Operation for validating the entry.
        /// </summary>
        /// <param name="strMessage">This is the message that the console will print in the console</param>
        /// <param name="myRepoLayer">Our Repository Layer to access into the Database.</param>
        /// <returns>Returns a true when the Option selected is 'Y', else, a false.</returns>
        private static bool ValidateIfUserWantToRedoAction(string strMessage, P0_RLayer myRepoLayer)
        {
            Console.WriteLine(strMessage);
            string strUIUserChoice = Console.ReadLine();

            if (myRepoLayer.ChangeLocationBranch(strUIUserChoice)) //Y
                return true;
            else //N
                return false;
        }

    }
}

