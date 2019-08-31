using AirtableClientWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            AirtableOrders ATbase = new AirtableOrders();

            var order = ATbase.newOrderData(1234567);

            order.Notes = "this is a test order";
            order.ShippingCost = 10.21;
            order.TotalPrice = 504.23;
            order.Description = "test order";
            //  order.PrintOperator = "Al Billington";
            //   order.Shipper = "Leah";
            // order.ShippingCharge = Math.Min(7;
            order.MaterialCost = 12.34;
            order.DueDate = DateTime.Now;
            order.Channel = "Etsy";
            ATbase.CreateOrderRecord(order);

            ATbase.CreateOrderRecord(order);
        }
    }
}
