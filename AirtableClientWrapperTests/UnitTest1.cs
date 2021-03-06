using System;
using Xunit;
using System.Text;
using System.IO;
using System.Net.Mail;
using System.Collections.Specialized;
using AirtableClientWrapper;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;

namespace AirtableClientWrapperTests
{
    public class SystemTests
    {
        [Fact]
        public void ReadData()
        {
            OrderData result;
            AirtableOrders ATbase = new AirtableOrders();
            result = ATbase.GetRecordByOrderID("1124", out _);
            Assert.NotNull(result);

        }
        [Fact]
        public void CreateAndUpdateExpense()
        {
            var ATbase = new AirtableExpenses();
            var data = new ExpensesData("DUMMY EXPENSE")
            {
                Value = 1.2,
                Date = DateTime.Now,
                OrderId = "1234",
            };
            ATbase.CreateExpensesRecord(data);
        }


        [Fact]
        public void CreateAndUpdateOrder()
        {
            AirtableOrders ATbase = new AirtableOrders(true);
            string orderID = "1234567";
            var order = ATbase.newOrderData(orderID);

            order.Notes = "this is a test order";
            order.ShippingCost = 10.21;
            order.Description = "test\n order";

            order.Channel = "Direct";
            order.AsanaTaskID = "3242342634652352";
            order.PrintOperator = "Kyle Perkuhn";

            ATbase.CreateOrderRecord(order);

            order.TotalPrice = 504.23;
            order.ValueOfInventory = 233.04;
            order.AmountRefunded = 5;

            //            order.ShipDate = DateTime.Now;
            var retrievedRecord = ATbase.GetRecordByOrderID(orderID, out _);

            Assert.Equal(order.Notes, retrievedRecord.Notes);
            Assert.Equal(order.ShippingCost, retrievedRecord.ShippingCost);
            Assert.Equal(order.Description, retrievedRecord.Description);
            Assert.Equal(order.Channel, retrievedRecord.Channel);
            Assert.Equal(order.AsanaTaskID, retrievedRecord.AsanaTaskID);
            Assert.NotEqual(order.TotalPrice, retrievedRecord.TotalPrice);

            ATbase.CreateOrderRecord(order, true);

            retrievedRecord = ATbase.GetRecordByOrderID(orderID, out _);

            Assert.Equal(order.TotalPrice, retrievedRecord.TotalPrice);
            Assert.Equal(order.ValueOfInventory, retrievedRecord.ValueOfInventory);

           ATbase.DeleteOrderRecord(order);
        }

        [Fact]
        public void CreateAndUpdateOrderTracking()
        {
            AirtableOrderTracking ATbase = new AirtableOrderTracking();
            AirtableTransactions TransactionsBase = new AirtableTransactions();

            string orderID = "1234567";
            var order = ATbase.NewOrderTrackingData(orderID);

            order.Notes = "this is a test order";
            order.Description = "test order";
            order.PrintOperator = "";
            order.IncludedItems = new List<string> {"zzz - dummy item" };
            order.DesignerURL = "test";
            order.RequestedQuantity = 23;

            ATbase.CreateOrderRecord(order, out _);

            //            order.ShipDate = DateTime.Now;
            var retrievedRecord = ATbase.GetRecordByOrderID(orderID, out _);

            Assert.Equal(order.Notes, retrievedRecord.Notes);
            Assert.Equal(order.Description, retrievedRecord.Description);

            ATbase.CreateOrderRecord(order, out _, true);

            retrievedRecord = ATbase.GetRecordByOrderID(orderID, out _);

            ATbase.DeleteOrderRecord(order);
        }
        [Fact]
        public void RetrieveOrderTracking()
        {
            AirtableOrderTracking ATbase = new AirtableOrderTracking();
            AirtableTransactions TransactionsBase = new AirtableTransactions();
            AirtableItemLookup ProductsBase = new AirtableItemLookup();


            string orderID = "1908720100";

            var retrievedRecord = ATbase.GetRecordByOrderID(orderID, out _);

            var order = TransactionsBase.GetTransactionByRecordID(retrievedRecord.Transactions[0]);
            var productName = ProductsBase.GetItemRecordByRecordID(order.ItemRecordId).DisplayName;
        }
        [Fact]
        public void FindInventoryLocationEntry()
        {
            var inventoryLookup = new AirtableInventory();
            var item = new AirtableItemLookup();
            var component = item.GetComponentByName("Clitoris, Gold");
            // var recordID = inventoryLookup.FindRecordByName("Lamp Base", "fgdfg");
           // inventoryLookup.IncrementQuantityOfItemByName("Blocking Board, Purple, 9\"", "James", 5);
            inventoryLookup.IncrementQuantityOfItem(component.id, "James English", 5);


        }

