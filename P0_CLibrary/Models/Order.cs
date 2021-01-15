using System;
using System.Collections.Generic;

namespace P0_CLibrary.Models
{
    public class Order
    {
        private int intOrderID;
        public int OrderID
        {
            get { return intOrderID; }
            set { intOrderID = value; }
        }
        private Location objLocation;
        public Location Location
        {
            get { return objLocation; }
            set { objLocation = value; }
        }
        private List<OrderDetail> lstOrderDetails = new List<OrderDetail>();
        public List<OrderDetail> OrderDetails
        {
            get { return lstOrderDetails; }
            set { lstOrderDetails = value; }
        }
        private Customer objCustomer;
        public Customer Customer
        {
            get { return objCustomer; }
            set { objCustomer = value; }
        }
                
        private DateTime dtmDate;
        public DateTime Date
        {
            get { return dtmDate; }
            set { dtmDate = value; }
        }
        
        /// <summary>
        /// This method is used to get all the TotalAmount from the OrderDetails.
        /// </summary>
        /// <returns>Returns a double parameter with the totalAmount.</returns>
        public double GetTotalAmountFromOrderDetail()
        {
            double result = 0;

            foreach (OrderDetail orderDetail in this.OrderDetails)
            {
                result += (orderDetail.Quantity * orderDetail.Product.Price);
            }

            return result;
        }


        

    }
}