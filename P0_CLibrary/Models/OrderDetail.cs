namespace P0_CLibrary.Models
{
    public class OrderDetail
    {
        private int intOrderDetailID;
        public int OrderDetailID
        {
            get { return intOrderDetailID; }
            set { intOrderDetailID = value; }
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