        [Fact]
        public void CreateTransaction()
        {

            AirtableTransactions ATbase = new AirtableTransactions();

            var productName = "zzz - dummy item";
            var sut = new AirtableItemLookup();

            var order = ATbase.NewTransactionData(sut.FindItemRecord(productName));

            order.Quantity = 5;
            //order.Item ="zzz - dummy item";

            ATbase.CreateOrderRecord(order);



        }


        class EmailData
        {
            public string Email;
            public string ID;
            public string ItemNames;
            public DateTime date;
            public string RawEmail;
            public string RawDate;

        }

        [Fact]
        public void UpdateComponentQuantityByName_test()
        {
            var componentName = "ZZZ - Dummy Component";
            var sut = new AirtableItemLookup();

            var retrievedcomponent = sut.GetComponentByName(componentName);
            var originalQuantity = retrievedcomponent.Quantity;
            var originalPending = retrievedcomponent.Pending;

            sut.UpdateComponentQuantityByName(componentName, 4, 10);
            retrievedcomponent = sut.GetComponentByName(componentName);

            Assert.Equal(retrievedcomponent.Quantity, originalQuantity + 4);
            Assert.Equal(retrievedcomponent.Pending, originalPending - 10);
       }


        [Fact]
        public void UpdateComponentForInventoryRequest_test()
        {
            var componentName = "Clock Kit (small)";
            var sut = new AirtableItemLookup();

            var retrievedcomponent = sut.GetComponentByName(componentName);

            var originalQuantity = retrievedcomponent.Quantity;
            var originalPending = retrievedcomponent.Pending;

            sut.LogInventoryRequestCreation(retrievedcomponent, retrievedcomponent.NumberOfBatches * retrievedcomponent.BatchSize);
            retrievedcomponent = sut.GetComponentByName(componentName);

            Assert.Equal(originalQuantity, retrievedcomponent.Quantity);
            Assert.Equal(originalPending + retrievedcomponent.NumberOfBatches * retrievedcomponent.BatchSize, retrievedcomponent.Pending);
        }
        [Fact]
        public void AddProductRecord_test()
        {
            var sut = new AirtableItemLookup();
            
            sut.AddProductRecord("new untracked product","",0, "https://i.etsystatic.com/14966514/d/il/dbb99f/2210030945/il_75x75.2210030945_k8h3.jpg?version=0");
        }
        [Fact]

        public void AddImage_test()
        {
            var sut = new AirtableItemLookup();
            var product = sut.FindItemRecord("zzz - dummy item", "", 0);
            sut.AddImageToProduct(product, "https://i.etsystatic.com/14966514/d/il/dbb99f/2210030945/il_75x75.2210030945_k8h3.jpg?version=0");
        }

       [Fact]
        public void monthlyTest()
        {
            var monthly = new AirtableMonthly();
            var id = monthly.GetLatestMonthlyID();
        }
        //[Fact]
        public void UpdateCompletedOrderComponentEntries_test()
        {
            var sut = new AirtableItemLookup();
            sut.UpdateCompletedOrderComponentEntries("21");
        }

