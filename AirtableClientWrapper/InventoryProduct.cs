using AirtableApiClient;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirtableClientWrapper
{
    /// <summary>
    ///  Maps to the Products Database table in the Inventory base
    /// </summary>
    public class InventoryProduct
    {
        public AirtableRecord Record { get; }

        public InventoryProduct(AirtableRecord record)
        {
            Record = record;
        }

        private const string CostKey = "Cost";
        private const string NameKey = "Name";
        private const string DisplayNameKey = "Display Name";
        private const string UniqueNameKey = "UniqueName";
        private const string URLKey = "Designer URL Base";
        private readonly string maximumInventoryQuantityKey = "Maximum Inventory Quantity";

        public double MaterialCost
        {
            get
            {
                if (Record.Fields.ContainsKey(CostKey))
                {
                    return Double.Parse(Record.Fields[CostKey].ToString());
                }
                return 0;
            }
        }

        public string Image
        {
            get
            {
                if (Record.Fields.ContainsKey("Image"))
                {
                    return Record.Fields["Image"].ToString();
                }
                return "";
            }
        }

        public string ItemName
        {
            get
            {
                if (Record.Fields.ContainsKey(NameKey))
                {
                    return Record.Fields[NameKey].ToString();
                }
                return "";
            }
        }

        public string DisplayName
        {
            get
            {
                if (Record.Fields.ContainsKey(DisplayNameKey))
                {
                    return Record.Fields[DisplayNameKey].ToString();
                }
                return "";
            }
        }
        public string UniqueName
        {
            get
            {
                if (Record.Fields.ContainsKey(UniqueNameKey))
                {
                    return Record.Fields[UniqueNameKey].ToString();
                }
                return "";
            }
        }

        private string ColorKey = "Color";
        public string Color
        {
            get
            {
                if (Record.Fields.ContainsKey(ColorKey))
                {
                    return Record.Fields[ColorKey].ToString();
                }
                return "";
            }
        }

        private string SKUKey = "SKU";
        public string SKU
        {
            get
            {
                if (Record.Fields.ContainsKey(SKUKey))
                {
                    return Record.Fields[SKUKey].ToString();
                }
                return "";
            }
        }


        private string SizeKey = "Size";
        public int Size
        {
            get
            {
                if (Record.Fields.ContainsKey(SizeKey))
                {
                    return int.Parse(Record.Fields[SizeKey].ToString());
                }
                return 0;
            }
        }

        private const string ProcessingTimeKey = "Processing Time";
        public int ProcessingTime
        {
            get
            {
                if (Record.Fields.ContainsKey(ProcessingTimeKey))
                {
                    return int.Parse(Record.Fields[ProcessingTimeKey].ToString());
                }
                return 0;
            }
        }

        public string BaseUrl {
        get
        {
                if (Record.Fields.ContainsKey(URLKey))
                {
                    return Record.Fields[URLKey].ToString();
                }
                return "";
            }
        }

        public int MaximumInventoryQuantity
        {
            get
            {
                if (Record.Fields.ContainsKey(maximumInventoryQuantityKey))
                {
                    return int.Parse(Record.Fields[maximumInventoryQuantityKey].ToString());
                }
                return 1000000000;
            }
        }

        private readonly string componentsFieldName = "Components";

        public List<InventoryComponent> getComponents(AirtableItemLookup ItemLookup)
        {
            var components = new List<InventoryComponent>();
            var productRecord = this.Record;
            if (productRecord != null)
            {
                if (productRecord.Fields.ContainsKey(componentsFieldName))
                {
                    foreach (var componentID in (JArray)(productRecord.Fields[componentsFieldName]))
                    {
                        var record = ItemLookup.RetreiveComponentsRecord(componentID.ToString());
                        components.Add(new InventoryComponent(record));
                    }
                }
            }
            return components;
        }

        public bool IsInventory()
        {
            if (Record.Fields.TryGetValue("Inventory Only", out object isInventory))
                return bool.Parse(isInventory.ToString());
            return false;
        }

    }
}
