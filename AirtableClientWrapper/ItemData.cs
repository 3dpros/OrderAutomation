using AirtableApiClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirtableClientWrapper
{
    public class ItemData
    {
        public AirtableRecord Record { get; }

            public ItemData(AirtableRecord record)
            {
               Record = record;
            }

        public double GetMaterialCost()
        {
            if (Record.Fields.ContainsKey("Cost"))
            {
                return Double.Parse(Record.Fields["Cost"].ToString());
            }
            return 0;
        }

        public string ItemName()
        {
            if (Record.Fields.ContainsKey("Name"))
            {
                return Record.Fields["Name"].ToString();
            }
            return "";
        }

        public bool IsInventory()
        {
            if (Record.Fields.TryGetValue("Inventory Only", out object isInventory))
                return bool.Parse(isInventory.ToString());
            return false;
        }
    }
}