      //  [Fact]
        public void parseOldEtsyEmails()
        {
            AirtableOrders ATbase = new AirtableOrders();

            string fullFile = File.ReadAllText(@"C:\Users\Al\Dropbox\3DPros - General\Tools\EmailParser\etsyOrderEmailDump\Takeout\Mail\etsy order.mbox");
            string[] emails = fullFile.Split("X-Gmail-Labels: Inbox,Unread,etsy order");
            var emailDataList = new List<EmailData>();
            var badDataList = new List<EmailData>();
            string csv = "";
           


            foreach (string item in emails)
            {
                try
                {
                    var cleanString = item.Replace("=", "");
                    cleanString = cleanString.Replace("\n", "");
                    cleanString = cleanString.Replace("\r", "");
                    string rawdate = Regex.Match(cleanString, @"[a-zA-Z]{3}, \d{2} [a-zA-Z]{3} 20\d{2}").Groups[0].Value;
                    DateTime emaildate = DateTime.MinValue;
                        DateTime.TryParse(rawdate, out emaildate);

                    var emailInfo = new EmailData()
                    {
                        RawEmail = item,
                        Email = Regex.Match(cleanString, @"""mailto:([^\""]*)\""").Groups[1].Value.Trim(),
                        ID = Regex.Match(cleanString, @"http:\/\/www\.etsy\.com\/your\/orders\/(\d{10})").Groups[1].Value,
                        ItemNames = Regex.Match(cleanString, @"Item:\s*?(.*)--").Groups[1].Value,
                        date = emaildate,
                        RawDate = rawdate

                    };
                    if (emailInfo.Email != "" && emailInfo.ID != "")
                    {
                        emailDataList.Add(emailInfo);
                        csv += emailInfo.Email + "\t" + emailInfo.ID + "\t" + emailInfo.ItemNames + "\t" + emailInfo.date.ToString() + "\n";
                        var record = ATbase.GetRecordByOrderID(emailInfo.ID, out _);
                        if(record != null)
                        {
                            if (record.CustomerEmail == "")
                            {
                                record.CustomerEmail = emailInfo.Email;
                                ATbase.CreateOrderRecord(record, true);
                                Thread.Sleep(250);
                            }
                        }
                    }
                    else
                    {
                        badDataList.Add(emailInfo);
                    }

                }
                catch
                { }
            }


        }


      //  [Fact]
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

        public void ItemInventory()
        {
            var materials = new AirtableItemLookup();
            var components = new List<InventoryComponent>();
            var product = materials.FindItemRecord("zzz - dummy item", "purple", 6);
            var componentData = materials.GetComponentByName("ZZZ - Dummy Component");
            var originalQuantity = componentData.Quantity;

            bool value = materials.UpdateInventoryCountForTransaction(product, 10, out components);

            Assert.Single(components);
            Assert.Equal("ZZZ - Dummy Component", components[0].Name);

            componentData = materials.GetComponentByName("ZZZ - Dummy Component");

            Assert.Equal(originalQuantity - 10, componentData.Quantity);

         //   Assert.Equal(componentData.IsBelowThreshhold())


        }

        [Fact]

        public void GetInventoryItem()
        {
            
            var materials = new AirtableItemLookup();
            var product = materials.FindItemRecord("zzz - dummy item");
            var component = materials.GetComponentByName("ZZZ - Dummy Component");

            Assert.Equal("Dummy Item (test)", product.DisplayName);
            Assert.Equal("www.dummyurl.com", product.BaseUrl);

            List<string> printers;
            string preferredPrinter;
            bool value = materials.GetPotentialPrintersList(product, out printers, out preferredPrinter);
            value = materials.GetPreferredShipper(product, out string preferredShipper);
            preferredPrinter = materials.GetPreferredPrinter(product);
            Assert.True(printers.Count > 0);
            Assert.Equal("Kyle Perkuhn", preferredPrinter);
            value = materials.GetPotentialPrintersList(component, out printers, out preferredPrinter);
            preferredPrinter = materials.GetPreferredPrinter(component);

            Assert.True(printers.Count > 0);
            Assert.Equal("Al Billington", preferredPrinter);
        }




        public void getAllItemsInView()
        {
            AirtableOrders ATbase = new AirtableOrders();
            var orders = ATbase.GetAllRecordsInView("Etsy Shipped Last Week", new List<string>() { "weight", "kettlebell", "squat", "dumbbell" });
            var emailList = new List<string>();
            var bccLine = "";
            foreach (var order in orders)
            {
                if (!string.IsNullOrEmpty(order.OptinSentType))
                {
                    break;
                }
                order.OptinSentType = "fitness";
                ATbase.UpdateOrderRecord(order);
                emailList.Add(order.CustomerEmail);
                bccLine = string.Join(",", emailList);
            }
        }
    }
}
