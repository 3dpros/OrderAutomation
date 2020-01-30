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

        }

        [Fact]
        public void getAllItemsInView()
        {
            AirtableOrders ATbase = new AirtableOrders();
            var orders = ATbase.GetAllRecordsInView("Etsy Shipped Last Week", new List<string>() {"weight", "kettlebell", "squat", "dumbbell"});
            var emailList = new List<string>();
            var bccLine = "";
            foreach (var order in orders)
            {
                if(!string.IsNullOrEmpty(order.OptinSentType))
                {
                    break;
                }
                order.OptinSentType = "fitness";
                ATbase.UpdateOrderRecord(order);
                emailList.Add(order.CustomerEmail);
                bccLine = string.Join(",", emailList);
            }
        }
        [Fact]
        public void CreateAndUpdateOrder()
        {
            AirtableOrders ATbase = new AirtableOrders();

            var order = ATbase.newOrderData("1234567");

            order.Notes = "this is a test order";
            order.ShippingCost = 10.21;
            order.Description = "test order";
            //  order.PrintOperator = "Al Billington";
            //   order.Shipper = "Leah";
           // order.ShippingCharge = Math.Min(7;
            order.Channel = "Direct";
            order.AsanaTaskID = "3242342634652352";
            ATbase.CreateOrderRecord(order);

            order.MaterialCost = 12.34;
            order.TotalPrice = 504.23;
//            order.ShipDate = DateTime.Now;
            ATbase.CreateOrderRecord(order, true);

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
            var sut = new AirtableItemLookup();
            sut.UpdateComponentQuantityByName("Weight Plate Ornament, Black with Silver Text", 23);
        }

        [Fact]
        public void UpdateCompletedOrderComponentEntries_test()
        {
            var sut = new AirtableItemLookup();
            sut.UpdateCompletedOrderComponentEntries("21");
        }

        [Fact]
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

        public void ItemInventory()
        {
            var materials = new AirtableItemLookup();
            var components = new List<ItemComponentData>();
            var record = materials.FindItemRecord("Printed Crochet Blocking Board - extra", "purple", 6);
           bool value = materials.UpdateInventoryCountForTransaction(record, 3, out components);

        }

        [Fact]

        public void ItemInventoryBySKU()
        {
            var materials = new AirtableItemLookup();

       //     bool value = materials.UpdateInventoryQuantityForTransactionBySKU(out var materialCost, "WeightPlateOrnament_silver", 3);

        }
    }
}
