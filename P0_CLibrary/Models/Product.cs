namespace P0_CLibrary.Models
{
    public class Product
    {
        private int intProductID;
        public int ProductID
        {
            get { return intProductID; }
            set { intProductID = value; }
        }
        private string strName;
        public string Name
        {
            get { return strName; }
            set { strName = value; }
        }
        private double dblPrice;
        public double Price
        {
            get { return dblPrice; }
            set { dblPrice = value; }
        }
        private string strDescription;
        public string Description
        {
            get { return strDescription; }
            set { strDescription = value; }
        }
        
    }
}