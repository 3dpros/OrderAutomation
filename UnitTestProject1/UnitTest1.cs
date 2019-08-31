using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AirtableClientWrapper;
using System.Text;
using System.IO;
using System.Net.Mail;
using System.Collections.Specialized;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod2()
        {

        }
        [TestMethod]
        public void TestMethod1()
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
