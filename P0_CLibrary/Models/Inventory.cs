namespace P0_CLibrary.Models
{
    public class Inventory
    {
        private int intInventoryID;
        public int InventoryID
        {
            get { return intInventoryID; }
            set { intInventoryID = value; }
        }
        
        private Product objProduct;
        public Product Product
        {
            get { return objProduct; }
            set { objProduct = value; }
        }

        private int intQuantity;
        public int Quantity
        {
            get { return intQuantity; }
            set { intQuantity = value; }
        }
        
        
    }
}