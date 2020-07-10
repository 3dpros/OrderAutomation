using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirtableClientWrapper
{
    public class OrderTrackingData
    {
        public const string OrderIDKey = "Order ID";
        public const string OrderValueKey = "Order Value";
        public const string StageKey = "Stage";
        public const string DescriptionKey = "Short Description";
        public const string NotesKey = "Notes";
        public const string RushKey = "Priority";
        public const string DueDateKey = "Due Date";
        public const string InventoryRequestKey = "Inventory Request";
        public const string PrintOperatorKey = "Printer Operator";
        public const string ShipperKey = "Shipping";
        public const string ShippedDateKey = "Ship Date";
        public const string OrderURLKey = "Order URL";
        public const string DesignerURLKey = "Designer URL";
        public const string IncludedItemsKey = "Included Items";
        public const string IncludedComponentsKey = "Component";



        private Dictionary<string, string> _NameLookup;
        private Dictionary<string, string> _ItemsLookup;


        public string OrderID { get; set; }
        public double OrderValue { get; set; }
        public string Stage { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
        public bool Priority { get; set; }
        public bool IsInventoryRequest { get; set; }

        public DateTime DueDate { get; set; }
        public DateTime ShipDate { get; set; }
        public string PrintOperator { get; set; }
        public string Shipper { get; set; }
        public string OrderURL { get; set; }
        public string DesignerURL { get; set; }

        public List<string> IncludedItems { get; set; } = new List<string>();
        public string IncludedComponentId { get; set; }


        public OrderTrackingData(string orderID, Dictionary<string, string> nameLookup, Dictionary<string, string> itemsLookup)
        {
            _NameLookup = nameLookup;
            _ItemsLookup = itemsLookup;

            this.OrderID = orderID;
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
        public OrderTrackingData(Dictionary<string, object> fields, Dictionary<string, string> nameLookup, Dictionary<string, string> itemsLookup) : this(fields.GetString(OrderIDKey), nameLookup, itemsLookup)
        {
            Description = fields.GetString(DescriptionKey);
            Notes = fields.GetString(NotesKey).Trim();
            Stage = fields.GetString(StageKey);
            if (fields.GetString(DueDateKey) != "")
            { DueDate = DateTime.Parse(fields.GetString(DueDateKey)); }
            if (fields.GetString(ShippedDateKey) != "")
            { ShipDate = DateTime.Parse(fields.GetString(ShippedDateKey)); }
            Priority = (fields.GetString(RushKey).ToLower() == "true");
            IsInventoryRequest = (fields.GetString(InventoryRequestKey).ToLower() == "true");
            PrintOperator = GetNameFromIdIfPresent(fields.GetString(PrintOperatorKey), _NameLookup);
            Shipper = GetNameFromIdIfPresent(fields.GetString(ShipperKey), _NameLookup);
            OrderURL = fields.GetString(OrderURLKey);
            DesignerURL = fields.GetString(DesignerURLKey);
            OrderValue = NumberParseOrDefault(fields.GetString(OrderValueKey));
           // IncludedItems - need to make this readable


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
            string[] printOperatorID = GetIdFromNameIfPresent(PrintOperator, _NameLookup)?.ToArray();
            string[] shipperID = GetIdFromNameIfPresent(Shipper, _NameLookup)?.ToArray();
            string[] itemIDs = GetIdFromNameIfPresent(IncludedItems, _ItemsLookup)?.ToArray();
            if (!string.IsNullOrEmpty(IncludedComponentId))
            {
                orderDictionary.AddIfNotNull(IncludedComponentsKey, new string[] { IncludedComponentId });
            }

            orderDictionary.AddIfNotNull(OrderIDKey, OrderID);
            orderDictionary.AddIfNotNull(StageKey, Stage);
            orderDictionary.AddIfNotNull(OrderValueKey, OrderValue);
            orderDictionary.AddIfNotNull(DescriptionKey, Description);
            orderDictionary.AddIfNotNull(NotesKey, Notes);
            if (DueDate.Year > 2010)
            {
                orderDictionary.AddIfNotNull(DueDateKey, DueDate.ToString("d"));
            }
            if (ShipDate.Year > 2010)
            {
                orderDictionary.AddIfNotNull(ShippedDateKey, ShipDate.ToString("d"));
            }
            orderDictionary.AddIfNotNull(RushKey, Priority);
            orderDictionary.AddIfNotNull(InventoryRequestKey, IsInventoryRequest);
            orderDictionary.AddIfNotNull(PrintOperatorKey, printOperatorID);
            orderDictionary.AddIfNotNull(ShipperKey, shipperID);
            if (itemIDs.Length > 0)
            {
                orderDictionary.AddIfNotNull(IncludedItemsKey, itemIDs);
            }
            orderDictionary.AddIfNotNull(OrderURLKey, OrderURL);
            orderDictionary.AddIfNotNull(DesignerURLKey, DesignerURL);        

            return orderDictionary;
        }

    }

}

