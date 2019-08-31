using System;
using Xunit;
using System.Text;
using System.IO;
using System.Net.Mail;
using System.Collections.Specialized;
using AirtableClientWrapper;

namespace AirtableClientWrapperTests
{
    public class SystemTests
    {
        [Fact]
        public void ReadData()
        {
            OrderData result;
            AirtableOrders ATbase = new AirtableOrders();
            result = ATbase.GetRecordByOrderID(1234567);

        }
        [Fact]
        public void CreateOrder()
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

        [Fact]
        public void CreateExpenses()
        {
            var ATbase = new AirtableExpenses();

            var expensesEntry = new ExpensesData("test data");
            expensesEntry.Date = DateTime.Now;
            expensesEntry.DeliveredTo = "Al";
            expensesEntry.Value = 10.34;

            ATbase.CreateExpensesRecord(expensesEntry);
        }


        [Fact]
        public void LookupItemMaterial()
        {
           var materials = new AirtableMaterialsLookup();

           double value = materials.GetMaterialCostForItem("Cactus");

        }

    }
}
