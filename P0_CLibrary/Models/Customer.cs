namespace P0_CLibrary.Models
{
    public class Customer
    {
        private int intCustomerID;
        public int CustomerID
        {
            get { return intCustomerID; }
            set { intCustomerID = value; }
        }
        private string strFirstName;
        public string FirstName
        {
            get { return strFirstName; }
            set { strFirstName = value; }
        }
        private string strLastName;
        public string LastName
        {
            get { return strLastName; }
            set { strLastName = value; }
        }
        private Location objLocation;
        public Location Location
        {
            get { return objLocation; }
            set { objLocation = value; }
        }
        /// <summary>
        /// Overrided method ToString, it returns the complete name of the customer
        /// </summary>
        /// <returns>Returns the full name of the customer with space between the first and last names.</returns>
        public override string ToString()
        {
            return $"{strFirstName} {this.strLastName}";
        }

        
    }
}