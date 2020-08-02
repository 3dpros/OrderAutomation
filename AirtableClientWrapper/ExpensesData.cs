using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirtableClientWrapper
{
    public class ExpensesData
    {
        public const string NameKey = "Name";
        public const string DateKey = "Date";
        public const string ValueKey = "Value";
        public const string QuantityKey = "Quantity";
        public const string DeliveredToKey = "Delivered To";

        public string Name { get; set; }
        public DateTime Date { get; set; }
        public double Value { get; set; }
        public string DeliveredTo { get; set; }
        public int Quantity { get; set; } = 1;


        public ExpensesData(string name)
        {
            Name = name;
        }

        public ExpensesData(Dictionary<string, object> fields, Dictionary<string, string> nameLookup) 
        {
            

            Name = fields.GetString(NameKey);
            Date = DateTime.Parse(fields.GetString(DateKey));
            Value = Double.Parse(fields.GetString(ValueKey));
            Quantity = int.Parse(fields.GetString(QuantityKey));
            DeliveredTo = fields.GetString(DeliveredToKey);



        }

        public Dictionary<string, object> ToDictionary()
        { 
           var orderDictionary = new Dictionary<string, object>();

            orderDictionary.AddIfNotNull(NameKey, Name.ToString());
            orderDictionary.AddIfNotNull(DateKey, Date.ToString("d"));
            orderDictionary.AddIfNotNull(ValueKey, Value);
            orderDictionary.AddIfNotNull(QuantityKey, Quantity);
            orderDictionary.AddIfNotNull(DeliveredToKey, DeliveredTo);


            return orderDictionary;
        }

    }

}

