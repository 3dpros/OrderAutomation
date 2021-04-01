using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirtableClientWrapper
{
    public class OrderData
    {
        public const string OrderIDKey = "order ID";
        public const string DescriptionKey = "Description";
        public const string NotesKey = "Notes";
        public const string OrderNoteKey = "Order Note";
        public const string CustomerKey = "Customer";
        public const string TotalPaymentKey = "Total Payment";
        public const string ActualShippingKey = "Actual Shipping";
        public const string ShippingChargeKey = "Shipping Charge";
        public const string MaterialCostKey = "Supplies Cost";
        public const string RushKey = "Rush";
        public const string DueDateKey = "Due Date";
        public const string PrintOperatorKey = "Printer Operator";
        public const string ShipperKey = "Shipping";
        public const string ShipperPayKey = "Shipper Pay";
        public const string ChannelKey = "Channel";
        public const string ShippedDateKey = "Ship Date";
        public const string AsanaTaskIDKey = "Asana Task ID";
        public const string OptinSentTypeKey = "Optin Sent Type";
        public const string SalesTaxKey = "Sales Tax Charged";
        public const string ValueOfInventoryKey = "Value of Inventory Products";
        public const string AmountRefundedKey = "Amount Refunded";


        private Dictionary<string, string> _NameLookup;
        private Dictionary<string, string> _ChannelLookup;

        public string OrderID { get; set; }
        public string Description { get; set; }
        public string CustomerEmail { get; set; }
        public string Notes { get; set; }
        public string OrderNote { get; set; }
        public double TotalPrice { get; set; }
        public double ShippingCost { get; set; }
        public double ShippingCharge { get; set; }
        public double SalesTax { get; set; }

        public double MaterialCost { get; set; }
        public bool Rush { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime ShipDate { get; set; }
        public string PrintOperator { get; set; }
        public string Shipper { get; set; }
        public double ShipperPay { get; set; }

        public string Channel { get; set; }
        public string AsanaTaskID { get; set; }
        public string OptinSentType { get; set; }
        public double ValueOfInventory { get; set; }
        public string OrderURL { get; set; }
        public double AmountRefunded { get; set; }


        public OrderData(string orderID, Dictionary<string, string> nameLookup, Dictionary<string, string> channelLookup)
        {
            _NameLookup = nameLookup;
            _ChannelLookup = channelLookup;

            this.OrderID = orderID;

        }


        public OrderData(Dictionary<string, object> fields, Dictionary<string, string> nameLookup, Dictionary<string, string> channelLookup) : this(fields.GetString(OrderIDKey), nameLookup, channelLookup)
        {

            Description = fields.GetString(DescriptionKey);
            CustomerEmail = fields.GetString(CustomerKey);
            Notes = fields.GetString(NotesKey);
            OrderNote = fields.GetString(OrderNoteKey);
            TotalPrice = NumberParseOrDefault(fields.GetString(TotalPaymentKey));
            ShippingCost = NumberParseOrDefault(fields.GetString(ActualShippingKey));
            ShippingCharge = NumberParseOrDefault(fields.GetString(ShippingChargeKey));
            SalesTax = NumberParseOrDefault(fields.GetString(SalesTaxKey));
            MaterialCost = NumberParseOrDefault(fields.GetString(MaterialCostKey));
            if (fields.GetString(DueDateKey) != "")
            { DueDate = DateTime.Parse(fields.GetString(DueDateKey)); }
            if (fields.GetString(ShippedDateKey) != "")
            { ShipDate = DateTime.Parse(fields.GetString(ShippedDateKey)); }
            Rush = (fields.GetString(RushKey).ToLower() == "true");
            Channel = GetNameFromIdIfPresent(fields.GetString(ChannelKey), _ChannelLookup);
            PrintOperator = GetNameFromIdIfPresent(fields.GetString(PrintOperatorKey), _NameLookup);
            Shipper = GetNameFromIdIfPresent(fields.GetString(ShipperKey), _NameLookup);
            ShipperPay = NumberParseOrDefault(fields.GetString(ShipperPayKey));
            AsanaTaskID = fields.GetString(AsanaTaskIDKey);
            OptinSentType = fields.GetString(OptinSentTypeKey);
            ValueOfInventory = NumberParseOrDefault(fields.GetString(ValueOfInventoryKey));
            AmountRefunded = NumberParseOrDefault(fields.GetString(AmountRefundedKey));
        }

        private double NumberParseOrDefault(string value, double defaultVal = 0)
        {
            double result;
            if(Double.TryParse(value, out result))
            {
                return result;
            }
            return defaultVal;
        }

        private string GetNameFromIdIfPresent(object nameID, Dictionary<string, string> lookup)
        {
            var nameIDString = nameID.ToString();

            if (lookup?.ContainsKey(nameIDString) == true)
                return lookup[nameIDString];
            else if (lookup != null)
            {
                //if exact match isnt found, try for inexact match
                foreach (KeyValuePair<string, string> item in lookup)
                {
                    if (item.Key.Contains(nameIDString) || nameIDString.Contains(item.Key))
                    {
                        return (item.Value);
                    }
                }
            }
            return nameIDString;
        }


        private List<string> GetIdFromNameIfPresent(string name, Dictionary<string, string> lookup)
        {
            var IDs = new List<string>();

            if (lookup != null && name != null)
            {
                if(lookup.ContainsValue(name))
                {
                    IDs = (from item in lookup
                           where item.Value == name
                           select item.Key).ToList();
                    return IDs;
                }
                //if exact match isnt found, try for inexact match
                foreach (KeyValuePair<string, string> item in lookup)
                {
                    if (item.Value.Contains(name) || name.Contains(item.Value))
                    {
                        IDs.Add(item.Key);
                        return IDs;
                    }
                }
            }

            return null;
        }

        public Dictionary<string, object> ToDictionary()
        {
            Dictionary<string, object> orderDictionary = new Dictionary<string, object>();
            string[] printOperatorID = GetIdFromNameIfPresent(PrintOperator, _NameLookup)?.ToArray();
            string[] shipperID = GetIdFromNameIfPresent(Shipper, _NameLookup)?.ToArray();
            string[] channelID = GetIdFromNameIfPresent(Channel, _ChannelLookup)?.ToArray();


            orderDictionary.AddIfNotNull(OrderIDKey, OrderID.ToString());
            orderDictionary.AddIfNotNull(DescriptionKey, Description);
            orderDictionary.AddIfNotNull(NotesKey, Notes);
            orderDictionary.AddIfNotNull(OrderNoteKey, OrderNote);
            orderDictionary.AddIfNotNull(CustomerKey, CustomerEmail);
            orderDictionary.AddIfNotNull(TotalPaymentKey, TotalPrice);
            orderDictionary.AddIfNotNull(ActualShippingKey, ShippingCost);
            orderDictionary.AddIfNotNull(SalesTaxKey, SalesTax);
            orderDictionary.AddIfNotNull(ShippingChargeKey, ShippingCharge);
            orderDictionary.AddIfNotNull(MaterialCostKey, MaterialCost);
            if (DueDate.Year > 2010)
            {
                orderDictionary.AddIfNotNull(DueDateKey, DueDate.ToString("d"));
            }
            if (ShipDate.Year > 2010)
            {
                orderDictionary.AddIfNotNull(ShippedDateKey, ShipDate.ToString("d"));
            }
            orderDictionary.AddIfNotNull(RushKey, Rush);
            orderDictionary.AddIfNotNull(PrintOperatorKey, printOperatorID);
            orderDictionary.AddIfNotNull(ShipperKey, shipperID);
            orderDictionary.AddIfNotNull(ShipperPayKey, ShipperPay);
            orderDictionary.AddIfNotNull(ChannelKey, channelID);
            orderDictionary.AddIfNotNull(AsanaTaskIDKey, AsanaTaskID);
            orderDictionary.AddIfNotNull(OptinSentTypeKey, OptinSentType);
            orderDictionary.AddIfNotNull(ValueOfInventoryKey, ValueOfInventory);
            orderDictionary.AddIfNotNull(AmountRefundedKey, AmountRefunded);

            return orderDictionary;
        }

    }

}

