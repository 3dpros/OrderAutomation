using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirtableApiClient;

namespace AirtableClientWrapper
{
    public class TransactionData
    {
        public const string NameKey = "Name";
        public const string IncludedItemsKey = "Product";
        public const string QuantityKey = "Quantity";
        public const string OrderIdKey = "Order";
        public const string PaidKey = "Paid";



        private Dictionary<string, string> _ItemsLookup;

        public string Name { get; set; }
        public int Quantity { get; set; }
        public double Paid { get; set; }

        public string OrderID { get; set; }

        public string ItemRecordId { get; set; }


        public TransactionData(string productRecordID, Dictionary<string, string> itemsLookup)
        {
            _ItemsLookup = itemsLookup;
            this.ItemRecordId = productRecordID;

            this.Name = itemsLookup[ItemRecordId];
        }

        public TransactionData(InventoryProduct product, Dictionary<string, string> itemsLookup) : this (product.Record.Id, itemsLookup)
        {
            this.Name = product.UniqueName;
        }

        public TransactionData(Dictionary<string, object> fields, Dictionary<string, string> itemsLookup) : this(fields.GetString(IncludedItemsKey), itemsLookup)
        {
            this.Quantity = int.Parse(fields.GetString(QuantityKey));
            this.Name = fields.GetString(OrderIdKey);
            this.Paid = double.Parse(fields.GetString(PaidKey));
        }

        //todo this is a copy of the orders method, should create a common place for this
        private double NumberParseOrDefault(string value, double defaultVal = 0)
        {
            double result;
            if (Double.TryParse(value, out result))
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
            return GetIdFromNameIfPresent(new List<string> { name }, lookup);
        }

        private List<string> GetIdFromNameIfPresent(List<string> names, Dictionary<string, string> lookup)
        {
            var IDs = new List<string>();
            foreach(var name in names)
            {
                if (lookup != null && !string.IsNullOrEmpty(name))
                {
                    if (lookup.ContainsValue(name))
                    {
                        IDs.AddRange((from item in lookup
                                      where item.Value == name
                                      select item.Key).ToList());
                    }
                    else if (name != null)
                    {
                        //if exact match isnt found, try for inexact match
                        foreach (KeyValuePair<string, string> item in lookup)
                        {
                            if (item.Value.Contains(name) || name.Contains(item.Value))
                            {
                                IDs.Add(item.Key);
                            }
                        }
                    }
                }
            }
            return IDs;
        }

        public Dictionary<string, object> ToDictionary()
        {
            Dictionary<string, object> orderDictionary = new Dictionary<string, object>();

          //  var itemIDs = GetIdFromNameIfPresent(Item, _ItemsLookup).ToArray();
            orderDictionary.AddIfNotNull(NameKey, Name);
            orderDictionary.Add(IncludedItemsKey, new List<string> { ItemRecordId });
           // else if (itemIDs.Length > 0)
            {
          //      orderDictionary.AddIfNotNull(IncludedItemsKey, itemIDs);
            }
            orderDictionary.AddIfNotNull(QuantityKey, Quantity);
            orderDictionary.AddIfNotNull(PaidKey, Paid);
            if (OrderID != null)
            {
                orderDictionary.Add(OrderIdKey, new List<string> { OrderID });
            }
            return orderDictionary;
        }

    }

}

