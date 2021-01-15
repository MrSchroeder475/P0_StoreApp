using System.Collections.Generic;

namespace P0_CLibrary.Models
{
    public class Location
    {
        private int intLocationID;
        public int LocationID
        {
            get { return intLocationID; }
            set { intLocationID = value; }
        }
        private string strName;
        public string Name
        {
            get { return strName; }
            set { strName = value; }
        }
        private List<Inventory> objInventory;
        public List<Inventory> Inventory
        {
            get { return objInventory; }
            set { objInventory = value; }
        }
        
        // private string strAddress1;
        // public string Address1
        // {
        //     get { return strAddress1; }
        //     set { strAddress1 = value; }
        // }
        
    }
}